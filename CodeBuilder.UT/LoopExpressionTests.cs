using System;
using NUnit.Framework;

namespace CodeBuilder.UT
{
    [TestFixture]
    public class LoopExpressionTests : BuilderTestBase
    {
        [Test]
        public void ExpressionTypeTest()
        {
            Assert.That(Expr.Loop(Expr.Empty()).ExpressionType, Is.EqualTo(typeof(void)));
        }

        [Test]
        public void ExpressionTypeForNotNullExpressionTest()
        {
            Assert.That(Expr.Loop(Expr.Constant(1)).ExpressionType, Is.EqualTo(typeof(void)));
        }

        [Test]
        public void NullLoopExpressionTest()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => Expr.Loop(null));
            Assert.That(ex.Message, Is.StringContaining("loop"));
        }

        [Test]
        public void Should_iterate_n_times()
        {
            var loc = Expr.DeclareLocalVar(typeof(int), "i");
            var func = CreateFunc<int, int>(
                Expr.WriteLocal(loc, Expr.Constant(0)),
                Expr.Loop(Expr.IfThenElse(
                    Expr.Equal(Expr.ReadLocal(loc), Expr.Parameter(0, typeof(int))),
                    Expr.LoopBreak(),
                    Expr.WriteLocal(loc, Expr.Add(Expr.ReadLocal(loc), Expr.Constant(1))))),
                Expr.Return(Expr.ReadLocal(loc)));

            Assert.That(func(5), Is.EqualTo(5));
        }

        [Test]
        public void Should_iterate_n_times_with_continue()
        {
            var loc = Expr.DeclareLocalVar(typeof(int), "i");
            var func = CreateFunc<int, int>(
                Expr.WriteLocal(loc, Expr.Constant(0)),
                Expr.Loop(Expr.Block(
                    Expr.WriteLocal(loc, Expr.Add(Expr.ReadLocal(loc), Expr.Constant(1))),
                    Expr.IfThen(
                        Expr.Less(Expr.ReadLocal(loc), Expr.Parameter(0, typeof(int))),
                        Expr.LoopContinue()),
                    Expr.LoopBreak())),
                Expr.Return(Expr.ReadLocal(loc)));
            Assert.That(func(5), Is.EqualTo(5));
        }

        [Test]
        public void Should_iterate_n_times_with_return()
        {
            var loc = Expr.DeclareLocalVar(typeof(int), "i");
            var func = CreateFunc<int, int>(
                Expr.WriteLocal(loc, Expr.Constant(0)),
                Expr.Loop(Expr.Block(
                    Expr.WriteLocal(loc, Expr.Add(Expr.ReadLocal(loc), Expr.Constant(1))),
                    Expr.IfThen(
                        Expr.Equal(Expr.ReadLocal(loc), Expr.Parameter(0, typeof(int))),
                        Expr.Return(Expr.ReadLocal(loc))))),
                Expr.Return(Expr.Constant(0)));
            Assert.That(func(5), Is.EqualTo(5));
        }
    }
}