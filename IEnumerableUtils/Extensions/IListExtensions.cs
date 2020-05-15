using System;
using System.Collections.Generic;
#if NETCORE
using System.Collections.Immutable;
#endif
using System.Linq;

namespace IEnumerableUtils.Tests
{
    /// <summary>
    /// Provides extension methods to be used on <see cref="IList{T}"/> items.
    /// </summary>
    public static class IListExtensions
    {
        /// <summary>
        /// Synchronizes every items of the collection with another one used as a reference.
        /// </summary>
        /// <typeparam name="T1">Type of the calling collection.</typeparam>
        /// <typeparam name="T2">Type of the reference collection.</typeparam>
        /// <param name="target">The collection that is synced against the reference one.</param>
        /// <param name="reference">A reference collection that will be used to reorder, add, or delete items in the target collection.</param>
        /// <param name="converter">If collection types are not interchangeable, provides a ways to transform a reference item into a target item.</param>
        /// <remarks>Your collection must be writeable, otherwise no operation is performed.</remarks>
        public static void SyncWith<T1, T2>(this IList<T1> target, IList<T2> reference, Func<T2, T1> converter = null)
        {
            // Check writability:
            lock (target)
            {
                if (target.IsReadOnly || reference == null)
                    return;

                // Init converter to default is not provided:
                if (converter == null)
                {
                    if (typeof(T1) == typeof(T2))
                        converter = (obj) => (T1)(object)obj;
                    else if (typeof(T1) == typeof(IConvertible) && typeof(T2) == typeof(IConvertible))  // for auto support to any convertible.
                        converter = (obj) => (T1)((IConvertible)(object)obj).ToType(obj.GetType(), null);
                    else
                        throw new NotSupportedException("A converter method must be provided for non-native dissimilar comparable types.");
                }

                lock (reference)
                {
                    #if NETCORE
                    var diff = (reference.Select(z => converter(z)).ToImmutableHashSet()).SymmetricExcept(target);
                    #else
                    var diff = new HashSet<T1>(reference.Select(z => converter(z)));
                    diff.SymmetricExceptWith(target);
                    #endif

                    var uRef = new List<T1>(); // stores items from target list that are not yet found in source list.
                    var uTarget = new List<(int?, T1)>();  // stores items from source list that are not yet found in target list.
                    int target_count = target.Count;
                    int reference_count = reference.Count;
                    var x = 0;
                    var y = 0;

                    while (x < target_count || y < reference_count)
                    {
                        // If target list is exhausted:
                        if (y >= reference_count)
                        {
                            target.RemoveAt(y);
                            x++;
                        }
                        // If source list is exhausted:
                        else if (x >= target_count)
                        {
                            target.Add(converter(reference[y]));
                            y++;
                        }
                        // If items are different:
                        else
                        {
                            var tval = target[x];
                            var rval = converter(reference[y]);
                            if (!tval.Equals(rval))
                            {
                                if (diff.Contains(tval))  // if not existing anymore
                                {
                                    target.Remove(tval);
                                    target_count--;
                                    diff.Remove(tval);
                                }
                                else if (diff.Contains(rval))  // if added
                                {
                                    target.Insert(y, rval);
                                    x++;
                                    y++;
                                    target_count++;
                                    diff.Remove(rval);
                                }
                                else  // if moved
                                {
                                    if (uRef.Exists(item => item.Equals(rval)))  // if already seen in target
                                    {
                                        target.Remove(rval);
                                        target.Insert(x, rval);
                                        uRef.Remove(rval);
                                        x++;
                                        y++;
                                    }
                                    else
                                        uTarget.Add((x, rval));

                                    var match = uTarget.FirstOrDefault(item => item.Item2.Equals(tval));
                                    if (match.Item2 != null && match.Item1 is int indexa && match.Item2.Equals(tval))  // if already seen in ref
                                    {
                                        target.Insert(indexa, match.Item2);
                                        uTarget.Remove(match);
                                        y++;
                                    }
                                    else
                                    {
                                        uRef.Add(tval);
                                        target.Remove(tval);
                                        target.Add(tval);
                                    }
                                }
                            }
                            else
                            {
                                x++; y++;
                            }
                        }
                    }
                }
            }
        }
    }
}
