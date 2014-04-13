using System.Reflection.Emit;

namespace CodeBuilder.Context
{
    public class LoopData
    {
        public Label ContinueLabel { get; private set; }
        public Label BreakLabel { get; private set; }

        public LoopData(Label continueLabel, Label breakLabel)
        {
            ContinueLabel = continueLabel;
            BreakLabel = breakLabel;
        }

        protected bool Equals(LoopData other)
        {
            return ContinueLabel.Equals(other.ContinueLabel) && BreakLabel.Equals(other.BreakLabel);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((LoopData)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (ContinueLabel.GetHashCode() * 397) ^ BreakLabel.GetHashCode();
            }
        }
    }
}