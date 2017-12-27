using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Enums
{
    public class StringFieldDisplayType : Enumeration<StringFieldDisplayType>
    {
        public static readonly StringFieldDisplayType Text = new StringFieldDisplayType(0, "text");
        public static readonly StringFieldDisplayType Number = new StringFieldDisplayType(0, "number");
        public static readonly StringFieldDisplayType Bool = new StringFieldDisplayType(0, "bool");
        
        protected StringFieldDisplayType(int value, string displayName) : base(value, displayName)
        {
        }
    }
}
