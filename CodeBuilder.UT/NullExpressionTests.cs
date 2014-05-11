using System;
using NUnit.Framework;

namespace CodeBuilder.UT
{
    [TestFixture]
    public class NullExpressionTests : BuilderTestBase
    {
        [Test]
        public void ExpressionTypeTest()
        {
            Assert.That(Expr.Null(typeof(string)).ExpressionType, Is.EqualTo(typeof(string)));
            Assert.That(Expr.Null(typeof(Exception)).ExpressionType, Is.EqualTo(typeof(Exception)));
        }

        [Test]
        public void NullParamererTest()
        {
            Assert.Throws<ArgumentNullException>(() => Expr.Null(null));
        }

        [Test]
        public void Should_not_allow_non_reference_types()
        {
            var ex = Assert.Throws<ArgumentException>(() => Expr.Null(typeof(DateTime)));
            Assert.That(ex.Message, Is.StringContaining("Expected reference type, got: System.DateTime"));
        }

        [Test]
        public void Should_return_null()
        {
            Assert.That(NullFunc<string>(), Is.Null);
            Assert.That(NullFunc<object>(), Is.Null);
            Assert.That(NullFunc<IComparable>(), Is.Null);
        }

        private static T NullFunc<T>()
        {
            return CreateFunc<T>(Expr.Return(Expr.Null(typeof(T))))();
        }
    }
}