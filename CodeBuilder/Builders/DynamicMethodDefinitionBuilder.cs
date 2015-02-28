using System;
using System.Reflection;
using System.Reflection.Emit;
using CodeBuilder.Expressions;

namespace CodeBuilder.Builders
{
    public class DynamicMethodDefinitionBuilder
    {
        private readonly string _name;
        private readonly Type _returnType;
        private readonly Type[] _parameters;
        private Expression[] _bodyExpressions = new Expression[0];

        public DynamicMethodDefinitionBuilder(string name, Type returnType, params Type[] parameters)
        {
            _name = name;
            _returnType = returnType;
            _parameters = parameters;
        }

        public DynamicMethodDefinitionBuilder SetBody(params Expression[] body)
        {
            _bodyExpressions = body;
            return this;
        }

        public DynamicMethod Compile(bool skipVisibility = false)
        {
            var dynamicMethod = new DynamicMethod(_name, _returnType, _parameters, skipVisibility);
            MethodBodyBuilder.ForDynamicMethod(dynamicMethod, _parameters).AddStatements(_bodyExpressions).Compile();
            return dynamicMethod;
        }

        public DynamicMethod Compile(Type owner, bool skipVisibility = false, MethodAttributes attributes = MethodAttributes.Public|MethodAttributes.Static, CallingConventions callingConventions = CallingConventions.Standard)
        {
            var dynamicMethod = new DynamicMethod(_name, attributes, callingConventions, _returnType, _parameters, owner, skipVisibility);
            MethodBodyBuilder.ForDynamicMethod(dynamicMethod, _parameters).AddStatements(_bodyExpressions).Compile();
            return dynamicMethod;
        }

        public DynamicMethod Compile(Module owner, bool skipVisibility = false, MethodAttributes attributes = MethodAttributes.Public|MethodAttributes.Static, CallingConventions callingConventions = CallingConventions.Standard)
        {
            var dynamicMethod = new DynamicMethod(_name, attributes, callingConventions, _returnType, _parameters, owner, skipVisibility);
            MethodBodyBuilder.ForDynamicMethod(dynamicMethod, _parameters).AddStatements(_bodyExpressions).Compile();
            return dynamicMethod;
        }
    }
}