using System;
using System.Collections.Generic;
using System.Text; 
 
namespace DAL.Enums
{
    public class FieldHolderType : Enumeration<FieldHolderType>
    {
        public static readonly FieldHolderType AppIcon = new FieldHolderType(1, "App icon");
        public static readonly FieldHolderType Packaging = new FieldHolderType(2, "Packaging");
        public static readonly FieldHolderType Splash = new FieldHolderType(3, "Splash");
        public static readonly FieldHolderType BuildConfigFieldSet = new FieldHolderType(4, "Build config field set");

        protected FieldHolderType(int value, string displayName) : base(value, displayName)
        {
        }
    }
}