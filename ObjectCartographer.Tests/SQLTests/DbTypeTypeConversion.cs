using ObjectCartographer.SQL.Converters;
using ObjectCartographer.Tests.BaseClasses;
using System;
using System.Data;
using Xunit;

namespace ObjectCartographer.Tests.SQLTests
{
    public class DbTypeTypeConversionTests : TestBaseClass<DbTypeConverter>
    {
        public DbTypeTypeConversionTests()
        {
            TestObject = new DbTypeConverter();
        }

        public static readonly TheoryData<DbType, Type> DBToTypeData = new()
        {
            { DbType.Int16, typeof(short) },
            { DbType.Int64, typeof(long) },
            { DbType.Int32, typeof(int) },
            { DbType.Boolean, typeof(bool) },
            { DbType.String, typeof(string) },
            { DbType.DateTime2, typeof(DateTime) },
            { DbType.Decimal, typeof(decimal) },
            { DbType.Single, typeof(float) },
            { DbType.String, typeof(string) },
            { DbType.Time, typeof(TimeSpan) },
            { DbType.Binary, typeof(byte[]) }
        };

        public static readonly TheoryData<DbType, Type> DBToTypeDataSpecialCases = new()
        {
            { DbType.String, typeof(Uri) }
        };

        public static readonly TheoryData<SqlDbType, DbType> SQLToDbTypeData = new()
        {
            { SqlDbType.SmallInt, DbType.Int16 },
            { SqlDbType.BigInt, DbType.Int64 },
            { SqlDbType.Int, DbType.Int32 },
            { SqlDbType.Bit, DbType.Boolean },
            { SqlDbType.NVarChar, DbType.String },
            { SqlDbType.DateTime2, DbType.DateTime2 },
            { SqlDbType.Decimal, DbType.Decimal },
            { SqlDbType.Real, DbType.Single },
            { SqlDbType.NVarChar, DbType.String },
            { SqlDbType.Time, DbType.Time },
            { SqlDbType.VarBinary, DbType.Binary },
        };

        [Theory]
        [MemberData(nameof(DBToTypeData))]
        public void ConvertFrom(DbType sqlDbType, Type type)
        {
            var Temp = new DbTypeConverter();
            Assert.Equal(sqlDbType, Temp.ConvertFrom(type));
        }

        [Theory]
        [MemberData(nameof(SQLToDbTypeData))]
        public void ConvertFrom2(SqlDbType sqlDbType, DbType dbType)
        {
            var Temp = new DbTypeConverter();
            Assert.Equal(dbType, Temp.ConvertFrom(sqlDbType));
        }

        [Theory]
        [MemberData(nameof(DBToTypeDataSpecialCases))]
        public void ConvertFromSpecialCases(DbType sqlDbType, Type type)
        {
            var Temp = new DbTypeConverter();
            Assert.Equal(sqlDbType, Temp.ConvertFrom(type));
        }

        [Theory]
        [MemberData(nameof(SQLToDbTypeData))]
        public void ConvertTo(SqlDbType sqlDbType, DbType dbType)
        {
            var Temp = new DbTypeConverter();
            Assert.Equal(sqlDbType, Temp.ConvertTo<SqlDbType>(dbType));
        }

        [Theory]
        [MemberData(nameof(DBToTypeData))]
        public void ConvertTo2(DbType sqlDbType, Type type)
        {
            var Temp = new DbTypeConverter();
            Assert.Equal(type, Temp.ConvertTo<Type>(sqlDbType));
        }
    }
}