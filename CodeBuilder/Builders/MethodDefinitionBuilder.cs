using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using CodeBuilder.Expressions;
using CodeBuilder.Symbols;

namespace CodeBuilder.Builders
{
    public class MethodDefinitionBuilder
    {
        private readonly Type _returnType;
        private readonly string _name;
        private readonly MethodAttributes _attributes;
        private readonly Type[] _parameters;
        private CallingConventions _callingConvention = CallingConventions.Standard;
        private readonly IList<CustomAttributeBuilder> _customAttributes = new List<CustomAttributeBuilder>();
        private MethodImplAttributes _methodImplAttributes = MethodImplAttributes.Managed;
        private Expression[] _bodyExpressions = new Expression[0];

        public MethodDefinitionBuilder(string name, MethodAttributes attributes, Type returnType, params Type[] parameters)
        {
            _name = name;
            _returnType = returnType; _attributes = attributes;
            _parameters = parameters;
        }

        public MethodDefinitionBuilder SetCustomAttribute(CustomAttributeBuilder builder)
        {
            _customAttributes.Add(builder);
            return this;
        }

        public MethodDefinitionBuilder SetMethodImplAttributes(MethodImplAttributes attributes)
        {
            _methodImplAttributes = attributes;
            return this;
        }

        public MethodDefinitionBuilder SetCallingConvention(CallingConventions callingConvention)
        {
            _callingConvention = callingConvention;
            return this;
        }

        public MethodDefinitionBuilder SetBody(params Expression[] body)
        {
            _bodyExpressions = body;
            return this;
        }

        public MethodInfo Compile(TypeBuilder builder)
        {
            var methodBuilder = DefineMethod(builder);

            MethodBodyBuilder.ForMethod(methodBuilder, _parameters)
                             .AddStatements(_bodyExpressions)
                             .Compile();

            return methodBuilder;
        }

        public MethodInfo Compile(TypeBuilder builder, ITypeSymbolGenerator symbolGenerator)
        {
            var methodBuilder = DefineMethod(builder);

            symbolGenerator.GenerateMethod(_attributes, _returnType, _name, _parameters, _methodImplAttributes, _customAttributes, methodSymbolGenerator =>
                MethodBodyBuilder.ForMethod(methodSymbolGenerator, methodBuilder, _parameters)
                                 .AddStatements(_bodyExpressions)
                                 .Compile());

            return methodBuilder;
        }

        private MethodBuilder DefineMethod(TypeBuilder builder)
        {
            var methodBuilder = builder.DefineMethod(_name, _attributes, _callingConvention, _returnType, _parameters);
            foreach (var customAttribute in _customAttributes)
                methodBuilder.SetCustomAttribute(customAttribute);
            methodBuilder.SetImplementationFlags(_methodImplAttributes);
            return methodBuilder;
        }
    }
}