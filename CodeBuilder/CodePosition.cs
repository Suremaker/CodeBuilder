namespace CodeBuilder
{
    public struct CodePosition
    {
        public readonly int Line;
        public readonly int Column;

        public CodePosition(int line, int column)
            : this()
        {
            Line = line;
            Column = column;
        }

        public CodeBlock BlockTo(CodePosition other)
        {
            if (Line < other.Line || (Line == other.Line && Column < other.Column))
                return new CodeBlock(this, other);
            return new CodeBlock(other, this);
        }
    }

    public class CodeBlock
    {
        public CodePosition Start { get; private set; }
        public CodePosition End { get; private set; }

        public CodeBlock(CodePosition start, CodePosition end)
        {
            Start = start;
            End = end;
        }
    }
}