using System;
using SourceGenLib;
namespace MyApp // Note: actual namespace depends on the project name.
{
    public partial class Program
    {
        static void Main(string[] args)
        {
            var bob = Colour.Green;
            Console.WriteLine(bob.ToStringFast());
            var ssssss = TestStuff.ijdskjfdksdf;
            var ss =ssssss.ToStringFast();
        }

        [EnumExtensions(extensionClassName: "sdasdas", 15)]
        public enum Colour
        {
            Red = 0,
            Green = 1,
            Blue = 2,
            Yellow = 4,
        }

        [EnumExtensions(Bob = 89, ExtensionClassName = "john")]
        public enum TestStuff
        {
            ijdskjfdksdf
        }

    }


}