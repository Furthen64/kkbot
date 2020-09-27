using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using kkbot.Singletons;
using System.ComponentModel.DataAnnotations;
using kkbot.DS.SMHI;
using System.Runtime.CompilerServices;
using System.Threading;

/// 2020.06.26 Haft nåt strul med !joke, att den inte minns skämt den redan dragit samma dag.. Men testade nu och det verkar fungera. 
/// 2020.05.12  De flesta kommandon fungerar. addjoke, smhi, addjoke, jokestats. "poll" är inte så himla bra dock.
///  
/// 


namespace kkbot.commands
{
    public class FunCommands : BaseCommandModule
    {


        // returns 0 on OK
        // (-+) seems to work
        private int dlFileToTempFolder(string uriStr, string outputFilename)
        {
            string errorStr = "";
            int errorCode = 0;

            string tempFolder = @"./temp/";
            try
            {
                if (!System.IO.File.Exists(tempFolder)) {

                    System.IO.Directory.CreateDirectory(tempFolder);
                }

            }
            catch(Exception)
            {
                errorCode = -1;
                errorStr = "Could not create temp folder!";
            }

            try
            {

                // Try to dl with webclient library
                string address = uriStr;
                string fileName = tempFolder + outputFilename;
                Console.WriteLine("smhi: Downloaded to " + fileName);

                WebClient client = new WebClient();
                client.DownloadFile(address, fileName);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
              
            }

            if (errorCode != 0)
            {
                Console.WriteLine(errorStr);
                Console.WriteLine("error with dlFileToTempFolder() !");

                return -1;
            }
            return 0;

        }



        // (--) untested
        void deleteOldSmhiFiles()
        {
            List<string> filenames = new List<string>();

            filenames.Add("./temp/smhi_hagfors.txt");
            filenames.Add("./temp/smhi_munkfors.txt");
            filenames.Add("./temp/smhi_kristinehamn.txt");


            foreach (string currFilename in filenames)
            {
                try
                {
                    if (File.Exists(currFilename))
                    {
                        // If file found, delete it    
                        File.Delete(currFilename);
                        Console.WriteLine("deleteOldSmhiFiles: File " + currFilename + " deleted.");
                    }
                    else Console.WriteLine("deleteOldSmhiFiles: " + currFilename + " not found ");
                }
                catch (IOException ioExp)
                {
                    Console.WriteLine(ioExp.Message);
                }
            }
        }

