using System;
using Build.Client.Constants;
using Build.Client.Models;
using Microsoft.Build.Framework;

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

        public static string ApplyImageSetExt(this string s)
        {
            if (s.Contains(".imageset"))
                return s;
            else
                return string.Concat(s, ".imageset");
        }

        public static string RemovePngExt(this string s)
        {
            if (s.Contains(".png"))
                return s.Replace(".png", String.Empty);
            else
                return s;
        }

        public static string ApplyFieldId(this string s, BaseFieldClientDto field)
        {
            if (s.Contains(field.Value))
                return s;
            return String.Concat(s, "_", field.Value);

        }

        public static string ApplyFieldId(this string s, ITaskItem field)
        {
            if (s.Contains(field.GetMetadata(MetadataType.MediaFileId)))
                return s;
            return String.Concat(s, "_", field.GetMetadata(MetadataType.MediaFileId));

        }

        public static string GetPathRelativeToProject(this string s, string projectDir)
        {
            if (String.IsNullOrEmpty(projectDir))
                throw new Exception("Must provide projectDir to get relative path");

            var o = s.Replace(projectDir, String.Empty);
            //remove \ or /
            o = o.Substring(1, o.Length - 1);
            return o;
        }
    }
}
