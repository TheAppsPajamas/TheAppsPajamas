using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Enums
{
    public class StringFieldType : FieldType
    {
        private StringFieldDisplayType _fieldDisplayType;
        public StringFieldDisplayType FieldDisplayType { get { return _fieldDisplayType; } }

        private bool _isShared;
        public bool IsShared { get { return _isShared;  } }
        public StringFieldType(int value
            , string displayName
            , ProjectType projectType
            , FieldHolderType fieldHolderType
            , bool isShared
            , bool isForClient
            , StringFieldDisplayType fieldDisplayType) : base(value, displayName, projectType, fieldHolderType, isForClient)
        {
            _isShared = isShared;
            _fieldDisplayType = fieldDisplayType;
        }

    }
}
