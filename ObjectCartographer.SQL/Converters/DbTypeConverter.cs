using ObjectCartographer.ExpressionBuilder;
using ObjectCartographer.ExpressionBuilder.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq.Expressions;

namespace ObjectCartographer.SQL.Converters
{
    /// <summary>
    /// DbType converter
    /// </summary>
    /// <seealso cref="IConverter"/>
    public class DbTypeConverter : IConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DbTypeConverter"/> class.
        /// </summary>
        public DbTypeConverter()
        {
            ConvertToTypes = new Dictionary<Type, Func<DbType, object>>
            {
                { typeof(Type), DbTypeToType },
                { typeof(SqlDbType), DbTypeToSqlDbType }
            };
        }

        /// <summary>
        /// Gets the convert to types.
        /// </summary>
        /// <value>The convert to types.</value>
        public Dictionary<Type, Func<DbType, object>> ConvertToTypes { get; }

        /// <summary>
        /// Gets the order.
        /// </summary>
        /// <value>The order.</value>
        public int Order => int.MinValue;

        /// <summary>
        /// Conversions
        /// </summary>
        protected static Dictionary<Type, DbType> Conversions { get; } = new Dictionary<Type, DbType>
            {
                { typeof(byte), DbType.Byte },
                { typeof(byte?), DbType.Byte },
                { typeof(sbyte), DbType.Byte },
                { typeof(sbyte?), DbType.Byte },
                { typeof(short), DbType.Int16 },
                { typeof(short?), DbType.Int16 },
                { typeof(ushort), DbType.Int16 },
                { typeof(ushort?), DbType.Int16 },
                { typeof(int), DbType.Int32 },
                { typeof(int?), DbType.Int32 },
                { typeof(uint), DbType.Int32 },
                { typeof(uint?), DbType.Int32 },
                { typeof(long), DbType.Int64 },
                { typeof(long?), DbType.Int64 },
                { typeof(ulong), DbType.Int64 },
                { typeof(ulong?), DbType.Int64 },
                { typeof(float), DbType.Single },
                { typeof(float?), DbType.Single },
                { typeof(double), DbType.Double },
                { typeof(double?), DbType.Double },
                { typeof(decimal), DbType.Decimal },
                { typeof(decimal?), DbType.Decimal },
                { typeof(bool), DbType.Boolean },
                { typeof(bool?), DbType.Boolean },
                { typeof(string), DbType.String },
                { typeof(char), DbType.StringFixedLength },
                { typeof(char?), DbType.StringFixedLength },
                { typeof(Guid), DbType.Guid },
                { typeof(Guid?), DbType.Guid },
                { typeof(DateTime), DbType.DateTime2 },
                { typeof(DateTime?), DbType.DateTime2 },
                { typeof(DateTimeOffset), DbType.DateTimeOffset },
                { typeof(DateTimeOffset?), DbType.DateTimeOffset },
                { typeof(TimeSpan), DbType.Time },
                { typeof(Uri), DbType.String },
                { typeof(TimeSpan?), DbType.Time },
                { typeof(byte[]), DbType.Binary }
            };

        /// <summary>
        /// Gets the database type to type conversions.
        /// </summary>
        /// <value>The database type to type conversions.</value>
        protected static Dictionary<DbType, Type> DbTypeToTypeConversions { get; } = new Dictionary<DbType, Type>
            {
                { DbType.Byte , typeof(byte)},
                {DbType.SByte , typeof(sbyte)},
                {DbType.Int16 , typeof(short)},
                {DbType.UInt16 , typeof(ushort)},
                {DbType.Int32 , typeof(int)},
                {DbType.UInt32 , typeof(uint)},
                {DbType.Int64 , typeof(long)},
                {DbType.UInt64 , typeof(ulong)},
                {DbType.Single , typeof(float)},
                {DbType.Double , typeof(double)},
                {DbType.Decimal , typeof(decimal)},
                {DbType.Boolean , typeof(bool)},
                {DbType.String , typeof(string)},
                {DbType.StringFixedLength , typeof(char)},
                {DbType.Guid , typeof(Guid)},
                {DbType.DateTime2 , typeof(DateTime)},
                {DbType.DateTime , typeof(DateTime)},
                {DbType.DateTimeOffset , typeof(DateTimeOffset)},
                {DbType.Binary , typeof(byte[])},
                {DbType.Time , typeof(TimeSpan)},
            };

