using System;
using System.Reflection.Emit;

namespace CodeBuilder
{
    class BuildContext : IBuildContext
    {
        public BuildContext(ILGenerator generator, Type returnType, Type[] parameters)
        {
            Parameters = parameters;
            ReturnType = returnType;
            Generator = generator;
        }

        public Type ReturnType { get; private set; }
        public Type[] Parameters { get; private set; }
        public ILGenerator Generator { get; private set; }
    }
}