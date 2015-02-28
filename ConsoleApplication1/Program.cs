using System;
using System.Diagnostics;
using System.Diagnostics.SymbolStore;
using System.Reflection;
using System.Reflection.Emit;

namespace ConsoleApplication1
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

            Type daType = typeof(DebuggableAttribute);
            ConstructorInfo daCtor = daType.GetConstructor(new Type[] { typeof(DebuggableAttribute.DebuggingModes) });
            CustomAttributeBuilder daBuilder = new CustomAttributeBuilder(daCtor, new object[] { 
            DebuggableAttribute.DebuggingModes.DisableOptimizations | 
            DebuggableAttribute.DebuggingModes.Default|DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints });
            asmBuilder.SetCustomAttribute(daBuilder);
            modBuilder.SetCustomAttribute(daBuilder);

            ISymbolDocumentWriter doc = modBuilder.DefineDocument(@"d:\dev\dotnet\CodeBuilder\ConsoleApplication1\bin\Debug\doc.cs", Guid.Empty, Guid.Empty, Guid.Empty);


            var mb = typeBuilder.DefineMethod("foo", MethodAttributes.Public | MethodAttributes.Static);
            mb.SetReturnType(typeof(string));
            var gen = mb.GetILGenerator();
            gen.MarkSequencePoint(doc, 1, 1, 1, 10);
            gen.Emit(OpCodes.Ldstr, "result");
            gen.MarkSequencePoint(doc, 2, 1, 2, 10);
            gen.Emit(OpCodes.Ldstr, "result");
            gen.MarkSequencePoint(doc, 3, 1, 3, 10);
            gen.Emit(OpCodes.Pop);
            gen.Emit(OpCodes.Nop);
            gen.Emit(OpCodes.Ret);

            var tt=typeBuilder.CreateType();
            //asmBuilder.Save(fileName);
            //var tt = Assembly.LoadFile(@"d:\dev\dotnet\CodeBuilder\ConsoleApplication1\bin\Debug\PlayGround.Generated.dll").GetType("PlayGround.Generated.MyClass");
            var methodInfo = tt.GetMethod("foo");
            //var fun = (Func<string>)Delegate.CreateDelegate(typeof(Func<string>), methodInfo);
            //Console.WriteLine(fun());
            Console.WriteLine(methodInfo.Invoke(null, new object[0]));
            Console.ReadLine();
        }
    }
}
