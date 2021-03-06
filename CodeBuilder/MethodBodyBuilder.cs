﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using CodeBuilder.Context;
using CodeBuilder.Expressions;
using CodeBuilder.Helpers;

namespace CodeBuilder
{
    public class MethodBodyBuilder
    {
        private readonly List<Expression> _statements = new List<Expression>();
        private readonly BuildContext _ctx;

        public MethodBodyBuilder(MethodBuilder methodBuilder, params Type[] parameters) : this(null, methodBuilder, parameters) { }
        public MethodBodyBuilder(IMethodSymbolGenerator symbolGenerator, MethodBuilder methodBuilder, params Type[] parameters)
        {
            Validators.NullCheck(methodBuilder, "methodBuilder");
            Validators.NullCollectionElementsCheck(parameters, "parameters");
            _ctx = new BuildContext(methodBuilder.GetILGenerator(), methodBuilder.ReturnType, PrepareParameters(methodBuilder, parameters), symbolGenerator);
        }

        public MethodBodyBuilder(ConstructorBuilder ctorBuilder, params Type[] parameters) : this(null, ctorBuilder, parameters) { }
        public MethodBodyBuilder(IMethodSymbolGenerator symbolGenerator, ConstructorBuilder ctorBuilder, params Type[] parameters)
        {
            Validators.NullCheck(ctorBuilder, "ctorBuilder");
            Validators.NullCollectionElementsCheck(parameters, "parameters");
            _ctx = new BuildContext(ctorBuilder.GetILGenerator(), typeof(void), PrepareParameters(ctorBuilder, parameters), symbolGenerator);
        }

        public MethodBodyBuilder(DynamicMethod dynamicMethod, params Type[] parameters)
        {
            Validators.NullCheck(dynamicMethod, "methodBuilder");
            Validators.NullCollectionElementsCheck(parameters, "parameters");
            _ctx = new BuildContext(dynamicMethod.GetILGenerator(), dynamicMethod.ReturnType, PrepareParameters(dynamicMethod, parameters), null, false);
        }

        private Type[] PrepareParameters(MethodBase methodBase, Type[] parameters)
        {
            var result = new Type[parameters.Length + (methodBase.IsStatic ? 0 : 1)];
            int i = 0;
            if (!methodBase.IsStatic)
                result[i++] = methodBase.DeclaringType;
            foreach (var parameter in parameters)
                result[i++] = parameter;
            return result;
        }

        public MethodBodyBuilder AddStatement(Expression expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            _statements.Add(ExprHelper.PopIfNeeded(expression));
            return this;
        }

        public MethodBodyBuilder AddStatements(params Expression[] expressions)
        {
            foreach (var expression in expressions)
                AddStatement(expression);
            return this;
        }

        public void Compile()
        {
            if (_statements.Count == 0 || !(_statements[_statements.Count - 1] is ReturnExpression))
            {
                if (_ctx.ReturnType != typeof(void))
                    throw new InvalidOperationException("Non void method body last expression is not return expression");
                AddStatement(Expr.Return());
            }

            _ctx.Compile(_statements);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var expression in _statements)
                sb.AppendLine(expression.ToString());
            return sb.ToString();
        }
    }
}