using System;
using NUnit.Framework;

namespace CodeBuilder.UT.Expressions
{
    [TestFixture]
    public class LocalExpressionTests : BuilderTestBase
    {
        private readonly LocalVariable _local = Expr.LocalVariable(typeof(int), "x");

        [Test]
        public void LocalReadExpressionTypeTest()
        {
            Assert.That(Expr.ReadLocal(_local).ExpressionType, Is.EqualTo(_local.VariableType));
        }

        [Test]
        public void LocalWriteExpressionTypeTest()
        {
            Assert.That(Expr.WriteLocal(_local, Expr.Constant(1)).ExpressionType, Is.EqualTo(typeof(void)));
        }

        [Test]
        public void NullLocalVariableForLocalReadTest()
        {
            Assert.Throws<ArgumentNullException>(() => Expr.ReadLocal(null));
        }

        [Test]
        public void NullLocalVariableForLocalWriteTest()
        {
            Assert.Throws<ArgumentNullException>(() => Expr.WriteLocal(null, Expr.Constant(1)));
        }

        [Test]
        public void NullValueForLocalWriteTest()
        {
            Assert.Throws<ArgumentNullException>(() => Expr.WriteLocal(_local, null));
        }

        [Test]
        public void Should_not_allow_implicit_boxing_on_write()
        {
            var ex = Assert.Throws<ArgumentException>(() => Expr.WriteLocal(Expr.LocalVariable(typeof(object), "o"), Expr.Constant(1)));
            Assert.That(ex.Message, Is.StringContaining("Unable to assign System.Int32 to variable of type System.Object"));
        }

        [Test]
        public void Should_write_and_read_local_variable()
        {
            var func = CreateFunc<int, int>(
                Expr.DeclareLocal(_local, Expr.Parameter(0, typeof(int))),
                Expr.Return(Expr.ReadLocal(_local)));

            Assert.That(func(32), Is.EqualTo(32));
        }

        [Test]
        public void Should_write_and_read_local_variables()
        {
            var l1 = Expr.LocalVariable(typeof(int), "i1");
            var l2 = Expr.LocalVariable(typeof(object), "i2");
            var l3 = Expr.LocalVariable(typeof(int), "i3");
            var l4 = Expr.LocalVariable(typeof(long), "i4");
            var l5 = Expr.LocalVariable(typeof(short), "i5");

            var func = CreateFunc<int, short>(
                Expr.DeclareLocal(l1, Expr.Parameter(0, typeof(int))),
                Expr.DeclareLocal(l2, Expr.Convert(Expr.ReadLocal(l1), typeof(object))),
                Expr.DeclareLocal(l3, Expr.Convert(Expr.ReadLocal(l2), typeof(int))),
                Expr.DeclareLocal(l4, Expr.Convert(Expr.ReadLocal(l3), typeof(long))),
                Expr.DeclareLocal(l5, Expr.Convert(Expr.ReadLocal(l4), typeof(short))),
                Expr.Return(Expr.ReadLocal(l5)));

            Assert.That(func(322), Is.EqualTo(322));
        }

        [Test]
        public void Should_not_allow_reading_uninitialized_variable()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => CreateFunc<int>(Expr.Return(Expr.ReadLocal(_local))));
            Assert.That(ex.Message, Is.EqualTo("Uninitialized local variable access: System.Int32 x"));
        }
    }
}