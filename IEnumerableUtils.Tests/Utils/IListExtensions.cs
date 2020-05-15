using System.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EMA.TestUtils.Collections
{
    public static class IListExtensions
    { 
        public static IList<T> Shuffle<T>(this IList<T> list)
        {
            var shuffled = new List<T>(list);
            var rand = new Random((int)DateTime.Now.Ticks);
            var size = list.Count;
            if (size < 2) return shuffled;

            for (int i = 0; i < size; i++)
            {
                if (rand.Next() % (((rand.Next() + 1) % 2) + 1) == 1)
                {
                    var tomove = shuffled[rand.Next() % size];
                    shuffled.Remove(tomove);
                    if (rand.Next() % (((rand.Next() + 1) % 2) + 1) == 1)
                        shuffled.Add(tomove);
                    else shuffled.Insert(0, tomove);
                }
            }

            // Check we did well shuffle (might not be the case
            // due to randomness) and return result at first confirmation:
            for (int i = 0; i < list.Count; i++)
            {
                if (!list[i].Equals(shuffled[i]))
                    return shuffled;
            }

            // If no shuffled then do it manualy:
            var item = shuffled[0];
            shuffled.RemoveAt(0);
            shuffled.Add(item);

            return shuffled;
        }

        public static IList<T> DecreaseSize<T>(this IList<T> list)
        {
            var decreased = new List<T>(list);
            var size = decreased.Count;
            if (size == 0) return decreased;

            var rand = new Random((int)DateTime.Now.Ticks);
            var max = rand.Next() % size;
            max += max > 0 ? 0 : 1;  // ensure at least 1 item decrease.

            for (int k = 0; k < max ; k++)
                decreased.RemoveAt(rand.Next() % (size-- - 1));

            return decreased;
        }

        public static IList<T> IncreaseSize<T>(this IList<T> list, Func<T> initialize)
        {
            var increased = new List<T>(list);
            if (initialize == null) return increased;

            var rand = new Random((int)DateTime.Now.Ticks);
            var size = list.Count;
            var max = (rand.NextDouble() * 100) % (size + 1);
            var at_least_one = false;  // ensures that at least one item is added.
            var newvalue = default(T);

            for (int k = 0; k < max; k++)
            {
                newvalue = initialize();
                if (!increased.Contains(newvalue)) // ensure distinct values.
                {
                    if (size > 1)
                        increased.Insert(rand.Next() % (size++ - 1), newvalue);
                    else increased.Add(newvalue);
                    at_least_one = true;
                }
            }

            if (!at_least_one)
            {
                do newvalue = initialize();
                while (increased.Contains(newvalue));
                if (size > 1)
                    increased.Insert(rand.Next() % (size - 1), newvalue);
                else increased.Add(newvalue);
            }
            
            return increased; 
        }

        public static IList<int> ConvertToInt32<T>(this IList<T> list)
        {
            if (list == null) return null;
            if (!list.Any()) return new List<int>();

            var converted = new List<int>();

            if (typeof(T) == typeof(string))
            {
                int val = 0;
                foreach (var item in list.Cast<string>())
                    if (int.TryParse(item, NumberStyles.Integer, CultureInfo.InvariantCulture, out val))
                        converted.Add(val);
            }
            else if (typeof(T) == typeof(IConvertible))
            {
                foreach (var item in list.Cast<IConvertible>())
                    converted.Add(item.ToInt32(null));
            }
            else throw new NotSupportedException("Type to convert is not supported.");
            return converted; 
        }

        public static IList<string> ConvertToString<T>(this IList<T> list)
        {
            if (list == null) return null;
            if (!list.Any()) return new List<string>();

            var converted = new List<string>();
            foreach (var item in list)
                converted.Add(item.ToString());
            return converted; 
        }
    }
}
