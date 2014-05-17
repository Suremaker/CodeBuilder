using NUnit.Framework;

namespace CodeBuilder.UT.Expressions
{
    [TestFixture]
    public class EmptyExpressionTests : BuilderTestBase
    {
        [Test]
        public void ExpressionTypeTest()
        {
            Assert.That(Expr.Empty().ExpressionType, Is.EqualTo(typeof(void)));
        }
    }
}