using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using CodeBuilder.Expressions;
using CodeBuilder.Symbols;

namespace CodeBuilder.Builders
{
    public class ConstructorDefinitionBuilder
    {
        private readonly MethodAttributes _attributes;
        private readonly Type[] _parameters;
        private CallingConventions _callingConvention = CallingConventions.Standard;
        private readonly IList<CustomAttributeBuilder> _customAttributes = new List<CustomAttributeBuilder>();
        private MethodImplAttributes _methodImplAttributes = MethodImplAttributes.Managed;
        private Expression[] _bodyExpressions = new Expression[0];

        public ConstructorDefinitionBuilder(MethodAttributes attributes, params Type[] parameters)
        {
            _attributes = attributes;
            _parameters = parameters;
        }

        public ConstructorDefinitionBuilder SetCustomAttribute(CustomAttributeBuilder builder)
        {
            _customAttributes.Add(builder);
            return this;
        }

        public ConstructorDefinitionBuilder SetMethodImplAttributes(MethodImplAttributes attributes)
        {
            _methodImplAttributes = attributes;
            return this;
        }

        public ConstructorDefinitionBuilder SetCallingConvention(CallingConventions callingConvention)
        {
            _callingConvention = callingConvention;
            return this;
        }

        public ConstructorDefinitionBuilder SetBody(params Expression[] body)
        {
            _bodyExpressions = body;
            return this;
        }

        public ConstructorInfo Compile(TypeBuilder builder)
        {
            var ctorBuilder = DefineCtor(builder);

            MethodBodyBuilder.ForConstructor(ctorBuilder, _parameters)
                             .AddStatements(_bodyExpressions)
                             .Compile();

            return ctorBuilder;
        }

        public ConstructorInfo Compile(TypeBuilder builder, ITypeSymbolGenerator symbolGenerator)
        {
            var ctorBuilder = DefineCtor(builder);

            symbolGenerator.GenerateConstructor(_attributes, _parameters, _methodImplAttributes, _customAttributes,
                gen => MethodBodyBuilder.ForConstructor(gen, ctorBuilder, _parameters)
                    .AddStatements(_bodyExpressions)
                    .Compile());

            return ctorBuilder;
        }

        private ConstructorBuilder DefineCtor(TypeBuilder builder)
        {
            var ctorBuilder = builder.DefineConstructor(_attributes, _callingConvention, _parameters);
            foreach (var customAttribute in _customAttributes)
                ctorBuilder.SetCustomAttribute(customAttribute);
            ctorBuilder.SetImplementationFlags(_methodImplAttributes);
            return ctorBuilder;
        }
    }
}