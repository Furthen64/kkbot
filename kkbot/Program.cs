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

            bot.RunAsync().GetAwaiter().GetResult(); 
            
            // GetAwaiter means to run the bot async
            //GetResult ends the await so we can put code below, here in main
             

        }
    }
}
