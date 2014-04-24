using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using CodeBuilder;

namespace Playground
{
    class Program
    {
        static void Main(string[] args)
        {
            var asmName = "PlayGround.Generated";
            var fileName = "PlayGround.Generated.dll";
            var asmBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(asmName), AssemblyBuilderAccess.RunAndSave);
            var modBuilder = asmBuilder.DefineDynamicModule(asmName, fileName, true);
            var typeBuilder = modBuilder.DefineType("PlayGround.Generated.MyClass", TypeAttributes.Class | TypeAttributes.Public);
            var fieldBuilder = typeBuilder.DefineField("field", typeof(string), FieldAttributes.InitOnly | FieldAttributes.Static | FieldAttributes.Public);

            var cb = typeBuilder.DefineTypeInitializer();
            var builder = new MethodBodyBuilder(cb);
            builder.AddStatements(
                Expr.WriteField(fieldBuilder, Expr.Constant("abc")),
                Expr.Call(typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expr.Constant("Hello my friend!")),
                Expr.Call(typeof(Console).GetMethod("WriteLine", new[] { typeof(string), typeof(object) }), Expr.Constant("What a {0} day!"), Expr.Constant("beautiful")),
                Expr.Call(typeof(Console).GetMethod("ReadKey", new Type[0])));
            Console.WriteLine(builder.ToString());
            builder.Compile();

            var mb = typeBuilder.DefineMethod("foo", MethodAttributes.Public);
            mb.SetReturnType(typeof(string));
            mb.SetParameters(typeof(int));
            mb.DefineParameter(1, ParameterAttributes.None, "val");
            builder = new MethodBodyBuilder(mb, typeof(int));
            builder.AddStatements(
                Expr.IfThen(Expr.Parameter(1, typeof(int)), Expr.Call(typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expr.Constant("aha!"))),
                Expr.Return(Expr.Call(
                    Expr.Call(
                        Expr.New(typeof(StringBuilder)),
                        typeof(StringBuilder).GetMethod("AppendFormat", new[] { typeof(string), typeof(object), typeof(object) }),
                        Expr.Constant("{0}_{1}"),
                        Expr.ReadField(fieldBuilder),
                        Expr.Convert(Expr.Parameter(1, typeof(int)), typeof(object))),
                    typeof(StringBuilder).GetMethod("ToString", new Type[0]))));
            Console.WriteLine(builder.ToString());

            builder.Compile();

            mb = typeBuilder.DefineMethod("bar", MethodAttributes.Public);
            mb.SetReturnType(typeof(string));
            mb.SetParameters(typeof(string));
            mb.DefineParameter(1, ParameterAttributes.None, "text");

            builder = new MethodBodyBuilder(mb, typeof(string));

            builder.AddStatements(
                Expr.Return(
                Expr.Call(typeof(string).GetMethod("Format", new[] { typeof(string), typeof(object), typeof(object) }), Expr.Constant("Result: {0}-{1}"),
                    Expr.IfThenElse(
                        Expr.Parameter(1, typeof(string)),
                        Expr.Call(typeof(string).GetMethod("Format", new[] { typeof(string), typeof(object) }), Expr.Constant("Date: {0}"), Expr.Convert(Expr.Parameter(1, typeof(string)), typeof(object))),
                        Expr.Constant("Undefined!")),
                Expr.Convert(Expr.Convert(Expr.Convert(Expr.Constant(16), typeof(object)), typeof(int)), typeof(object)))
                ));

            Console.WriteLine(builder.ToString());

            builder.Compile();


            mb = typeBuilder.DefineMethod("tryFinally", MethodAttributes.Public);
            mb.SetParameters(typeof(string));
            mb.SetReturnType(typeof(void));
            mb.DefineParameter(1, ParameterAttributes.None, "value");

            builder = new MethodBodyBuilder(mb, typeof(string));

            builder.AddStatements(
                Expr.TryFinally(
                    Expr.IfThenElse(Expr.Parameter(1, typeof(string)),
                                    Expr.Call(typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expr.Parameter(1, typeof(string))),
                                    Expr.Throw(Expr.New(typeof(Exception), Expr.Constant("No string provided!")))),
                    Expr.Call(typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expr.Constant("finally!")))
                );

            Console.WriteLine(builder.ToString());
            builder.Compile();


            mb = typeBuilder.DefineMethod("tryCatchFinally", MethodAttributes.Public);
            mb.SetParameters(typeof(string));
            mb.SetReturnType(typeof(void));
            mb.DefineParameter(1, ParameterAttributes.None, "value");

