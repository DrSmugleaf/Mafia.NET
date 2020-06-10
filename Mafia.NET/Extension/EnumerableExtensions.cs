﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Mafia.NET.Extension
{
    public static class EnumerableExtensions
    {
        public static T Random<T>(this IEnumerable<T> source, Random random)
        {
            var sourceArray = source.ToArray();
            return sourceArray.ElementAt(random.Next(sourceArray.Length));
        }
    }
}