using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using CodeBuilder.Expressions;
using CodeBuilder.Symbols;

namespace CodeBuilder.Builders
{
    public class TypeInitializerDefinitionBuilder
    {
        private readonly IList<CustomAttributeBuilder> _customAttributes = new List<CustomAttributeBuilder>();
        private MethodImplAttributes _methodImplAttributes = MethodImplAttributes.Managed;
        private Expression[] _bodyExpressions = new Expression[0];

        public TypeInitializerDefinitionBuilder SetCustomAttribute(CustomAttributeBuilder builder)
        {
            _customAttributes.Add(builder);
            return this;
        }

        public TypeInitializerDefinitionBuilder SetMethodImplAttributes(MethodImplAttributes attributes)
        {
            _methodImplAttributes = attributes;
            return this;
        }

        public TypeInitializerDefinitionBuilder SetBody(params Expression[] body)
        {
            _bodyExpressions = body;
            return this;
        }

        public ConstructorInfo Compile(TypeBuilder builder)
        {
            var ctorBuilder = DefineCtor(builder);

            MethodBodyBuilder.ForConstructor(ctorBuilder)
                             .AddStatements(_bodyExpressions)
                             .Compile();

            return ctorBuilder;
        }

        public ConstructorInfo Compile(TypeBuilder builder, ITypeSymbolGenerator symbolGenerator)
        {
            var ctorBuilder = DefineCtor(builder);

            symbolGenerator.GenerateDefaultConstructor(_methodImplAttributes, _customAttributes, methodSymbolGenerator =>
                MethodBodyBuilder.ForConstructor(methodSymbolGenerator, ctorBuilder)
                                 .AddStatements(_bodyExpressions)
                                 .Compile());

            return ctorBuilder;
        }

        private ConstructorBuilder DefineCtor(TypeBuilder builder)
        {
            var ctorBuilder = builder.DefineTypeInitializer();
            foreach (var customAttribute in _customAttributes)
                ctorBuilder.SetCustomAttribute(customAttribute);
            ctorBuilder.SetImplementationFlags(_methodImplAttributes);
            return ctorBuilder;
        }
    }
}