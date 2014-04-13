using System;
using CodeBuilder.Helpers;

namespace CodeBuilder
{
    public sealed class LocalVariable
    {
        public Type VariableType { get; private set; }
        public string Name { get; private set; }

        public LocalVariable(Type variableType, string name)
        {
            Validators.NullCheck(variableType, "variableType");
            Validators.NullCheck(name, "name");
            VariableType = variableType;
            Name = name;
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", VariableType, Name);
        }

        private bool Equals(LocalVariable other)
        {
            return VariableType == other.VariableType && string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is LocalVariable && Equals((LocalVariable) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((VariableType != null ? VariableType.GetHashCode() : 0)*397) ^ (Name != null ? Name.GetHashCode() : 0);
            }
        }
    }
}