using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace CodeBuilder
{
    class BuildContext : IBuildContext
    {
        private readonly Stack<Label> _exceptionBlocks = new Stack<Label>();
        private readonly Stack<Label> _finallyBlocks = new Stack<Label>();
        public BuildContext(ILGenerator generator, Type returnType, Type[] parameters)
        {
            Parameters = parameters;
            ReturnType = returnType;
            Generator = generator;
        }

        public Type ReturnType { get; private set; }
        public Type[] Parameters { get; private set; }
        public ILGenerator Generator { get; private set; }
        public bool IsInExceptionBlock { get { return _exceptionBlocks.Count > 0; } }
        public bool IsInFinallyBlock { get { return _finallyBlocks.Count > 0; } }
        public bool IsInCatchBlock { get { return false; } }

        public void SetExceptionBlock(Label label)
        {
            _exceptionBlocks.Push(label);
        }

        public void SetFinallyBlock(Label label)
        {
            _finallyBlocks.Push(label);
        }

        public void ResetFinallyBlock(Label label)
        {
            if(_finallyBlocks.Peek()!=label)
                throw new InvalidOperationException("Trying to reset finally block for wrong label!");
            _finallyBlocks.Pop();
        }

        public void ResetExceptionBlock(Label label)
        {
            if (_exceptionBlocks.Peek() != label)
                throw new InvalidOperationException("Trying to reset exception block for wrong label!");
            _exceptionBlocks.Pop();
        }
    }
}