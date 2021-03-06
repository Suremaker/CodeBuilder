﻿using System;
using PlayGround.Generated;
using Playground;

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
            try { myClass.tryCatchFinally(null); }
            catch (Exception e) { Console.WriteLine(e); }

            myClass.localVar();
            Console.WriteLine(myClass.negNot(6));
            myClass.loop();
            Console.WriteLine(myClass.intTS());
            var s = new MyStruct {MyField = "321"};
            Console.WriteLine(myClass.loadStructField(s));
            Console.WriteLine(myClass.saveStructField(s,"aaa222").MyField);
            Console.ReadKey();
            Console.ReadKey();
        }
    }

}
