using System;
using System.Threading;
using System.Threading.Tasks;

namespace kkbot
{
    class Program
    {
        static void Main(string[] args)
        {
            var bot = new Bot();    // I love C# ! <3 <3  
            try {
              bot.RunAsync().GetAwaiter().GetResult(); 
              } catch(DSharpPlus.CommandsNext.Exceptions.DuplicateOverloadException exception)
              {
                Console.WriteLine("System error: Check all commands [Command(\"uniqueName\")] to ensure that uniqueName isnt used twice in funcommands!");
                Console.WriteLine(exception.Message);
              
              }
            
            // GetAwaiter means to run the bot async
            //GetResult ends the await so we can put code below, here in main
             

        }
    }
}
