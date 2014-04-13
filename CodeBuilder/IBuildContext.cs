using System;
using System.Reflection.Emit;

namespace CodeBuilder
{
    public interface IBuildContext
    {
        Type ReturnType { get; }
        Type[] Parameters { get; }
        ILGenerator Generator { get; }
        bool IsInExceptionBlock { get; }
        bool IsInFinallyBlock { get; }
        bool IsInCatchBlock { get; }
        void SetExceptionBlock(Label label);
        void SetFinallyBlock(Label label);
        void ResetFinallyBlock(Label label);
        void ResetExceptionBlock(Label label);
        LocalBuilder GetOrDeclareLocalIndex(LocalVariable variable);
        LocalBuilder GetLocalIndex(LocalVariable variable);
        void SetCatchBlock(Label label);
        void ResetCatchBlock(Label label);
    }
}