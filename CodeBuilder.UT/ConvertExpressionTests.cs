using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace CodeBuilder.UT
{
    [TestFixture]
    public class ConvertExpressionTests : BuilderTestBase
    {
        public class BaseType { }
        public class DerivedType : BaseType { }
        private static readonly IDictionary<Type, IDictionary<Type, Func<object, object>>> _castTable = new Dictionary<Type, IDictionary<Type, Func<object, object>>>();

        public class ConversionExpectation
        {
            public ConversionExpectation(Type key, Func<object, object> value)
            {
                ExpectedConversion = value;
                ToType = key;
            }

            public Type ToType { get; private set; }
            public Func<object, object> ExpectedConversion { get; private set; }

            public override string ToString()
            {
                return string.Format("=> {0}", ToType);
            }
        }

        private static void AddCast<TFrom, TTo>(Func<TFrom, TTo> cast)
        {
            IDictionary<Type, Func<object, object>> dict;
            if (!_castTable.TryGetValue(typeof(TFrom), out dict))
                _castTable.Add(typeof(TFrom), dict = new Dictionary<Type, Func<object, object>>());

            dict.Add(typeof(TTo), p => cast((TFrom)p));
        }

        #region SetUp
        static ConvertExpressionTests()
        {
            AddCast<byte, sbyte>(v => (sbyte)v);
            AddCast<byte, byte>(v => (byte)v);
            AddCast<byte, short>(v => (short)v);
            AddCast<byte, ushort>(v => (ushort)v);
            AddCast<byte, char>(v => (char)v);
            AddCast<byte, uint>(v => (uint)v);
            AddCast<byte, int>(v => (int)v);
            AddCast<byte, long>(v => (long)v);
            AddCast<byte, ulong>(v => (ulong)v);
            AddCast<byte, float>(v => (float)v);
            AddCast<byte, double>(v => (double)v);

            AddCast<sbyte, sbyte>(v => (sbyte)v);
            AddCast<sbyte, byte>(v => (byte)v);
            AddCast<sbyte, short>(v => (short)v);
            AddCast<sbyte, ushort>(v => (ushort)v);
            AddCast<sbyte, char>(v => (char)v);
            AddCast<sbyte, uint>(v => (uint)v);
            AddCast<sbyte, int>(v => (int)v);
            AddCast<sbyte, long>(v => (long)v);
            AddCast<sbyte, ulong>(v => (ulong)v);
            AddCast<sbyte, float>(v => (float)v);
            AddCast<sbyte, double>(v => (double)v);

            AddCast<ushort, sbyte>(v => (sbyte)v);
            AddCast<ushort, byte>(v => (byte)v);
            AddCast<ushort, short>(v => (short)v);
            AddCast<ushort, ushort>(v => (ushort)v);
            AddCast<ushort, char>(v => (char)v);
            AddCast<ushort, uint>(v => (uint)v);
            AddCast<ushort, int>(v => (int)v);
            AddCast<ushort, long>(v => (long)v);
            AddCast<ushort, ulong>(v => (ulong)v);
            AddCast<ushort, float>(v => (float)v);
            AddCast<ushort, double>(v => (double)v);

            AddCast<short, sbyte>(v => (sbyte)v);
            AddCast<short, byte>(v => (byte)v);
            AddCast<short, short>(v => (short)v);
            AddCast<short, ushort>(v => (ushort)v);
            AddCast<short, char>(v => (char)v);
            AddCast<short, uint>(v => (uint)v);
            AddCast<short, int>(v => (int)v);
            AddCast<short, long>(v => (long)v);
            AddCast<short, ulong>(v => (ulong)v);
            AddCast<short, float>(v => (float)v);
            AddCast<short, double>(v => (double)v);

            AddCast<char, sbyte>(v => (sbyte)v);
            AddCast<char, byte>(v => (byte)v);
            AddCast<char, short>(v => (short)v);
            AddCast<char, ushort>(v => (ushort)v);
            AddCast<char, char>(v => (char)v);
            AddCast<char, uint>(v => (uint)v);
            AddCast<char, int>(v => (int)v);
            AddCast<char, long>(v => (long)v);
            AddCast<char, ulong>(v => (ulong)v);
            AddCast<char, float>(v => (float)v);
            AddCast<char, double>(v => (double)v);

            AddCast<int, sbyte>(v => (sbyte)v);
            AddCast<int, byte>(v => (byte)v);
            AddCast<int, short>(v => (short)v);
            AddCast<int, ushort>(v => (ushort)v);
            AddCast<int, char>(v => (char)v);
            AddCast<int, uint>(v => (uint)v);
            AddCast<int, int>(v => (int)v);
            AddCast<int, long>(v => (long)v);
            AddCast<int, ulong>(v => (ulong)v);
            AddCast<int, float>(v => (float)v);
            AddCast<int, double>(v => (double)v);

            AddCast<uint, sbyte>(v => (sbyte)v);
            AddCast<uint, byte>(v => (byte)v);
            AddCast<uint, short>(v => (short)v);
            AddCast<uint, ushort>(v => (ushort)v);
            AddCast<uint, char>(v => (char)v);
            AddCast<uint, uint>(v => (uint)v);
            AddCast<uint, int>(v => (int)v);
            AddCast<uint, long>(v => (long)v);
            AddCast<uint, ulong>(v => (ulong)v);
            AddCast<uint, float>(v => (float)v);
            AddCast<uint, double>(v => (double)v);

            AddCast<long, sbyte>(v => (sbyte)v);
            AddCast<long, byte>(v => (byte)v);
            AddCast<long, short>(v => (short)v);
            AddCast<long, ushort>(v => (ushort)v);
            AddCast<long, char>(v => (char)v);
            AddCast<long, uint>(v => (uint)v);
            AddCast<long, int>(v => (int)v);
            AddCast<long, long>(v => (long)v);
            AddCast<long, ulong>(v => (ulong)v);
            AddCast<long, float>(v => (float)v);
            AddCast<long, double>(v => (double)v);

            AddCast<ulong, sbyte>(v => (sbyte)v);
            AddCast<ulong, byte>(v => (byte)v);
            AddCast<ulong, short>(v => (short)v);
            AddCast<ulong, ushort>(v => (ushort)v);
            AddCast<ulong, char>(v => (char)v);
            AddCast<ulong, uint>(v => (uint)v);
            AddCast<ulong, int>(v => (int)v);
            AddCast<ulong, long>(v => (long)v);
            AddCast<ulong, ulong>(v => (ulong)v);
            AddCast<ulong, float>(v => (float)v);
            AddCast<ulong, double>(v => (double)v);

            AddCast<float, sbyte>(v => (sbyte)v);
            AddCast<float, byte>(v => (byte)v);
            AddCast<float, short>(v => (short)v);
            AddCast<float, ushort>(v => (ushort)v);
            AddCast<float, char>(v => (char)v);
            AddCast<float, uint>(v => (uint)v);
            AddCast<float, int>(v => (int)v);
            AddCast<float, long>(v => (long)v);
            AddCast<float, ulong>(v => (ulong)v);
            AddCast<float, float>(v => (float)v);
            AddCast<float, double>(v => (double)v);

            AddCast<double, sbyte>(v => (sbyte)v);
            AddCast<double, byte>(v => (byte)v);
            AddCast<double, short>(v => (short)v);
            AddCast<double, ushort>(v => (ushort)v);
            AddCast<double, char>(v => (char)v);
            AddCast<double, uint>(v => (uint)v);
            AddCast<double, int>(v => (int)v);
            AddCast<double, long>(v => (long)v);
            AddCast<double, ulong>(v => (ulong)v);
            AddCast<double, float>(v => (float)v);
            AddCast<double, double>(v => (double)v);
        }
        #endregion

        [Test]
        public void ExpressionTypeTest()
        {
            Assert.That(Expr.Convert(Expr.Constant(3.14), typeof(short)).ExpressionType, Is.EqualTo(typeof(short)));
        }

        [Test]
        public void NullTypeParamererTest()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => Expr.Convert(Expr.Constant("abc"), null));
            Assert.That(ex.Message, Is.StringContaining("a"));
        }

        [Test]
        public void NullParamererTest()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => Expr.Convert(null, typeof(char)));
            Assert.That(ex.Message, Is.StringContaining("a"));
        }

        [Test]
        public void Should_convert_int(
            [ValueSource("GetIntValues")] int value,
            [ValueSource("GetIntConversions")] ConversionExpectation expectation)
        {
            var fun = CreateConvertFunc(typeof(int), expectation.ToType);
            Assert.That(fun(value), Is.EqualTo(expectation.ExpectedConversion(value)));
        }

        [Test]
        public void Should_convert_uint(
            [ValueSource("GetUIntValues")] uint value,
            [ValueSource("GetUIntConversions")] ConversionExpectation expectation)
        {
            var fun = CreateConvertFunc(typeof(uint), expectation.ToType);
            Assert.That(fun(value), Is.EqualTo(expectation.ExpectedConversion(value)));
        }

        [Test]
        public void Should_convert_byte(
            [ValueSource("GetByteValues")] byte value,
            [ValueSource("GetByteConversions")] ConversionExpectation expectation)
        {
            var fun = CreateConvertFunc(typeof(byte), expectation.ToType);
            Assert.That(fun(value), Is.EqualTo(expectation.ExpectedConversion(value)));
        }

        [Test]
        public void Should_convert_sbyte(
            [ValueSource("GetSByteValues")] sbyte value,
            [ValueSource("GetSByteConversions")] ConversionExpectation expectation)
        {
            var fun = CreateConvertFunc(typeof(sbyte), expectation.ToType);
            Assert.That(fun(value), Is.EqualTo(expectation.ExpectedConversion(value)));
        }

        [Test]
        public void Should_convert_short(
            [ValueSource("GetShortValues")] short value,
            [ValueSource("GetShortConversions")] ConversionExpectation expectation)
        {
            var fun = CreateConvertFunc(typeof(short), expectation.ToType);
            Assert.That(fun(value), Is.EqualTo(expectation.ExpectedConversion(value)));
        }

        [Test]
        public void Should_convert_ushort(
            [ValueSource("GetUShortValues")] ushort value,
            [ValueSource("GetUShortConversions")] ConversionExpectation expectation)
        {
            var fun = CreateConvertFunc(typeof(ushort), expectation.ToType);
            Assert.That(fun(value), Is.EqualTo(expectation.ExpectedConversion(value)));
        }

        [Test]
        public void Should_convert_char(
            [ValueSource("GetCharValues")] char value,
            [ValueSource("GetCharConversions")] ConversionExpectation expectation)
        {
            var fun = CreateConvertFunc(typeof(char), expectation.ToType);
            Assert.That(fun(value), Is.EqualTo(expectation.ExpectedConversion(value)));
        }

        [Test]
        public void Should_convert_long(
            [ValueSource("GetLongValues")] long value,
            [ValueSource("GetLongConversions")] ConversionExpectation expectation)
        {
            var fun = CreateConvertFunc(typeof(long), expectation.ToType);
            Assert.That(fun(value), Is.EqualTo(expectation.ExpectedConversion(value)));
        }

        [Test]
        public void Should_convert_ulong(
            [ValueSource("GetULongValues")] ulong value,
            [ValueSource("GetULongConversions")] ConversionExpectation expectation)
        {
            var fun = CreateConvertFunc(typeof(ulong), expectation.ToType);
            Assert.That(fun(value), Is.EqualTo(expectation.ExpectedConversion(value)));
        }

        [Test]
        public void Should_convert_float(
            [ValueSource("GetFloatValues")] float value,
            [ValueSource("GetFloatConversions")] ConversionExpectation expectation)
        {
            var fun = CreateConvertFunc(typeof(float), expectation.ToType);
            Assert.That(fun(value), Is.EqualTo(expectation.ExpectedConversion(value)));
        }

        [Test]
        public void Should_convert_double(
            [ValueSource("GetDoubleValues")] double value,
            [ValueSource("GetDoubleConversions")] ConversionExpectation expectation)
        {
            var fun = CreateConvertFunc(typeof(double), expectation.ToType);
            Assert.That(fun(value), Is.EqualTo(expectation.ExpectedConversion(value)));
        }

        [Test]
        public void Should_throw_OverflowException_for_byte()
        {
            AssertOverflow<byte, sbyte>(0, (a, b) => (sbyte)(a + b), -1);
            AssertOverflow<byte, short>(byte.MinValue, byte.MaxValue, (a, b) => (short)(a + b));
            AssertOverflow<byte, ushort>(byte.MaxValue, (a, b) => (ushort)(a + b), 1);
            AssertOverflow<byte, char>((char)byte.MaxValue, (a, b) => (char)(a + b), 1);
            AssertOverflow<byte, int>(byte.MinValue, byte.MaxValue, (a, b) => a + b);
            AssertOverflow<byte, uint>(byte.MaxValue, (a, b) => (uint)(a + b), 1);
            AssertOverflow<byte, long>(byte.MinValue, byte.MaxValue, (a, b) => a + b);
            AssertOverflow<byte, ulong>(byte.MaxValue, (a, b) => a + (uint)b, 1);
        }

        [Test]
        public void Should_throw_OverflowException_for_sbyte()
        {
            AssertOverflow<sbyte, byte>((byte)sbyte.MaxValue, (a, b) => (byte)(a + b), 1);
            AssertOverflow<sbyte, short>(sbyte.MinValue, sbyte.MaxValue, (a, b) => (short)(a + b));
            AssertOverflow<sbyte, ushort>((ushort)sbyte.MaxValue, (a, b) => (ushort)(a + b), 1);
            AssertOverflow<sbyte, char>((char)sbyte.MaxValue, (a, b) => (char)(a + b), 1);
            AssertOverflow<sbyte, int>(sbyte.MinValue, sbyte.MaxValue, (a, b) => a + b);
            AssertOverflow<sbyte, uint>((uint)sbyte.MaxValue, (a, b) => (uint)(a + b), 1);
            AssertOverflow<sbyte, long>(sbyte.MinValue, sbyte.MaxValue, (a, b) => a + b);
            AssertOverflow<sbyte, ulong>((ulong)sbyte.MaxValue, (a, b) => a + (uint)b, 1);
        }

        [Test]
        public void Should_throw_OverflowException_for_ushort()
        {
            AssertOverflow<ushort, sbyte>(0, (a, b) => (sbyte)(a + b), -1);
            AssertOverflow<ushort, short>(0, (a, b) => (short)(a + b), -1);
            AssertOverflow<ushort, int>(ushort.MinValue, ushort.MaxValue, (a, b) => a + b);
            AssertOverflow<ushort, uint>(ushort.MaxValue, (a, b) => (uint)(a + b), 1);
            AssertOverflow<ushort, long>(ushort.MinValue, ushort.MaxValue, (a, b) => a + b);
            AssertOverflow<ushort, ulong>(ushort.MaxValue, (a, b) => a + (uint)b, 1);
        }

        [Test]
        public void Should_throw_OverflowException_for_char()
        {
            AssertOverflow<char, sbyte>(0, (a, b) => (sbyte)(a + b), -1);
            AssertOverflow<char, short>(0, (a, b) => (short)(a + b), -1);
            AssertOverflow<char, int>(char.MinValue, char.MaxValue, (a, b) => a + b);
            AssertOverflow<char, uint>(char.MaxValue, (a, b) => (uint)(a + b), 1);
            AssertOverflow<char, long>(char.MinValue, char.MaxValue, (a, b) => a + b);
            AssertOverflow<char, ulong>(char.MaxValue, (a, b) => a + (uint)b, 1);
        }

        [Test]
        public void Should_throw_OverflowException_for_short()
        {
            AssertOverflow<short, ushort>((ushort)short.MaxValue, (a, b) => (ushort)(a + b), 1);
            AssertOverflow<short, char>((char)short.MaxValue, (a, b) => (char)(a + b), 1);
            AssertOverflow<short, int>(short.MinValue, short.MaxValue, (a, b) => a + b);
            AssertOverflow<short, uint>((uint)short.MaxValue, (a, b) => (uint)(a + b), 1);
            AssertOverflow<short, long>(short.MinValue, short.MaxValue, (a, b) => a + b);
            AssertOverflow<short, ulong>((ulong)short.MaxValue, (a, b) => a + (uint)b, 1);
        }

        [Test]
        public void Should_throw_OverflowException_for_uint()
        {
            AssertOverflow<uint, sbyte>(0, (a, b) => (sbyte)(a + b), -1);
            AssertOverflow<uint, short>(0, (a, b) => (short)(a + b), -1);
            AssertOverflow<uint, int>(0, (a, b) => a + b, -1);
            AssertOverflow<uint, long>(uint.MinValue, uint.MaxValue, (a, b) => a + b);
            AssertOverflow<uint, ulong>(uint.MaxValue, (a, b) => a + (uint)b, 1);
        }

        [Test]
        public void Should_throw_OverflowException_for_int()
        {
            AssertOverflow<int, uint>(int.MaxValue, (a, b) => (uint)(a + b), 1);
            AssertOverflow<int, long>(int.MinValue, int.MaxValue, (a, b) => a + b);
            AssertOverflow<int, ulong>(int.MaxValue, (a, b) => a + (uint)b, 1);
        }

        [Test]
        public void Should_throw_OverflowException_for_ulong()
        {
            AssertOverflow<ulong, sbyte>(0, (a, b) => (sbyte)(a + b), -1);
            AssertOverflow<ulong, short>(0, (a, b) => (short)(a + b), -1);
            AssertOverflow<ulong, int>(0, (a, b) => a + b, -1);
            AssertOverflow<ulong, long>(0, (a, b) => a + b, -1);
        }

        [Test]
        public void Should_throw_OverflowException_for_long()
        {
            AssertOverflow<long, ulong>(long.MaxValue, (a, b) => a + (uint)b, 1);
        }

        [Test]
        public void Should_box_and_unbox_primitives()
        {
            AssertBoxing<byte, object>(3);
            AssertBoxing<sbyte, object>(3);
            AssertBoxing<short, object>(3);
            AssertBoxing<ushort, object>(3);
            AssertBoxing<char, object>((char)3);
            AssertBoxing<int, object>(3);
            AssertBoxing<uint, object>(3);
            AssertBoxing<long, object>(3);
            AssertBoxing<ulong, object>(3);
            AssertBoxing<float, object>(3.14f);
            AssertBoxing<double, object>(3.14);
        }

        [Test]
        public void Should_box_and_unbox_structs()
        {
            AssertBoxing<DateTime, object>(new DateTime(2015, 03, 02, 22, 23, 32, 647));
        }

        [Test]
        public void Should_box_and_unbox_primitive_to_interface()
        {
            AssertBoxing<int, IComparable>(3);
        }

        [Test]
        public void Should_cast_to_base_type()
        {
            var derivedType = new DerivedType();
            BaseType casted = CreateConvertFunc<DerivedType, BaseType>().Invoke(derivedType);
            Assert.That(casted, Is.SameAs(derivedType));
        }

        [Test]
        public void Should_cast_to_derived_type()
        {
            BaseType baseType = new DerivedType();
            DerivedType casted = CreateConvertFunc<BaseType, DerivedType>().Invoke(baseType);
            Assert.That(casted, Is.SameAs(baseType));
        }

        [Test]
        public void Should_cast_fail_on_casting_to_derived_type_if_type_does_not_match()
        {
            Assert.Throws<InvalidCastException>(() => CreateConvertFunc<BaseType, DerivedType>().Invoke(new BaseType()));
        }

        [Test]
        public void Should_not_allow_conversion_of_unrelated_types()
        {
            var ex = Assert.Throws<ArgumentException>(() => CreateConvertFunc<string, DateTime>());
            Assert.That(ex.Message, Is.StringContaining("Cannot convert type from System.String to System.DateTime"));
        }

        private void AssertBoxing<TUnboxed, TBoxed>(TUnboxed value)
            where TUnboxed : struct, TBoxed
            where TBoxed : class
        {
            TBoxed boxed = CreateConvertFunc<TUnboxed, TBoxed>()(value);
            Assert.That(boxed, Is.EqualTo(value));
            TUnboxed unboxed = CreateConvertFunc<TBoxed, TUnboxed>()(boxed);
            Assert.That(unboxed, Is.EqualTo(value));
        }

        private void AssertOverflow<TConverted, T>(T min, T max, Func<T, int, T> addition)
        {
            AssertOverflow<TConverted, T>(min, addition, -1);
            AssertOverflow<TConverted, T>(max, addition, 1);
        }

        private static void AssertOverflow<TConverted, T>(T value, Func<T, int, T> addition, int delta)
        {
            var expected = _castTable[typeof(T)][typeof(TConverted)](value);
            var func = CreateConvertCheckedFunc<T, TConverted>();
            Assert.That(func(value), Is.EqualTo(expected));

            Assert.Throws<OverflowException>(() => func(addition(value, delta)));
        }

        private static Func<TParam, TRet> CreateConvertFunc<TParam, TRet>()
        {
            return CreateFunc<TParam, TRet>(Expr.Return(Expr.Convert(Expr.Parameter(0, typeof(TParam)), typeof(TRet))));
        }

        private static Func<TParam, TRet> CreateConvertCheckedFunc<TParam, TRet>()
        {
            return CreateFunc<TParam, TRet>(Expr.Return(Expr.ConvertChecked(Expr.Parameter(0, typeof(TParam)), typeof(TRet))));
        }

        private static Func<object, object> CreateConvertFunc(Type from, Type to)
        {
            Func<Func<object, object>> fun = CreateConvertFunc<object, object>;
            var generic = (MulticastDelegate)fun.Method.GetGenericMethodDefinition().MakeGenericMethod(@from, to).Invoke(null, new object[0]);
            return p => generic.DynamicInvoke(p);
        }
        #region Data factories
        public static IEnumerable<ConversionExpectation> GetUIntConversions()
        {
            return _castTable[typeof(uint)].Select(kv => new ConversionExpectation(kv.Key, kv.Value));
        }

        public static uint[] GetUIntValues()
        {
            return new[] { uint.MinValue, byte.MaxValue, ushort.MaxValue, uint.MaxValue };
        }

        public static IEnumerable<ConversionExpectation> GetIntConversions()
        {
            return _castTable[typeof(int)].Select(kv => new ConversionExpectation(kv.Key, kv.Value));
        }

        public static int[] GetIntValues()
        {
            return new[] { int.MinValue, short.MinValue, sbyte.MinValue, -1, 0, sbyte.MaxValue, byte.MaxValue, short.MaxValue, ushort.MaxValue, int.MaxValue };
        }

        public static IEnumerable<ConversionExpectation> GetByteConversions()
        {
            return _castTable[typeof(byte)].Select(kv => new ConversionExpectation(kv.Key, kv.Value));
        }

        public static byte[] GetByteValues()
        {
            return new[] { byte.MinValue, byte.MaxValue };
        }

        public static IEnumerable<ConversionExpectation> GetSByteConversions()
        {
            return _castTable[typeof(sbyte)].Select(kv => new ConversionExpectation(kv.Key, kv.Value));
        }

        public static sbyte[] GetSByteValues()
        {
            return new[] { sbyte.MinValue, (sbyte)-1, (sbyte)0, sbyte.MaxValue };
        }

        public static IEnumerable<ConversionExpectation> GetUShortConversions()
        {
            return _castTable[typeof(ushort)].Select(kv => new ConversionExpectation(kv.Key, kv.Value));
        }

        public static ushort[] GetUShortValues()
        {
            return new[] { ushort.MinValue, ushort.MaxValue };
        }

        public static IEnumerable<ConversionExpectation> GetCharConversions()
        {
            return _castTable[typeof(char)].Select(kv => new ConversionExpectation(kv.Key, kv.Value));
        }

        public static char[] GetCharValues()
        {
            return new[] { char.MinValue, char.MaxValue };
        }

        public static IEnumerable<ConversionExpectation> GetShortConversions()
        {
            return _castTable[typeof(short)].Select(kv => new ConversionExpectation(kv.Key, kv.Value));
        }

        public static short[] GetShortValues()
        {
            return new[] { short.MinValue, (short)-1, (short)0, short.MaxValue };
        }

        public static IEnumerable<ConversionExpectation> GetULongConversions()
        {
            return _castTable[typeof(ulong)].Select(kv => new ConversionExpectation(kv.Key, kv.Value));
        }

        public static ulong[] GetULongValues()
        {
            return new[] { ulong.MinValue, byte.MaxValue, ushort.MaxValue, uint.MaxValue, ulong.MaxValue };
        }

        public static IEnumerable<ConversionExpectation> GetLongConversions()
        {
            return _castTable[typeof(long)].Select(kv => new ConversionExpectation(kv.Key, kv.Value));
        }

        public static long[] GetLongValues()
        {
            return new[] { long.MinValue, int.MinValue, short.MinValue, sbyte.MinValue, -1, 0, sbyte.MaxValue, byte.MaxValue, short.MaxValue, ushort.MaxValue, int.MaxValue, long.MaxValue };
        }

        public static IEnumerable<ConversionExpectation> GetFloatConversions()
        {
            return _castTable[typeof(float)].Select(kv => new ConversionExpectation(kv.Key, kv.Value));
        }

        public static float[] GetFloatValues()
        {
            return new[] { float.MinValue, -3.14f, -3, 0, 3, 3.14f, float.MaxValue };
        }

        public static IEnumerable<ConversionExpectation> GetDoubleConversions()
        {
            return _castTable[typeof(double)].Select(kv => new ConversionExpectation(kv.Key, kv.Value));
        }

        public static double[] GetDoubleValues()
        {
            return new[] { double.MinValue, float.MinValue, -3.14, -3, 0, 3, 3.14, float.MaxValue, double.MaxValue };
        }
        #endregion

    }
}