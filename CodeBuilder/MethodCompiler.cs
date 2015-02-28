using System;
using System.Reflection;
using CodeBuilder.Builders;

namespace CodeBuilder
{
    public static class MethodCompiler
    {
        public static MethodDefinitionBuilder DefineMethod(string name, MethodAttributes attributes, Type returnType, params Type[] parameters)
        {
            return new MethodDefinitionBuilder(name, attributes, returnType, parameters);
        }

        public static ConstructorDefinitionBuilder DefineConstructor(MethodAttributes attributes, params Type[] parameters)
        {
            return new ConstructorDefinitionBuilder(attributes, parameters);
        }

        public static TypeInitializerDefinitionBuilder DefineTypeInitializer()
        {
            return new TypeInitializerDefinitionBuilder();
        }

        public static DynamicMethodDefinitionBuilder DefineDynamicMethod(string name, Type returnType, params Type[] parameters)
        {
            return new DynamicMethodDefinitionBuilder(name, returnType, parameters);
        }
    }
}