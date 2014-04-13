using System;
using System.Reflection.Emit;
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
                throw new ArgumentException("Try-Catch block has to have finally block or at least one catch block");
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
            var label = ctx.Generator.BeginExceptionBlock();
            ctx.SetExceptionBlock(label);
            _tryExpression.Compile(ctx);

            foreach (var catchBlock in _catchBlocks)
                CompileCatchBlock(ctx, catchBlock,label);

            if (_finallyExpression != null)
                CompileFinallyBlock(ctx, label);

            ctx.ResetExceptionBlock(label);
            ctx.Generator.EndExceptionBlock();
        }

        private void CompileCatchBlock(IBuildContext ctx, CatchBlock catchBlock, Label label)
        {
            ctx.Generator.BeginCatchBlock(catchBlock.ExceptionType);
            ctx.SetCatchBlock(label);
            catchBlock.PreCatchExpression.Compile(ctx);
            catchBlock.CatchExpression.Compile(ctx);
            ctx.ResetCatchBlock(label);
        }

        private void CompileFinallyBlock(IBuildContext ctx, Label label)
        {
            ctx.Generator.BeginFinallyBlock();
            ctx.SetFinallyBlock(label);
            _finallyExpression.Compile(ctx);

            ctx.ResetFinallyBlock(label);
        }

        internal override StringBuilder Dump(StringBuilder builder)
        {
            builder.AppendLine(".try").AppendLine("{");
            _tryExpression.Dump(builder);
            builder.AppendLine("}");

            foreach (var catchBlock in _catchBlocks)
                DumpCatchBlock(builder, catchBlock);

            if (_finallyExpression != null)
                DumpFinally(builder);
            return builder;
        }

        private static void DumpCatchBlock(StringBuilder builder, CatchBlock catchBlock)
        {
            builder.AppendFormat(".catch ({0})", catchBlock.ExceptionType).AppendLine().AppendLine("{");
            catchBlock.PreCatchExpression.Dump(builder);
            catchBlock.CatchExpression.Dump(builder);
            builder.AppendLine("}");
        }

        private void DumpFinally(StringBuilder builder)
        {
            builder.AppendLine(".finally").AppendLine("{");
            _finallyExpression.Dump(builder);
            builder.AppendLine("}");
        }
    }
}