        /// <summary>
        /// Determines whether this instance can handle the specified types.
        /// </summary>
        /// <param name="sourceType">Type of the source.</param>
        /// <param name="destinationType">Type of the destination.</param>
        /// <returns>
        /// <c>true</c> if this instance can handle the specified types; otherwise, <c>false</c>.
        /// </returns>
        public bool CanHandle(Type sourceType, Type destinationType)
        {
            return (destinationType == typeof(DbType) && (sourceType == typeof(SqlDbType) || sourceType == typeof(Type) || sourceType == typeof(Type).GetType()))
                || (sourceType == typeof(DbType) && (destinationType == typeof(SqlDbType) || destinationType == typeof(Type) || destinationType == typeof(Type).GetType()));
        }

        /// <summary>
        /// Convert from an object to a DbType
        /// </summary>
        /// <param name="value">Value</param>
        /// <returns>The DbType version</returns>
        public DbType ConvertFrom<TSource>(TSource value)
        {
            if (value is null)
                return DbType.Int32;

            if (value is Type TypeObject)
                return TypeToDbType(TypeObject);
            if (value is SqlDbType SqlDBType)
                return SqlDbTypeToDbType(SqlDBType);

            return DbType.Int32;
        }

        /// <summary>
        /// Converts the DbType object to another type
        /// </summary>
        /// <typeparam name="TDestination">The type of the destination.</typeparam>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public TDestination ConvertTo<TDestination>(DbType value)
        {
            var DestinationType = typeof(TDestination);
            if (ConvertToTypes.ContainsKey(DestinationType))
            {
                return (TDestination)ConvertToTypes[DestinationType](value);
            }

            return default!;
        }

        /// <summary>
        /// Maps the specified source to the destination.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="destination">The destination.</param>
        /// <param name="sourceType">Type of the source.</param>
        /// <param name="destinationType">Type of the destination.</param>
        /// <param name="mapping">The mapping.</param>
        /// <param name="manager">The manager.</param>
        /// <returns>The resulting expression.</returns>
        public Expression Map(Expression source, Expression? destination, Type sourceType, Type destinationType, IExpressionMapping mapping, ExpressionBuilderManager manager)
        {
            if (sourceType == typeof(DbType))
                return Expression.Call(Expression.Constant(this), typeof(DbTypeConverter).GetMethod(nameof(DbTypeConverter.ConvertTo)).MakeGenericMethod(destinationType), source);
            if (destinationType == typeof(DbType))
                return Expression.Call(Expression.Constant(this), typeof(DbTypeConverter).GetMethod(nameof(DbTypeConverter.ConvertFrom)).MakeGenericMethod(sourceType), source);
            return Expression.Empty();
        }

        /// <summary>
        /// Databases the type of the type to SQL database.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private static object DbTypeToSqlDbType(DbType value)
        {
            if (value == DbType.Time)
                return SqlDbType.Time;
            if (value == DbType.UInt64)
                return SqlDbType.BigInt;
            var Parameter = new SqlParameter
            {
                DbType = value
            };
            return Parameter.SqlDbType;
        }

        /// <summary>
        /// Databases the type of the type to.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        private static object DbTypeToType(DbType value)
        {
            return DbTypeToTypeConversions.TryGetValue(value, out var returnValue) ? returnValue : typeof(int);
        }

        /// <summary>
        /// SQLs the type of the database type to database.
        /// </summary>
        /// <param name="Temp">The temporary.</param>
        /// <returns></returns>
        private static DbType SqlDbTypeToDbType(SqlDbType Temp)
        {
            if (Temp == SqlDbType.Time)
                return DbType.Time;

            var Parameter = new SqlParameter
            {
                SqlDbType = Temp
            };
            return Parameter.DbType;
        }

        /// <summary>
        /// Types the type of to database.
        /// </summary>
        /// <param name="TempValue">The temporary value.</param>
        /// <returns></returns>
        private static DbType TypeToDbType(Type TempValue)
        {
            if (TempValue.IsEnum)
            {
                TempValue = Enum.GetUnderlyingType(TempValue);
            }
            return Conversions.TryGetValue(TempValue, out var returnValue) ? returnValue : DbType.Int32;
        }
    }
}