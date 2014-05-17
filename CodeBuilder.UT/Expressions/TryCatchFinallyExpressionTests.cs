using System;
using System.Text;
using CodeBuilder.Expressions;
using NUnit.Framework;

namespace CodeBuilder.UT.Expressions
{
    [TestFixture]
    public class TryCatchFinallyExpressionTests : BuilderTestBase
    {
        [Test]
        public void ExpressionTypeTest()
        {
            Assert.That(Expr.TryCatchFinally(Expr.Constant(1), Expr.Constant(2), new CatchBlock(typeof(Exception), Expr.Constant(3))).ExpressionType, Is.EqualTo(typeof(void)));
        }

        [Test]
        public void NullTryParamererTest()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => Expr.TryCatchFinally(null, Expr.Empty()));
            Assert.That(ex.Message, Is.StringContaining("tryExpression"));
        }

        [Test]
        public void NullCatchBlockParamererTest()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => Expr.TryCatchFinally(Expr.Empty(), Expr.Empty(), null));
            Assert.That(ex.Message, Is.StringContaining("catchBlocks"));
        }

        [Test]
        public void Should_not_allow_omitting_finally_and_catch_blocks()
        {
            var ex = Assert.Throws<ArgumentException>(() => Expr.TryCatchFinally(Expr.Empty(), null));
            Assert.That(ex.Message, Is.StringContaining("Try-Catch-Finally block has to have finally block or at least one catch block"));
        }

        [Test]
        public void Should_not_allow_defining_catch_blocks_that_would_be_covered_by_previous_ones()
        {
            var ex = Assert.Throws<ArgumentException>(() => Expr.TryCatch(Expr.Empty(), new CatchBlock(typeof(Exception), Expr.Empty()), new CatchBlock(typeof(InvalidOperationException), Expr.Empty())));
            Assert.That(ex.Message, Is.StringContaining("Catch block of System.InvalidOperationException type cannot be defined after block of System.Exception type"));

            ex = Assert.Throws<ArgumentException>(() => Expr.TryCatch(Expr.Empty(), new CatchBlock(Expr.Empty()), new CatchBlock(typeof(Exception), Expr.Empty())));
            Assert.That(ex.Message, Is.StringContaining("Catch block of System.Exception type cannot be defined after block of System.Object type"));
        }

        [Test]
        public void Should_catch_exceptions()
        {
            var builder = new StringBuilder();
            Func<char, StringBuilder> appendFn = builder.Append;

            var argParam = Expr.Parameter(0, typeof(int));
            var sbParam = Expr.Parameter(1, typeof(StringBuilder));
            var func = CreateAction<int, StringBuilder>(Expr.TryCatchFinally(
                Expr.Block(
                    Expr.IfThen(Expr.Equal(argParam, Expr.Constant(1)), Expr.Throw(Expr.New(typeof(InvalidOperationException)))),
                    Expr.IfThen(Expr.Equal(argParam, Expr.Constant(2)), Expr.Throw(Expr.New(typeof(ArgumentNullException)))),
                    Expr.IfThen(Expr.Equal(argParam, Expr.Constant(3)), Expr.Throw(Expr.New(typeof(ArgumentException)))),
                    Expr.IfThen(Expr.Equal(argParam, Expr.Constant(4)), Expr.Throw(Expr.New(typeof(ArithmeticException)))),
                    Expr.Call(sbParam, appendFn.Method, ConstChar('X'))
                ),
                Expr.Call(sbParam, appendFn.Method, ConstChar('F')),
                new CatchBlock(typeof(InvalidOperationException), Expr.Call(sbParam, appendFn.Method, ConstChar('I'))),
                new CatchBlock(typeof(ArgumentNullException), Expr.Block(Expr.Call(sbParam, appendFn.Method, ConstChar('N')), Expr.Rethrow())),
                new CatchBlock(typeof(ArgumentException), Expr.Call(sbParam, appendFn.Method, ConstChar('A'))),
                new CatchBlock(Expr.Call(sbParam, appendFn.Method, ConstChar('D')))));

            func(0, builder.Clear());
            Assert.That(builder.ToString(), Is.EqualTo("XF"));

            func(1, builder.Clear());
            Assert.That(builder.ToString(), Is.EqualTo("IF"));

            func(3, builder.Clear());
            Assert.That(builder.ToString(), Is.EqualTo("AF"));

            func(4, builder.Clear());
            Assert.That(builder.ToString(), Is.EqualTo("DF"));

            Assert.Throws<ArgumentNullException>(() => func(2, builder.Clear()));
            Assert.That(builder.ToString(), Is.EqualTo("NF"));
        }

        [Test]
        public void Should_allow_to_use_catched_exception()
        {
            var local = Expr.DeclareLocalVar(typeof(Exception), "e");
            var func = CreateFunc<Exception>(
                Expr.TryCatch(Expr.Throw(Expr.New(typeof(InvalidOperationException), Expr.Constant("abc"))), new CatchBlock(typeof(InvalidOperationException), local, Expr.Empty())),
                Expr.Return(Expr.ReadLocal(local)));
            Assert.That(func().Message, Is.EqualTo("abc"));
        }

        [Test]
        public void Should_not_allow_passing_null_to_CatchBlock()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new CatchBlock(null, Expr.Empty()));
            Assert.That(ex.Message, Is.StringContaining("exceptionType"));

            ex = Assert.Throws<ArgumentNullException>(() => new CatchBlock(typeof(Exception), null));
            Assert.That(ex.Message, Is.StringContaining("catchExpression"));

            ex = Assert.Throws<ArgumentNullException>(() => new CatchBlock(typeof(Exception), null, Expr.Empty()));
            Assert.That(ex.Message, Is.StringContaining("exceptionVariable"));
        }

        [Test]
        public void Should_not_allow_passing_wrong_exception_types_to_CatchBlock()
        {
            var ex = Assert.Throws<ArgumentException>(() => new CatchBlock(typeof(object), Expr.Empty()));
            Assert.That(ex.Message, Is.StringContaining("Provided type System.Object has to be deriving from System.Exception"));

            ex = Assert.Throws<ArgumentException>(() => new CatchBlock(typeof(Exception), Expr.DeclareLocalVar(typeof(InvalidOperationException), "e"), Expr.Empty()));
            Assert.That(ex.Message, Is.StringContaining("Unable to assign exception of type System.Exception to local of type System.InvalidOperationException"));
        }

        Expression ConstChar(char c)
        {
            return Expr.Convert(Expr.Constant(c), typeof(char));
        }
    }
}