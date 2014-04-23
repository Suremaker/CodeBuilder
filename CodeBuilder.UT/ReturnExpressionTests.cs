using System;
using NUnit.Framework;

namespace CodeBuilder.UT
{
    [TestFixture]
    public class ReturnExpressionTests : BuilderTestBase
    {
        public class BaseType { }
        public class DerivedType : BaseType { }

        [Test]
        public void VoidTypeTest()
        {
            Assert.That(Expr.Return().ExpressionType, Is.EqualTo(typeof(void)));
        }

        [Test]
        public void ParameterTypeTest()
        {
            Assert.That(Expr.Return(Expr.Constant(3.14f)).ExpressionType, Is.EqualTo(typeof(void)));
        }

        [Test]
        public void NullParamererTest()
        {
            Assert.Throws<ArgumentNullException>(() => Expr.Return(null));
        }

        [Test]
        public void Should_return_value()
        {
            const string value = "123";
            var func = CreateFunc<string>(Expr.Return(Expr.Constant(value)));
            Assert.That(func(), Is.EqualTo(value));
        }

        [Test]
        public void Should_return_without_value()
        {
            var action = CreateAction(Expr.Return());
            Assert.DoesNotThrow(() => action());
        }

        [Test]
        public void Should_not_allow_returning_wrong_type()
        {
            var ex = Assert.Throws<ArgumentException>(() => CreateFunc<string>(Expr.Return()));
            Assert.That(ex.Message, Is.StringContaining("Method return type is System.Void, while return statement is returning System.String"));
        }

        [Test]
        public void Should_return_polymorphic_type()
        {
            var func = CreateFunc<BaseType>(Expr.Return(Expr.New(typeof(DerivedType))));
            Assert.That(func(), Is.TypeOf<DerivedType>());
        }

        [Test]
        public void Should_not_allow_returning_from_finally_block()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => CreateAction(Expr.TryFinally(Expr.Empty(), Expr.Return())));
            Assert.That(ex.Message, Is.StringContaining("Return expression is forbidden in finally blocks"));
        }

        [Test]
        [Ignore("Not implemented yet")]
        public void Should_return_from_catch_block()
        {
            const int expected = 32;

            var func = CreateFunc<int>(Expr.TryCatch(
                Expr.Throw(Expr.New(typeof(Exception), Expr.Constant("reason"))),
                new CatchBlock(Expr.Return(Expr.Constant(expected)))),
                Expr.Return(Expr.Constant(21)));
            Assert.That(func(), Is.EqualTo(expected));
        }
    }
}