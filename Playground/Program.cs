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
                Expr.FieldWrite(fieldBuilder, Expr.Constant("abc")),
                Expr.Call(typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }), Expr.Constant("Hello my friend!")),
                Expr.Call(typeof(Console).GetMethod("WriteLine", new[] { typeof(string), typeof(object) }), Expr.Constant("What a {0} day!"), Expr.Constant("beautiful")),
                Expr.Call(typeof(Console).GetMethod("ReadKey",new Type[0])));
            Console.WriteLine(builder.ToString());
            builder.Compile();

            var mb = typeBuilder.DefineMethod("foo", MethodAttributes.Public);
            mb.SetReturnType(typeof(string));
            mb.SetParameters(typeof(int));
            mb.DefineParameter(1, ParameterAttributes.None, "val");
            builder = new MethodBodyBuilder(mb, typeof(int));
            builder.AddStatements(
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
            typeBuilder.CreateType();
            asmBuilder.Save(fileName);
        }
    }
}
