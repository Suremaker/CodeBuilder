using System;

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
