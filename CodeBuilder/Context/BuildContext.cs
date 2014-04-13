using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Emit;

namespace CodeBuilder.Context
{
    class BuildContext : IBuildContext
    {
        private readonly Stack<Label> _exceptionBlocks = new Stack<Label>();
        private readonly Stack<Label> _finallyBlocks = new Stack<Label>();
        private readonly Stack<Label> _catchBlocks = new Stack<Label>();
        private readonly Stack<LoopData> _loopData = new Stack<LoopData>();
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
        public bool IsInCatchBlock { get { return _catchBlocks.Count > 0; } }

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
            Reset(_finallyBlocks, label, "Trying to reset finally block for wrong label!");
        }

        public void ResetExceptionBlock(Label label)
        {
            Reset(_exceptionBlocks, label, "Trying to reset exception block for wrong label!");
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

        public void SetCatchBlock(Label label)
        {
            _catchBlocks.Push(label);
        }

        public void ResetCatchBlock(Label label)
        {
            Reset(_catchBlocks, label, "Trying to reset catch block for wrong label!");
        }

        public void SetLoopData(LoopData data)
        {
            _loopData.Push(data);
        }

        public void ResetLoopData(LoopData data)
        {
            Reset(_loopData, data, "Trying to reset loop data for wrong loop!");
        }

        public LoopData GetLoopData()
        {
            return (_loopData.Count > 0) ? _loopData.Peek() : null;
        }

        private void Reset<T>(Stack<T> stack, T value, string errorMessage)
        {
            var actual = (stack.Count > 0) ? stack.Peek() : default(T);

            if (!Equals(actual, value))
                throw new InvalidOperationException(errorMessage);
            stack.Pop();
        }
    }
}