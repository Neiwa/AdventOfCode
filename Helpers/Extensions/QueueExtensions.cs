﻿namespace Helpers.Extensions
{
    public static class QueueExtensions
    {
        public static void EnqueueRange<T>(this Queue<T> queue, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                queue.Enqueue(item);
            }
        }
        public static void EnqueueRange<T>(this Queue<T> queue, params T[] items)
        {
            foreach (var item in items)
            {
                queue.Enqueue(item);
            }
        }
    }

    public static class HashSetExtensions
    {
        public static void AddRange<T>(this HashSet<T> set, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                set.Add(item);
            }
        }
    }
}
