using System.Collections.Generic;
using System.Reflection.Emit;

namespace CodeBuilder.Symbols
{
    public class SymbolGenerator : ISymbolGenerator
    {
        private readonly ModuleBuilder _moduleBuilder;
        private readonly IDictionary<TypeBuilder, ITypeSymbolGenerator> _symbolGenerators = new Dictionary<TypeBuilder, ITypeSymbolGenerator>();
        private readonly string _symbolDirectory;

        public SymbolGenerator(ModuleBuilder moduleBuilder, string symbolDirectory = ".")
        {
            _moduleBuilder = moduleBuilder;
            _symbolDirectory = symbolDirectory;
        }

        public ITypeSymbolGenerator GetTypeSymbolGenerator(TypeBuilder type)
        {
            ITypeSymbolGenerator generator;
            if (!_symbolGenerators.TryGetValue(type, out generator))
                _symbolGenerators.Add(type, generator = new TypeSymbolGenerator(type, _moduleBuilder, _symbolDirectory));
            return generator;
        }
    }
}