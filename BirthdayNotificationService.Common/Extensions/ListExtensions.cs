using System;
using System.Collections.Generic;

namespace BirthdayNotificationService.Common.Extensions
{
    /// <summary>
    /// List class extensions
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// Splits list into smaller chunks
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <param name="chunkSize"></param>
        /// <returns></returns>
        public static List<List<T>> Split<T>(this List<T> items, int chunkSize = 100)
        {
            var list = new List<List<T>>();
            for (int i = 0; i < items.Count; i += chunkSize)
                list.Add(items.GetRange(i, Math.Min(chunkSize, items.Count - i)));
            return list;
        }
    }
}