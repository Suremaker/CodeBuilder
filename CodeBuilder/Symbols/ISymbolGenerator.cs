using System.Reflection.Emit;

namespace CodeBuilder.Symbols
{
    public interface ISymbolGenerator
    {
        ITypeSymbolGenerator GetTypeSymbolGenerator(TypeBuilder type);
    }
}