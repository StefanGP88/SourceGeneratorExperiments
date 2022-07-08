using System;

namespace SourceGenLib.Markers
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class MyClassMarkerAttribute : Attribute
    {
    }
}
