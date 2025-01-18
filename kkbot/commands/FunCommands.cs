using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.EventHandling;
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
using DSharpPlus.Interactivity.Extensions;
using System.Text.Json;


// 2024.09.20 Förbättra !smhi
// 2022.08.20 lägger till Recommend för spel och serier och sånt
// 2022.08.20 Behövde uppdatera libs igen :) 
// 2020.06.26 Haft nåt strul med !joke, att den inte minns skämt den redan dragit samma dag.. Men testade nu och det verkar fungera. 
// 2020.05.12  De flesta kommandon fungerar. addjoke, smhi, addjoke, jokestats. "poll" är inte så himla bra dock.



namespace kkbot.commands
{
    public class FunCommands : BaseCommandModule
    {
      [Command("addsemester")]
      public async Task addsemester(CommandContext ctx)
      {
        string filename = "./semester.txt";

        try
        {
          using (StreamWriter sw = File.AppendText(filename))
          {
            sw.WriteLine(ctx.Message.Content.Remove(0, 9));
          }
          await ctx.Channel.SendMessageAsync("Äntligen!").ConfigureAwait(false);

        }
        catch (Exception)
        {
          await ctx.Channel.SendMessageAsync("Barka käpprätt åt helvete. Systemfel!").ConfigureAwait(false);

        }
      }




      [Command("semester")]
      public async Task semester(CommandContext ctx)
      {
        Randomizur randInst = Randomizur.Instance;
        Memorizur memInst = Memorizur.Instance;
        string outputStr = "herpaderpa";
        int nr = 0;
        int nrLines = 0;
        string filename = "semester.txt";
        int randomLineNr = 0;
       
        string[] lines = null;

        try
        {
          lines = File.ReadAllLines(filename);
        }
        catch (Exception)
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
          memInst.recentlyUsedSemesterLineNumbers.TryGetValue(randomLineNr, out prevquoted);

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
          memInst.recentlyUsedSemesterLineNumbers.Add(randomLineNr, DateTime.Now);
        }
      }


        [Command("addtips")] 
        public async Task addTips(CommandContext ctx)
        {
            string filename = "./tips.txt";

            try
            {
                using (StreamWriter sw = File.AppendText(filename))
                {
                    sw.WriteLine(ctx.Message.Content.Remove(0,6));
                }
                await ctx.Channel.SendMessageAsync("bra där").ConfigureAwait(false);

            }
            catch (Exception)
            {
                await ctx.Channel.SendMessageAsync("Barka käpprätt åt helvete. Systemfel!").ConfigureAwait(false);

            }
        }
          
        [Command("tips")]
        public async Task findTips(CommandContext ctx)
        {
          string filename = "./tips.txt";
          string resultStr = "";

          List<string> result = new List<string>();
           
          string[] lines = File.ReadAllLines(filename);   


          foreach(string str in lines) 
          { 
            Console.WriteLine(str.ToLowerInvariant());
            Console.WriteLine( ctx.Message.Content.Remove(0, 9).ToLowerInvariant());
         

            // "!söktips "
            // "123456789"

            if(str.ToLowerInvariant().IndexOf( ctx.Message.Content.Remove(0, 9).ToLowerInvariant() ) != -1) {
              result.Add(str);
            }
          }

       
          resultStr = "";
          foreach(string str in result) 
          {
            resultStr += str + "\n"; 

          }


          if(resultStr.Equals("")) {
            resultStr = "nothing found."; 
          }



          // Reply to channel
          try
          { 
              await ctx.Channel.SendMessageAsync(resultStr).ConfigureAwait(false); 
          }
          catch (Exception)
          {
              await ctx.Channel.SendMessageAsync("Barka käpprätt åt helvete. Systemfel findTips()").ConfigureAwait(false); 
          }
           
        }

