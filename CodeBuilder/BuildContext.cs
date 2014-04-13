using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Emit;

namespace CodeBuilder
{
    class BuildContext : IBuildContext
    {
        private readonly Stack<Label> _exceptionBlocks = new Stack<Label>();
        private readonly Stack<Label> _finallyBlocks = new Stack<Label>();
        private readonly IDictionary<LocalVariable, LocalBuilder> _localVars = new Dictionary<LocalVariable, LocalBuilder>();
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
            if (_finallyBlocks.Peek() != label)
                throw new InvalidOperationException("Trying to reset finally block for wrong label!");
            _finallyBlocks.Pop();
        }

        public void ResetExceptionBlock(Label label)
        {
            if (_exceptionBlocks.Peek() != label)
                throw new InvalidOperationException("Trying to reset exception block for wrong label!");
            _exceptionBlocks.Pop();
        }

        public LocalBuilder GetOrDeclareLocalIndex(LocalVariable variable)
        {
            LocalBuilder value;
            if (_localVars.TryGetValue(variable, out value))
                return value;

            return DeclareLocal(variable);
        }

        private LocalBuilder DeclareLocal(LocalVariable variable)
        {
            foreach (var v in _localVars)
            {
                if (v.Key.Name == variable.Name)
                    throw new ArgumentException(string.Format("Unable to declare '{0}' local variable, '{1}' with same name already exist", variable, v.Key));
            }
            var local = Generator.DeclareLocal(variable.VariableType);
            local.SetLocalSymInfo(variable.Name);
            _localVars.Add(variable, local);
            return local;
        }

        public LocalBuilder GetLocalIndex(LocalVariable variable)
        {
            LocalBuilder value;
            if (_localVars.TryGetValue(variable, out value))
                return value;
            throw new IOException(string.Format("Uninitialized local variable access: {0}", variable));
        }
    }
}