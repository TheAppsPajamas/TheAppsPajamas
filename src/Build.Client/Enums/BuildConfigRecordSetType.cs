using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Enums
{
    public class BuildConfigRecordSetType : StringFieldType
    {
        public BuildConfigRecordSetType(int value
            , string displayName
            , ProjectType projectType
            , bool isForClient
            , StringFieldDisplayType fieldDisplayType) : base(value, displayName, projectType, FieldHolderType.Packaging, true, isForClient, fieldDisplayType)
        {
        }

        public class Shared : BuildConfigRecordSetType
        {
            public Shared(int value
                , string displayName
                , bool isForClient
                , StringFieldDisplayType fieldDisplayType)
                : base(value, displayName, ProjectType.Shared, isForClient, fieldDisplayType)
            {
            }
        }
    }
}
