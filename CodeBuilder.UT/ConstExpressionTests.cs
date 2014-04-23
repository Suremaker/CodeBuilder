using NUnit.Framework;

namespace CodeBuilder.UT
{
    [TestFixture]
    public class ConstExpressionTests : BuilderTestBase
    {
        [Test]
        public void IntTypeTest()
        {
            Assert.That(Expr.Constant(1).ExpressionType, Is.EqualTo(typeof(int)));
        }

        [Test]
        public void LongTypeTest()
        {
            Assert.That(Expr.Constant(long.MaxValue).ExpressionType, Is.EqualTo(typeof(long)));
        }

        [Test]
        public void FloatTypeTest()
        {
            Assert.That(Expr.Constant(float.MaxValue).ExpressionType, Is.EqualTo(typeof(float)));
        }

        [Test]
        public void DoubleTypeTest()
        {
            Assert.That(Expr.Constant(double.MaxValue).ExpressionType, Is.EqualTo(typeof(double)));
        }

        [Test]
        public void StringTypeTest()
        {
            Assert.That(Expr.Constant("abc").ExpressionType, Is.EqualTo(typeof(string)));
        }

        [Test]
        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        [TestCase(6)]
        [TestCase(7)]
        [TestCase(8)]
        [TestCase(9)]
        [TestCase(sbyte.MinValue)]
        [TestCase(sbyte.MaxValue)]
        [TestCase(int.MinValue)]
        [TestCase(int.MaxValue)]
        public void Should_return_const_int(int value)
        {
            var func = CreateFunc<int>(Expr.Return(Expr.Constant(value)));
            Assert.That(func(), Is.EqualTo(value));
        }

        [Test]
        [TestCase(-1)]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(5)]
        [TestCase(6)]
        [TestCase(7)]
        [TestCase(8)]
        [TestCase(9)]
        [TestCase(sbyte.MinValue)]
        [TestCase(sbyte.MaxValue)]
        [TestCase(int.MinValue)]
        [TestCase(int.MaxValue)]
        [TestCase(long.MinValue)]
        [TestCase(long.MaxValue)]
        public void Should_return_const_long(long value)
        {
            var func = CreateFunc<long>(Expr.Return(Expr.Constant(value)));
            Assert.That(func(), Is.EqualTo(value));
        }

        [Test]
        public void Should_return_const_float()
        {
            const float value = 3.14f;
            var func = CreateFunc<float>(Expr.Return(Expr.Constant(value)));
            Assert.That(func(), Is.EqualTo(value));
        }

        [Test]
        public void Should_return_const_double()
        {
            const double value = 3.14;
            var func = CreateFunc<double>(Expr.Return(Expr.Constant(value)));
            Assert.That(func(), Is.EqualTo(value));
        }

        [Test]
        [TestCase("abc")]
        [TestCase(null)]
        public void Should_return_const_string(string value)
        {
            var func = CreateFunc<string>(Expr.Return(Expr.Constant(value)));
            Assert.That(func(), Is.EqualTo(value));
        }
    }
}