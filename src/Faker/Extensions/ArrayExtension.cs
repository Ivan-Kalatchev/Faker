﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace RimuTec.Faker.Extensions {
   internal static class ArrayExtensions {
      /// <summary>
      /// Select a random element from the array.
      /// </summary>
      public static string Random(this string[] array) {
         if (array.Count() == 0) {
            throw new InvalidOperationException($"Array {nameof(array)} must contain at least one item");
         }

         return array[RandomNumber.Next(0, array.Length)];
      }

      /// <summary>
      /// Select a random string from the Enumerable list.
      /// </summary>
      public static string Random(this IEnumerable<Func<string>> enumerable) {
         if (enumerable.Count() == 0) {
            throw new InvalidOperationException($"Enumerable {nameof(enumerable)} must contain at least one item");
         }

         return enumerable.ElementAt(RandomNumber.Next(0, enumerable.Count())).Invoke();
      }

      /// <summary>
      /// Select a random string from a list containing string objects.
      /// </summary>
      public static string Random(this List<string> list) {
         if (list.Count() == 0) {
            throw new InvalidOperationException($"List<string> {nameof(list)} must contain at least one item");
         }
         return list.ElementAt(RandomNumber.Next(0, list.Count()));
      }

      /// <summary>
      /// Select a random string array from the Enumerable list.
      /// </summary>
      public static string[] Random(this IEnumerable<Func<string[]>> enumerable) {
         if (enumerable.Count() == 0) {
            throw new InvalidOperationException($"Enumerable {nameof(enumerable)} must contain at least one item");
         }

         return enumerable.ElementAt(RandomNumber.Next(0, enumerable.Count())).Invoke();
      }
   }
}