    // returns 0 on OK
    // (--) 
    private int dlFileToTempFolder(string uriStr, string outputFilename)
    {
      // Decide on your temp folder
      string tempFolder = "./temp/";

      try
      {
        // Check if the directory exists; if not, create it
        if (!Directory.Exists(tempFolder))
        {
          Directory.CreateDirectory(tempFolder);
        }

        // Build the full file path
        string filePath = Path.Combine(tempFolder, outputFilename);

        // Log what we're about to do
        Console.WriteLine($"Downloading file from '{uriStr}' to '{filePath}' ...");

        // Use a WebClient in a using block so it disposes automatically
        using (WebClient client = new WebClient())
        {
          client.DownloadFile(uriStr, filePath);
        }

        Console.WriteLine("Download succeeded!");
        return 0; // success
      }
      catch (Exception ex)
      {
        Console.WriteLine($"Error in dlFileToTempFolder: {ex.Message}");
        return -1; // failure
      }
    }




    //Old
    // private int dlFileToTempFolder(string uriStr, string outputFilename)
    //{
    //    string errorStr = "";
    //    int errorCode = 0;

    //    string tempFolder = @"./temp/";
    //    try
    //    {
    //        if (!System.IO.File.Exists(tempFolder)) {

    //            System.IO.Directory.CreateDirectory(tempFolder);
    //        }

    //    }
    //    catch(Exception)
    //    {
    //        errorCode = -1;
    //        errorStr = "Could not create temp folder!";
    //    }

    //    try
    //    {

    //        // Try to dl with webclient library
    //        string address = uriStr;
    //        string fileName = tempFolder + outputFilename;
    //        Console.WriteLine("smhi: Downloaded to " + fileName);

    //        WebClient client = new WebClient();
    //        client.DownloadFile(address, fileName);
    //    }
    //    catch(Exception e)
    //    {
    //        Console.WriteLine(e.Message);

    //    }

    //    if (errorCode != 0)
    //    {
    //        Console.WriteLine(errorStr);
    //        Console.WriteLine("error with dlFileToTempFolder() !");

    //        return -1;
    //    }
    //    return 0;

    //}



