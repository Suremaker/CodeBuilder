using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Reflection.Emit;
using CodeBuilder.Expressions;

namespace CodeBuilder.Symbols
{
    internal class MethodSymbolGenerator : IMethodSymbolGenerator
    {
        private int _expressionIds;
        private readonly IDictionary<int, CodeBlock> _debugLocations = new Dictionary<int, CodeBlock>();
        private readonly ISymbolDocumentWriter _symbolWriter;
        private readonly CodeWriter _codeWriter;

        public MethodSymbolGenerator(ISymbolDocumentWriter symbolWriter, CodeWriter codeWriter)
        {
            _symbolWriter = symbolWriter;
            _codeWriter = codeWriter;
        }

        public void MarkSequencePoint(ILGenerator generator, int expressionId)
        {
            var loc = _debugLocations[expressionId];
            generator.MarkSequencePoint(_symbolWriter, loc.Start.Line, loc.Start.Column, loc.End.Line, loc.End.Column);
        }

        public IMethodSymbolGenerator Write(Expression expression)
        {
            var id = _expressionIds++;
            _debugLocations.Add(id, expression.WriteDebugCode(this));
            return this;
        }

        public CodePosition GetCurrentPosition()
        {
            return new CodePosition(_codeWriter.CurrentLine, _codeWriter.CurrentColumn);
        }

        public IMethodSymbolGenerator Write(string code)
        {
            _codeWriter.Write(code);
            return this;
        }

        public CodePosition WriteStatementEnd(string statementEnd)
        {
            Write(statementEnd);
            var pos = GetCurrentPosition();
            _codeWriter.WriteNewLine();
            return pos;
        }

        public IMethodSymbolGenerator EnterScope()
        {
            _codeWriter.EnterScope();
            return this;
        }

        public IMethodSymbolGenerator LeaveScope()
        {
            _codeWriter.LeaveScope();
            return this;
        }

        public IMethodSymbolGenerator WriteNamedBlock(string blockHeader, params Expression[] blockStatements)
        {
            WriteStatementEnd(blockHeader);
            if (blockStatements.Length == 1 && blockStatements[0] is BlockExpression)
                Write(blockStatements[0]);
            else
            {
                Write("{");
                EnterScope();
                foreach (var statement in blockStatements)
                    Write(statement);
                LeaveScope();
                WriteStatementEnd("}");
            }
            return this;
        }
    }
}