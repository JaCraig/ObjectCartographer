using ObjectCartographer.SQL.Converters;
using ObjectCartographer.Tests.BaseClasses;
using System;
using System.Data;
using Xunit;

namespace ObjectCartographer.Tests.SQLTests
{
    public class SqlDbTypeConversion : TestBaseClass<SqlDbTypeConverter>
    {
        public SqlDbTypeConversion()
        {
            TestObject = new SqlDbTypeConverter();
        }

        public static readonly TheoryData<SqlDbType, DbType> SQLToDbTypeData = new TheoryData<SqlDbType, DbType>
        {
            {SqlDbType.SmallInt,DbType.Int16 },
            {SqlDbType.BigInt,DbType.Int64 },
            {SqlDbType.Int,DbType.Int32 },
            {SqlDbType.Bit,DbType.Boolean },
            {SqlDbType.NVarChar,DbType.String },
            {SqlDbType.DateTime2,DbType.DateTime2 },
            {SqlDbType.Decimal,DbType.Decimal },
            {SqlDbType.Real,DbType.Single },
            {SqlDbType.NVarChar,DbType.String },
            {SqlDbType.Time,DbType.Time },
            {SqlDbType.VarBinary,DbType.Binary },
        };

        public static readonly TheoryData<SqlDbType, Type> SQLToTypeData = new TheoryData<SqlDbType, Type>
        {
            {SqlDbType.SmallInt,typeof(short) },
            {SqlDbType.BigInt,typeof(long) },
            {SqlDbType.Int,typeof(int) },
            {SqlDbType.Bit,typeof(bool) },
            {SqlDbType.NChar,typeof(char) },
            {SqlDbType.DateTime2,typeof(DateTime) },
            {SqlDbType.Decimal,typeof(decimal) },
            {SqlDbType.Real,typeof(float) },
            {SqlDbType.NVarChar,typeof(string) },
            {SqlDbType.Time,typeof(TimeSpan) },
            {SqlDbType.VarBinary,typeof(byte[]) },
        };

        [Theory]
        [MemberData(nameof(SQLToTypeData))]
        public void ConvertFrom(SqlDbType sqlDbType, Type type)
        {
            var Temp = new SqlDbTypeConverter();
            Assert.Equal(sqlDbType, Temp.ConvertFrom(type));
        }

        [Theory]
        [MemberData(nameof(SQLToDbTypeData))]
        public void ConvertFrom2(SqlDbType sqlDbType, DbType dbType)
        {
            var Temp = new SqlDbTypeConverter();
            Assert.Equal(sqlDbType, Temp.ConvertFrom(dbType));
        }

        [Fact]
        public void ConvertFromUri()
        {
            var Temp = new SqlDbTypeConverter();
            Assert.Equal(SqlDbType.NVarChar, Temp.ConvertFrom(typeof(Uri)));
        }

        [Theory]
        [MemberData(nameof(SQLToDbTypeData))]
        public void ConvertTo(SqlDbType sqlDbType, DbType dbType)
        {
            var Temp = new SqlDbTypeConverter();
            Assert.Equal(dbType, Temp.ConvertTo<DbType>(sqlDbType));
        }

        [Theory]
        [MemberData(nameof(SQLToTypeData))]
        public void ConvertTo2(SqlDbType sqlDbType, Type type)
        {
            var Temp = new SqlDbTypeConverter();
            Assert.Equal(type, Temp.ConvertTo<Type>(sqlDbType));
        }
    }
}