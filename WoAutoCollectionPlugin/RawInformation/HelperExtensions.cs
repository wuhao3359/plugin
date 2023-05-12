﻿using System.Linq;

namespace WoAutoCollectionPlugin.RawInformation
{
    internal static class HelperExtensions
    {
        public static string GetNumbers(this string input)
        {
            return new string(input.Where(c => char.IsDigit(c)).ToArray());
        }

        public static string GetLast(this string source, int tail_length)
        {
            if (tail_length >= source.Length)
                return source;
            return source.Substring(source.Length - tail_length);
        }
    }
}
