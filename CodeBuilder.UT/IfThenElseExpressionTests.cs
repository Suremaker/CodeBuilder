using System;
using NUnit.Framework;

namespace CodeBuilder.UT
{
    [TestFixture]
    public class IfThenElseExpressionTests : BuilderTestBase
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
        public void VoidExpressionTypeTest()
        {
            Assert.That(Expr.IfThenElse(Expr.Constant(1), Expr.Empty(), Expr.Empty()).ExpressionType, Is.EqualTo(typeof(void)));
        }
        [Test]
        public void VoidExpressionTypeIfThenIsVoidTest()
        {
            Assert.That(Expr.IfThenElse(Expr.Constant(1), Expr.Empty(), Expr.Constant(1)).ExpressionType, Is.EqualTo(typeof(void)));
        }

        [Test]
        public void NonVoidExpressionTypeTest()
        {
            Assert.That(Expr.IfThenElse(Expr.Constant(1), Expr.Constant("abc"), Expr.Constant("def")).ExpressionType, Is.EqualTo(typeof(string)));
        }

        [Test]
        public void NonVoidExpressionTypeWithHierarchyImplicitCastTest()
        {
            Assert.That(Expr.IfThenElse(Expr.Constant(1), Expr.Parameter(0, typeof(object)), Expr.Parameter(1, typeof(string))).ExpressionType, Is.EqualTo(typeof(object)));
        }

        [Test]
        public void NullPredicateTest()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => Expr.IfThenElse(null, Expr.Empty(), Expr.Empty()));
            Assert.That(ex.Message, Is.StringContaining("predicate"));
        }

        [Test]
        public void NullThenTest()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => Expr.IfThenElse(Expr.Constant(1), null, Expr.Empty()));
            Assert.That(ex.Message, Is.StringContaining("thenExpression"));
        }

        [Test]
        public void NullElseTest()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => Expr.IfThenElse(Expr.Constant(1), Expr.Empty(), null));
            Assert.That(ex.Message, Is.StringContaining("elseExpression"));
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
            AssertCounter<IComparable>("str", true);
            AssertCounter<Counter>(new Counter(), true);
            AssertCounter<Counter>(null, false);
        }

        [Test]
        public void Should_not_allow_value_types_as_predicates()
        {
            var ex = Assert.Throws<ArgumentException>(() => Expr.IfThenElse(Expr.Parameter(0, typeof(DateTime)), Expr.Empty(), Expr.Empty()));
            Assert.That(ex.Message, Is.StringContaining("Expected primitive or reference type, got: System.DateTime"));
        }

        [Test]
        public void Should_not_allow_returning_non_matching_types()
        {
            var ex = Assert.Throws<ArgumentException>(() => Expr.IfThenElse(Expr.Constant(0), Expr.Constant("a"), Expr.Constant(1)));
            Assert.That(ex.Message, Is.StringContaining("Else expression type System.Int32 is not assignable to then expression type System.String"));
        }

        [Test]
        public void Should_return_from_if_then_else()
        {
            var func=CreateFunc<int, object>(Expr.Return(Expr.IfThenElse(
                Expr.Parameter(0, typeof (int)),
                Expr.New(typeof (object)),
                Expr.Constant("b"))));

            Assert.That(func(0).ToString(),Is.EqualTo("b"));
            Assert.That(func(1).ToString(),Is.EqualTo("System.Object"));
        }

        private void AssertCounter<T>(T value, bool expectedResult)
        {
            var func = CreateAction<T, Counter, Counter>(Expr.IfThenElse(
                Expr.Parameter(0, typeof(T)),
                Expr.Call(Expr.Parameter(1, typeof(Counter)), typeof(Counter).GetMethod("Count")),
                Expr.Call(Expr.Parameter(2, typeof(Counter)), typeof(Counter).GetMethod("Count"))));

            var counter1 = new Counter();
            var counter2 = new Counter();
            func(value, counter1, counter2);
            Assert.That(counter1.Value, Is.EqualTo(expectedResult ? 1 : 0));
            Assert.That(counter2.Value, Is.EqualTo(expectedResult ? 0 : 1));
        }
    }
}