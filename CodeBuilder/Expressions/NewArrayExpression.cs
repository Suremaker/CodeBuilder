﻿using System;
using System.Reflection.Emit;
using System.Text;
using CodeBuilder.Context;
using CodeBuilder.Helpers;

namespace CodeBuilder.Expressions
{
    public class NewArrayExpression : Expression
    {
        private readonly Type _elementType;
        private readonly Expression _count;

        internal NewArrayExpression(Type elementType, Expression count)
            : base(Validators.NullCheck(elementType, "elementType").MakeArrayType())
        {
            Validators.NullCheck(count, "count");
            Validators.IntegralCheck(count.ExpressionType, "Array construction requires parameter of integral type, got: {0}", "count");
            _elementType = elementType;
            _count = count;
        }

        internal override void Compile(IBuildContext ctx)
        {
            _count.Compile(ctx);
            EmitHelper.ConvertToNativeInt(ctx, _count.ExpressionType);
            ctx.Generator.Emit(OpCodes.Newarr, _elementType);
        }

        internal override StringBuilder Dump(StringBuilder builder)
        {
            builder.AppendFormat(".array ").Append(_elementType).Append(" [");
            _count.Dump(builder);
            return builder.Append("]");
        }
    }
}