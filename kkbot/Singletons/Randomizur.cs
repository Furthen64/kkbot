using System;
using System.Collections.Generic;
using System.Text;



namespace kkbot.Singletons
{


    // Example usage:
    //
    //  Randomizur randInst = Randomizur.Instance;
    // int randomNr = randInst.getInt(0,200);
    public sealed class Randomizur
    {
        // Constructor
        private Randomizur()
        {
            randomObj = new Random();
        }

        public static Randomizur Instance { get { return Nested.instance; } }




        // Non Singleton stuff:
        private Random randomObj;

        // (--)
        public int getInt(int fromInt, int maxInt)
        {
            return fromInt + randomObj.Next(maxInt);
        }



        private class Nested
        {
            // Explicit static constructor to tell C# compiler
            // not to mark type as beforefieldinit
            static Nested()
            {
            }

            internal static readonly Randomizur instance = new Randomizur();
        }
    }

}
