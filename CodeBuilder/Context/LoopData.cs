namespace CodeBuilder.Context
{
    public class LoopData
    {
        public JumpLabel ContinueLabel { get; private set; }
        public JumpLabel BreakLabel { get; private set; }

        public LoopData(JumpLabel continueLabel, JumpLabel breakLabel)
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
            if (obj.GetType() != GetType()) return false;
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