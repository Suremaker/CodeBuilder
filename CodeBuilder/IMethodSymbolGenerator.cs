using System.Reflection.Emit;
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
}