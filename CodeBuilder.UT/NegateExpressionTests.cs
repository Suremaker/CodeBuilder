using System;
using NUnit.Framework;

namespace CodeBuilder.UT
{
    [TestFixture]
    public class NegateExpressionTests : BuilderTestBase
    {
        [Test]
        public void IntTypeTest()
        {
            Assert.That(Expr.Negate(Expr.Parameter(0, typeof(byte))).ExpressionType, Is.EqualTo(typeof(int)));
            Assert.That(Expr.Negate(Expr.Parameter(0, typeof(sbyte))).ExpressionType, Is.EqualTo(typeof(int)));
            Assert.That(Expr.Negate(Expr.Parameter(0, typeof(ushort))).ExpressionType, Is.EqualTo(typeof(int)));
            Assert.That(Expr.Negate(Expr.Parameter(0, typeof(char))).ExpressionType, Is.EqualTo(typeof(int)));
            Assert.That(Expr.Negate(Expr.Parameter(0, typeof(short))).ExpressionType, Is.EqualTo(typeof(int)));
            Assert.That(Expr.Negate(Expr.Parameter(0, typeof(int))).ExpressionType, Is.EqualTo(typeof(int)));
        }

        [Test]
        public void PreservedTypeTest()
        {
            Assert.That(Expr.Negate(Expr.Parameter(0, typeof(long))).ExpressionType, Is.EqualTo(typeof(long)));
            Assert.That(Expr.Negate(Expr.Parameter(0, typeof(float))).ExpressionType, Is.EqualTo(typeof(float)));
            Assert.That(Expr.Negate(Expr.Parameter(0, typeof(double))).ExpressionType, Is.EqualTo(typeof(double)));
        }

        [Test]
        public void NullParamererTest()
        {
            Assert.Throws<ArgumentNullException>(() => Expr.Negate(null));
        }

        [Test]
        public void Should_not_allow_negation_with_overflow()
        {
            var ex = Assert.Throws<ArgumentException>(() => Expr.Negate(Expr.Parameter(0, typeof(uint))));
            Assert.That(ex.Message, Is.StringContaining("Unsupported operation for type System.UInt32. Please try to cast them first to type allowing negation without overflow."));
            ex = Assert.Throws<ArgumentException>(() => Expr.Negate(Expr.Parameter(0, typeof(ulong))));
            Assert.That(ex.Message, Is.StringContaining("Unsupported operation for type System.UInt64. Please try to cast them first to type allowing negation without overflow."));
        }

        [Test]
        public void Should_not_allow_negation_for_non_primitive_types()
        {
            var ex = Assert.Throws<ArgumentException>(() => Expr.Negate(Expr.Parameter(0, typeof(decimal))));
            Assert.That(ex.Message, Is.StringContaining("Expected numeric primitive type, got: System.Decimal"));

            ex = Assert.Throws<ArgumentException>(() => Expr.Negate(Expr.Parameter(0, typeof(string))));
            Assert.That(ex.Message, Is.StringContaining("Expected numeric primitive type, got: System.String"));

            ex = Assert.Throws<ArgumentException>(() => Expr.Negate(Expr.Parameter(0, typeof(bool))));
            Assert.That(ex.Message, Is.StringContaining("Expected numeric primitive type, got: System.Boolean"));

            ex = Assert.Throws<ArgumentException>(() => Expr.Negate(Expr.Parameter(0, typeof(IntPtr))));
            Assert.That(ex.Message, Is.StringContaining("Expected numeric primitive type, got: System.IntPtr"));

            ex = Assert.Throws<ArgumentException>(() => Expr.Negate(Expr.Parameter(0, typeof(UIntPtr))));
            Assert.That(ex.Message, Is.StringContaining("Expected numeric primitive type, got: System.UIntPtr"));
        }

        [Test]
        public void Should_negate_values()
        {
            Assert.That(Negate<int, byte>(1), Is.EqualTo(-1));
            Assert.That(Negate<int, ushort>(1), Is.EqualTo(-1));
            Assert.That(Negate<int, char>((char)1), Is.EqualTo(-1));
            Assert.That(Negate<int, sbyte>(-1), Is.EqualTo(1));
            Assert.That(Negate<int, sbyte>(1), Is.EqualTo(-1));
            Assert.That(Negate<int, short>(-1), Is.EqualTo(1));
            Assert.That(Negate<int, short>(1), Is.EqualTo(-1));
            Assert.That(Negate<int, int>(-1), Is.EqualTo(1));
            Assert.That(Negate<int, int>(1), Is.EqualTo(-1));
            Assert.That(Negate<long, long>(-1), Is.EqualTo(1));
            Assert.That(Negate<long, long>(1), Is.EqualTo(-1));
            Assert.That(Negate<float, float>(3.27f), Is.EqualTo(-3.27f));
            Assert.That(Negate<float, float>(-3.27f), Is.EqualTo(3.27f));
            Assert.That(Negate<double, double>(3.27), Is.EqualTo(-3.27));
            Assert.That(Negate<double, double>(-3.27), Is.EqualTo(3.27));
        }

        private T2 Negate<T2, T1>(T1 value)
        {
            return CreateFunc<T1, T2>(Expr.Return(Expr.Negate(Expr.Parameter(0, typeof(T1))))).Invoke(value);
        }
    }
}