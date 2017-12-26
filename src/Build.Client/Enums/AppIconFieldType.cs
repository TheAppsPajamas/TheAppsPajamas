using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Enums
{
    public class AppIconFieldType : MasterMediaFieldType
    {
        public AppIconFieldType(int value
            , string displayName
            , ProjectType projectType
            , bool isMaster
            , bool isForClient
            , string osFileName
            , double width) 
            : base(value, displayName, projectType, FieldHolderType.AppIcon, isMaster, isForClient, osFileName, width)
        {
        }

        public class Shared : AppIconFieldType
        {
            public Shared(int value
            , string displayName
            , bool isMaster
            , bool isForClient
            , string osFileName
            , double width)
                : base(value, displayName, ProjectType.Shared, isMaster, isForClient, osFileName, width)
            {
            }
        }

        public class Droid : AppIconFieldType
        {
            public static string LauncherName = "ic_launcher.png";
            public Droid(int value
                , string displayName
                , bool isMaster
                , bool isForClient
                , string osFileName
                , double width)
                : base(value, displayName, ProjectType.Droid, isMaster, isForClient, osFileName, width)
            {
            }
        }

        public class Ios : AppIconFieldType
        {
            public Ios(int value
                , string displayName
                , bool isMaster
                , bool isForClient
                , string osFileName
                , double width)
                : base(value, displayName, ProjectType.Ios, isMaster, isForClient, osFileName, width)
            {
            }
        }
    }
}
