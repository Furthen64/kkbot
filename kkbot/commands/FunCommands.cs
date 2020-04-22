using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kkbot.commands
{
    public class FunCommands : BaseCommandModule
    {
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
