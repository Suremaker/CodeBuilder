using System;
using System.Reflection.Emit;
using CodeBuilder.Context;

namespace CodeBuilder.Expressions
{
    internal class ExceptionCatchInitializerExpression : VoidExpression
    {
        private readonly Type _exceptionType;
        private readonly LocalVariable _exceptionVariable;
        private readonly bool _declareVariable;

        public ExceptionCatchInitializerExpression(Type exceptionType, LocalVariable exceptionVariable, bool declareVariable)
        {
            _exceptionType = exceptionType;
            _exceptionVariable = exceptionVariable;
            _declareVariable = declareVariable;
        }

        internal override void Compile(IBuildContext ctx, int expressionId)
        {
            if (_exceptionVariable == null)
                ctx.Generator.Emit(OpCodes.Pop);
            else
            {
                var local = _declareVariable ? ctx.DeclareLocal(_exceptionVariable) : ctx.GetLocal(_exceptionVariable);
                LocalWriteExpression.EmitWriteLocal(ctx, local);
            }
        }

        internal override CodeBlock WriteDebugCode(IMethodSymbolGenerator symbolGenerator)
        {
            symbolGenerator.Write("catch");
            if (_exceptionType == null)
                return symbolGenerator.GetCurrentPosition().BlockTo(symbolGenerator.GetCurrentPosition());
            symbolGenerator.Write(" (");
            var begin = symbolGenerator.GetCurrentPosition();
            symbolGenerator.Write(_exceptionType.FullName);
            if (_exceptionVariable != null)
                symbolGenerator.Write(" " + _exceptionVariable.Name);
            return begin.BlockTo(symbolGenerator.Write(")").GetCurrentPosition());
        }
    }
}