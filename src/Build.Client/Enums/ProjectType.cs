using System;
using System.Collections.Generic;
using System.Text;

namespace DAL.Enums
{
    public class ProjectType : Enumeration<ProjectType>
    {
        public static readonly ProjectType Shared = new ProjectType(1, "Shared");
        public static readonly ProjectType Droid = new ProjectType(2, "Droid");
        public static readonly ProjectType Ios = new ProjectType(3, "Ios");

        public ProjectType(int value, string displayName) : base(value, displayName) { }
    }
}
