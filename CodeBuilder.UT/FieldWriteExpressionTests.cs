using System;
using NUnit.Framework;

namespace CodeBuilder.UT
{
    [TestFixture]
    public class FieldWriteExpressionTests : FieldAccessTests
    {
        public class StructHolder
        {
            public TestStruct Struct;
        }

        [Test]
        public void ExpressionTypeTest()
        {
            Assert.That(Expr.WriteField(_classStatic, Expr.Constant(22)).ExpressionType, Is.EqualTo(typeof(void)));
        }

        [Test]
        public void NullFieldInfoTest()
        {
            Assert.Throws<ArgumentNullException>(() => Expr.WriteField(null, Expr.Constant("123")));
        }

        [Test]
        public void NullInstanceTest()
        {
            Assert.Throws<ArgumentNullException>(() => Expr.WriteField(null, _classLocal, Expr.Constant(1)));
        }

        [Test]
        public void NullValueTest()
        {
            Assert.Throws<ArgumentNullException>(() => Expr.WriteField(Expr.Parameter(0, typeof(TestClass)), _classLocal, null));
        }

        [Test]
        public void Should_not_allow_instance_of_wrong_type()
        {
            var ex = Assert.Throws<ArgumentException>(() => Expr.WriteField(Expr.Constant("abc"), _structLocal, Expr.Constant(2)));
            Assert.That(ex.Message, Is.StringContaining("Instance expression of type System.String does not match to type: CodeBuilder.UT.FieldAccessTests+TestStruct"));
        }

        [Test]
        public void Should_not_allow_instance_for_static_field()
        {
            var ex = Assert.Throws<ArgumentException>(() => Expr.WriteField(Expr.Parameter(0, typeof(TestStruct)), _structStatic, Expr.Constant(22)));
            Assert.That(ex.Message, Is.StringContaining("Static field cannot be written with instance parameter"));
        }

        [Test]
        public void Should_write_static_field_of_class()
        {
            var func = CreateAction<int>(Expr.WriteField(_classStatic, Expr.Parameter(0, typeof(int))));
            func(3);
            Assert.That(TestClass.StaticField, Is.EqualTo(3));
        }

        [Test]
        public void Should_write_static_field_of_struct()
        {
            var func = CreateAction<int>(Expr.WriteField(_structStatic, Expr.Parameter(0, typeof(int))));
            func(3);
            Assert.That(TestStruct.StaticField, Is.EqualTo(3));
        }

        [Test]
        public void Should_write_local_field_of_class()
        {
            var func = CreateAction<TestClass, string>(Expr.WriteField(Expr.Parameter(0, typeof(TestClass)), _classLocal, Expr.Parameter(1, typeof(string))));
            var tc = new TestClass();
            func(tc, "abc");
            Assert.That(tc.GetLocal(), Is.EqualTo("abc"));
        }

        [Test]
        public void Should_write_local_field_of_struct_via_parameter()
        {
            var func = CreateFunc<TestStruct, string, TestStruct>(
                Expr.WriteField(Expr.Parameter(0, typeof(TestStruct)), _structLocal, Expr.Parameter(1, typeof(string))),
                Expr.Return(Expr.Parameter(0, typeof(TestStruct))));
            Assert.That(func(new TestStruct(), "abc321").GetLocal(), Is.EqualTo("abc321"));
        }

        [Test]
        public void Should_write_local_field_of_struct_via_field()
        {
            var func = CreateAction<StructHolder, string>(Expr.WriteField(Expr.ReadField(Expr.Parameter(0, typeof(StructHolder)), typeof(StructHolder).GetField("Struct")), _structLocal, Expr.Parameter(1, typeof(string))));
            var holder = new StructHolder();
            func(holder, "abc321");
            Assert.That(holder.Struct.GetLocal(), Is.EqualTo("abc321"));
        }

        [Test]
        public void Should_write_local_field_of_struct_via_local_variable()
        {
            var local = Expr.DeclareLocalVar(typeof(TestStruct), "s");
            var func = CreateFunc<TestStruct, string, TestStruct>(
                Expr.WriteLocal(local, Expr.Parameter(0, typeof(TestStruct))),
                Expr.WriteField(Expr.ReadLocal(local), _structLocal, Expr.Parameter(1, typeof(string))),
                Expr.Return(Expr.ReadLocal(local)));
            Assert.That(func(new TestStruct(), "abc321").GetLocal(), Is.EqualTo("abc321"));
        }
    }
}