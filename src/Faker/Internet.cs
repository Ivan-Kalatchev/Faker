﻿using RimuTec.Faker.Extensions;
using RimuTec.Faker.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using YamlDotNet.Serialization;

namespace RimuTec.Faker {
   /// <summary>
   /// Generators for internet related data, e.g. email addresses, domain names, IP addresses, MAC addresses,
   /// passwords, etc.
   /// </summary>
   public static class Internet {
      static Internet() {
         const string yamlFileName = "RimuTec.Faker.locales.en.internet.yml";
         locale locale = YamlLoader.Read<locale>(yamlFileName);
         _internet = locale.en.faker.internet;
         _freeEmail = _internet.FreeEmail;
      }

      /// <summary>
      /// Gets a random domain name. Example: "effertz.info"
      /// </summary>
      /// <returns></returns>
      public static string DomainName() {
         return $"{DomainWord()}.{DomainSuffix()}";
      }

      /// <summary>
      /// Gets a random but valid domain suffix. Example: "info"
      /// </summary>
      /// <returns></returns>
      public static string DomainSuffix() {
         return _internet.DomainSuffix.Sample();
      }

      /// <summary>
      /// Generates a word that can be used in a domain name. Example: "haleyziemann"
      /// </summary>
      /// <returns></returns>
      public static string DomainWord() {
         return Company.Name().Split(' ').First().Prepare();
      }

      /// <summary>
      /// Generates an email address, optionally with a specific and/or separator. Example: "eliza@mann.net"
      /// </summary>
      /// <param name="name">Name to use for the email address. Default value is null in which case a random name will be used.</param>
      /// <param name="separators">String with separators. Default is "._". Separator is used between first and last name.</param>
      /// <returns></returns>
      public static string Email(string name = null, string separators = null) {
         return string.Join("@", UserName(name, separators), DomainName());
      }

      /// <summary>
      /// Generates an email address at a free email provider, optionally with a specific name. 
      /// Examples: FreeEmail() => "freddy@gmail.com", FreeEmail("Nancy") => "nancy@yahoo.com"
      /// </summary>
      /// <param name="name">Name to use. Default is null in which case a random name will be used.</param>
      /// <returns></returns>
      public static string FreeEmail(string name = null) {
         if (string.IsNullOrEmpty(name)) {
            name = null;
         }
         return string.Join("@", UserName(name), _internet.FreeEmail.Sample());
      }

      /// <summary>
      /// Generates an IP V4 address. Example: "24.29.18.175"
      /// </summary>
      /// <remarks>The method may return IP V4 addresses that may be reserved, private or public. If you need 
      /// a private address outside of any reserved range use <see cref="PrivateIPv4Address"/>. If you 
      /// need a public address outside of any reserved range use <see cref="PublicIPv4Address"/>.</remarks>
      /// <returns></returns>
      public static string IPv4Address() {
         var ary = IntHelper.Repeat(0, 255, x => x).ToArray();
         return $"{ary.Sample()}.{ary.Sample()}.{ary.Sample()}.{ary.Sample()}";
      }

