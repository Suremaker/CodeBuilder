using System;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace CodeBuilder.Context
{
    public class JumpLabel
    {
        public delegate void JumpValidator(Scope jumpFrom, Scope jumpTo);
        private readonly List<KeyValuePair<Scope, JumpValidator>> _deferredValidations = new List<KeyValuePair<Scope, JumpValidator>>();

        private readonly IBuildContext _ctx;
        private readonly Label _label;
        public bool IsMarked { get { return MarkScope != null; } }
        public Scope MarkScope { get; private set; }

        internal JumpLabel(IBuildContext ctx)
        {
            _ctx = ctx;
            _label = ctx.Generator.DefineLabel();
        }

        public void Mark()
        {
            if (IsMarked)
                throw new InvalidOperationException("Label is already marked!");
            _ctx.Generator.MarkLabel(_label);
            MarkScope = _ctx.CurrentScope;
            RunDeferredValidations();
        }

        private void RunDeferredValidations()
        {
            foreach (var deferred in _deferredValidations)
                deferred.Value(deferred.Key, MarkScope);
        }

        public void EmitGoto(OpCode gotoOpCode, JumpValidator validator = null)
        {
            if (validator != null)
                TryValidate(validator);
            _ctx.Generator.Emit(gotoOpCode, _label);
        }

        private void TryValidate(JumpValidator validator)
        {
            var current = _ctx.CurrentScope;
            if (IsMarked)
                validator(current, MarkScope);
            else
                _deferredValidations.Add(new KeyValuePair<Scope, JumpValidator>(current, validator));
        }
    }
}