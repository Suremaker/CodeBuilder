using System;
using CodeBuilder.Expressions;
using NUnit.Framework;

namespace CodeBuilder.UT
{
    [TestFixture]
    public class NewExpressionTests : BuilderTestBase
    {
        public class SomeClass
        {
        }

        public class ProtectedCtorClass
        {
            protected ProtectedCtorClass() { }
        }

        public class PrivateCtorClass
        {
            private PrivateCtorClass() { }
        }

        public class OtherClass
        {
            public OtherClass(int x)
            {
                Value = x;
            }

            public int Value { get; private set; }
        }

        public struct SomeStruct
        {
            public int X { get; private set; }
            public short Y { get; private set; }

            public SomeStruct(int x, short y)
                : this()
            {
                X = x;
                Y = y;
            }
        }

        [Test]
        public void ExpressionTypeTest()
        {
            Assert.That(Expr.New(typeof(SomeClass)).ExpressionType, Is.EqualTo(typeof(SomeClass)));
        }

        [Test]
        public void Should_create_class_with_default_ctor()
        {
            AssertInstance<SomeClass>(Expr.New(typeof(SomeClass)));
        }

        [Test]
        public void Should_create_class_with_protected_ctor()
        {
            AssertInstance<ProtectedCtorClass>(Expr.New(typeof(ProtectedCtorClass)));
        }

        [Test]
        public void Should_create_class_with_private_ctor()
        {
            AssertInstance<PrivateCtorClass>(Expr.New(typeof(PrivateCtorClass)));
        }

        [Test]
        public void Should_create_class_with_parameter_ctor()
        {
            var instance = AssertInstance<OtherClass>(Expr.New(typeof(OtherClass), Expr.Constant(23)));
            Assert.That(instance.Value, Is.EqualTo(23));
        }

        [Test]
        public void Should_create_struct_with_parameter_ctor()
        {
            var value = AssertInstance<SomeStruct>(Expr.New(typeof(SomeStruct), Expr.Constant(23), Expr.Convert(Expr.Constant(short.MaxValue), typeof(short))));
            Assert.That(value.X, Is.EqualTo(23));
            Assert.That(value.Y, Is.EqualTo(short.MaxValue));
        }

        [Test]
        public void Should_create_struct_with_default_ctor()
        {
            var value = AssertInstance<SomeStruct>(Expr.New(typeof(SomeStruct)));
            Assert.That(value.X, Is.EqualTo(0));
            Assert.That(value.Y, Is.EqualTo(0));
        }

        [Test]
        public void Should_not_allow_creating_struct_if_no_parameter_constructor_is_present()
        {
            var ex = Assert.Throws<ArgumentException>(() => Expr.New(typeof (SomeStruct), Expr.Constant("abc")));
            Assert.That(ex.Message, Is.StringContaining("No matching constructor found for type CodeBuilder.UT.NewExpressionTests+SomeStruct with parameters: [System.String]"));
        }

        [Test]
        public void Should_not_allow_creating_class_if_no_specified_constructor_is_present()
        {
            var ex = Assert.Throws<ArgumentException>(() => Expr.New(typeof(SomeClass), Expr.Constant("abc")));
            Assert.That(ex.Message, Is.StringContaining("No matching constructor found for type CodeBuilder.UT.NewExpressionTests+SomeClass with parameters: [System.String]"));
        }

        private static T AssertInstance<T>(NewExpression newExpression)
        {
            var value = CreateFunc<T>(Expr.Return(newExpression))();
            Assert.That(value, Is.InstanceOf<T>());
            return value;
        }
    }
}