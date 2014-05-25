using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.IO;
using System.Reflection.Emit;
using System.Text;
using CodeBuilder.Expressions;

namespace CodeBuilder
{
    public interface IMethodSymbolGenerator
    {
        void MarkSequencePoint(ILGenerator generator, int expressionId);
        IMethodSymbolGenerator Write(Expression expression);
        CodePosition GetCurrentPosition();
        IMethodSymbolGenerator Write(string debugCode);
        CodePosition WriteStatementEnd(string statementEnd);
        IMethodSymbolGenerator EnterScope();
        IMethodSymbolGenerator LeaveScope();
        IMethodSymbolGenerator WriteNamedBlock(string blockHeader, params Expression[] blockStatements);
    }

    class MethodSymbolGenerator : IMethodSymbolGenerator
    {
        private int _expressionIds = 0;
        private readonly IDictionary<int, CodeBlock> _debugLocations = new Dictionary<int, CodeBlock>();
        private readonly ISymbolDocumentWriter _documentWriter;
        private int _currentColumn;
        private readonly StringBuilder _writer;
        private int _currentLine = 0;
        private int _scope = 0;

        public MethodSymbolGenerator(ISymbolDocumentWriter documentWriter)
        {
            _documentWriter = documentWriter;
            _writer = new StringBuilder();
        }

        public void MarkSequencePoint(ILGenerator generator, int expressionId)
        {
            var loc = _debugLocations[expressionId];
            generator.MarkSequencePoint(_documentWriter, loc.Start.Line, loc.Start.Column, loc.End.Line, loc.End.Column);
        }

        public IMethodSymbolGenerator Write(Expression expression)
        {
            var id = _expressionIds++;
            _debugLocations.Add(id, expression.WriteDebugCode(this));
            return this;
        }

        public CodePosition GetCurrentPosition()
        {
            return new CodePosition(_currentLine, _currentColumn);
        }

        public IMethodSymbolGenerator Write(string debugCode)
        {
            if (debugCode.Contains("\n") || debugCode.Contains("\r"))
                throw new ArgumentException("Debug code cannot contain \n or \r characters");
            _currentColumn += debugCode.Length;
            _writer.Append(debugCode);
            return this;
        }

        public CodePosition WriteStatementEnd(string statementEnd)
        {
            Write(statementEnd);
            var pos = GetCurrentPosition();
            MoveToNewLine();
            return pos;
        }

        public IMethodSymbolGenerator EnterScope()
        {
            ++_scope;
            MoveToNewLine();
            return this;
        }

        public IMethodSymbolGenerator LeaveScope()
        {
            --_scope;
            MoveToNewLine();
            return this;
        }

        public IMethodSymbolGenerator WriteNamedBlock(string blockHeader, params Expression[] blockStatements)
        {
            WriteStatementEnd(blockHeader);
            if (blockStatements.Length == 1 && blockStatements[0] is BlockExpression)
                Write(blockStatements[0]);
            else
            {
                WriteStatementEnd("{");
                EnterScope();
                foreach (var statement in blockStatements)
                    Write(statement);
                LeaveScope();
                WriteStatementEnd("}");
            }
            return this;
        }

        private void MoveToNewLine()
        {
            _writer.AppendLine();
            ++_currentLine;
            _currentColumn = _scope;
            for (int i = 0; i < _scope; ++i)
                _writer.Append('\t');
        }
    }
}