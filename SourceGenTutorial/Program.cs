using SourceGenLib.Markers;

namespace MyApp // Note: actual namespace depends on the project name.
{
    public partial class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("test");
        }

        [MyMethodMarker]
        public void Eric(int integer, string str)
        {
            Console.WriteLine(integer + ": " + str);
        }
    }
}