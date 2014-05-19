using System;
using CodeBuilder.Context;
using NUnit.Framework;

namespace CodeBuilder.UT.Scenarios
{
    [TestFixture]
    public class ScopeChangeTests : BuilderTestBase
    {
        [Test]
        public void Should_allow_inner_loop_with_break_and_continue_in_value_block()
        {
            var i = Expr.LocalVariable(typeof(int), "i");
            var v = Expr.LocalVariable(typeof(int), "v");
            var func = CreateFunc<int, int>(
                Expr.Return(Expr.ValueBlock(typeof(int),
                    Expr.DeclareLocal(i, Expr.Parameter(0, typeof(int))),
                    Expr.DeclareLocal(v, Expr.Constant(0)),
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
            var ex = Assert.Throws<ScopeChangeException>(() => CreateAction(
                Expr.Loop(
                Expr.ValueBlock(typeof(int),
                    Expr.LoopBreak(),
                    Expr.Constant(1)))));
            Assert.That(ex.Message, Is.EqualTo("Loop break expression is forbidden in value block scope"));
        }

        [Test]
        public void Should_not_allow_leave_value_block_with_loop_continue()
        {
            var ex = Assert.Throws<ScopeChangeException>(() => CreateAction(
                Expr.Loop(
                Expr.ValueBlock(typeof(int),
                    Expr.LoopContinue(),
                    Expr.Constant(1)))));
            Assert.That(ex.Message, Is.EqualTo("Loop continue expression is forbidden in value block scope"));
        }

        [Test]
        public void Should_not_allow_leave_value_block_with_return()
        {
            var ex = Assert.Throws<ScopeChangeException>(() => CreateAction(
                Expr.ValueBlock(typeof(int),
                    Expr.Return(),
                    Expr.Constant(1))));
            Assert.That(ex.Message, Is.EqualTo("Return expression is forbidden in value block scope"));
        }

        [Test]
        public void Should_allow_inner_loop_with_break_continue_in_catch_block()
        {
            var result = Expr.LocalVariable(typeof(int), "r");
            var func = CreateFunc<int>(
                Expr.DeclareLocal(result, Expr.Constant(0)),
                Expr.TryCatch(
                        Expr.Throw(Expr.New(typeof(Exception))),
                    new CatchBlock(Expr.Block(
                        Expr.Loop(Expr.Block(
                            Expr.WriteLocal(result, Expr.Add(Expr.ReadLocal(result), Expr.Constant(1))),
                            Expr.IfThen(
                                Expr.Less(Expr.ReadLocal(result), Expr.Constant(2)),
                                Expr.LoopContinue()),
                            Expr.LoopBreak()))))),
                Expr.Return(Expr.ReadLocal(result)));

            Assert.That(func(), Is.EqualTo(2));
        }

        [Test]
        public void Should_not_allow_to_leave_catch_block_with_loop_break()
        {
            var ex = Assert.Throws<ScopeChangeException>(() => CreateAction(
                Expr.Loop(
                Expr.TryCatch(
                    Expr.Throw(Expr.New(typeof(Exception))),
                    new CatchBlock(Expr.LoopBreak())))));
            Assert.That(ex.Message, Is.EqualTo("Loop break expression is forbidden in catch block scope"));
        }

        [Test]
        public void Should_not_allow_to_leave_try_block_with_loop_break()
        {
            var ex = Assert.Throws<ScopeChangeException>(() => CreateAction(
                Expr.Loop(
                Expr.TryCatch(
                    Expr.LoopBreak(),
                    new CatchBlock(Expr.Empty())))));
            Assert.That(ex.Message, Is.EqualTo("Loop break expression is forbidden in try block scope"));
        }

        [Test]
        public void Should_not_allow_to_leave_finally_block_with_loop_break()
        {
            var ex = Assert.Throws<ScopeChangeException>(() => CreateAction(
                Expr.Loop(
                Expr.TryFinally(
                    Expr.Throw(Expr.New(typeof(Exception))),
                    Expr.LoopBreak()))));
            Assert.That(ex.Message, Is.EqualTo("Loop break expression is forbidden in finally block scope"));
        }

        [Test]
        public void Should_not_allow_to_leave_try_block_with_loop_continue()
        {
            var ex = Assert.Throws<ScopeChangeException>(() => CreateAction(
                Expr.Loop(
                Expr.TryCatch(
                    Expr.LoopContinue(),
                    new CatchBlock(Expr.Empty())))));
            Assert.That(ex.Message, Is.EqualTo("Loop continue expression is forbidden in try block scope"));
        }

        [Test]
        public void Should_not_allow_to_leave_finally_block_with_loop_continue()
        {
            var ex = Assert.Throws<ScopeChangeException>(() => CreateAction(
                Expr.Loop(
                Expr.TryFinally(
                    Expr.Throw(Expr.New(typeof(Exception))),
                    Expr.LoopContinue()))));
            Assert.That(ex.Message, Is.EqualTo("Loop continue expression is forbidden in finally block scope"));
        }

        [Test]
        public void Should_not_allow_to_leave_try_block_with_return()
        {
            var ex = Assert.Throws<ScopeChangeException>(() => CreateAction(
                Expr.Loop(
                Expr.TryCatch(
                    Expr.Return(),
                    new CatchBlock(Expr.Empty())))));
            Assert.That(ex.Message, Is.EqualTo("Return expression is forbidden in try block scope"));
        }

        [Test]
        public void Should_not_allow_to_leave_finally_block_with_return()
        {
            var ex = Assert.Throws<ScopeChangeException>(() => CreateAction(
                Expr.Loop(
                Expr.TryFinally(
                    Expr.Throw(Expr.New(typeof(Exception))),
                    Expr.Return()))));
            Assert.That(ex.Message, Is.EqualTo("Return expression is forbidden in finally block scope"));
        }
    }
}
