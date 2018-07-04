﻿using System;
using System.Collections.Generic;

namespace RimuTec.Faker.Extensions {
   internal static class IntExtensions {
      public static IEnumerable<T> Times<T>(this int count, Func<int, T> func) {
         for (var i = 0; i < count; i++) {
            yield return func.Invoke(i);
         }
      }

      public static void Do(this int count, Action<int> action) {
         for(var i = 0; i < count; i++) {
            action.Invoke(i);
         }
      }
   }
}
