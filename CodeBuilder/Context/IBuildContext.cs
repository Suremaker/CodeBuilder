using System;
using System.Reflection.Emit;

namespace CodeBuilder.Context
{
    public interface IBuildContext
    {
        Type ReturnType { get; }
        Type[] Parameters { get; }
        ILGenerator Generator { get; }

        LocalBuilder GetOrDeclareLocal(LocalVariable variable);
        LocalBuilder GetLocal(LocalVariable variable);
        void SetLoopData(LoopData data);
        void ResetLoopData(LoopData data);
        LoopData GetLoopData();

        Scope EnterScope<TScope>() where TScope : Scope, new();
        void LeaveScope(Scope scope);
        Scope CurrentScope { get; }
        bool IsSymbolInfoSupported { get; }
        bool IsInScope<TScope>() where TScope : Scope;
        JumpLabel DefineLabel();
    }
}