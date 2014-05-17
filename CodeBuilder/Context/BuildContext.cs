using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace CodeBuilder.Context
{
    class BuildContext : IBuildContext
    {
        private readonly Stack<Scope> _scopes = new Stack<Scope>();
        private readonly Stack<LoopData> _loopData = new Stack<LoopData>();
        private readonly IDictionary<LocalVariable, LocalBuilder> _localVars = new Dictionary<LocalVariable, LocalBuilder>();

        public BuildContext(ILGenerator generator, Type returnType, Type[] parameters, bool isSymbolInfoSupported = true)
        {
            Parameters = parameters;
            IsSymbolInfoSupported = isSymbolInfoSupported;
            ReturnType = returnType;
            Generator = generator;
            _scopes.Push(new MethodBodyScope());
        }

        public Type ReturnType { get; private set; }
        public Type[] Parameters { get; private set; }
        public ILGenerator Generator { get; private set; }
        public bool IsSymbolInfoSupported { get; private set; }
        public bool IsInScope<TScope>() where TScope : Scope
        {
            var scope = CurrentScope;
            while (scope != null)
            {
                if (scope is TScope) return true;
                scope = scope.Outer;
            }
            return false;
        }

        public JumpLabel DefineLabel()
        {
            return new JumpLabel(this);
        }

        public Scope CurrentScope { get { return _scopes.Peek(); } }

        public LocalBuilder GetOrDeclareLocal(LocalVariable variable)
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
            if (IsSymbolInfoSupported)
                local.SetLocalSymInfo(variable.Name);
            _localVars.Add(variable, local);
            return local;
        }

        public LocalBuilder GetLocal(LocalVariable variable)
        {
            LocalBuilder value;
            if (_localVars.TryGetValue(variable, out value))
                return value;
            throw new InvalidOperationException(string.Format("Uninitialized local variable access: {0}", variable));
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

        public Scope EnterScope<TScope>() where TScope : Scope, new()
        {
            var scope = Scope.Create<TScope>(CurrentScope);
            _scopes.Push(scope);
            return scope;
        }

        public void LeaveScope(Scope scope)
        {
            if (_scopes.Peek() != scope)
                throw new InvalidOperationException(string.Format("Unable to leave requested scope {0} because it is not current scope.", scope));
            _scopes.Pop();
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