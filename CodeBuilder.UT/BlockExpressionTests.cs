using System;
using System.Linq;
using NUnit.Framework;

namespace CodeBuilder.UT
{
    [TestFixture]
    public class BlockExpressionTests : BuilderTestBase
    {
        public class Counter
        {
            public int Value { get; private set; }
            public void Add()
            {
                ++Value;
            }
        }

        [Test]
        public void ExpressionTypeTest()
        {
            Assert.That(Expr.Block().ExpressionType, Is.EqualTo(typeof(void)));
        }

        [Test]
        public void Should_not_allow_null_expression_array()
        {
            Assert.Throws<ArgumentNullException>(() => Expr.Block(null));
        }

        [Test]
        public void Should_not_allow_null_expression_in_array()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => Expr.Block(Expr.Empty(), null, Expr.Empty()));
            Assert.That(ex.Message, Is.StringContaining("Value cannot be null."));
            Assert.That(ex.Message, Is.StringContaining("expressions[1]"));
        }

        [Test]
        public void Should_execute_all_statements_in_block()
        {
            var addMethod = typeof(Counter).GetMethod("Add");
            var func = CreateAction<Counter>(Expr.Block(
                Expr.Call(Expr.Parameter(0, typeof(Counter)), addMethod),
                Expr.Call(Expr.Parameter(0, typeof(Counter)), addMethod)));

            var cnt = new Counter();
            func(cnt);
            Assert.That(cnt.Value, Is.EqualTo(2));
        }

        [Test]
        public void Should_wrap_non_void_expressions_with_pop_expression()
        {
            var block = Expr.Block(Expr.Constant(32), Expr.Empty());
            Assert.That(block.Expressions.All(e => e.ExpressionType == typeof(void)));
        }
    }
}