using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Enums
{
    public class PackagingFieldType : StringFieldType
    {

        public PackagingFieldType(int value
            , string displayName
            , ProjectType projectType
            , bool isShared
            , bool isForClient
            , StringFieldDisplayType fieldDisplayType) : base(value, displayName, projectType, FieldHolderType.Packaging, isShared, isForClient, fieldDisplayType)
        {
        }

        public class Shared : PackagingFieldType
        {
            public Shared(int value
                , string displayName
                , bool isShared
                , bool isForClient
                , StringFieldDisplayType fieldDisplayType)
                : base(value, displayName, ProjectType.Shared, isShared, isForClient,fieldDisplayType)
            {
            }
        }

        public class Droid : PackagingFieldType
        {
            public Droid(int value
                , string displayName
                , bool isShared
                , bool isForClient
                , StringFieldDisplayType fieldDisplayType)
                : base(value, displayName, ProjectType.Droid, isShared, isForClient, fieldDisplayType)
            {
            }
        }

        public class Ios : PackagingFieldType
        {
            public Ios(int value
                , string displayName
                , bool isShared
                , bool isForClient
                , StringFieldDisplayType fieldDisplayType)
                : base(value, displayName, ProjectType.Ios, isShared, isForClient, fieldDisplayType)
            {
            }
        }

    }
}
