using System;
using System.IO;
using System.Reflection;
using CodeBuilder.Helpers;

namespace CodeBuilder.Symbols
{
    internal class CodeWriter : IDisposable
    {
        private readonly TextWriter _writer;

        public CodeWriter(TextWriter writer)
        {
            _writer = writer;
        }

        public int CurrentLine { get; private set; }
        public int CurrentColumn { get; private set; }
        public int Scope { get; private set; }

        public CodeWriter Write(string text)
        {
            if (text.Contains("\n") || text.Contains("\r"))
                throw new ArgumentException("Written text cannot contain \n or \r characters");
            CurrentColumn += text.Length;
            _writer.Write(text);
            return this;
        }

        public CodeWriter WriteLine(string textFormat, params object[] args)
        {
            return Write(string.Format(textFormat, args)).WriteNewLine();
        }

        public CodeWriter EnterScope()
        {
            ++Scope;
            return WriteNewLine();
        }

        public CodeWriter LeaveScope()
        {
            --Scope;
            return WriteNewLine();
        }

        public CodeWriter WriteNewLine()
        {
            _writer.WriteLine();
            ++CurrentLine;
            CurrentColumn = Scope;
            for (int i = 0; i < Scope; ++i)
                _writer.Write('\t');
            return this;
        }

        public CodeWriter WriteTypeAttributes(Type type)
        {
            var attributes = type.Attributes;
            if (EnumHelper.IsSet(attributes, TypeAttributes.Public))
                Write("public ");
            else if (EnumHelper.IsSet(attributes, TypeAttributes.NotPublic))
                Write("internal ");

            if (EnumHelper.IsSet(attributes, TypeAttributes.Abstract) && EnumHelper.IsSet(attributes, TypeAttributes.Sealed))
                Write("static ");
            else if (EnumHelper.IsSet(attributes, TypeAttributes.Abstract))
                Write("abstract ");
            else if (EnumHelper.IsSet(attributes, TypeAttributes.Sealed))
                Write("sealed ");

            if (EnumHelper.IsSet(attributes, TypeAttributes.Interface))
                Write("interface");
            else
                Write(type.IsValueType ? "struct" : "class");

            return this;
        }

        public CodeWriter WriteSpace()
        {
            return Write(" ");
        }

        public void Dispose()
        {
            _writer.Dispose();
        }

        public CodeWriter Write(Type type)
        {
            return Write(type.FullName);
        }

        public CodeWriter Write(MethodAttributes attributes)
        {
            if (EnumHelper.IsSet(attributes, MethodAttributes.Public))
                Write("public");
            else if (EnumHelper.IsSet(attributes, MethodAttributes.FamORAssem))
                Write("internal protected");
            else if (EnumHelper.IsSet(attributes, MethodAttributes.Private))
                Write("private");
            else if (EnumHelper.IsSet(attributes, MethodAttributes.Family))
                Write("protected");
            else if (EnumHelper.IsSet(attributes, MethodAttributes.Assembly))
                Write("internal");
            return this;
        }
    }
}