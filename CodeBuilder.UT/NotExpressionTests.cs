using System;
using NUnit.Framework;

namespace CodeBuilder.UT
{
    [TestFixture]
    public class NotExpressionTests : BuilderTestBase
    {
        [Test]
        public void IntTypeTest()
        {
            Assert.That(Expr.Not(Expr.Parameter(0, typeof(byte))).ExpressionType, Is.EqualTo(typeof(int)));
            Assert.That(Expr.Not(Expr.Parameter(0, typeof(sbyte))).ExpressionType, Is.EqualTo(typeof(int)));
            Assert.That(Expr.Not(Expr.Parameter(0, typeof(ushort))).ExpressionType, Is.EqualTo(typeof(int)));
            Assert.That(Expr.Not(Expr.Parameter(0, typeof(char))).ExpressionType, Is.EqualTo(typeof(int)));
            Assert.That(Expr.Not(Expr.Parameter(0, typeof(short))).ExpressionType, Is.EqualTo(typeof(int)));
            Assert.That(Expr.Not(Expr.Parameter(0, typeof(int))).ExpressionType, Is.EqualTo(typeof(int)));
        }

        [Test]
        public void PreservedTypeTest()
        {
            Assert.That(Expr.Not(Expr.Parameter(0, typeof(long))).ExpressionType, Is.EqualTo(typeof(long)));
            Assert.That(Expr.Not(Expr.Parameter(0, typeof(uint))).ExpressionType, Is.EqualTo(typeof(uint)));
            Assert.That(Expr.Not(Expr.Parameter(0, typeof(ulong))).ExpressionType, Is.EqualTo(typeof(ulong)));
        }

        [Test]
        public void NullParamererTest()
        {
            Assert.Throws<ArgumentNullException>(() => Expr.Not(null));
        }

        [Test]
        public void Should_not_not_allow_non_integral_types()
        {
            var ex = Assert.Throws<ArgumentException>(() => Expr.Not(Expr.Parameter(0, typeof(float))));
            Assert.That(ex.Message, Is.StringContaining("Expected integral type, got: System.Single"));
        }

        [Test]
        public void Should_compute_not_of_values()
        {
            Assert.That(Not<int, byte>(10), Is.EqualTo(~10));
            Assert.That(Not<int, ushort>(10), Is.EqualTo(~10));
            Assert.That(Not<int, char>((char)10), Is.EqualTo(~10));
            Assert.That(Not<int, sbyte>(10), Is.EqualTo(~10));
            Assert.That(Not<int, short>(10), Is.EqualTo(~10));
            Assert.That(Not<int, int>(10), Is.EqualTo(~10));
            Assert.That(Not<long, long>(-10), Is.EqualTo(~-10));
            Assert.That(Not<uint, uint>(45), Is.EqualTo(~45u));
            Assert.That(Not<ulong, ulong>(45), Is.EqualTo(~45ul));
        }

        private T2 Not<T2, T1>(T1 value)
        {
            return CreateFunc<T1, T2>(Expr.Return(Expr.Not(Expr.Parameter(0, typeof(T1))))).Invoke(value);
        }
    }
}