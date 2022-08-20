using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using kkbot.commands;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;

namespace kkbot
{
    public class Bot
    {

        

        public DiscordClient client { get; private set; }
        //         get:   betyder att vi kan nå den genom "bot.Client" 
        // private set:   men för att ändra den så måste det ske privat här i klassen

        public InteractivityExtension Interactivity { get; private set; }
        // Se hans video Part 3

        public CommandsNextExtension commands { get; private set; } // Handles command registration

        private OutgoingMessages outMessages;


        public async Task RunAsync()
        {

            Console.WriteLine(" *** BOOTING BOT *** ");
            outMessages = new OutgoingMessages(); 

            var json = string.Empty;
            string fileName = "./config.json";
            try
            {
                using (var fs = File.OpenRead(fileName))
                {
                    using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                    {
                        json = await sr.ReadToEndAsync().ConfigureAwait(false); // ?? added performance
                    }
                }
            } catch(Exception)
            {
                Console.WriteLine("ioerror, could not read file: " + fileName);
                return;
            }

            // Now deserialize and convert the json to our Struct
            var configJson = JsonConvert.DeserializeObject<ConfigJson>(json);
            
            


            // Load up the "config.json" and populate this config object
            var config = new DiscordConfiguration
            {
                Token = configJson.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true, 
            };

            //  MinimumLogLevel = 
             // LogLevel = LogLevel.Debug,      deleted 2022.08.20 due to changes to DSharp
            // UseInternalLogHandler = true

            
            client = new DiscordClient(config);

            var interactivity = Client​Extensions.GetInteractivity(client);

            ClientExtensions.UseInteractivity(client, new InteractivityConfiguration()
                                                  {
                                                    Timeout = TimeSpan.FromMinutes(2) 
                                                  });

 
            


            // Assign a function to run whenever the Client reaches the "Ready" state
            // client.Ready += onClientReady;

            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new string[] { configJson.Prefix },
                EnableMentionPrefix = true,
                EnableDms = false,
                DmHelp = true,
                IgnoreExtraArguments = true
            };

            
            commands = client.UseCommandsNext(commandsConfig);
            // Whenever a command happen, this takes care of it


            // Register all the commands from the classes
            commands.RegisterCommands<FunCommands>();

            // Now we want to connect the bot
            await client.ConnectAsync();

            await Task.Delay(-1);    // Make sure the bot doesn't quit on us ! Wait forever.
  
        }


        private Task onClientReady(ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }
    }
}
