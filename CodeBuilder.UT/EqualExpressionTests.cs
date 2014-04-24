using System;
using NUnit.Framework;

namespace CodeBuilder.UT
{
    [TestFixture]
    public class EqualExpressionTests : BuilderTestBase
    {
        public class BaseType { }
        public class DerivedType : BaseType { }
        public enum E { A, B };

        [Test]
        public void ExpressionTypeTest()
        {
            Assert.That(Expr.Equal(Expr.Constant(1), Expr.Constant(1)).ExpressionType, Is.EqualTo(typeof(bool)));
        }

        [Test]
        public void NullLeftTest()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => Expr.Equal(null, Expr.Constant(1)));
            Assert.That(ex.Message, Is.StringContaining("left"));
        }

        [Test]
        public void NullRightTest()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => Expr.Equal(Expr.Constant(1), null));
            Assert.That(ex.Message, Is.StringContaining("right"));
        }

        [Test]
        public void Should_not_allow_different_types()
        {
            var ex = Assert.Throws<ArgumentException>(() => Expr.Equal(Expr.Constant(1), Expr.Constant("abc")));
            Assert.That(ex.Message, Is.StringContaining("Comparison of System.Int32 and System.String is not supported. Try to cast left or right value to the same type as other."));
        }

        [Test]
        public void Should_allow_comparison_of_the_same_class_hierarchy()
        {
            var val = new DerivedType();
            Assert.That(IsEqual<BaseType, DerivedType>(val,val), Is.EqualTo(true));
            Assert.That(IsEqual(new BaseType(), new DerivedType()), Is.EqualTo(false));
        }

        [Test]
        public void Should_allow_int_type_mixins_up_to_int()
        {
            Assert.That(IsEqual<byte, int>(5, 5), Is.EqualTo(true));
            Assert.That(IsEqual<sbyte, short>(5, 5), Is.EqualTo(true));
            Assert.That(IsEqual<short, int>(5, 5), Is.EqualTo(true));
            Assert.That(IsEqual<ushort, sbyte>(5, 5), Is.EqualTo(true));
            Assert.That(IsEqual<int, int>(5, 5), Is.EqualTo(true));
            Assert.That(IsEqual<int, int>(5, 4), Is.EqualTo(false));
            Assert.That(IsEqual<sbyte, int>(-2, -2), Is.EqualTo(true));
            Assert.That(IsEqual<short, int>(-2, -2), Is.EqualTo(true));
            Assert.That(IsEqual<int, int>(-2, -2), Is.EqualTo(true));
            Assert.That(IsEqual<char, int>((char)2, 2), Is.EqualTo(true));
        }

        [Test]
        public void Should_allow_comparisons_of_the_same_types()
        {
            Assert.That(IsEqual(32UL, 32UL), Is.True);
            Assert.That(IsEqual(32.5f, 32.5f), Is.True);
            Assert.That(IsEqual(32.5, 32.5), Is.True);
            Assert.That(IsEqual(32.5, -32.5), Is.False);
            Assert.That(IsEqual(new object(), new object()), Is.False);
            var obj = new object();
            Assert.That(IsEqual(obj, obj), Is.True);
            Assert.That(IsEqual(E.A, E.A), Is.True);
            Assert.That(IsEqual(E.A, E.B), Is.False);
        }

        [Test]
        public void Should_not_allow_comparison_of_integers_having_different_value_range()
        {
            Assert.Throws<ArgumentException>(() => IsEqual<int, uint>(-1, 1));
            Assert.Throws<ArgumentException>(() => IsEqual<long, ulong>(-1, 1));
            Assert.Throws<ArgumentException>(() => IsEqual<uint, ulong>(1, 1));
        }

        [Test]
        public void Should_not_allow_comparison_of_non_primitive_structs()
        {
            var ex = Assert.Throws<ArgumentException>(() => IsEqual(DateTime.Today, DateTime.Today));
            Assert.That(ex.Message, Is.EqualTo("Comparison of value types is not supported."));
        }

        private bool IsEqual<T1, T2>(T1 arg1, T2 arg2)
        {
            return CreateFunc<T1, T2, bool>(Expr.Return(Expr.Equal(Expr.Parameter(0, typeof(T1)), Expr.Parameter(1, typeof(T2)))))(arg1, arg2);
        }
    }
}