    // (-+) tested
    void deleteOldSmhiFiles()
        {
            List<string> filenames = new List<string>();

            filenames.Add("./temp/smhi_hagfors.txt");
            filenames.Add("./temp/smhi_munkfors.txt");
            filenames.Add("./temp/smhi_kristinehamn.txt");
            filenames.Add("./temp/smhi_ojebyn.txt");


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



        // (-+) Funkar än så länge        
        
        [Description("Ger nuvarande temperatur i tre städer. Detta är beräknade värden, inte siffror från en väderstation.")]
        [Command("smhi")]
        public async Task smhi(CommandContext ctx)
        {

            List<string> outputStr = new List<string>();            
            string finalOutString = "";
            int milliseconds = 500;
                                          



      // Om vi vill få ner data om senaste timme från en väderstation, kör GET mot denna:

      //              GET / api / version /{ version}/ parameter /{ parameter}/ station /{ station}/ period /{ period}.{ ext}
      //                                                                       103090                       period= latest-hour

      // Example: https://opendata-download-metfcst.smhi.se/api/version/1.0/parameter/1/station/159880.json
      // Example: https://opendata-download-metobs.smhi.se/api/version/1.0/parameter/1/station/159880.json
      // använd då SMHIStationJSon.cs klassen




      /*
      *      Förklaring av koderna i jsonen:
      *      https://opendata.smhi.se/apidocs/metfcst/parameters.html#parameter-table
      * 
      *    behöver emote för alla de här'
      *    wsymb2 	code 	hl 	0 	Weather symbol 	Integer, 1-27
      1   Clear sky
      2 	Nearly clear sky              
      3 	Variable cloudiness
      4 	Halfclear sky
      5 	Cloudy sky
      6 	Overcast
      7 	Fog
      8 	Light rain showers
      9 	Moderate rain showers
      10 	Heavy rain showers
      11 	Thunderstorm
      12 	Light sleet showers
      13 	Moderate sleet showers
      14 	Heavy sleet showers
      15 	Light snow showers
      16 	Moderate snow showers
      17 	Heavy snow showers
      18 	Light rain
      19 	Moderate rain
      20 	Heavy rain
      21 	Thunder
      22 	Light sleet
      23 	Moderate sleet
      24 	Heavy sleet
      25 	Light snowfall
      26 	Moderate snowfall
      27 	Heavy snowfall
      * */



      string tempFolder = @"./temp/";



      //
      // New AI code to read from json file instead of hardcoded     
      //


      // Filen innehåller saker, så vi måste läsa upp filen, filen läser upp till datastrukturer som är en SMHIBotData -3x strings

      string json = "";
      try
      {
        json = File.ReadAllText($"{tempFolder}smhidata.json");
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
        await ctx.Channel.SendMessageAsync("Could not find smhidata.json").ConfigureAwait(false);
        return;
      }


      List<SMHIBotData> smhiDataList = null;
      try
      {
        smhiDataList = JsonSerializer.Deserialize<List<SMHIBotData>>(json);
        Console.WriteLine("smhibotdata in json file:");

        foreach (var item in smhiDataList)
        {
          Console.WriteLine($"{item.Filename} {item.OutputPreString} {item.OutputPostString} {item.SmhiUri}");
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine(ex.ToString());
        await ctx.Channel.SendMessageAsync("Could not parse smhidata.json").ConfigureAwait(false);
        return;
      }
            
      deleteOldSmhiFiles(); // Do this before we start the new downloads below


      // Now:
      // - Download smhi json file for each item
      // - Parse it
      // - Prepare output string

      Console.WriteLine("Downloading.");
      foreach (var item in smhiDataList)
      {
        Console.WriteLine($"Downloading {item.Filename} {item.OutputPreString} {item.OutputPostString} {item.SmhiUri}");

        int result = dlFileToTempFolder(item.SmhiUri, item.Filename);

        Thread.Sleep(milliseconds);

        if(result ==0)
        {
          try
          {
            // lets parse the heck out of this json file

            // Vi får lov att skapa en C# class till just SMHI:s json-fil.  
            string[] lines = File.ReadAllLines(tempFolder + item.Filename);
            SMHIJson smjson = SMHIJson.FromJson(lines[0]);

            // 1. hitta rätt tid i TimeSeris[0] eller [1]    
            // 2. hitta rätt Parameters[1] t.ex.där är name = "t"
            foreach (Parameter param in smjson.TimeSeries[1].Parameters)
            {
              if (param.Name == "t")
              {
                item.FinalString = $"{item.OutputPreString} {param.Values[0]}  C {item.OutputPostString}";
                //outputStr.Add("" + preString[nr] + param.Values[0] + " C");
              }
              if (param.Name.ToLower() == "wsymb2")
              {
                item.FinalString += " " + SmhiGetWeatherEmoji(param.Values[0]);                            // UNTESTED 
              }
            }


          }
          catch (Exception ex)
          {
            System.Console.WriteLine($"Exception smhi! {ex.ToString()}");
            await ctx.Channel.SendMessageAsync("Jag är Error.").ConfigureAwait(false);
          }
        }
                                                         
      }


                       


          // Samla ihop alla delsträngar till en slutsträng och skriv till kanal.


          Console.WriteLine("Compiling.");
          foreach (var item in smhiDataList)
          {
            finalOutString += item.FinalString + "\n";
          }
          //foreach(string outStr in outputStr)
          //{
          //    finalOutString += outStr + "\n";
          //}

         await ctx.Channel.SendMessageAsync(finalOutString).ConfigureAwait(false);

        }

     
    // (-+)
    public static string SmhiGetWeatherEmoji(double dbl)
    {
      int nr = (int)Math.Round(dbl);
      return nr switch
      {
        1 => "☀️ Clear sky",   // 
        2 => "🌤 Nearly clear sky",   // 
        3 => "⛅️ Variable cloudiness",   // 
        4 => "🌥 Halfclear sky",   // 
        5 => "☁️ Cloudy sky",   // 
        6 => "☁️ Overcast",   // 
        7 => "🌫 Fog",   // 
        8 => "🌦 Light rain showers",   // 
        9 => "🌧🌧 Moderate rain showers",   // 
        10 => "🌧🌧🌧 Heavy rain showers",   // 
        11 => "⛈ Thunderstorm",   // 
        12 => "🌨 Light sleet showers",   // 
        13 => "🌨🌨 Moderate sleet showers",   // 
        14 => "🌨🌨🌨 Heavy sleet showers",   // 
        15 => "🌨 Light snow showers",   // 
        16 => "🌨🌨 Moderate snow showers",   // 
        17 => "🌨🌨🌨 Heavy snow showers",   // 
        18 => "🌧 Light rain",   // 
        19 => "🌧🌧 Moderate rain",   // 
        20 => "🌧🌧🌧 Heavy rain",   // 
        21 => "🌩 Thunder",   // 
        22 => "🌨 Light sleet",   // 
        23 => "🌨🌨 Moderate sleet",   // 
        24 => "🌨🌨🌨 Heavy sleet",   // 
        25 => "❄️ Light snowfall",   // 
        26 => "❄️❄️ Moderate snowfall",   // 
        27 => "❄️❄️❄️ Heavy snowfall",   // 
        _ => "❓ Unknown code, smhi changed something in the matrix!"     // 
      };


      /*
        1 => "☀️",   // Clear sky
        2 => "🌤",   // Nearly clear sky
        3 => "⛅️",   // Variable cloudiness
        4 => "🌥",   // Halfclear sky
        5 => "☁️",   // Cloudy sky
        6 => "☁️",   // Overcast
        7 => "🌫",   // Fog
        8 => "🌦",   // Light rain showers
        9 => "🌧🌧",   // Moderate rain showers
        10 => "🌧🌧🌧",   // Heavy rain showers
        11 => "⛈",   // Thunderstorm
        12 => "🌨",   // Light sleet showers
        13 => "🌨🌨",   // Moderate sleet showers
        14 => "🌨🌨🌨",   // Heavy sleet showers
        15 => "🌨",   // Light snow showers
        16 => "🌨🌨",   // Moderate snow showers
        17 => "🌨🌨🌨",   // Heavy snow showers
        18 => "🌧",   // Light rain
        19 => "🌧🌧",   // Moderate rain
        20 => "🌧🌧🌧",   // Heavy rain
        21 => "🌩",   // Thunder
        22 => "🌨",   // Light sleet
        23 => "🌨🌨",   // Moderate sleet
        24 => "🌨🌨🌨",   // Heavy sleet
        25 => "❄️",   // Light snowfall
        26 => "❄️❄️",   // Moderate snowfall
        27 => "❄️❄️❄️",   // Heavy snowfall
      */
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
         
        
        // var interactivity = ctx.Client.GetInteractivity() they moved the getINteractivity somewhere else
        // 
        var interactivity = Client​Extensions.GetInteractivity(ctx.Client);
            
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

        
        [Command("hälga")]
        public async Task helg4(CommandContext ctx)
        {
          await helg(ctx);
        }

        
        [Command("helg")]
        public async Task helg3(CommandContext ctx)
        {
          await helg(ctx);
        }

        [Command("helga")]
        public async Task helg2(CommandContext ctx)
        {
          await helg(ctx);
        }

        // (++) 

        [Command("hälja")]
        public async Task helg(CommandContext ctx)
        {
            Randomizur randInst = Randomizur.Instance;
            Memorizur memInst = Memorizur.Instance;
            string outputStr = "herpaderpa";
            int nr = 0;
            int nrLines = 0;
            string filename = "helg.txt";
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
                memInst.recentlyUsedHelgLineNumbers.TryGetValue(randomLineNr, out prevquoted);

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
                memInst.recentlyUsedHelgLineNumbers.Add(randomLineNr, DateTime.Now);
            }
        }











        [Command("addhelg")]
        public async Task addhelg(CommandContext ctx)
        {
            string filename = "./helg.txt";

            try
            {
                using (StreamWriter sw = File.AppendText(filename))
                {
                    sw.WriteLine(ctx.Message.Content.Remove(0, 9));
                }
                await ctx.Channel.SendMessageAsync("Helgen är räddad!").ConfigureAwait(false);

            }
            catch (Exception)
            {
                await ctx.Channel.SendMessageAsync("Barka käpprätt åt helvete. Systemfel!").ConfigureAwait(false);

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
