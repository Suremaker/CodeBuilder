using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;

namespace CodeBuilder.Symbols
{
    internal class TypeSymbolGenerator : ITypeSymbolGenerator
    {
        private readonly TypeBuilder _type;
        private readonly ISymbolDocumentWriter _symbolWriter;
        private readonly CodeWriter _codeWriter;

        public TypeSymbolGenerator(TypeBuilder type, ModuleBuilder moduleBuilder, string symbolDirectory)
        {
            _type = type;
            var codePath = Path.Combine(symbolDirectory, type.Name + ".cs");
            _symbolWriter = moduleBuilder.DefineDocument(codePath, Guid.Empty, Guid.Empty, Guid.Empty);
            _codeWriter = new CodeWriter(new StreamWriter(new FileStream(codePath, FileMode.Create, FileAccess.Write)));
            WriteNamespace();
            WriteType();
        }

        private void WriteType()
        {
            _codeWriter
                .WriteTypeAttributes(_type)
                .WriteSpace()
                .Write(_type.Name);

            var hasBase = _type.BaseType != typeof(object);
            var interfaces = _type.GetInterfaces();
            var hasInterfaces = interfaces.Length > 0;

            if (hasBase || hasInterfaces)
                _codeWriter.Write(": ");

            if (hasBase)
                _codeWriter.Write(_type.BaseType.FullName);

            for (int i = 0; i < interfaces.Length; ++i)
            {
                if (i > 0 || hasBase)
                    _codeWriter.Write(", ");
                _codeWriter.Write(interfaces[i].FullName);
            }

            _codeWriter
                .WriteNewLine()
                .Write("{")
                .EnterScope();
        }

        private void WriteNamespace()
        {
            _codeWriter
                .WriteLine("namespace {0}", _type.Namespace)
                .Write("{")
                .EnterScope();
        }

        public void GenerateMethod(MethodAttributes attributes, Type returnType, string name, Type[] parameters, MethodImplAttributes methodImplAttributes, IEnumerable<CustomAttributeBuilder> customAttributes, MethodGenerator methodGenerator)
        {
            _codeWriter
                .Write(attributes)
                .WriteSpace()
                .Write(returnType)
                .WriteSpace()
                .Write(name)
                .Write("(");
            WriteParameters(parameters);
            _codeWriter
                .WriteLine(")")
                .Write("{")
                .EnterScope();
            methodGenerator.Invoke(CreateMethodSymbolGenerator());
            _codeWriter
                .LeaveScope()
                .Write("}")
                .WriteNewLine();
        }

        public void GenerateConstructor(MethodAttributes attributes, Type[] parameters, MethodImplAttributes methodImplAttributes, IList<CustomAttributeBuilder> customAttributes, MethodGenerator methodGenerator)
        {
            _codeWriter
                .Write(attributes)
                .WriteSpace()
                .Write(_type.Name)
                .Write("(");
            WriteParameters(parameters);
            _codeWriter
                .WriteLine(")")
                .Write("{")
                .EnterScope();
            methodGenerator.Invoke(CreateMethodSymbolGenerator());
            _codeWriter
                .LeaveScope()
                .Write("}")
                .WriteNewLine();
        }

        public void GenerateDefaultConstructor(MethodImplAttributes methodImplAttributes, IList<CustomAttributeBuilder> customAttributes, MethodGenerator methodGenerator)
        {
            _codeWriter
                .Write("static")
                .WriteSpace()
                .Write(_type.Name)
                .WriteLine("()")
                .Write("{")
                .EnterScope();
            methodGenerator.Invoke(CreateMethodSymbolGenerator());
            _codeWriter
                .LeaveScope()
                .Write("}")
                .WriteNewLine();
        }

        private MethodSymbolGenerator CreateMethodSymbolGenerator()
        {
            return new MethodSymbolGenerator(_symbolWriter, _codeWriter);
        }

        private void WriteParameters(Type[] parameters)
        {
            for (int i = 0; i < parameters.Length; ++i)
            {
                if (i > 0)
                    _codeWriter.Write(", ");
                _codeWriter.Write(parameters[i]).WriteSpace().Write("p" + (i + 1));
            }
        }

        public void Dispose()
        {
            _codeWriter
                .Write("}").LeaveScope()
                .Write("}").LeaveScope();
            _codeWriter.Dispose();
        }
    }
}