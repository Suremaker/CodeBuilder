﻿using System;
using System.Text;
using CodeBuilder.Context;
using CodeBuilder.Helpers;

namespace CodeBuilder.Expressions
{
    public class TryCatchFinallyExpression : VoidExpression
    {
        private readonly CatchBlock[] _catchBlocks;
        private readonly Expression _tryExpression;
        private readonly Expression _finallyExpression;

        public TryCatchFinallyExpression(Expression tryExpression, Expression finallyExpression, params CatchBlock[] catchBlocks)
        {
            Validators.NullCheck(tryExpression, "tryExpression");
            Validators.NullCollectionElementsCheck(catchBlocks, "catchBlocks");
            if (catchBlocks.Length == 0 && finallyExpression == null)
                throw new ArgumentException("Try-Catch-Finally block has to have finally block or at least one catch block");
            ValidateOrderOfCatchBlocks(catchBlocks);

            _tryExpression = ExprHelper.PopIfNeeded(tryExpression);
            if (finallyExpression != null)
                _finallyExpression = ExprHelper.PopIfNeeded(finallyExpression);
            _catchBlocks = catchBlocks;
        }

        private void ValidateOrderOfCatchBlocks(CatchBlock[] catchBlocks)
        {
            for (int i = 1; i < catchBlocks.Length; ++i)
                for (int j = 0; j < i; ++j)
                {
                    if (catchBlocks[i].ExceptionType.IsSubclassOf(catchBlocks[j].ExceptionType))
                        throw new ArgumentException(string.Format("Catch block of {0} type cannot be defined after block of {1} type", catchBlocks[i].ExceptionType, catchBlocks[j].ExceptionType));
                }
        }

        internal override void Compile(IBuildContext ctx)
        {
            ctx.Generator.BeginExceptionBlock();

            CompileTryBlock(ctx);

            foreach (var catchBlock in _catchBlocks)
                catchBlock.Compile(ctx);

            if (_finallyExpression != null)
                CompileFinallyBlock(ctx);

            ctx.Generator.EndExceptionBlock();
        }

        private void CompileTryBlock(IBuildContext ctx)
        {
            var scope = ctx.EnterScope<TryScope>();
            _tryExpression.Compile(ctx);
            ctx.LeaveScope(scope);
        }

        private void CompileFinallyBlock(IBuildContext ctx)
        {
            ctx.Generator.BeginFinallyBlock();

            var scope = ctx.EnterScope<FinallyScope>();
            _finallyExpression.Compile(ctx);
            ctx.LeaveScope(scope);
        }

        internal override StringBuilder Dump(StringBuilder builder)
        {
            builder.AppendLine(".try").AppendLine("{");
            _tryExpression.Dump(builder);
            builder.AppendLine("}");

            foreach (var catchBlock in _catchBlocks)
                catchBlock.Dump(builder);

            if (_finallyExpression != null)
                DumpFinally(builder);
            return builder;
        }

        private void DumpFinally(StringBuilder builder)
        {
            builder.AppendLine(".finally").AppendLine("{");
            _finallyExpression.Dump(builder);
            builder.AppendLine("}");
        }
    }
}