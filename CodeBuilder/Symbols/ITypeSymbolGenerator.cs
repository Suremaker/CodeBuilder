using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace CodeBuilder.Symbols
{
    public delegate void MethodGenerator(IMethodSymbolGenerator methodSymbolGenerator);
    public interface ITypeSymbolGenerator : IDisposable
    {
        void GenerateMethod(MethodAttributes attributes, Type returnType, string name, Type[] parameters, MethodImplAttributes methodImplAttributes, IEnumerable<CustomAttributeBuilder> customAttributes,MethodGenerator methodGenerator);
        void GenerateConstructor(MethodAttributes attributes, Type[] parameters, MethodImplAttributes methodImplAttributes, IList<CustomAttributeBuilder> customAttributes, MethodGenerator methodGenerator);
        void GenerateDefaultConstructor(MethodImplAttributes methodImplAttributes, IList<CustomAttributeBuilder> customAttributes, MethodGenerator methodGenerator);
    }
}