        // (-+) Funkar än så länge. 
        // Wishlist: 
        //      * Så den kan dra ner Munkfors och Kristinehamn också 
        [Description("Ger nuvarande temperatur i tre städer. Detta är beräknade värden, inte siffror från en väderstation.")]
        [Command("smhi")]
        public async Task smhi(CommandContext ctx)
        {

            List<string> outputStr = new List<string>();            
            string finalOutString = "";
            int milliseconds = 500;

            // Preppa det som ska laddas ner

            List<string> filenames = new List<string>();
            List<string> smhiUris = new List<string>();
            List<string> preString = new List<string>();



            // Om vi vill få ner data om senaste timme från en väderstation:

            //              GET / api / version /{ version}/ parameter /{ parameter}/ station /{ station}/ period /{ period}.{ ext}
            //                                                                       103090                       period= latest-hour

            // Example: https://opendata-download-metfcst.smhi.se/api/version/1.0/parameter/1/station/159880.json
            // Example: https://opendata-download-metobs.smhi.se/api/version/1.0/parameter/1/station/159880.json
            // använd då SMHIStationJSon.cs klassen








            string tempFolder = @"./temp/";
            filenames.Add("smhi_hagfors.txt");
            smhiUris.Add("https://opendata-download-metfcst.smhi.se/api/category/pmp3g/version/2/geotype/point/lon/13.677855/lat/60.044464/data.json");
            preString.Add("Temp i Hagfors nu: ");

            filenames.Add("smhi_munkfors.txt");
            smhiUris.Add("https://opendata-download-metfcst.smhi.se/api/category/pmp3g/version/2/geotype/point/lon/13.546200/lat/59.836500/data.json");
            preString.Add("Temp i Munkfors nu: ");


            filenames.Add("smhi_kristinehamn.txt");
            smhiUris.Add("https://opendata-download-metfcst.smhi.se/api/category/pmp3g/version/2/geotype/point/lon/14.093200/lat/59.320700/data.json");
            preString.Add("Temp i Kristinehamn nu: ");
             




            // Delete the old files
            deleteOldSmhiFiles();



            // Ladda ner alla smhi-jsons
            // Parsa filerna 
            // Lägg den relevanta informationen i sträng-arrayen "outputStr"
            int nr = 0;

            foreach(string smhiUri in smhiUris)
            {
                int result = dlFileToTempFolder(smhiUri,
                                           filenames[nr]) ;

                
                Thread.Sleep(milliseconds); // Vänta ett tag så inte SMHI bannar oss? jag har ingen aning.


                if (result == 0)
                {
                    try
                    {
                        // lets parse the heck out of this json file

                        // Vi får lov att skapa en C# class till just SMHI:s json-fil. Så därså.
                        string[] lines = File.ReadAllLines(tempFolder + filenames[nr]);                  // WORRY MAYBE IS TOO BIG FILE YA?
                        SMHIJson smjson = SMHIJson.FromJson(lines[0]);

                        // 1. hitta rätt tid i TimeSeris[0] eller [1]

                        // 2.hitta rätt Parameters[1] t.ex.där är name = "t"
                        foreach (Parameter param in smjson.TimeSeries[1].Parameters)
                        {
                            if (param.Name == "t")
                            {
                                outputStr.Add("" + preString[nr] + param.Values[0] + " C");
                            }
                        }
                         

                    }
                    catch (Exception)
                    {
                        System.Console.WriteLine("Exception smhi!");
                        await ctx.Channel.SendMessageAsync("Jag är Error.").ConfigureAwait(false);
                    }
                }
                nr++;

            }


            // Samla ihop alla delsträngar till en slutsträng och skriv till kanal.

            foreach(string outStr in outputStr)
            {
                finalOutString += outStr + "\n";
            }

            await ctx.Channel.SendMessageAsync(finalOutString).ConfigureAwait(false);

        }
         
        
        // (++)
        [Description("Statistik om skämtdatabasen")]
        [Command("jokestats")]
        public async Task jokestats(CommandContext ctx)
        {
            string filename = "./jokes.txt";
            string[] lines = File.ReadAllLines(filename);
            int nrLines = lines.Length; 
            string str = "Antal skämt i db: " + nrLines; 
            await ctx.Channel.SendMessageAsync(str).ConfigureAwait(false);
        }

        [Command("poll")]
        public async Task poll(CommandContext ctx, TimeSpan duration, params DiscordEmoji [] emojiOptions )
        {
            var interactivity = ctx.Client.GetInteractivity();
            var options = emojiOptions.Select(x => x.ToString());

            var pollEmbed = new DiscordEmbedBuilder
            {
                Title = "Poll",
                Description = string.Join(" ", options)
            };

            var pollMessage = await ctx.Channel.SendMessageAsync(embed: pollEmbed).ConfigureAwait(false);

            foreach (var option in emojiOptions)
            {
                await pollMessage.CreateReactionAsync(option).ConfigureAwait(false);
            }

            var result = await interactivity.CollectReactionsAsync(pollMessage, duration).ConfigureAwait(false);
            var distinctResult = result.Distinct();

            var results = distinctResult.Select(x => $"{x.Emoji}: {x.Total}");

            await ctx.Channel.SendMessageAsync(string.Join("\n", results)).ConfigureAwait(false);
             

        }


        [Command("addjoke")]
        public async Task addjoke(CommandContext ctx)
        {
            string filename = "./jokes.txt";

            try
            {
                using (StreamWriter sw = File.AppendText(filename))
                {
                    sw.WriteLine(ctx.Message.Content.Remove(0, 9));                    
                }
                await ctx.Channel.SendMessageAsync("KUL").ConfigureAwait(false);

            }
            catch (Exception)
            {
                await ctx.Channel.SendMessageAsync("Barka käpprätt åt helvete. Systemfel!").ConfigureAwait(false);

            }
        }




