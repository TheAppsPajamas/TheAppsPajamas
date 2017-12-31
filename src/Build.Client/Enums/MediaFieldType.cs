using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Enums
{
    public class MediaFieldType : FieldType
    {
        private string _osFileName;
        public virtual string OsFileName { get { return _osFileName; } }


        private int _width;
        public virtual int Width { get { return _width; } }

        private int _height;
        public virtual int Height { get { return _height; } }

        public MediaFieldType(int value
            , string displayName
            , ProjectType projectType
            , FieldHolderType fieldHolderType
            , bool isForClient
            , string osFileName
            , int width
            , int height = 0) : base(value, displayName, projectType, fieldHolderType, isForClient)
        {
            _osFileName = osFileName;
            _width = width;
            if (height == 0)
            {
                _height = width;
            }
            else
            {
                _height = height;
            }
        }

    }
}
