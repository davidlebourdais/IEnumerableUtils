using System;
using Xunit;
using System.Collections.Generic;
using System.Linq;
using EMA.TestUtils.Collections;
using EMA.TestUtils.DataGeneration;

namespace IEnumerableUtils.Tests
{
    public class ListSyncWithTest
    {
        [Theory]
        [ClassData(typeof(ShuffledSameSizeArrays<string, string>))]
        [ClassData(typeof(ShuffledSameSizeArrays<int, int>))]
        #pragma warning disable xUnit1026 
        public void SyncsWithSameTypeSameSizeShuffled<T>(IList<T> a, IList<T> b, Func<T, T> converter = null)
        {
            if (a.Count > 0 && b.Count > 0)
                Assert.NotEqual(b, a);
            a.SyncWith(b);
            Assert.Equal(b, a);
        }
        #pragma warning restore xUnit1026

        [Theory]
        [ClassData(typeof(ShuffledSmallerSizeArrays<string, string>))]
        [ClassData(typeof(ShuffledSmallerSizeArrays<int, int>))]
        #pragma warning disable xUnit1026 
        public void SyncsWithSameTypeSmallerSizeShuffled<T>(IList<T> a, IList<T> b, Func<T, T> converter = null)
        {
            if (a.Count > 0 && b.Count > 0)
                Assert.NotEqual(b, a);
            a.SyncWith(b);
            Assert.Equal(b, a);            
        }
        #pragma warning restore xUnit1026

        [Theory]
        [ClassData(typeof(ShuffledLargerSizeArrays<string, string>))]
        [ClassData(typeof(ShuffledLargerSizeArrays<int, int>))]
        #pragma warning disable xUnit1026 
        public void SyncsWithSameTypeLargerSizeShuffled<T>(IList<T> a, IList<T> b, Func<T, T> converter = null)
        {
            if (a.Count > 0 && b.Count > 0)
                Assert.NotEqual(b, a);
            a.SyncWith(b);
            Assert.Equal(b, a);
        }
        #pragma warning restore xUnit1026

        [Theory]
        [ClassData(typeof(ShuffledSameSizeArrays<string, int>))]
        [ClassData(typeof(ShuffledSameSizeArrays<int, string>))]
        public void SyncsWithDifferentTypeSameSizeShuffled<T1, T2>(IList<T1> a, IList<T2> b, Func<T2, T1> converter)
        {
            if (a.Count > 0 && b.Count > 0)
                Assert.NotEqual(b.Select(x => converter(x)), a);
            a.SyncWith(b, converter);
            Assert.Equal(b.Select(x => converter(x)), a);
        }
        
        [Theory]
        [ClassData(typeof(ShuffledSmallerSizeArrays<string, int>))]
        [ClassData(typeof(ShuffledSmallerSizeArrays<int, string>))]
        public void SyncsWithDifferentTypeSmallerSizeShuffled<T1, T2>(IList<T1> a, IList<T2> b, Func<T2, T1> converter)
        {
            if (a.Count > 0 && b.Count > 0)
                Assert.NotEqual(b.Select(x => converter(x)), a);
            a.SyncWith(b, converter);
            Assert.Equal(b.Select(x => converter(x)), a);
        }

        [Theory]
        [ClassData(typeof(ShuffledLargerSizeArrays<string, int>))]
        [ClassData(typeof(ShuffledLargerSizeArrays<int, string>))]
        public void SyncsWithDifferentTypeLargerSizeShuffled<T1, T2>(IList<T1> a, IList<T2> b, Func<T2, T1> converter)
        {
            if (a.Count > 0 && b.Count > 0)
                Assert.NotEqual(b.Select(x => converter(x)), a);
            a.SyncWith(b, converter);
            Assert.Equal(b.Select(x => converter(x)), a);
        }
    }

        #region Test data
        /// Base class for test data.
        public abstract class DataArrays<T1,T2> : TheoryData<List<T1>, List<T2>, Func<T2,T1>>
        {
            public const int array_size = 10;  // increase to increase tested list sizes
            public const int data_set_size = 10;  // increase to increase test count
            protected Random rand = new Random((int)DateTime.Now.Ticks);
            public static Func<T2, T1> GenericTypeConverter { get; } = GenerateConverter();

