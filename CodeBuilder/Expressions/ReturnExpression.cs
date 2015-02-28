using System;
using System.Reflection.Emit;
using CodeBuilder.Context;
using CodeBuilder.Helpers;
using CodeBuilder.Symbols;

namespace CodeBuilder.Expressions
{
    public class ReturnExpression : VoidExpression
    {
        private readonly Expression _value;

        public ReturnExpression()
        {
            ReturnType = typeof(void);
            _value = null;
        }

        public ReturnExpression(Expression value)
        {
            _value = value;
            if (value == null)
                throw new ArgumentNullException("value");
            if (value.ExpressionType == typeof(void))
                throw new ArgumentException("Void expressions cannot be returned explicitly. Please use Expr.Return() instead.");
            ReturnType = value.ExpressionType;
        }

        public Type ReturnType { get; private set; }

        private void ValidateScope(IBuildContext ctx)
        {
            ctx.CurrentScope.ValidateJumpOutTo(null, ScopeJumpType.Return, "Return expression is forbidden in {0} scope");
        }

        private void ValidateReturnType(IBuildContext ctx)
        {
            Validators.HierarchyCheck(ReturnType, ctx.ReturnType, "Method return type is {0}, while return statement is returning {1}", "ReturnType");
        }

        internal override void Compile(IBuildContext ctx, int expressionId)
        {
            ValidateReturnType(ctx);
            ValidateScope(ctx);

            if (_value != null)
                ctx.Compile(_value);

            ctx.MarkSequencePointFor(expressionId);
            ctx.Generator.Emit(OpCodes.Ret);
        }

        internal override CodeBlock WriteDebugCode(IMethodSymbolGenerator symbolGenerator)
        {
            var start = symbolGenerator.GetCurrentPosition();
            if (_value != null)
                symbolGenerator.Write("return ").Write(_value);
            else
                symbolGenerator.Write("return");
            return start.BlockTo(symbolGenerator.Write(";").GetCurrentPosition());
        }
    }
}