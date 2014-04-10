using System;
using System.Reflection;
using System.Reflection.Emit;

namespace CodeBuilder
{
    public interface IBuildContext
    {
        Type ReturnType { get; }
        Type[] Parameters { get; }
        ILGenerator Generator { get; }
    }
}