        // (+-)
        [Command("joke")]
        public async Task joke(CommandContext ctx)
        {
            Randomizur randInst = Randomizur.Instance;
            Memorizur memInst = Memorizur.Instance;
            string outputStr = "herpaderpa";
            int nr = 0;
            int nrLines = 0;
            string filename = "jokes.txt";
            int randomLineNr = 0;
            string[] lines = File.ReadAllLines(filename);
            nrLines = lines.Length;
            bool keepTrying = true;
            int maxTries = 2000;
            int currTry = 0;
            bool tellJoke = false;
            DateTime prevJoked;



            // Try 2000 or so times to find a joke that has not been said
            while (keepTrying && currTry++ < maxTries)
            {
                // Get random Line Number from the "jokes.txt"
                randomLineNr = randInst.getInt(0, nrLines);



                // Now try that against the memory
                memInst.recentlyUsedJokeLineNumbers.TryGetValue(randomLineNr, out prevJoked);                

                // Check if we got year 1 which means there is no match in "recentlyUsedJokes"

                if (prevJoked.Year == 1)                 
                {
                    keepTrying = false;                     // Could not find it, tell the joke!
                    tellJoke = true;

                }
                // else : This was a recently used joke, but how recent?
                else if(prevJoked.Date < DateTime.Today)
                {
                    keepTrying = false;                     // It was told yesterday or earlier, go ahead, tell it :D
                    tellJoke = true;
                } 
                // else : Recently used joke, today. Keep trying.


            }

            if(tellJoke)
            {

                // Tell joke
                foreach (string line in lines)
                {
                    if (nr++ == randomLineNr)
                    {
                        outputStr = line;
                    }
                    Console.WriteLine(line);
                }

                await ctx.Channel.SendMessageAsync(outputStr).ConfigureAwait(false);


                // Add to memory
                memInst.recentlyUsedJokeLineNumbers.Add(randomLineNr, DateTime.Now);
            }
        }




        // (--)
        [Command("quote")]
        public async Task quote(CommandContext ctx)
        {
            Randomizur randInst = Randomizur.Instance;
            Memorizur memInst = Memorizur.Instance;
            string outputStr = "herpaderpa";
            int nr = 0;
            int nrLines = 0;
            string filename = "moviequotes.txt";
            int randomLineNr = 0;

            // TODO: Lägg till denna try catch block till joke också
            string[] lines = null;

            try
            {
                lines = File.ReadAllLines(filename);
            }
            catch(Exception)
            {
                await ctx.Channel.SendMessageAsync("Barka käpprätt åt helvete. Systemfel!").ConfigureAwait(false);
            }
            nrLines = lines.Length;
            bool keepTrying = true;
            int maxTries = 2000;
            int currTry = 0;
            bool tellquote = false;
            DateTime prevquoted;

            // Try 2000 or so times to find a quote that has not been said
            while (keepTrying && currTry++ < maxTries)
            {
                // Get random Line Number from the textfile
                randomLineNr = randInst.getInt(0, nrLines);


                // Now try that against the memory
                memInst.recentlyUsedQuoteLineNumbers.TryGetValue(randomLineNr, out prevquoted);

                // Check if we got year 1 which means there is no match in "recentlyUsedquotes"
                if (prevquoted.Year == 1)
                {
                    keepTrying = false;                     // Could not find it, tell the quote!
                    tellquote = true;

                }
                // else : This was a recently used quote, but how recent?
                else if (prevquoted.Date < DateTime.Today)
                {
                    keepTrying = false;                     // It was told yesterday or earlier, go ahead, tell it :D
                    tellquote = true;
                }
                // else : Recently used quote, today. Keep trying.


            }

            if (tellquote)
            {

                // Tell quote
                foreach (string line in lines)
                {
                    if (nr++ == randomLineNr)
                    {
                        outputStr = line;
                    }
                    Console.WriteLine(line);
                }

                await ctx.Channel.SendMessageAsync(outputStr).ConfigureAwait(false);


                // Add to memory
                memInst.recentlyUsedQuoteLineNumbers.Add(randomLineNr, DateTime.Now);
            }
        }




        [Command("addquote")]
        public async Task addquote(CommandContext ctx)
        {
            string filename = "./moviequotes.txt";

            try
            {
                using (StreamWriter sw = File.AppendText(filename))
                {
                    sw.WriteLine(ctx.Message.Content.Remove(0, 9));
                }
                await ctx.Channel.SendMessageAsync("You've made me an offer I can't refuse. Quote added!").ConfigureAwait(false);

            }
            catch (Exception)
            {
                await ctx.Channel.SendMessageAsync("Barka käpprätt åt helvete. Systemfel!").ConfigureAwait(false);

            }
        }







    }
}
