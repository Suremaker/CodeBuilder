using System;
using CodeBuilder.Context;
using NUnit.Framework;

namespace CodeBuilder.UT.Expressions
{
    [TestFixture]
    public class LoopBreakExpressionTests : BuilderTestBase
    {
        [Test]
        public void ExpressionTypeTest()
        {
            Assert.That(Expr.LoopBreak().ExpressionType, Is.EqualTo(typeof(void)));
        }

        [Test]
        public void Should_not_allow_continue_outside_loop()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => CreateAction(Expr.LoopBreak()));
            Assert.That(ex.Message, Is.EqualTo("Break expression can be used only inside loop"));
        }

        [Test]
        public void Should_not_allow_continue_in_finally_block()
        {
            var ex = Assert.Throws<ScopeChangeException>(() => CreateAction(Expr.Loop(Expr.TryFinally(Expr.Empty(), Expr.LoopBreak()))));
            Assert.That(ex.Message, Is.EqualTo("Loop break expression is forbidden in finally block scope"));
        }

        [Test]
        [Ignore("Not implemented yet")]
        public void Should_allow_break_in_catch_block()
        {
            var loc = Expr.LocalVariable(typeof(int), "i");
            var func = CreateFunc<int>(
                Expr.WriteLocal(loc, Expr.Constant(0)),
                Expr.Loop(Expr.TryCatch(
                    Expr.Block(
                        Expr.WriteLocal(loc, Expr.Add(Expr.ReadLocal(loc), Expr.Constant(1))),
                        Expr.Throw(Expr.New(typeof (Exception)))),
                    new CatchBlock(Expr.LoopBreak()))),
                Expr.Return(Expr.ReadLocal(loc)));
            Assert.That(func(), Is.EqualTo(2));
        }
    }
}