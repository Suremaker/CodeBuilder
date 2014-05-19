using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace CodeBuilder.Context
{

    [Flags]
    public enum ScopeJumpType
    {
        None = 0,
        Return = 1,
        Break = 2,
        Leave = 4,
        Jump = 8,
        All = Return | Break | Leave | Jump,
    };

    public class Scope
    {
        private readonly IDictionary<LocalVariable, LocalBuilder> _localVars = new Dictionary<LocalVariable, LocalBuilder>();
        public ScopeJumpType JumpOutPolicy { get; private set; }
        public string Name { get; private set; }
        public Scope Outer { get; private set; }

        public Scope(string name, ScopeJumpType jumpOutPolicy)
        {
            Name = name;
            JumpOutPolicy = jumpOutPolicy;
        }

        public override string ToString()
        {
            return Name;
        }

        public static TScope Create<TScope>(Scope parent) where TScope : Scope, new()
        {
            return new TScope { Outer = parent };
        }

        public void ValidateJumpOutTo(Scope target, ScopeJumpType jumpType, string exceptionMessageFormat)
        {
            var scope = this;
            while (scope != target)
            {
                if (scope == null)
                    throw new ScopeChangeException("The target scope has to be one of outer scopes of current scope.");
                if ((scope.JumpOutPolicy & jumpType) != jumpType)
                    throw new ScopeChangeException(string.Format(exceptionMessageFormat, scope));
                scope = scope.Outer;
            }
        }

        internal LocalBuilder DeclareLocal(IBuildContext ctx, LocalVariable variable)
        {
            foreach (var v in _localVars)
            {
                if (v.Key.Name == variable.Name)
                    throw new ArgumentException(string.Format("Unable to declare '{0}' local variable, '{1}' with same name already exist", variable, v.Key));
            }
            var local = ctx.Generator.DeclareLocal(variable.VariableType);
            if (ctx.IsSymbolInfoSupported)
                local.SetLocalSymInfo(variable.Name);
            _localVars.Add(variable, local);
            return local;
        }

        internal LocalBuilder FindLocal(LocalVariable variable)
        {
            LocalBuilder value;
            _localVars.TryGetValue(variable, out value);
            return value;
        }

        internal LocalVariable FindLocal(string name)
        {
            foreach (var localVar in _localVars)
                if (localVar.Key.Name == name)
                    return localVar.Key;
            return null;
        }
    }

    public class MethodBodyScope : Scope
    {
        internal MethodBodyScope() : base("method body", ScopeJumpType.Return | ScopeJumpType.Jump) { }
    }
    public class VoidBlockScope : Scope
    {
        public VoidBlockScope() : base("block", ScopeJumpType.All) { }
    }
    public class TryScope : Scope
    {
        public TryScope() : base("try block", ScopeJumpType.Leave) { }
    }
    public class CatchScope : Scope
    {
        public CatchScope() : base("catch block", ScopeJumpType.Leave) { }
    }
    public class FinallyScope : Scope
    {
        public FinallyScope() : base("finally block", ScopeJumpType.None) { }
    }
    public class ValueBlockScope : Scope
    {
        public ValueBlockScope() : base("value block", ScopeJumpType.Jump) { }
    }
}
