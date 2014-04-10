using System;
using PlayGround.Generated;

namespace Reflector
{
    class Program
    {
        static void Main(string[] args)
        {
            for (int i = 0; i < 10;++i )
                Console.WriteLine(new MyClass().foo(i));
            Console.ReadKey();
        }
    }
}