            builder = new MethodBodyBuilder(mb, typeof(string));
            var loc1 = Expr.DeclareLocalVar(typeof(Exception), "e");
            builder.AddStatements(
                Expr.TryCatchFinally(
                    Expr.IfThenElse(Expr.Parameter(1, typeof(string)),
                                    Expr.Call(typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expr.Parameter(1, typeof(string))),
                                    Expr.Throw(Expr.New(typeof(ArgumentNullException), Expr.Constant("No string provided!")))),
                    Expr.Call(typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expr.Constant("finally!")),
                    new CatchBlock(typeof(ArgumentNullException), loc1, Expr.Rethrow())
                    ));

            Console.WriteLine(builder.ToString());
            builder.Compile();


            mb = typeBuilder.DefineMethod("localVar", MethodAttributes.Public);
            mb.SetReturnType(typeof(void));


            var var1 = Expr.DeclareLocalVar(typeof(string), "o");
            var var2 = Expr.DeclareLocalVar(typeof(int), "i");
            builder = new MethodBodyBuilder(mb, typeof(string));
            builder.AddStatements(
                Expr.WriteLocal(var1, Expr.Constant("High {0}!")),
                Expr.WriteLocal(var2, Expr.Constant(5)),
                Expr.Call(typeof(Console).GetMethod("WriteLine", new[] { typeof(string), typeof(object) }), Expr.ReadLocal(var1), Expr.Convert(Expr.ReadLocal(var2), typeof(object))));

            Console.WriteLine(builder.ToString());
            builder.Compile();

            mb = typeBuilder.DefineMethod("negNot", MethodAttributes.Public);
            mb.SetReturnType(typeof(int));
            mb.SetParameters(typeof(byte));

            builder = new MethodBodyBuilder(mb, typeof(byte));
            builder.AddStatements(Expr.Return(Expr.Not(Expr.Negate(Expr.Parameter(1, typeof(byte))))));

            Console.WriteLine(builder.ToString());
            builder.Compile();

            mb = typeBuilder.DefineMethod("loop", MethodAttributes.Public);
            mb.SetReturnType(typeof(void));


            var iVar = Expr.DeclareLocalVar(typeof(int), "i");
            builder = new MethodBodyBuilder(mb);
            builder.AddStatements(
                Expr.WriteLocal(iVar, Expr.Constant(10)),
                Expr.Loop(Expr.IfThenElse(
                    Expr.ReadLocal(iVar),
                    Expr.Block(
                        Expr.WriteLocal(iVar, Expr.Add(Expr.ReadLocal(iVar), Expr.Constant(-1))),
                        Expr.Call(typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expr.Constant("loop!..."))),
                   Expr.LoopBreak()))
                );

            Console.WriteLine(builder.ToString());
            builder.Compile();

            mb = typeBuilder.DefineMethod("conv", MethodAttributes.Public);
            mb.SetReturnType(typeof(ulong));


            builder = new MethodBodyBuilder(mb);
            builder.AddStatements(Expr.Return(Expr.Convert(Expr.Constant(-1), typeof(ulong))));

            Console.WriteLine(builder.ToString());
            builder.Compile();


            mb = typeBuilder.DefineMethod("intTS", MethodAttributes.Public);
            mb.SetReturnType(typeof(string));


            builder = new MethodBodyBuilder(mb);
            builder.AddStatements(Expr.Return(Expr.Call(Expr.Constant(21), typeof(int).GetMethod("ToString", new Type[0]))));

            Console.WriteLine(builder.ToString());
            builder.Compile();

            mb = typeBuilder.DefineMethod("loadStructField", MethodAttributes.Public);
            mb.SetParameters(typeof(MyStruct));
            mb.SetReturnType(typeof(string));


            builder = new MethodBodyBuilder(mb, typeof(MyStruct));
            builder.AddStatements(Expr.Return(Expr.ReadField(Expr.Parameter(1, typeof(MyStruct)), typeof(MyStruct).GetField("MyField"))));

            Console.WriteLine(builder.ToString());
            builder.Compile();


            mb = typeBuilder.DefineMethod("saveStructField", MethodAttributes.Public);
            mb.SetParameters(typeof(MyStruct),typeof(string));
            mb.SetReturnType(typeof(MyStruct));


            builder = new MethodBodyBuilder(mb, typeof(MyStruct),typeof(string));
            builder.AddStatements(
                Expr.WriteField(Expr.Parameter(1,typeof(MyStruct)),typeof(MyStruct).GetField("MyField"),Expr.Parameter(2,typeof(string))),
                Expr.Return(Expr.Parameter(1,typeof(MyStruct))));

            Console.WriteLine(builder.ToString());
            builder.Compile();

            typeBuilder.CreateType();
            asmBuilder.Save(fileName);
        }
    }

    public struct MyStruct
    {
        public long MyValue;
        public string MyField;

        public string MyMethod()
        {
            return "abc";
        }
    }
}
