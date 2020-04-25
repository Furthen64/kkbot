using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;

namespace kkbot.commands
{
    public class FunCommands : BaseCommandModule
    {


        // returns 0 on OK
        private int dlFileToTempFolder(string uriStr)
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
                string fileName = tempFolder + "output.json";

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

        [Description("Ger nuvarande temperatur i hagfors, skålviksvägen specifikt")]
        [Command("smhi")]

        public async Task smhi(CommandContext ctx)
        {
            int result = dlFileToTempFolder("https://opendata-download-metfcst.smhi.se/api/category/pmp3g/version/2/geotype/point/lon/13.677855/lat/60.044464/data.json");
            if(result == 0)
            {
                try
                {
                    // lets parse the heck out of this json file

                }
            }
        }

        [Description("Test ladda ner fil")]
        [Command("dl")]
        public async Task dl(CommandContext ctx)
        {
            int result = dlFileToTempFolder("kattmåten");
            
            if(result != 0)
            {
                await ctx.Channel.SendMessageAsync("Fel vid nedladdning!").ConfigureAwait(false);
            } else
            {
                await ctx.Channel.SendMessageAsync("Fil nedladdad.").ConfigureAwait(false);
            }
        }


        // Register "pung" as a command with BaseCommandModule 
        [Description("Pläpomatens utvalda favoriter!")]
        [Command("pung")]
        public async Task pung(CommandContext ctx)
        {
            // Ctx gives us access to all different things we might want, channels, client, commands, commandsNext the thing were using with config 
            // To reply, we need the Channel

            
            string str = "Vet du varför tomten har så stor pung? \n Han kommer bara en gång om året!";
            // await before the async function makes it wait for it to complete until moving down the line

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
        public async Task addjoke(CommandContext ctx, Bot botten)
        {
            string filename = "./jokes.txt";

            try
            {
                using (StreamWriter sw = File.AppendText(filename))
                {
                    sw.WriteLine(ctx.Message.Content.Remove(0, 9));
                    
                }
            } catch (Exception)
            {
                await ctx.Channel.SendMessageAsync("Barka käpprätt åt helvete. Systemfel!").ConfigureAwait(false);

            }
        }




        [Command("joke")]
        public async Task joke(CommandContext ctx)
        {
            Random random = new Random();
            string outputStr = "herpaderpa";

            int nr = 0;
            int nrLines = 0;
            string filename = "jokes.txt";
            int randomLineNr = 0;


            string[] lines = File.ReadAllLines(filename);

            nrLines = lines.Length;
            randomLineNr = random.Next(nrLines);

            foreach (string line in lines)
            {
                if (nr++ == randomLineNr)
                {
                    outputStr = line;
                }
                Console.WriteLine(line);
            } 

            await ctx.Channel.SendMessageAsync(outputStr).ConfigureAwait(false);     

        }

    }
}
