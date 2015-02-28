namespace CodeBuilder.Symbols
{
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