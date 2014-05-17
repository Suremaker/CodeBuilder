using System;
using NUnit.Framework;

namespace CodeBuilder.UT.Expressions
{
    [TestFixture]
    public class GreaterExpressionTests : BuilderTestBase
    {
        public class BaseType { }
        public class DerivedType : BaseType { }
        public enum E { A, B };

        [Test]
        public void ExpressionTypeTest()
        {
            Assert.That(Expr.Greater(Expr.Constant(1), Expr.Constant(1)).ExpressionType, Is.EqualTo(typeof(bool)));
        }

        [Test]
        public void NullLeftTest()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => Expr.Greater(null, Expr.Constant(1)));
            Assert.That(ex.Message, Is.StringContaining("left"));
        }

        [Test]
        public void NullRightTest()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => Expr.Greater(Expr.Constant(1), null));
            Assert.That(ex.Message, Is.StringContaining("right"));
        }

        [Test]
        public void Should_not_allow_non_primitive_structs()
        {
            var ex = Assert.Throws<ArgumentException>(() => Expr.Greater(Expr.Parameter(0, typeof(DateTime)), Expr.Parameter(0, typeof(DateTime))));
            Assert.That(ex.Message, Is.StringContaining("Comparison of non primitive types is not supported: Left=System.DateTime, Right=System.DateTime"));
        }

        [Test]
        public void Should_not_allow_classes()
        {
            var ex = Assert.Throws<ArgumentException>(() => Expr.Greater(Expr.Parameter(0, typeof(string)), Expr.Parameter(0, typeof(string))));
            Assert.That(ex.Message, Is.StringContaining("Comparison of non primitive types is not supported: Left=System.String, Right=System.String"));
        }

        [Test]
        public void Should_allow_int_type_mixins_up_to_int()
        {
            Assert.That(IsGreater<byte, int>(5, 4), Is.EqualTo(true));
            Assert.That(IsGreater<sbyte, short>(5, 4), Is.EqualTo(true));
            Assert.That(IsGreater<short, int>(5, 4), Is.EqualTo(true));
            Assert.That(IsGreater<ushort, sbyte>(5, 4), Is.EqualTo(true));
            Assert.That(IsGreater<int, int>(5, 4), Is.EqualTo(true));
            Assert.That(IsGreater<int, int>(-5, -5), Is.EqualTo(false));
            Assert.That(IsGreater<sbyte, int>(-2, -3), Is.EqualTo(true));
            Assert.That(IsGreater<short, int>(-2, -3), Is.EqualTo(true));
            Assert.That(IsGreater<int, int>(-2, -3), Is.EqualTo(true));
            Assert.That(IsGreater<char, int>((char)2, 1), Is.EqualTo(true));
            Assert.That(IsGreater<ushort, short>(1,-1), Is.EqualTo(true));
        }

        [Test]
        public void Should_allow_comparisons_of_the_same_types()
        {
            Assert.That(IsGreater(33UL, 32UL), Is.True);
            Assert.That(IsGreater(32.51f, 32.5f), Is.True);
            Assert.That(IsGreater(32.51, 32.5), Is.True);
            Assert.That(IsGreater(-32.5, 32.5), Is.False);
            Assert.That(IsGreater(E.B, E.A), Is.True);
            Assert.That(IsGreater(E.A, E.B), Is.False);
            Assert.That(IsGreater(E.A, E.A), Is.False);
            Assert.That(IsGreater(uint.MaxValue, uint.MaxValue-1), Is.True);
            Assert.That(IsGreater(ulong.MaxValue, ulong.MaxValue-1), Is.True);
            Assert.That(IsGreater(long.MaxValue, long.MaxValue-1), Is.True);
        }

        [Test]
        public void Should_not_allow_comparison_of_integers_having_different_value_range()
        {
            Assert.Throws<ArgumentException>(() => IsGreater<int, uint>(-1, 1));
            Assert.Throws<ArgumentException>(() => IsGreater<long, ulong>(-1, 1));
            Assert.Throws<ArgumentException>(() => IsGreater<uint, ulong>(1, 1));
        }

        private bool IsGreater<T1, T2>(T1 arg1, T2 arg2)
        {
            return CreateFunc<T1, T2, bool>(Expr.Return(Expr.Greater(Expr.Parameter(0, typeof(T1)), Expr.Parameter(1, typeof(T2)))))(arg1, arg2);
        }
    }
}