            public virtual List<T1> MakeRandomReferenceList(int size)
            {
                if (size == 0) return new List<T1>();
                if (typeof(T1) == typeof(string))
                    return RandomDataGenerator.MakeStringList(array_size).Cast<T1>().ToList();
                else if (typeof(T1) == typeof(int))
                    return RandomDataGenerator.MakeIntList(array_size).Cast<T1>().ToList();
                else throw new NotSupportedException("Data type is not supported, own implementation required");
            }
            public virtual List<T2> MakeSyncedCopy(List<T1> reference)
            {
                if (reference == null) return null;
                if (typeof(T1) == typeof(T2)) return reference.Cast<T2>().ToList();
                else if (typeof(T2) == typeof(string)) return reference.ConvertToString().Cast<T2>().ToList();
                else if (typeof(T2) == typeof(int)) return reference.ConvertToInt32().Cast<T2>().ToList(); 
                else throw new NotSupportedException("Data type is not supported, own implementation required");
            }

            public void SetShuffledSameSizeArrays()
            {      
                // Random shuffled arrays to sync with:
                for (int i = 0; i < data_set_size; i++)
                {
                    var reference = MakeRandomReferenceList(array_size);
                    var toSyncWith = MakeSyncedCopy(reference);
                    Add(reference, toSyncWith.Shuffle().ToList(), GenericTypeConverter);
                }

                // Add empty list special case:
                Add(new List<T1>(), new List<T2>(), GenericTypeConverter);
            }

            public void SetShuffledSmallerSizeArrays()
            {
                // Random shuffled decreased arrays to sync with:
                for (int i = 0; i < data_set_size; i++)
                {
                    var reference = MakeRandomReferenceList(array_size);
                    var toSyncWith = MakeSyncedCopy(reference);
                    Add(reference, toSyncWith.Shuffle().DecreaseSize().ToList(), GenericTypeConverter);
                }

                // Add empty list special case:
                Add(MakeRandomReferenceList(array_size), new List<T2>(), GenericTypeConverter);
            }

            public  void SetShuffledLargerSizeArrays()
            {
                if (typeof(T2) == typeof(string)) SetShuffledLargerSizeArrays(() => (T2)(object)RandomDataGenerator.MakeStringItem());
                else if (typeof(T2) == typeof(int)) SetShuffledLargerSizeArrays(() => (T2)(object)RandomDataGenerator.MakeIntItem());
                else throw new NotSupportedException("Data type is not supported, own implementation required");
            }

            public void SetShuffledLargerSizeArrays(Func<T2> initialize)
            {
                // Random shuffled decreased arrays to sync with:
                for (int i = 0; i < data_set_size; i++)
                {
                    var reference = MakeRandomReferenceList(array_size);
                    var toSyncWith = MakeSyncedCopy(reference);
                    Add(reference, 
                        toSyncWith.Shuffle()
                                 .IncreaseSize(initialize)
                                 .ToList(),
                        GenericTypeConverter);
                }

                // Add empty list special case:
                Add(MakeRandomReferenceList(array_size), new List<T2>(), GenericTypeConverter);
            }

            private static Func<T2, T1> GenerateConverter()
            {
                if (typeof(T1) == typeof(T2))
                    return (x) => (T1)(object)x;
                else if (typeof(T1) == typeof(IConvertible) && typeof(T2) == typeof(IConvertible))
                    return (x) => (T1)((IConvertible)(object)x).ToType(x.GetType(), null);
                else if (typeof(T1) == typeof(string))
                    return (x) => (T1)(object)x.ToString();
                else if (typeof(T1) == typeof(int) && typeof(T2) == typeof(string))
                    return x => 
                    {
                        if (int.TryParse((string)(object)x, out int result))
                            return (T1)(object)result;
                        else return default;
                    };
                else throw new NotSupportedException("Data type is not supported, own implementation required");
            }
        }

        #region Test data classes
        public class ShuffledSameSizeArrays<T1, T2> : DataArrays<T1, T2>
        {
            public ShuffledSameSizeArrays() => base.SetShuffledSameSizeArrays();
        }

        public class ShuffledSmallerSizeArrays<T1, T2> : DataArrays<T1, T2>
        {
            public ShuffledSmallerSizeArrays() => base.SetShuffledSmallerSizeArrays();
        }

        public class ShuffledLargerSizeArrays<T1, T2> : DataArrays<T1, T2>
        {
            public ShuffledLargerSizeArrays() => base.SetShuffledLargerSizeArrays();
        }
        #endregion

        #endregion
}
