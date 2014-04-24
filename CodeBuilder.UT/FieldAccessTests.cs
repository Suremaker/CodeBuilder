using System.Reflection;
using NUnit.Framework;

namespace CodeBuilder.UT
{
    public class FieldAccessTests : BuilderTestBase
    {
        public class TestClass
        {
            private string _localField = "abc";
            public static int StaticField = 32;
            public string GetLocal()
            {
                return _localField;
            }
        }

        public struct TestStruct
        {
            private string _localField;
            public static int StaticField = 33;

            public TestStruct(string localField)
                : this()
            {
                _localField = localField;
            }

            public string GetLocal()
            {
                return _localField;
            }
        }

        protected readonly static FieldInfo _classLocal = typeof(TestClass).GetField("_localField", BindingFlags.Instance | BindingFlags.NonPublic);
        protected readonly static FieldInfo _classStatic = typeof(TestClass).GetField("StaticField");
        protected readonly static FieldInfo _structLocal = typeof(TestStruct).GetField("_localField", BindingFlags.Instance | BindingFlags.NonPublic);
        protected readonly static FieldInfo _structStatic = typeof(TestStruct).GetField("StaticField");

        [SetUp]
        public void SetUp()
        {
            TestClass.StaticField = 32;
            TestStruct.StaticField = 33;
        }
    }
}