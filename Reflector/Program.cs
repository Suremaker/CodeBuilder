using System;
using PlayGround.Generated;

namespace Reflector
{
    class Program
    {
        private static void Main(string[] args)
        {
            var myClass = new MyClass();
            for (int i = 0; i < 10; ++i)
                Console.WriteLine(myClass.foo(i));

            Console.WriteLine(myClass.bar(null));
            Console.WriteLine(myClass.bar("abc"));

            myClass.tryFinally("abc");
            try { myClass.tryFinally(null); }
            catch (Exception e) { Console.WriteLine(e);}

            myClass.localVar();
            Console.ReadKey();
        }
    }
}
