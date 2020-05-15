using System;
using System.Collections.Generic;

namespace EMA.TestUtils.DataGeneration
{
    public static class RandomDataGenerator
    {
        private static Random rand = new Random((int)DateTime.Now.Ticks);

        public static List<string> MakeStringList(int size)
        {
            var toReturn = new List<string>();
            
            // Create a random reference array with no duplicates:
            for (int i = 0; i < size; i++)
            {
                var value = (rand.NextDouble() * 100 * size).ToString("F0");
                while (toReturn.Contains(value))
                    value = (rand.NextDouble() * 100 * size).ToString("F0");
                toReturn.Add(value);
            }
            return toReturn;
        }

        public static string MakeStringItem() => (rand.NextDouble() * 100).ToString("F0");

        public static List<int> MakeIntList(int size)
        {
                var toReturn = new List<int>();
            
            // Create a random reference array with no duplicates:
            for (int i = 0; i < size; i++)
            {
                var value = (int)(rand.NextDouble() * 100 * size);
                while (toReturn.Contains(value))
                    value = (int)(rand.NextDouble() * 100 * size);
                toReturn.Add(value);
            }
            return toReturn;
        }
        public static int MakeIntItem() => (int)rand.NextDouble() * 100;
    }
}