      /// <summary>
      /// Generates a password. Example: "*%NkOnJsH4"
      /// </summary>
      /// <param name="minLength">Minimum length of the password. Default value is 8.</param>
      /// <param name="maxLength">Maximum length of the password. Default value is 15.</param>
      /// <param name="mixCase">Flag whether to use upper case characters or not. Default value is true (i.e. use upper case).</param>
      /// <param name="specialChars">Flag whether to use special characters (!@#$%^&*). Default value is true (i.e. use special characters).</param>
      /// <exception cref="ArgumentOutOfRangeException">If minLength or maxLength are outside of the valid ranges.</exception>
      /// <returns></returns>
      public static string Password(int minLength = 8, int maxLength = 15, bool mixCase = true, bool specialChars = true) {
         if(minLength < 1) {
            throw new ArgumentOutOfRangeException(nameof(minLength), "Must be greater than zero.");
         }
         if (maxLength < 1) {
            throw new ArgumentOutOfRangeException(nameof(maxLength), "Must be greater than zero.");
         }
         if(maxLength < minLength) {
            throw new ArgumentOutOfRangeException(nameof(maxLength), $"Must be equal to or greater than {nameof(minLength)}.");
         }
         var temp = Lorem.Characters(minLength);
         var diffLength = maxLength - minLength;
         if(diffLength > 0) {
            var diffRand = RandomNumber.Next(diffLength + 1);
            temp += Lorem.Characters(diffRand);
         }
         if(mixCase) {
            var sb = new StringBuilder(temp);
            for(var i = 0; i < temp.Length; i++) {
               if(i%2 == 0) {
                  sb[i] = char.ToUpper(sb[i]);
               }
            }
            temp = sb.ToString();
         }
         if(specialChars) {
            var chars = "!@#$%^&*";
            var sb = new StringBuilder(temp);
            RandomNumber.Next(1, minLength).TimesDo(i => sb[i] = chars.Sample()[0]);
            temp = sb.ToString();
         }
         return temp;
      }

      /// <summary>
      /// Generates a private IP V4 address. Example: "10.0.0.1"
      /// </summary>
      /// <returns></returns>
      public static string PrivateIPv4Address() {
         string addr = null;
         do {
            addr = IPv4Address();
         } while (!IsInPrivateNet(addr));
         return addr;
      }

      /// <summary>
      /// Generates a public IP V4 address, guaranteed not to be in the IP range from <see cref="PrivateIPv4Address"/>. 
      /// Example: "24.29.18.175"
      /// </summary>
      /// <returns></returns>
      public static string PublicIPv4Address() {
         string addr = null;
         do {
            addr = IPv4Address();
         } while (IsInPrivateNet(addr) || IsInReservedNet(addr));
         return addr;
      }

      /// <summary>
      /// Generates an email address that is safe, i.e. @example.org / @example.com / @example.net, optionally with
      /// a specific name. Examples: SafeEmail() => "christelle@example.org", SafeEmail("Nancy") => "nancy@example.net"
      /// </summary>
      /// <param name="name">Name to use for the email address. Default i null in which case a random first name will be used.</param>
      /// <returns></returns>
      public static string SafeEmail(string name = null) {
         if (string.IsNullOrEmpty(name)) {
            name = null;
         }
         var topLevelDomains = new string[] { "org", "com", "net" };
         return string.Join("@", UserName(name), $"example.{topLevelDomains.Sample()}");
      }

      /// <summary>
      /// Generates a user name. Examples: "alexie", "johnson-nancy"
      /// </summary>
      /// <param name="name">Name to be used instead of a random one.</param>
      /// <param name="separators">Separators to consider. May not appear in result. Default are '.' and '_'.</param>
      /// <returns></returns>
      public static string UserName(string name = null, string separators = null) {
         string result;
         if (separators == null) {
            separators = "._"; // if space is sampled, it will be removed by Prepare()
         }
         var separator = separators.Sample();
         if (name != null) {
            IEnumerable<string> parts = name.Scan(@"\w+").Shuffle();
            result = string.Join(separator, parts).ToLower();
         }
         else {
            var candidates = new List<string> {
               Name.FirstName().Prepare(),
               $"{Name.FirstName().Prepare()}{separator}{Name.LastName().Prepare()}"
            };
            result = candidates.Sample();
         }
         return result;
      }

