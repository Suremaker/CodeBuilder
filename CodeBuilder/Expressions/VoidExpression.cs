namespace CodeBuilder.Expressions
{
    public abstract class VoidExpression : Expression
    {
        protected VoidExpression() : base(typeof(void)) { }
    }
}