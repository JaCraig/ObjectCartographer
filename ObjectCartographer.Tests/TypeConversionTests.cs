using ObjectCartographer.Tests.BaseClasses;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using Xunit;

namespace ObjectCartographer.Tests
{
    public enum MyEnumTest
    {
        Item1,
        Item2,
        Item3
    }

    public interface IMyTestClass
    {
    }

    public class MyTestClass : IMyTestClass
    {
        public MyTestClass()
        {
            B = 10;
        }

        public virtual MyTestClass A { get; set; }

        public virtual int B { get; set; }
    }

    public class MyTestClass2
    {
        public MyTestClass2()
        {
            B = 20;
        }

        public virtual int B { get; set; }
    }

    public class ProjectionTestClass
    {
        public int FinalValue { get; set; }
    }

    public class TypeConversionTests : TestBaseClass
    {
        protected override Type ObjectType { get; set; } = null;

        [Fact]
        public void DbTypeToSqlDbType()
        {
            Assert.Equal(SqlDbType.Int, DbType.Int32.To(SqlDbType.Int));
            Assert.Equal(SqlDbType.NVarChar, DbType.String.To(SqlDbType.Int));
            Assert.Equal(SqlDbType.Real, DbType.Single.To(SqlDbType.Int));
        }

        [Fact]
        public void DbTypeToType()
        {
            Assert.Equal(typeof(int), DbType.Int32.To(typeof(int)));
            Assert.Equal(typeof(string), DbType.String.To(typeof(int)));
            Assert.Equal(typeof(float), DbType.Single.To(typeof(int)));
        }

        [Fact]
        public void ProjectionTest()
        {
            new MyTestClass2().Map<MyTestClass2, ProjectionTestClass>()
                .AddMapping(x => x.B + 20, (x, val) => x.FinalValue = val)
                .Build();
            Assert.Equal(40, new MyTestClass2().To<ProjectionTestClass>().FinalValue);
        }

        [Fact]
        public void SqlDbTypeToDbType()
        {
            Assert.Equal(DbType.Int32, SqlDbType.Int.To(DbType.Int32));
            Assert.Equal(DbType.String, SqlDbType.NVarChar.To(DbType.Int32));
            Assert.Equal(DbType.Single, SqlDbType.Real.To(DbType.Int32));
        }

        [Fact]
        public void SqlDbTypeToType()
        {
            Assert.Equal(typeof(int), SqlDbType.Int.To(typeof(int)));
            Assert.Equal(typeof(string), SqlDbType.NVarChar.To(typeof(int)));
            Assert.Equal(typeof(float), SqlDbType.Real.To(typeof(int)));
        }

        [Fact]
        public void To()
        {
            Assert.Equal(new Uri("http://A"), "http://A".To<Uri>());
            Assert.Equal("http://a/", new Uri("http://A").To<string>());
            var Value = DBNull.Value;
            Assert.Equal("A", Value.To("A"));
            Assert.Equal(default(DateTime), Value.To(typeof(DateTime), null));
            Assert.Equal(TestEnum.Value2, 1.To(typeof(TestEnum), null));
            Assert.Equal(TestEnum.Value3, "Value3".To(typeof(TestEnum), null));
        }

        [Fact]
        public void ToExpando()
        {
            var TestObject = new MyTestClass();
            var Object = TestObject.To<ExpandoObject>();
            Assert.Equal(10, ((IDictionary<string, object>)Object)["B"]);
            ((IDictionary<string, object>)Object)["B"] = 20;
            Assert.Equal(20, Object.To(new MyTestClass()).B);
        }

        [Fact]
        public void ToList()
        {
            var TestObject = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8 };
            Assert.Equal(new List<long>() { 1, 2, 3, 4, 5, 6, 7, 8 }, TestObject.To(new List<long>()));
        }

        [Fact]
        public void TryConvert()
        {
            Assert.Equal(1, (1.0f).To(0));
            Assert.Equal("2011", (2011).To(""));
            Assert.NotNull(new MyTestClass().To<IMyTestClass>());
            Assert.NotNull(((object)new MyTestClass()).To<IMyTestClass>());
            Assert.NotNull(new MyTestClass().To<MyTestClass2>());
            Assert.NotNull(((object)new MyTestClass()).To<MyTestClass2>());
            var Result = new MyTestClass().To<MyTestClass2>();
            Assert.Equal(10, Result.B);
        }

        [Fact]
        public void TypeToDbType()
        {
            Assert.Equal(DbType.Int32, typeof(int).To(DbType.Int32));
            Assert.Equal(DbType.String, typeof(string).To(DbType.Int32));
            Assert.Equal(DbType.Single, typeof(float).To(DbType.Int32));
            Assert.Equal(DbType.Int32, typeof(MyEnumTest).To(DbType.Int32));
        }

        [Fact]
        public void TypeToSqlDbType()
        {
            Assert.Equal(SqlDbType.Int, typeof(int).To(SqlDbType.Int));
            Assert.Equal(SqlDbType.NVarChar, typeof(string).To(SqlDbType.Int));
            Assert.Equal(SqlDbType.Real, typeof(float).To(SqlDbType.Int));
            Assert.Equal(SqlDbType.Int, typeof(MyEnumTest).To(SqlDbType.Int));
        }

        private enum TestEnum
        {
            Value1 = 0,
            Value2,
            Value3
        }
    }
}