using System;
using NUnit.Framework;

namespace CodeBuilder.UT
{
    [TestFixture]
    public class IfThenExpressionTests : BuilderTestBase
    {
        public class Counter
        {
            public void Count()
            {
                ++Value;
            }

            public int Value { get; private set; }
        }

        [Test]
        public void VoidTypeTest()
        {
            Assert.That(Expr.IfThen(Expr.Constant(1), Expr.Constant(1)).ExpressionType, Is.EqualTo(typeof(void)));
        }

        [Test]
        public void NullPredicateTest()
        {
            Assert.Throws<ArgumentNullException>(() => Expr.IfThen(null, Expr.Empty()));
        }

        [Test]
        public void NullThenTest()
        {
            Assert.Throws<ArgumentNullException>(() => Expr.IfThen(Expr.Constant(1), null));
        }

        [Test]
        public void Should_allow_primitive_predicates()
        {
            AssertCounter<byte>(10, true);
            AssertCounter<byte>(0, false);
            AssertCounter<sbyte>(0, false);
            AssertCounter<sbyte>(-1, true);
            AssertCounter<sbyte>(1, true);
            AssertCounter<short>(short.MaxValue, true);
            AssertCounter<short>(0, false);
            AssertCounter<ushort>(ushort.MaxValue, true);
            AssertCounter<ushort>(0, false);
            AssertCounter<char>(char.MaxValue, true);
            AssertCounter<char>((char)0, false);
            AssertCounter<int>(int.MaxValue, true);
            AssertCounter<int>(0, false);
            AssertCounter<uint>(uint.MaxValue, true);
            AssertCounter<uint>(0, false);
            AssertCounter<long>(long.MaxValue, true);
            AssertCounter<long>(0, false);
            AssertCounter<ulong>(ulong.MaxValue, true);
            AssertCounter<ulong>(0, false);
        }

        [Test]
        public void Should_test_not_null_instance()
        {
            AssertCounter<object>(null, false);
            AssertCounter<object>(new object(), true);
            AssertCounter<IComparable>("abc", true);
            AssertCounter<Counter>(new Counter(), true);
            AssertCounter<Counter>(null, false);
        }

        [Test]
        public void Should_not_allow_value_types_as_predicates()
        {
            var ex = Assert.Throws<ArgumentException>(() => Expr.IfThen(Expr.Parameter(0, typeof(DateTime)), Expr.Empty()));
            Assert.That(ex.Message, Is.StringContaining("Expected primitive or reference type, got: System.DateTime"));
        }

        private void AssertCounter<T>(T value, bool expectedResult)
        {
            var func = CreateAction<T, Counter>(Expr.IfThen(
                Expr.Parameter(0, typeof(T)),
                Expr.Call(Expr.Parameter(1, typeof(Counter)), typeof(Counter).GetMethod("Count"))));

            var counter = new Counter();
            func(value, counter);
            Assert.That(counter.Value, Is.EqualTo(expectedResult ? 1 : 0));
        }
    }
}