      /// <summary>
      /// Generates a user name with minimum length. Example: UserName(8) may return "bernhard_schiller"
      /// </summary>
      /// <param name="minLength">Minimal length of the user name.</param>
      /// <returns></returns>
      public static string UserName(int minLength) {
         if(minLength > 1000000) {
            throw new ArgumentOutOfRangeException(nameof(minLength), "Must be equal to or less than 10^6.");
         }
         if(minLength <= 0) {
            throw new ArgumentOutOfRangeException(nameof(minLength), "Must be greater than zero.");
         }
         var tries = 0;
         string result = null;
         do {
            result = UserName(null, "._");
            tries++;
         } while (result?.Length < minLength && tries < 7);
         if(minLength > 0) {
            result = string.Join("", (minLength / result.Length + 1).Times(x => result));
         }
         return result;
      }

      /// <summary>
      /// Generates a user name with minimum and maximum length. Example: UserName(5, 8) may return ""melany""
      /// </summary>
      /// <param name="minLength">Minimum length of the user name.</param>
      /// <param name="maxLength">Maximum length of the user name.</param>
      /// <returns></returns>
      /// <exception cref="ArgumentOutOfRangeException">If minLength or maxLength are outside of accepted ranges.</exception>
      public static string UserName(int minLength, int maxLength = int.MaxValue) {
         if(maxLength < minLength) {
            throw new ArgumentOutOfRangeException($"{nameof(maxLength)} must be equal to or greater than {nameof(minLength)}.", (Exception) null);
         }
         var tries = 0;
         string result = null;
         do {
            result = UserName(minLength);
            tries++;
         } while (result.Length > maxLength && tries < 7);
         return result.Substring(0, Math.Min(maxLength, result.Length));
      }

      internal static bool IsInPrivateNet(string addr) {
         return _privateNetRegex.Any(net => Regex.Match(addr, net, RegexOptions.Compiled).Success);
      }

      internal static bool IsInReservedNet(string addr) {
         return _reserved_nets_regex.Any(net => Regex.Match(addr, net, RegexOptions.Compiled).Success);
      }

      internal static string[] _freeEmail;

      private static string[] _privateNetRegex = {
          @"^10\.",                                        // 10.0.0.0    - 10.255.255.255
          @"^100\.(6[4-9]|[7-9]\d|1[0-1]\d|12[0-7])\.",   // 100.64.0.0  - 100.127.255.255
          @"^127\.",                                      // 127.0.0.0   - 127.255.255.255
          @"^169\.254\.",                                 // 169.254.0.0 - 169.254.255.255
          @"^172\.(1[6-9]|2\d|3[0-1])\.",                 // 172.16.0.0  - 172.31.255.255
          @"^192\.0\.0\.",                                // 192.0.0.0   - 192.0.0.255
          @"^192\.168\.",                                 // 192.168.0.0 - 192.168.255.255
          @"^198\.(1[8-9])\."                             // 198.18.0.0  - 198.19.255.255
      };

      private static string[] _reserved_nets_regex = {
          @"^0\.",                 // 0.0.0.0      - 0.255.255.255
          @"^192\.0\.2\.",         // 192.0.2.0    - 192.0.2.255
          @"^192\.88\.99\.",       // 192.88.99.0  - 192.88.99.255
          @"^198\.51\.100\.",      // 198.51.100.0 - 198.51.100.255
          @"^203\.0\.113\.",       // 203.0.113.0  - 203.0.113.255
          @"^(22[4-9]|23\d)\.",    // 224.0.0.0    - 239.255.255.255
          @"^(24\d|25[0-5])\."     // 240.0.0.0    - 255.255.255.254  and  255.255.255.255
      };

      private static internet _internet;

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
         public internet internet { get; set; }
      }

      internal class internet {
         [YamlMember(Alias = "free_email", ApplyNamingConventions = false)]
         public string[] FreeEmail { get; set; }

         [YamlMember(Alias = "domain_suffix", ApplyNamingConventions = false)]
         public string[] DomainSuffix { get; set; }

         [YamlMember(Alias = "user_agent", ApplyNamingConventions = false)]
         public Dictionary<string,List<string>> UserAgent { get; set; }
      }
#pragma warning restore IDE1006 // Naming Styles
   }
}
