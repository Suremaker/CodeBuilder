using System;
using NUnit.Framework;

namespace CodeBuilder.UT
{
    [TestFixture]
    public class LessExpressionTests : BuilderTestBase
    {
        public class BaseType { }
        public class DerivedType : BaseType { }
        public enum E { A, B };

        [Test]
        public void ExpressionTypeTest()
        {
            Assert.That(Expr.Less(Expr.Constant(1), Expr.Constant(1)).ExpressionType, Is.EqualTo(typeof(bool)));
        }

        [Test]
        public void NullLeftTest()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => Expr.Less(null, Expr.Constant(1)));
            Assert.That(ex.Message, Is.StringContaining("left"));
        }

        [Test]
        public void NullRightTest()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => Expr.Less(Expr.Constant(1), null));
            Assert.That(ex.Message, Is.StringContaining("right"));
        }

        [Test]
        public void Should_not_allow_non_primitive_structs()
        {
            var ex = Assert.Throws<ArgumentException>(() => Expr.Less(Expr.Parameter(0, typeof(DateTime)), Expr.Parameter(0, typeof(DateTime))));
            Assert.That(ex.Message, Is.StringContaining("Comparison of non primitive types is not supported: Left=System.DateTime, Right=System.DateTime"));
        }

        [Test]
        public void Should_not_allow_classes()
        {
            var ex = Assert.Throws<ArgumentException>(() => Expr.Less(Expr.Parameter(0, typeof(string)), Expr.Parameter(0, typeof(string))));
            Assert.That(ex.Message, Is.StringContaining("Comparison of non primitive types is not supported: Left=System.String, Right=System.String"));
        }

        [Test]
        public void Should_allow_int_type_mixins_up_to_int()
        {
            Assert.That(IsLess<byte, int>(3, 4), Is.EqualTo(true));
            Assert.That(IsLess<sbyte, short>(3, 4), Is.EqualTo(true));
            Assert.That(IsLess<short, int>(3, 4), Is.EqualTo(true));
            Assert.That(IsLess<ushort, sbyte>(3, 4), Is.EqualTo(true));
            Assert.That(IsLess<int, int>(3, 4), Is.EqualTo(true));
            Assert.That(IsLess<int, int>(-5, -5), Is.EqualTo(false));
            Assert.That(IsLess<sbyte, int>(-4, -3), Is.EqualTo(true));
            Assert.That(IsLess<short, int>(-4, -3), Is.EqualTo(true));
            Assert.That(IsLess<int, int>(-4, -3), Is.EqualTo(true));
            Assert.That(IsLess<char, int>((char)0, 1), Is.EqualTo(true));
            Assert.That(IsLess<short, ushort>(-1, 1), Is.EqualTo(true));
        }

        [Test]
        public void Should_allow_comparisons_of_the_same_types()
        {
            Assert.That(IsLess(31UL, 32UL), Is.True);
            Assert.That(IsLess(32.5f, 32.51f), Is.True);
            Assert.That(IsLess(32.5, 32.51), Is.True);
            Assert.That(IsLess(32.5, -32.5), Is.False);
            Assert.That(IsLess(E.A, E.B), Is.True);
            Assert.That(IsLess(E.B, E.A), Is.False);
            Assert.That(IsLess(E.A, E.A), Is.False);
            Assert.That(IsLess(uint.MaxValue - 1, uint.MaxValue), Is.True);
            Assert.That(IsLess(ulong.MaxValue - 1, ulong.MaxValue), Is.True);
            Assert.That(IsLess(long.MaxValue - 1, long.MaxValue), Is.True);
        }

        [Test]
        public void Should_not_allow_comparison_of_integers_having_different_value_range()
        {
            Assert.Throws<ArgumentException>(() => IsLess<int, uint>(-1, 1));
            Assert.Throws<ArgumentException>(() => IsLess<long, ulong>(-1, 1));
            Assert.Throws<ArgumentException>(() => IsLess<uint, ulong>(1, 1));
        }

        private bool IsLess<T1, T2>(T1 arg1, T2 arg2)
        {
            return CreateFunc<T1, T2, bool>(Expr.Return(Expr.Less(Expr.Parameter(0, typeof(T1)), Expr.Parameter(1, typeof(T2)))))(arg1, arg2);
        }
    }
}