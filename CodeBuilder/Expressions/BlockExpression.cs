using System.Collections.Generic;
using CodeBuilder.Context;
using CodeBuilder.Helpers;
using CodeBuilder.Symbols;

namespace CodeBuilder.Expressions
{
    public class BlockExpression : VoidExpression
    {
        private readonly Expression[] _expressions;

        public BlockExpression(Expression[] expressions)
        {
            Validators.NullCollectionElementsCheck(expressions, "expressions");
            _expressions = new Expression[expressions.Length];
            for (int i = 0; i < expressions.Length; i++)
                _expressions[i] = ExprHelper.PopIfNeeded(expressions[i]);
        }

        public IEnumerable<Expression> Expressions
        {
            get { return _expressions; }
        }

        internal override void Compile(IBuildContext ctx, int expressionId)
        {
            var scope = ctx.EnterScope<VoidBlockScope>();
            foreach (var expression in _expressions)
                ctx.Compile(expression);
            ctx.LeaveScope(scope);
        }

        internal override CodeBlock WriteDebugCode(IMethodSymbolGenerator symbolGenerator)
        {
            var begin = symbolGenerator.GetCurrentPosition();
            symbolGenerator.Write("{");
            symbolGenerator.EnterScope();

            for (int index = 0; index < _expressions.Length; index++)
            {
                symbolGenerator.Write(_expressions[index]);
                if (index + 1 < _expressions.Length)
                    symbolGenerator.WriteStatementEnd("");
            }

            var end = symbolGenerator
                .LeaveScope()
                .WriteStatementEnd("}");

            return begin.BlockTo(end);
        }
    }
}