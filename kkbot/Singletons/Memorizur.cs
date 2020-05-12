using System;
using System.Collections.Generic;
using System.Text;



/// <summary>
/// 2020-05-12 Fungerar bra
/// </summary>


namespace kkbot.Singletons
{


    // Example usage:
    //
    //  Memorizur memInst = Memorizur.Instance;
    //  memInst.recentlyUsedJokeLineNumbers.Add(3, DateTime.now);

    //                                     .TryGetValue(3);
    public sealed class Memorizur
    {
        // Constructor
        private Memorizur()
        {
            recentlyUsedJokeLineNumbers = new Dictionary<int, DateTime>();
        }

        public static Memorizur Instance { get { return Nested.instance; } }



        // Non Singleton stuff:
        public Dictionary <int, DateTime> recentlyUsedJokeLineNumbers;
        
        


        private class Nested
        {
            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }

            internal static readonly Memorizur instance = new Memorizur();
        }
    }

}
