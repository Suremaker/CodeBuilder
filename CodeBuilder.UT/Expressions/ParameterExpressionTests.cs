using System;
using System.Reflection;
using NUnit.Framework;

namespace CodeBuilder.UT.Expressions
{
    [TestFixture]
    public class ParameterExpressionTests : BuilderTestBase
    {
        public class BaseType { }
        public class DerivedType : BaseType
        {
            public DerivedType Call(int x)
            {
                X = x;
                return this;
            }

            public int X { get; set; }
        }

        [Test]
        public void ExpressionTypeTest()
        {
            Assert.That(Expr.Parameter(0, typeof(byte)).ExpressionType, Is.EqualTo(typeof(byte)));
        }

        [Test]
        public void Should_not_allow_null_type()
        {
            Assert.Throws<ArgumentNullException>(() => Expr.Parameter(0, null));
        }

        [Test]
        public void Should_not_allow_parameter_index_being_outside_of_bounds()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => CreateFunc<int, int>(Expr.Return(Expr.Parameter(1, typeof(int)))));
            Assert.That(ex.Message, Is.StringContaining("Parameter index 1 is outside of bounds. Expected parameters: [System.Int32]"));
        }

        [Test]
        public void Should_not_allow_parameter_type_not_matching_to_method_signature()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => CreateFunc<object, int>(Expr.Return(Expr.Parameter(0, typeof(int)))));
            Assert.That(ex.Message, Is.StringContaining("Parameter index 0 is of System.Int32 type, while type System.Object is expected"));
        }

        [Test]
        public void Should_not_allow_parameter_type_be_implicitly_boxed()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => CreateFunc<int, int>(Expr.Return(Expr.Convert(Expr.Parameter(0, typeof(object)), typeof(int)))));
            Assert.That(ex.Message, Is.StringContaining("Parameter index 0 is of System.Object type, while type System.Int32 is expected"));
        }

        [Test]
        public void Should_allow_hierarchy_conversion_for_parameter()
        {
            var func = CreateFunc<DerivedType, BaseType>(Expr.Return(Expr.Parameter(0, typeof(BaseType))));
            var value = new DerivedType();
            Assert.That(func(value), Is.SameAs(value));
        }

        [Test]
        public void Should_properly_map_parameters()
        {
            Func<int, char, string, long, bool, string> formatMethod = Format;
            var func = CreateFunc<int, char, string, long, bool, string>(Expr.Return(Expr.Call(
                formatMethod.Method,
                Expr.Parameter(0, typeof(int)),
                Expr.Parameter(1, typeof(char)),
                Expr.Parameter(2, typeof(string)),
                Expr.Parameter(3, typeof(long)),
                Expr.Parameter(4, typeof(bool)))));
            Assert.That(func(1, 'a', "text", -23, false), Is.EqualTo("1_a_text_-23_False"));
        }

        [Test]
        public void Should_properly_map_parameters_with_this()
        {
            MethodInfo mi = typeof(DerivedType).GetMethod("Call");
            var func = CreateFunc<DerivedType, int, DerivedType>(Expr.Return(Expr.Call(Expr.Parameter(0, typeof(DerivedType)), mi, Expr.Parameter(1, typeof(int)))));
            var obj = func(new DerivedType(), 32);
            Assert.That(obj.X, Is.EqualTo(32));
        }

        public static string Format(int arg1, char arg2, string arg3, long arg4, bool arg5)
        {
            return string.Format("{0}_{1}_{2}_{3}_{4}", arg1, arg2, arg3, arg4, arg5);
        }
    }
}