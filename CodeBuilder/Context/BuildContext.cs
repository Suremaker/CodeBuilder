using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using CodeBuilder.Expressions;

namespace CodeBuilder.Context
{
    class BuildContext : IBuildContext
    {
        private readonly Stack<Scope> _scopes = new Stack<Scope>();
        private readonly Stack<LoopData> _loopData = new Stack<LoopData>();
        private int _expressionId = 0;

        public BuildContext(ILGenerator generator, Type returnType, Type[] parameters, IMethodSymbolGenerator symbolGenerator, bool isSymbolInfoSupported = true)
        {
            Parameters = parameters;
            SymbolGenerator = symbolGenerator;
            IsSymbolInfoSupported = isSymbolInfoSupported;
            ReturnType = returnType;
            Generator = generator;
            _scopes.Push(new MethodBodyScope());
        }

        public Type ReturnType { get; private set; }
        public Type[] Parameters { get; private set; }
        public IMethodSymbolGenerator SymbolGenerator { get; private set; }

        public ILGenerator Generator { get; private set; }
        public bool IsSymbolInfoSupported { get; private set; }
        public bool IsSymbolInfoEnabled { get { return IsSymbolInfoSupported && SymbolGenerator != null; } }

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

        public void MarkSequencePointFor(int expressionId)
        {
            if(IsSymbolInfoEnabled)
                SymbolGenerator.MarkSequencePoint(Generator,expressionId);
        }

        public Scope CurrentScope { get { return _scopes.Peek(); } }

        public LocalBuilder DeclareLocal(LocalVariable variable)
        {
            var scope = CurrentScope;
            while (scope != null)
            {
                var localVar = scope.FindLocal(variable.Name);
                if (localVar != null)
                    throw new InvalidOperationException(string.Format("Local variable with {0} name is already declared", variable.Name));
                scope = scope.Outer;
            }
            return CurrentScope.DeclareLocal(this, variable);
        }

        public LocalBuilder GetLocal(LocalVariable variable)
        {
            var scope = CurrentScope;
            while (scope != null)
            {
                var value = scope.FindLocal(variable);
                if (value != null)
                    return value;
                scope = scope.Outer;
            }
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
            if (IsSymbolInfoSupported)
                Generator.BeginScope();
            return scope;
        }

        public void LeaveScope(Scope scope)
        {
            if (_scopes.Peek() != scope)
                throw new InvalidOperationException(string.Format("Unable to leave requested scope {0} because it is not current scope.", scope));
            if (IsSymbolInfoSupported)
                Generator.EndScope();
            _scopes.Pop();
        }

        private void Reset<T>(Stack<T> stack, T value, string errorMessage)
        {
            var actual = (stack.Count > 0) ? stack.Peek() : default(T);

            if (!Equals(actual, value))
                throw new InvalidOperationException(errorMessage);
            stack.Pop();
        }

        public void Compile(IEnumerable<Expression> expressions)
        {
            foreach (var expression in expressions)
            {
                if (IsSymbolInfoEnabled)
                    SymbolGenerator.Write(expression);
                Compile(expression);
            }
        }

        public void Compile(Expression expression)
        {
            expression.Compile(this, _expressionId++);
        }
    }
}