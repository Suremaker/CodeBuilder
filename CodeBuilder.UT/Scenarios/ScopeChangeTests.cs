using System;
using NUnit.Framework;

namespace CodeBuilder.UT.Scenarios
{
    [TestFixture]
    public class ScopeChangeTests : BuilderTestBase
    {
        [Test]
        [Ignore("WIP")]
        public void Should_allow_inner_loop_with_break_and_continue_in_value_block()
        {
            var i = Expr.DeclareLocalVar(typeof(int), "i");
            var v = Expr.DeclareLocalVar(typeof(int), "v");
            var func = CreateFunc<int, int>(
                Expr.Return(Expr.ValueBlock(typeof(int),
                    Expr.WriteLocal(i, Expr.Parameter(0, typeof(int))),
                    Expr.WriteLocal(v, Expr.Constant(0)),
                    Expr.Loop(Expr.Block(
                        Expr.IfThen(
                            Expr.ReadLocal(i),
                            Expr.Block(
                                Expr.WriteLocal(v, Expr.Add(Expr.ReadLocal(v), Expr.ReadLocal(i))),
                                Expr.WriteLocal(i, Expr.Add(Expr.ReadLocal(i), Expr.Constant(-1))),
                                Expr.LoopContinue())),
                        Expr.LoopBreak())),
                    Expr.ReadLocal(v))));

            Assert.That(func(5), Is.EqualTo(15));
        }

        [Test]
        public void Should_not_allow_leave_value_block_with_loop_break()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => CreateAction(
                Expr.Loop(
                Expr.ValueBlock(typeof(int),
                    Expr.LoopBreak(),
                    Expr.Constant(1)))));
            Assert.That(ex.Message, Is.EqualTo("Break expression is forbidden in value blocks"));
        }

        [Test]
        public void Should_not_allow_leave_value_block_with_loop_continue()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => CreateAction(
                Expr.Loop(
                Expr.ValueBlock(typeof(int),
                    Expr.LoopContinue(),
                    Expr.Constant(1)))));
            Assert.That(ex.Message, Is.EqualTo("Continue expression is forbidden in value blocks"));
        }

        [Test]
        public void Should_not_allow_leave_value_block_with_return()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => CreateAction(
                Expr.ValueBlock(typeof(int),
                    Expr.Return(),
                    Expr.Constant(1))));
            Assert.That(ex.Message, Is.EqualTo("Return expression is forbidden in value blocks"));
        }

        [Test]
        [Ignore("WIP")]
        public void Should_allow_inner_loop_with_break_in_catch_block()
        {
            var result = Expr.DeclareLocalVar(typeof(string), "r");
            var func = CreateFunc<string>(
                Expr.TryCatch(
                    Expr.Throw(Expr.New(typeof(Exception))),
                    new CatchBlock(Expr.Block(
                        Expr.Loop(Expr.Block(
                            Expr.WriteLocal(result, Expr.Constant("abc")),
                            Expr.LoopBreak()))))),
                Expr.Return(Expr.ReadLocal(result)));

            Assert.That(func(), Is.EqualTo("abc"));
        }

        [Test]
        [Ignore("WIP")]
        public void Should_allow_inner_loop_with_continue_in_catch_block()
        {
            var result = Expr.DeclareLocalVar(typeof(string), "r");
            var func = CreateFunc<string>(
                Expr.TryCatch(
                    Expr.IfThen(Expr.Equal(Expr.ReadLocal(result),
                        Expr.Null(typeof(string))),
                        Expr.Throw(Expr.New(typeof(Exception)))),
                    new CatchBlock(Expr.Block(
                        Expr.Loop(Expr.Block(
                            Expr.WriteLocal(result, Expr.Constant("abc")),
                            Expr.LoopContinue()))))),
                Expr.Return(Expr.ReadLocal(result)));

            Assert.That(func(), Is.EqualTo("abc"));
        }

        [Test]
        public void Should_not_allow_to_leave_catch_block_with_loop_break()
        {
            var ex = Assert.Throws<NotSupportedException>(() => CreateAction(
                Expr.Loop(
                Expr.TryCatch(
                    Expr.Throw(Expr.New(typeof(Exception))),
                    new CatchBlock(Expr.LoopBreak())))));
            Assert.That(ex.Message, Is.EqualTo("Break expression in try-catch blocks is not supported"));
        }

        [Test]
        public void Should_not_allow_to_leave_try_block_with_loop_break()
        {
            var ex = Assert.Throws<NotSupportedException>(() => CreateAction(
                Expr.Loop(
                Expr.TryCatch(
                    Expr.LoopBreak(),
                    new CatchBlock(Expr.Empty())))));
            Assert.That(ex.Message, Is.EqualTo("Break expression in try-catch blocks is not supported"));
        }

        [Test]
        public void Should_not_allow_to_leave_finally_block_with_loop_break()
        {
            var ex = Assert.Throws<NotSupportedException>(() => CreateAction(
                Expr.Loop(
                Expr.TryCatch(
                    Expr.Throw(Expr.New(typeof(Exception))),
                    new CatchBlock(Expr.LoopBreak())))));
            Assert.That(ex.Message, Is.EqualTo("Break expression in try-catch blocks is not supported"));
        }

        [Test]
        public void Should_not_allow_to_leave_try_block_with_loop_continue()
        {
            var ex = Assert.Throws<NotSupportedException>(() => CreateAction(
                Expr.Loop(
                Expr.TryCatch(
                    Expr.LoopContinue(),
                    new CatchBlock(Expr.Empty())))));
            Assert.That(ex.Message, Is.EqualTo("Continue expression in try-catch blocks is not supported"));
        }

        [Test]
        public void Should_not_allow_to_leave_finally_block_with_loop_continue()
        {
            var ex = Assert.Throws<NotSupportedException>(() => CreateAction(
                Expr.Loop(
                Expr.TryCatch(
                    Expr.Throw(Expr.New(typeof(Exception))),
                    new CatchBlock(Expr.LoopContinue())))));
            Assert.That(ex.Message, Is.EqualTo("Continue expression in try-catch blocks is not supported"));
        }

        [Test]
        public void Should_not_allow_to_leave_try_block_with_return()
        {
            var ex = Assert.Throws<NotSupportedException>(() => CreateAction(
                Expr.Loop(
                Expr.TryCatch(
                    Expr.Return(),
                    new CatchBlock(Expr.Empty())))));
            Assert.That(ex.Message, Is.EqualTo("Return expression in try-catch blocks is not supported"));
        }

        [Test]
        public void Should_not_allow_to_leave_finally_block_with_return()
        {
            var ex = Assert.Throws<NotSupportedException>(() => CreateAction(
                Expr.Loop(
                Expr.TryCatch(
                    Expr.Throw(Expr.New(typeof(Exception))),
                    new CatchBlock(Expr.Return())))));
            Assert.That(ex.Message, Is.EqualTo("Return expression in try-catch blocks is not supported"));
        }
    }
}
