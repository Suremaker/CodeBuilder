using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using CodeBuilder;
using CodeBuilder.Symbols;

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
            var symGen = new SymbolGenerator(modBuilder);
            using (var typeSymbolGenerator = symGen.GetTypeSymbolGenerator(typeBuilder))
            {
                MethodCompiler.DefineTypeInitializer()
                    .SetBody(
                        Expr.WriteField(fieldBuilder, Expr.Constant("abc")),
                        Expr.Call(typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expr.Constant("Hello my friend!")),
                        Expr.Call(typeof(Console).GetMethod("WriteLine", new[] { typeof(string), typeof(object) }), Expr.Constant("What a {0} day!"), Expr.Constant("beautiful")),
                        Expr.Call(typeof(Console).GetMethod("ReadKey", new Type[0])))
                    .Compile(typeBuilder, typeSymbolGenerator);

                MethodCompiler.DefineMethod("foo", MethodAttributes.Public, typeof(string), typeof(int))
                    .SetBody(
                        Expr.IfThen(Expr.Parameter(1, typeof(int)), Expr.Call(typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expr.Constant("aha!"))),
                        Expr.Return(Expr.Call(
                            Expr.Call(
                                Expr.New(typeof(StringBuilder)),
                                typeof(StringBuilder).GetMethod("AppendFormat", new[] { typeof(string), typeof(object), typeof(object) }),
                                Expr.Constant("{0}_{1}"),
                                Expr.ReadField(fieldBuilder),
                                Expr.Convert(Expr.Parameter(1, typeof(int)), typeof(object))),
                            typeof(StringBuilder).GetMethod("ToString", new Type[0]))))
                    .Compile(typeBuilder, typeSymbolGenerator);

                MethodCompiler.DefineMethod("bar", MethodAttributes.Public, typeof(string), typeof(string))
                    .SetBody(
                        Expr.Return(
                        Expr.Call(typeof(string).GetMethod("Format", new[] { typeof(string), typeof(object), typeof(object) }), Expr.Constant("Result: {0}-{1}"),
                            Expr.IfThenElse(
                                Expr.Parameter(1, typeof(string)),
                                Expr.Call(typeof(string).GetMethod("Format", new[] { typeof(string), typeof(object) }), Expr.Constant("Date: {0}"), Expr.Convert(Expr.Parameter(1, typeof(string)), typeof(object))),
                                Expr.Constant("Undefined!")),
                        Expr.Convert(Expr.Convert(Expr.Convert(Expr.Constant(16), typeof(object)), typeof(int)), typeof(object)))))
                    .Compile(typeBuilder, typeSymbolGenerator);

                MethodCompiler.DefineMethod("tryFinally", MethodAttributes.Public, typeof(void), typeof(string))
                    .SetBody(
                        Expr.TryFinally(
                            Expr.IfThenElse(Expr.Parameter(1, typeof(string)),
                                            Expr.Call(typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expr.Parameter(1, typeof(string))),
                                            Expr.Throw(Expr.New(typeof(Exception), Expr.Constant("No string provided!")))),
                            Expr.Call(typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expr.Constant("finally!"))))
                    .Compile(typeBuilder, typeSymbolGenerator);


                var loc1 = Expr.LocalVariable(typeof(Exception), "e");
                MethodCompiler.DefineMethod("tryCatchFinally", MethodAttributes.Public, typeof(void), typeof(string))
                    .SetBody(
                        Expr.TryCatchFinally(
                            Expr.IfThenElse(Expr.Parameter(1, typeof(string)),
                                            Expr.Call(typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expr.Parameter(1, typeof(string))),
                                            Expr.Throw(Expr.New(typeof(ArgumentNullException), Expr.Constant("No string provided!")))),
                            Expr.Call(typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expr.Constant("finally!")),
                            new CatchBlock(typeof(ArgumentNullException), loc1, Expr.Rethrow())))
                    .Compile(typeBuilder, typeSymbolGenerator);

                var var1 = Expr.LocalVariable(typeof(string), "o");
                var var2 = Expr.LocalVariable(typeof(int), "i");
                MethodCompiler.DefineMethod("localVar", MethodAttributes.Public, typeof(void))
                    .SetBody(
                        Expr.DeclareLocal(var1, Expr.Constant("High {0}!")),
                        Expr.DeclareLocal(var2, Expr.Constant(5)),
                        Expr.Call(typeof(Console).GetMethod("WriteLine", new[] { typeof(string), typeof(object) }), Expr.ReadLocal(var1), Expr.Convert(Expr.ReadLocal(var2), typeof(object))))
                    .Compile(typeBuilder, typeSymbolGenerator);

                MethodCompiler.DefineMethod("negNot", MethodAttributes.Public, typeof(int), typeof(byte))
                    .SetBody(Expr.Return(Expr.Not(Expr.Negate(Expr.Parameter(1, typeof(byte))))))
                    .Compile(typeBuilder, typeSymbolGenerator);

                var iVar = Expr.LocalVariable(typeof(int), "i");
                MethodCompiler.DefineMethod("loop", MethodAttributes.Public, typeof(void))
                    .SetBody(
                        Expr.DeclareLocal(iVar, Expr.Constant(10)),
                        Expr.Loop(Expr.IfThenElse(
                            Expr.ReadLocal(iVar),
                            Expr.Block(
                                Expr.WriteLocal(iVar, Expr.Add(Expr.ReadLocal(iVar), Expr.Constant(-1))),
                                Expr.Call(typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expr.Constant("loop!..."))),
                            Expr.LoopBreak())))
                    .Compile(typeBuilder, typeSymbolGenerator);

                MethodCompiler.DefineMethod("conv", MethodAttributes.Public, typeof(ulong))
                    .SetBody(Expr.Return(Expr.Convert(Expr.Constant(-1), typeof(ulong))))
                    .Compile(typeBuilder, typeSymbolGenerator);


                MethodCompiler.DefineMethod("intTS", MethodAttributes.Public, typeof(string))
                    .SetBody(Expr.Return(Expr.Call(Expr.Constant(21), typeof(int).GetMethod("ToString", new Type[0]))))
                    .Compile(typeBuilder, typeSymbolGenerator);

                MethodCompiler.DefineMethod("loadStructField", MethodAttributes.Public, typeof(string), typeof(MyStruct))
                    .SetBody(Expr.Return(Expr.ReadField(Expr.Parameter(1, typeof(MyStruct)), typeof(MyStruct).GetField("MyField"))))
                    .Compile(typeBuilder, typeSymbolGenerator);


                MethodCompiler.DefineMethod("saveStructField", MethodAttributes.Public, typeof(MyStruct), typeof(MyStruct), typeof(string))
                    .SetBody(
                        Expr.WriteField(Expr.Parameter(1, typeof(MyStruct)), typeof(MyStruct).GetField("MyField"), Expr.Parameter(2, typeof(string))),
                        Expr.Return(Expr.Parameter(1, typeof(MyStruct))))
                    .Compile(typeBuilder, typeSymbolGenerator);

                MethodCompiler.DefineMethod("createStruct", MethodAttributes.Public, typeof(MyStruct))
                    .SetBody(Expr.Return(Expr.New(typeof(MyStruct))))//, Expr.Constant(32L), Expr.Constant("abcd")
                    .Compile(typeBuilder, typeSymbolGenerator);


                var local = Expr.LocalVariable(typeof(Exception), "e");
                MethodCompiler.DefineMethod("catchException", MethodAttributes.Public, typeof(Exception))
                    .SetBody(
                        Expr.DeclareLocal(local, Expr.New(typeof(Exception))),
                        Expr.TryCatch(
                            Expr.Throw(Expr.New(typeof(InvalidOperationException), Expr.Constant("abc"))),
                            new CatchBlock(typeof(InvalidOperationException), local, false, Expr.Empty())),
                            Expr.Constant(true),
                        Expr.Return(Expr.ReadLocal(local)))
                    .Compile(typeBuilder, typeSymbolGenerator);

                MethodCompiler.DefineMethod("valueBlock", MethodAttributes.Public, typeof(string))
                    .SetBody(
                        Expr.Return(
                            Expr.Call(typeof(string).GetMethod("Format", new Type[] { typeof(string), typeof(object), typeof(object) }),
                            Expr.Constant("{0}-{1}"),
                            Expr.ValueBlock(typeof(string),
                                Expr.Call(typeof(Console), "WriteLine", Expr.Constant("abc123")),
                                Expr.Constant("abc")),
                            Expr.Constant("123")
                        )))
                    .Compile(typeBuilder, typeSymbolGenerator);
            }
            typeBuilder.CreateType();
            asmBuilder.Save(fileName);
        }
    }

    public struct MyStruct
    {
        public long MyValue;
        public string MyField;

        public MyStruct(long myValue, string myField)
        {
            MyValue = myValue;
            MyField = myField;
        }

        public string MyMethod()
        {
            return "abc";
        }
    }
}
