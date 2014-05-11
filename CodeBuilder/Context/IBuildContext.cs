using System;
using System.Reflection.Emit;

namespace CodeBuilder.Context
{
    public interface IBuildContext
    {
        Type ReturnType { get; }
        Type[] Parameters { get; }
        ILGenerator Generator { get; }
        bool IsInExceptionBlock { get; }
        bool IsInFinallyBlock { get; }
        bool IsInCatchBlock { get; }
        bool IsInValueBlock { get; }
        void SetExceptionBlock(Label label);
        void SetFinallyBlock(Label label);
        void ResetFinallyBlock(Label label);
        void ResetExceptionBlock(Label label);
        LocalBuilder GetOrDeclareLocalIndex(LocalVariable variable);
        LocalBuilder GetLocalIndex(LocalVariable variable);
        void SetCatchBlock(Label label);
        void ResetCatchBlock(Label label);
        void SetLoopData(LoopData data);
        void ResetLoopData(LoopData data);
        LoopData GetLoopData();
        void SetValueBlock();
        void ResetValueBlock();
    }
}