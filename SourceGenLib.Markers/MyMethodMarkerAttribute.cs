using System;

namespace SourceGenLib.Markers
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class MyMethodMarkerAttribute : Attribute
    {
        public int SomeInterger { get; set; }
        public string SomeString { get; set; }

        public MyMethodMarkerAttribute(int someInterger = 0, string someString = "Bob was here!")
        {
            SomeInterger = someInterger;
            SomeString = someString;
        }
    }
}
