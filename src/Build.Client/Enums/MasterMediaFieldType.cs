using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Enums
{
    public class MasterMediaFieldType : MediaFieldType
    {
        private bool _isMaster;
        public virtual bool IsMaster { get { return _isMaster; } }
        
        public MasterMediaFieldType(int value
            , string displayName
            , ProjectType projectType
            , FieldHolderType fieldHolderType
            , bool isMaster
            , bool isForClient
            , string osFileName
            , int width
            , int height = 0) : base(value, displayName, projectType, fieldHolderType, isForClient, osFileName, width, height = 0)
        {
            _isMaster = isMaster;
        }

    }
}
