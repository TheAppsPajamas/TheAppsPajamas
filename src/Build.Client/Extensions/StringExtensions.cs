using System;
using Build.Client.Models;

namespace Build.Client.Extensions
{
    public static class StringExtensions
    {
        public static string ApplyXcAssetsExt(this string s){
            if (s.Contains(".xcassets"))
                return s;
            else
                return string.Concat(s, ".xcassets");
        }

        public static string ApplyPngExt(this string s){
            if (s.Contains(".png"))
                return s;
            else
                return string.Concat(s, ".png");
        }

        public static string ApplyAppiconsetExt(this string s)
        {
            if (s.Contains(".appiconset"))
                return s;
            else
                return string.Concat(s, ".appiconset");
        }

        public static string ApplyLaunchimageExt(this string s)
        {
            if (s.Contains(".launchimage"))
                return s;
            else
                return string.Concat(s, ".launchimage");
        }

        public static string RemovePngExt(this string s)
        {
            if (s.Contains(".png"))
                return s.Replace(".png", String.Empty);
            else
                return s;
        }

        public static string ApplyFieldId(this string s, MediaFieldClientDto field)
        {
            if (s.Contains("_"))
                return s;
            else
                return String.Concat(s, "_", field.Value);

        }
    }
}
