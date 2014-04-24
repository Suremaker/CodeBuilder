using System;
using System.Reflection;
using NUnit.Framework;

namespace CodeBuilder.UT
{
    [TestFixture]
    public class CallExpressionTests : BuilderTestBase
    {
        public class BaseClass
        {
            public string Result { get; set; }
            public string Foo()
            {
                return Result;
            }

            public virtual string Virt()
            {
                return "base";
            }
        }

        public class DerivedClass : BaseClass
        {
            public override string Virt()
            {
                return "derived";
            }
        }

        public struct Counter
        {
            public void Count()
            {
                ++Value;
            }

            public int Value { get; private set; }
        }

        public class CounterHolder
        {
            public Counter Counter;
        }

        public static void Foo() { }

        private static readonly MethodInfo _formatInfo = ((Func<string, object, string>)string.Format).Method;
        private static readonly MethodInfo _fooInfo = ((Action)Foo).Method;
        private static readonly MethodInfo _toStringInfo = typeof(int).GetMethod("ToString", new Type[0]);
        private static readonly MethodInfo _parseInfo = typeof(int).GetMethod("Parse", new[] { typeof(string) });

        [Test]
        public void VoidMethodTypeTest()
        {
            Assert.That(Expr.Call(_fooInfo).ExpressionType, Is.EqualTo(typeof(void)));
        }

        [Test]
        public void NonVoidMethodTypeTest()
        {
            Assert.That(Expr.Call(_formatInfo, Expr.Constant("{0}"), Expr.Convert(Expr.Constant(1), typeof(object))).ExpressionType, Is.EqualTo(typeof(string)));
        }

        [Test]
        public void Should_not_allow_null_methodInfo()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => Expr.Call(null));
            Assert.That(ex.Message, Is.StringContaining("methodInfo"));
        }

        [Test]
        public void Should_not_allow_null_instance_for_non_static_method()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => Expr.Call(null, _toStringInfo));
            Assert.That(ex.Message, Is.StringContaining("instance"));
        }

        [Test]
        public void Should_not_allow_not_null_instance_for_static_method()
        {
            var ex = Assert.Throws<ArgumentException>(() => Expr.Call(Expr.Constant(21), _parseInfo, Expr.Constant("22")));
            Assert.That(ex.Message, Is.StringContaining("Static method cannot be called with instance parameter"));
        }

        [Test]
        public void Should_not_allow_null_expression_array()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => Expr.Call(_toStringInfo, null));
            Assert.That(ex.Message, Is.StringContaining("arguments"));
        }

        [Test]
        public void Should_not_allow_null_expression_in_array()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => Expr.Call(_formatInfo, Expr.Constant("abs"), null));
            Assert.That(ex.Message, Is.StringContaining("arguments[1]"));
        }

        [Test]
        public void Should_not_allow_exceeding_number_of_method_parameters()
        {
            var ex = Assert.Throws<ArgumentException>(() => Expr.Call(_fooInfo, Expr.Constant(1)));
            Assert.That(ex.Message, Is.StringContaining("Invalid amount of parameters supplied to method: Void Foo()"));
        }

        [Test]
        public void Should_not_allow_insufficient_number_of_method_parameters()
        {
            var ex = Assert.Throws<ArgumentException>(() => Expr.Call(_formatInfo));
            Assert.That(ex.Message, Is.StringContaining("Invalid amount of parameters supplied to method: System.String Format(System.String, System.Object)"));
        }

        [Test]
        public void Should_not_allow_implicit_boxing_for_method_parameters()
        {
            var ex = Assert.Throws<ArgumentException>(() => Expr.Call(_formatInfo, Expr.Constant("{0}"), Expr.Constant(32)));
            Assert.That(ex.Message, Is.StringContaining("Parameter expression of type System.Int32 does not match to type: System.Object"));
        }

        [Test]
        public void Should_not_allow_implicit_boxing_for_instance_expression()
        {
            MethodInfo objToStringInfo = typeof(object).GetMethod("ToString");
            var ex = Assert.Throws<ArgumentException>(() => Expr.Call(Expr.Constant(22), objToStringInfo));
            Assert.That(ex.Message, Is.StringContaining("Instance expression of type System.Int32 does not match to type: System.Object"));
        }

        [Test]
        public void Should_allow_hierarchy_conversion_for_parameter()
        {
            var func = CreateFunc<string, string>(Expr.Return(Expr.Call(_formatInfo, Expr.Constant("Hello {0}!"), Expr.Parameter(0, typeof(string)))));
            Assert.That(func("world"), Is.EqualTo("Hello world!"));
        }

        [Test]
        public void Should_allow_hierarchy_conversion_for_instance()
        {
            MethodInfo methodInfo = typeof(BaseClass).GetMethod("Foo");
            var func = CreateFunc<DerivedClass, string>(Expr.Return(Expr.Call(Expr.Parameter(0, typeof(DerivedClass)), methodInfo)));
            Assert.That(func(new DerivedClass { Result = "hi" }), Is.EqualTo("hi"));
        }

        [Test]
        public void Should_call_struct_instance_method()
        {
            var func = CreateFunc<string>(Expr.Return(Expr.Call(Expr.Constant(21), _toStringInfo)));
            Assert.That(func(), Is.EqualTo("21"));
        }

        [Test]
        public void Should_call_virtual_method()
        {
            var virtualMethodInfo = typeof(BaseClass).GetMethod("Virt");
            var func = CreateFunc<BaseClass, string>(Expr.Return(Expr.Call(Expr.Parameter(0, typeof(BaseClass)), virtualMethodInfo)));
            Assert.That(func(new BaseClass()), Is.EqualTo("base"));
            Assert.That(func(new DerivedClass()), Is.EqualTo("derived"));
        }

        [Test]
        public void Should_call_virtual_method_in_non_virtual_way()
        {
            var virtualMethodInfo = typeof(BaseClass).GetMethod("Virt");
            var func = CreateFunc<BaseClass, string>(Expr.Return(Expr.CallExact(Expr.Parameter(0, typeof(BaseClass)), virtualMethodInfo)));
            Assert.That(func(new BaseClass()), Is.EqualTo("base"));
            Assert.That(func(new DerivedClass()), Is.EqualTo("base"));
        }

        [Test]
        public void Should_call_struct_instance_method_via_parameter()
        {
            var func = CreateFunc<Counter, Counter>(
                Expr.Call(Expr.Parameter(0, typeof(Counter)), typeof(Counter).GetMethod("Count")),
                Expr.Return(Expr.Parameter(0, typeof(Counter))));
            Assert.That(func(new Counter()).Value, Is.EqualTo(1));
        }

        [Test]
        public void Should_call_struct_instance_method_via_field()
        {
            var func = CreateAction<CounterHolder>(Expr.Call(
                Expr.ReadField(Expr.Parameter(0, typeof(CounterHolder)), typeof(CounterHolder).GetField("Counter")),
                typeof(Counter).GetMethod("Count")));
            var holder = new CounterHolder();
            func(holder);
            Assert.That(holder.Counter.Value, Is.EqualTo(1));
        }

        [Test]
        public void Should_call_struct_instance_method_via_local_variable()
        {
            var local = Expr.DeclareLocalVar(typeof(Counter), "s");
            var func = CreateFunc<Counter, Counter>(
                Expr.WriteLocal(local, Expr.Parameter(0, typeof(Counter))),
                Expr.Call(Expr.ReadLocal(local), typeof(Counter).GetMethod("Count")),
                Expr.Return(Expr.ReadLocal(local)));

            Assert.That(func(new Counter()).Value, Is.EqualTo(1));
        }
    }
}