using System;
using NUnit.Framework;

namespace CodeBuilder.UT
{
    [TestFixture]
    public class FieldReadExpressionTests : FieldAccessTests
    {
        [Test]
        public void ExpressionTypeTest()
        {
            Assert.That(Expr.ReadField(_classStatic).ExpressionType, Is.EqualTo(typeof(int)));
        }

        [Test]
        public void NullFieldInfoTest()
        {
            Assert.Throws<ArgumentNullException>(() => Expr.ReadField(null));
        }

        [Test]
        public void NullInstanceTest()
        {
            Assert.Throws<ArgumentNullException>(() => Expr.ReadField(null, _classLocal));
        }

        [Test]
        public void Should_not_allow_instance_of_wrong_type()
        {
            var ex = Assert.Throws<ArgumentException>(() => Expr.ReadField(Expr.Constant("abc"), _structLocal));
            Assert.That(ex.Message, Is.StringContaining("Instance expression of type System.String does not match to type: CodeBuilder.UT.FieldAccessTests+TestStruct"));
        }

        [Test]
        public void Should_not_allow_instance_for_static_field()
        {
            var ex = Assert.Throws<ArgumentException>(() => Expr.ReadField(Expr.Parameter(0, typeof(TestStruct)), _structStatic));
            Assert.That(ex.Message, Is.StringContaining("Static field cannot be read with instance parameter"));
        }

        [Test]
        public void Should_read_static_field_of_class()
        {
            var func = CreateFunc<int>(Expr.Return(Expr.ReadField(_classStatic)));
            Assert.That(func(), Is.EqualTo(TestClass.StaticField));
        }

        [Test]
        public void Should_read_static_field_of_struct()
        {
            var func = CreateFunc<int>(Expr.Return(Expr.ReadField(_structStatic)));
            Assert.That(func(), Is.EqualTo(TestStruct.StaticField));
        }

        [Test]
        public void Should_read_local_field_of_class()
        {
            var func = CreateFunc<TestClass, string>(Expr.Return(Expr.ReadField(Expr.Parameter(0, typeof(TestClass)), _classLocal)));
            var tc = new TestClass();
            Assert.That(func(tc), Is.EqualTo(tc.GetLocal()));
        }

        [Test]
        public void Should_read_local_field_of_struct()
        {
            var func = CreateFunc<TestStruct, string>(Expr.Return(Expr.ReadField(Expr.Parameter(0, typeof(TestStruct)), _structLocal)));
            var ts = new TestStruct("aaa");
            Assert.That(func(ts), Is.EqualTo(ts.GetLocal()));
        }
    }
}