﻿using RimuTec.Faker.Extensions;
using RimuTec.Faker.Helper;
using YamlDotNet.Serialization;

namespace RimuTec.Faker {
   public static class PhoneNumber {
      static PhoneNumber() {
         const string yamlFileName = "RimuTec.Faker.locales.en.phone_number.yml";
         locale locale = YamlLoader.Read<locale>(yamlFileName);
         _faker = locale.en.faker;
      }

      public static string CellPhone() {
         var numberTemplate = _faker.CellPhone.Formats.Random();
         return numberTemplate.Numerify();
      }

      private static faker _faker;

#pragma warning disable IDE1006 // Naming Styles
      // Helper classes for reading the yaml file. Note that the class names are
      // intentionally lower case.

      internal class locale {
         public en en { get; set; }
      }

      internal class en {
         public faker faker { get; set; }
      }

      internal class faker {
         [YamlMember(Alias = "phone_number", ApplyNamingConventions = false)]
         public phone_number PhoneNumber { get; set; }

         [YamlMember(Alias = "cell_phone", ApplyNamingConventions = false)]
         public cell_phone CellPhone { get; set; }
      }

      internal class phone_number {
         [YamlMember(Alias = "formats", ApplyNamingConventions = false)]
         public string[] Formats { get; set; }
      }

      internal class cell_phone {
         [YamlMember(Alias = "formats", ApplyNamingConventions = false)]
         public string[] Formats { get; set; }
      }

#pragma warning restore IDE1006 // Naming Styles
   }
}
