using System;

namespace CodeBuilder.Context
{
    public class ScopeChangeException : InvalidOperationException
    {
        public ScopeChangeException(string message, Exception inner = null) : base(message, inner) { }
    }
}