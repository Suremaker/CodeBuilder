using System;
using NUnit.Framework;

namespace CodeBuilder.UT
{
    [TestFixture]
    public class RethrowExpressionTests : BuilderTestBase
    {
        [Test]
        public void VoidTypeTest()
        {
            Assert.That(Expr.Rethrow().ExpressionType, Is.EqualTo(typeof(void)));
        }

        [Test]
        public void Should_not_allow_rethrow_being_outside_of_catch_block()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => CreateAction(Expr.TryFinally(Expr.Rethrow(), Expr.Empty())));
            Assert.That(ex.Message, Is.StringContaining("Unable to rethrow - not in catch block."));

            ex = Assert.Throws<InvalidOperationException>(() => CreateAction(Expr.TryFinally(Expr.Empty(), Expr.Rethrow())));
            Assert.That(ex.Message, Is.StringContaining("Unable to rethrow - not in catch block."));
        }

        [Test]
        public void Should_rethrow_from_nested_block()
        {
            var func = CreateAction(Expr.TryCatch(
                Expr.Throw(Expr.New(typeof(Exception))),
                new CatchBlock(Expr.TryFinally(Expr.Rethrow(), Expr.Empty()))));

            Assert.Throws<Exception>(() => func());
        }
    }
}