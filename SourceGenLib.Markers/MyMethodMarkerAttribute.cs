namespace SourceGenLib.Markers
{
    [System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = true)]
    public class MyMethodMarkerAttribute : System.Attribute
    {
        public MyMethodMarkerAttribute()
        {
        }
    }
}
