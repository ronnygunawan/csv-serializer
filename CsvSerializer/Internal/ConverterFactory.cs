using Csv.Internal.Converters;
using System;
using System.Collections.Concurrent;

namespace Csv.Internal {
	/***************************************
	 | THIS CLASS WAS AUTO GENERATED USING |
	 | ConveterFactory.tt TEXT TEMPLATE.   |
	 | DO NOT MODIFY THIS CLASS!!!         |
	 ***************************************/
	internal static class ConverterFactory {
		private static readonly ConcurrentDictionary<Type, object> _cache = new ConcurrentDictionary<Type, object>();

		public static IConverterEmitter GetOrCreateEmitter(Type type) {
			return (IConverterEmitter)_cache.GetOrAdd(type, type => {
				if (type == typeof(string)) {
					return new StringConverter();
				} else if (type.IsEnum) {
					return Activator.CreateInstance(typeof(EnumConverter<>).MakeGenericType(type))!;
				} else if (Nullable.GetUnderlyingType(type) is Type t && t.IsEnum) {
					return Activator.CreateInstance(typeof(NullableEnumConverter<>).MakeGenericType(t))!;
				} else if (type == typeof(Boolean)) {
					return new BooleanConverter();
				} else if (Nullable.GetUnderlyingType(type) == typeof(Boolean)) {
					return new NullableBooleanConverter();
				} else if (type == typeof(Byte)) {
					return new ByteConverter();
				} else if (Nullable.GetUnderlyingType(type) == typeof(Byte)) {
					return new NullableByteConverter();
				} else if (type == typeof(SByte)) {
					return new SByteConverter();
				} else if (Nullable.GetUnderlyingType(type) == typeof(SByte)) {
					return new NullableSByteConverter();
				} else if (type == typeof(Int16)) {
					return new Int16Converter();
				} else if (Nullable.GetUnderlyingType(type) == typeof(Int16)) {
					return new NullableInt16Converter();
				} else if (type == typeof(UInt16)) {
					return new UInt16Converter();
				} else if (Nullable.GetUnderlyingType(type) == typeof(UInt16)) {
					return new NullableUInt16Converter();
				} else if (type == typeof(Int32)) {
					return new Int32Converter();
				} else if (Nullable.GetUnderlyingType(type) == typeof(Int32)) {
					return new NullableInt32Converter();
				} else if (type == typeof(UInt32)) {
					return new UInt32Converter();
				} else if (Nullable.GetUnderlyingType(type) == typeof(UInt32)) {
					return new NullableUInt32Converter();
				} else if (type == typeof(Int64)) {
					return new Int64Converter();
				} else if (Nullable.GetUnderlyingType(type) == typeof(Int64)) {
					return new NullableInt64Converter();
				} else if (type == typeof(UInt64)) {
					return new UInt64Converter();
				} else if (Nullable.GetUnderlyingType(type) == typeof(UInt64)) {
					return new NullableUInt64Converter();
				} else if (type == typeof(Single)) {
					return new SingleConverter();
				} else if (Nullable.GetUnderlyingType(type) == typeof(Single)) {
					return new NullableSingleConverter();
				} else if (type == typeof(Double)) {
					return new DoubleConverter();
				} else if (Nullable.GetUnderlyingType(type) == typeof(Double)) {
					return new NullableDoubleConverter();
				} else if (type == typeof(Decimal)) {
					return new DecimalConverter();
				} else if (Nullable.GetUnderlyingType(type) == typeof(Decimal)) {
					return new NullableDecimalConverter();
				} else if (type == typeof(DateTime)) {
					return new DateTimeConverter();
				} else if (Nullable.GetUnderlyingType(type) == typeof(DateTime)) {
					return new NullableDateTimeConverter();
				} else if (type == typeof(Uri)) {
					return new UriConverter();
				} else {
					throw new NotImplementedException();
				}
			});
		}

		public static INativeConverter<T> GetOrCreate<T>() where T : notnull => (INativeConverter<T>)GetOrCreateEmitter(typeof(T));
	}
}
