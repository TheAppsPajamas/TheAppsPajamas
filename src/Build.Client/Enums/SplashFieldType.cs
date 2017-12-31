using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Enums
{
    public class SplashFieldType : MasterMediaFieldType
    {
        public SplashFieldType(int value
            , string displayName
            , ProjectType projectType
            , bool isMaster
            , bool isForClient
            , string osFileName
            , int width
            , int height) 
            : base(value, displayName, projectType, FieldHolderType.AppIcon, isMaster, isForClient, osFileName, width, height)
        {
        }

        public class Shared : SplashFieldType
        {
            public Shared(int value
                , string displayName
                , bool isMaster
                , bool isForClient
                , string osFileName
                , int width
                , int height)
                : base(value, displayName, ProjectType.Shared, isMaster, isForClient, osFileName, width, height)
            {
            }
        }

        public class Droid : SplashFieldType
        {
            public static string SplashName = "ic_splash.png";
            public Droid(int value
                , string displayName
                , bool isMaster
                , bool isForClient
                , string osFileName
                , int width
                , int height)
                : base(value, displayName, ProjectType.Droid, isMaster, isForClient, osFileName, width, height)
            {
            }
        }

        public class Ios : SplashFieldType
        {
            public Ios(int value
                , string displayName
                , bool isMaster
                , bool isForClient
                , string osFileName
                , int width
                , int height)
                : base(value, displayName, ProjectType.Ios, isMaster, isForClient, osFileName, width, height)
            {
            }
        }
    }
}
