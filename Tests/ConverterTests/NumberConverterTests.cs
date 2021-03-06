﻿using Csv;
using Csv.Internal;
using Csv.Internal.Converters;
using FluentAssertions;
using Missil;
using System;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Xunit;

namespace Tests.ConverterTests {
	public class NumberConverterTests {
		[Theory]
		[InlineData(typeof(ByteConverter), typeof(byte), (byte)0x88, "136")]
		[InlineData(typeof(ByteConverter), typeof(byte), (byte)0xff, "255")]
		[InlineData(typeof(NullableByteConverter), typeof(byte?), (byte)0x88, "136")]
		[InlineData(typeof(NullableByteConverter), typeof(byte?), (byte)0xff, "255")]
		[InlineData(typeof(NullableByteConverter), typeof(byte?), null, "")]
		[InlineData(typeof(SByteConverter), typeof(sbyte), (sbyte)-128, "-128")]
		[InlineData(typeof(SByteConverter), typeof(sbyte), (sbyte)127, "127")]
		[InlineData(typeof(Int16Converter), typeof(short), (short)32000, "32000")]
		[InlineData(typeof(Int16Converter), typeof(short), (short)-32000, "-32000")]
		[InlineData(typeof(Int64Converter), typeof(long), 9223372036854775807L, "9223372036854775807")]
		[InlineData(typeof(Int64Converter), typeof(long), -9223372036854775808L, "-9223372036854775808")]
		[InlineData(typeof(NullableInt64Converter), typeof(long?), 9223372036854775807L, "9223372036854775807")]
		[InlineData(typeof(NullableInt64Converter), typeof(long?), -9223372036854775808L, "-9223372036854775808")]
		[InlineData(typeof(NullableInt64Converter), typeof(long?), null, "")]
		[InlineData(typeof(SingleConverter), typeof(float), 123.45f, "123.45")]
		[InlineData(typeof(SingleConverter), typeof(float), -123.45f, "-123.45")]
		[InlineData(typeof(DoubleConverter), typeof(double), 123.45, "123.45")]
		[InlineData(typeof(DoubleConverter), typeof(double), -123.45, "-123.45")]
		[InlineData(typeof(NullableDoubleConverter), typeof(double?), 123.45, "123.45")]
		[InlineData(typeof(NullableDoubleConverter), typeof(double?), -123.45, "-123.45")]
		[InlineData(typeof(NullableDoubleConverter), typeof(double?), null, "")]
		[InlineData(typeof(DecimalConverter), typeof(decimal), 123.45, "123.45")]
		[InlineData(typeof(DecimalConverter), typeof(decimal), -123.45, "-123.45")]
		[InlineData(typeof(NullableDecimalConverter), typeof(decimal?), 123.45, "123.45")]
		[InlineData(typeof(NullableDecimalConverter), typeof(decimal?), -123.45, "-123.45")]
		[InlineData(typeof(NullableDecimalConverter), typeof(decimal?), null, "")]
		[InlineData(typeof(BooleanConverter), typeof(bool), true, "True")]
		[InlineData(typeof(BooleanConverter), typeof(bool), false, "False")]
		[InlineData(typeof(NullableBooleanConverter), typeof(bool?), true, "True")]
		[InlineData(typeof(NullableBooleanConverter), typeof(bool?), false, "False")]
		[InlineData(typeof(NullableBooleanConverter), typeof(bool?), null, "")]
		public void AppendToStringBuilderIsValid(Type converterType, Type valueType, object testValue, string expectedResult) {
			if (valueType == typeof(decimal)) {
				testValue = (decimal)(double)testValue;
			} else if (valueType == typeof(decimal?)) {
				testValue = (decimal?)(double?)testValue;
			}
			object converterObj = Activator.CreateInstance(converterType)!;
			MethodInfo serialize = converterType.GetMethod("AppendToStringBuilder", new Type[] { typeof(StringBuilder), typeof(IFormatProvider), valueType, typeof(CsvColumnAttribute), typeof(char) })!;
			StringBuilder stringBuilder = new StringBuilder();
			serialize.Invoke(converterObj, new[] { stringBuilder, CultureInfo.InvariantCulture, testValue, null, ',' });
			string serialized = stringBuilder.ToString();
			serialized.Should().Be(expectedResult);
		}

		[Theory]
		[InlineData(typeof(ByteConverter), "136", (byte)0x88)]
		[InlineData(typeof(ByteConverter), "255", (byte)0xff)]
		[InlineData(typeof(NullableByteConverter), "136", (byte)0x88)]
		[InlineData(typeof(NullableByteConverter), "255", (byte)0xff)]
		[InlineData(typeof(NullableByteConverter), "", null)]
		[InlineData(typeof(SByteConverter), "-128", (sbyte)-128)]
		[InlineData(typeof(SByteConverter), "127", (sbyte)127)]
		[InlineData(typeof(Int16Converter), "32000", (short)32000)]
		[InlineData(typeof(Int16Converter), "-32000", (short)-32000)]
		[InlineData(typeof(Int64Converter), "9223372036854775807", 9223372036854775807L)]
		[InlineData(typeof(Int64Converter), "-9223372036854775808", -9223372036854775808L)]
		[InlineData(typeof(NullableInt64Converter), "9223372036854775807", 9223372036854775807L)]
		[InlineData(typeof(NullableInt64Converter), "-9223372036854775808", -9223372036854775808L)]
		[InlineData(typeof(NullableInt64Converter), "", null)]
		[InlineData(typeof(SingleConverter), "123.45", 123.45f)]
		[InlineData(typeof(SingleConverter), "-123.45", -123.45f)]
		[InlineData(typeof(DoubleConverter), "123.45", 123.45)]
		[InlineData(typeof(DoubleConverter), "-123.45", -123.45)]
		[InlineData(typeof(NullableDoubleConverter), "123.45", 123.45)]
		[InlineData(typeof(NullableDoubleConverter), "-123.45", -123.45)]
		[InlineData(typeof(NullableDoubleConverter), "", null)]
		[InlineData(typeof(DecimalConverter), "123.45", 123.45)]
		[InlineData(typeof(DecimalConverter), "-123.45", -123.45)]
		[InlineData(typeof(NullableDecimalConverter), "123.45", 123.45)]
		[InlineData(typeof(NullableDecimalConverter), "-123.45", -123.45)]
		[InlineData(typeof(NullableDecimalConverter), "", null)]
		[InlineData(typeof(NullableBooleanConverter), "True", true)]
		[InlineData(typeof(NullableBooleanConverter), "False", false)]
		[InlineData(typeof(NullableBooleanConverter), "", null)]
		public void DeserializeIsValid(Type converterType, string testLiteral, object expectedResult) {
			if (converterType == typeof(DecimalConverter)) {
				expectedResult = (decimal)(double)expectedResult;
			} else if (converterType == typeof(NullableDecimalConverter)) {
				expectedResult = (decimal?)(double?)expectedResult;
			}
			object converterObj = Activator.CreateInstance(converterType)!;
			MethodInfo deserialize = converterType.GetMethod("Deserialize", new Type[] { typeof(ReadOnlyMemory<char>), typeof(IFormatProvider), typeof(CsvColumnAttribute) })!;
			object deserialized = deserialize.Invoke(converterObj, new object?[] { testLiteral.AsMemory(), CultureInfo.InvariantCulture, null })!;
			deserialized.Should().Be(expectedResult);
		}

		[Theory]
		[InlineData(typeof(ByteConverter), typeof(byte), (byte)0x88, "136")]
		[InlineData(typeof(ByteConverter), typeof(byte), (byte)0xff, "255")]
		[InlineData(typeof(NullableByteConverter), typeof(byte?), (byte)0x88, "136")]
		[InlineData(typeof(NullableByteConverter), typeof(byte?), (byte)0xff, "255")]
		[InlineData(typeof(NullableByteConverter), typeof(byte?), null, "")]
		[InlineData(typeof(SByteConverter), typeof(sbyte), (sbyte)-128, "-128")]
		[InlineData(typeof(SByteConverter), typeof(sbyte), (sbyte)127, "127")]
		[InlineData(typeof(Int16Converter), typeof(short), (short)32000, "32000")]
		[InlineData(typeof(Int16Converter), typeof(short), (short)-32000, "-32000")]
		[InlineData(typeof(Int64Converter), typeof(long), 9223372036854775807L, "9223372036854775807")]
		[InlineData(typeof(Int64Converter), typeof(long), -9223372036854775808L, "-9223372036854775808")]
		[InlineData(typeof(NullableInt64Converter), typeof(long?), 9223372036854775807L, "9223372036854775807")]
		[InlineData(typeof(NullableInt64Converter), typeof(long?), -9223372036854775808L, "-9223372036854775808")]
		[InlineData(typeof(NullableInt64Converter), typeof(long?), null, "")]
		[InlineData(typeof(SingleConverter), typeof(float), 123.45f, "123.45")]
		[InlineData(typeof(SingleConverter), typeof(float), -123.45f, "-123.45")]
		[InlineData(typeof(DoubleConverter), typeof(double), 123.45, "123.45")]
		[InlineData(typeof(DoubleConverter), typeof(double), -123.45, "-123.45")]
		[InlineData(typeof(NullableDoubleConverter), typeof(double?), 123.45, "123.45")]
		[InlineData(typeof(NullableDoubleConverter), typeof(double?), -123.45, "-123.45")]
		[InlineData(typeof(NullableDoubleConverter), typeof(double?), null, "")]
		[InlineData(typeof(DecimalConverter), typeof(decimal), 123.45, "123.45")]
		[InlineData(typeof(DecimalConverter), typeof(decimal), -123.45, "-123.45")]
		[InlineData(typeof(NullableDecimalConverter), typeof(decimal?), 123.45, "123.45")]
		[InlineData(typeof(NullableDecimalConverter), typeof(decimal?), -123.45, "-123.45")]
		[InlineData(typeof(NullableDecimalConverter), typeof(decimal?), null, "")]
		[InlineData(typeof(BooleanConverter), typeof(bool), true, "True")]
		[InlineData(typeof(BooleanConverter), typeof(bool), false, "False")]
		[InlineData(typeof(NullableBooleanConverter), typeof(bool?), true, "True")]
		[InlineData(typeof(NullableBooleanConverter), typeof(bool?), false, "False")]
		[InlineData(typeof(NullableBooleanConverter), typeof(bool?), null, "")]
		public void EmittedAppendToStringBuilderIsValid(Type emitterType, Type valueType, object testValue, string expectedResult) {
			if (valueType == typeof(decimal)) {
				testValue = (decimal)(double)testValue;
			} else if (valueType == typeof(decimal?)) {
				testValue = (decimal?)(double?)testValue;
			}
			IConverterEmitter emitter = (IConverterEmitter)Activator.CreateInstance(emitterType)!;
			DynamicMethod serialize = new DynamicMethod("Serialize", typeof(string), new Type[] { valueType, typeof(IFormatProvider), typeof(char) }, typeof(NumberConverterTests));
			LocalBuilder? local = null;
			LocalBuilder? secondaryLocal = null;
			serialize.GetILGenerator()
				.Emit(gen => {
					if (valueType == typeof(float) || valueType == typeof(double) || valueType == typeof(decimal)) {
						gen.DeclareLocal(valueType, out local);
					} else if (Nullable.GetUnderlyingType(valueType) is Type underlyingType) {
						if (underlyingType == typeof(float) || underlyingType == typeof(double) || underlyingType == typeof(decimal)) {
							gen.DeclareLocal(typeof(Nullable<>).MakeGenericType(underlyingType), out local);
							gen.DeclareLocal(valueType, out secondaryLocal);
						} else {
							gen.DeclareLocal(typeof(Nullable<>).MakeGenericType(underlyingType), out local);
						}
					}
				})
				.Newobj<StringBuilder>()
				.Ldarg_0()
				.Emit(gen => emitter.EmitAppendToStringBuilder(gen, local, secondaryLocal, null))
				.Callvirt<StringBuilder>("ToString")
				.Ret();
			string serialized = (string)serialize.Invoke(null, new[] { testValue, CultureInfo.InvariantCulture, ',' })!;
			serialized.Should().Be(expectedResult);
		}

		[Theory]
		[InlineData(typeof(ByteConverter), typeof(byte), "136", (byte)0x88)]
		[InlineData(typeof(ByteConverter), typeof(byte), "255", (byte)0xff)]
		[InlineData(typeof(NullableByteConverter), typeof(byte?), "136", (byte)0x88)]
		[InlineData(typeof(NullableByteConverter), typeof(byte?), "255", (byte)0xff)]
		[InlineData(typeof(NullableByteConverter), typeof(byte?), "", null)]
		[InlineData(typeof(SByteConverter), typeof(sbyte), "-128", (sbyte)-128)]
		[InlineData(typeof(SByteConverter), typeof(sbyte), "127", (sbyte)127)]
		[InlineData(typeof(Int16Converter), typeof(short), "32000", (short)32000)]
		[InlineData(typeof(Int16Converter), typeof(short), "-32000", (short)-32000)]
		[InlineData(typeof(Int64Converter), typeof(long), "9223372036854775807", 9223372036854775807L)]
		[InlineData(typeof(Int64Converter), typeof(long), "-9223372036854775808", -9223372036854775808L)]
		[InlineData(typeof(NullableInt64Converter), typeof(long?), "9223372036854775807", 9223372036854775807L)]
		[InlineData(typeof(NullableInt64Converter), typeof(long?), "-9223372036854775808", -9223372036854775808L)]
		[InlineData(typeof(NullableInt64Converter), typeof(long?), "", null)]
		[InlineData(typeof(SingleConverter), typeof(float), "123.45", 123.45f)]
		[InlineData(typeof(SingleConverter), typeof(float), "-123.45", -123.45f)]
		[InlineData(typeof(DoubleConverter), typeof(double), "123.45", 123.45)]
		[InlineData(typeof(DoubleConverter), typeof(double), "-123.45", -123.45)]
		[InlineData(typeof(NullableDoubleConverter), typeof(double?), "123.45", 123.45)]
		[InlineData(typeof(NullableDoubleConverter), typeof(double?), "-123.45", -123.45)]
		[InlineData(typeof(NullableDoubleConverter), typeof(double?), "", null)]
		[InlineData(typeof(DecimalConverter), typeof(decimal), "123.45", 123.45)]
		[InlineData(typeof(DecimalConverter), typeof(decimal), "-123.45", -123.45)]
		[InlineData(typeof(NullableDecimalConverter), typeof(decimal?), "123.45", 123.45)]
		[InlineData(typeof(NullableDecimalConverter), typeof(decimal?), "-123.45", -123.45)]
		[InlineData(typeof(NullableDecimalConverter), typeof(decimal?), "", null)]
		[InlineData(typeof(BooleanConverter), typeof(bool), "True", true)]
		[InlineData(typeof(BooleanConverter), typeof(bool), "False", false)]
		[InlineData(typeof(NullableBooleanConverter), typeof(bool?), "True", true)]
		[InlineData(typeof(NullableBooleanConverter), typeof(bool?), "False", false)]
		[InlineData(typeof(NullableBooleanConverter), typeof(bool?), "", null)]
		public void EmittedDeserializeIsValid(Type emitterType, Type valueType, string testLiteral, object expectedResult) {
			if (valueType == typeof(decimal)) {
				expectedResult = (decimal)(double)expectedResult;
			} else if (valueType == typeof(decimal?)) {
				expectedResult = (decimal?)(double?)expectedResult;
			}
			IConverterEmitter emitter = (IConverterEmitter)Activator.CreateInstance(emitterType)!;
			DynamicMethod deserialize = new DynamicMethod("Deserialize", valueType, new Type[] { typeof(ReadOnlyMemory<char>), typeof(IFormatProvider) }, typeof(NumberConverterTests));
			LocalBuilder? local = null;
			deserialize.GetILGenerator()
				.Emit(gen => Nullable.GetUnderlyingType(valueType) switch {
					{ } => gen.DeclareLocal(valueType, out local),
					_ => gen
				})
				.Ldarga_S(0)
				.Emit(gen => emitter.EmitDeserialize(gen, local, null, null))
				.Ret();
			object deserialized = deserialize.Invoke(this, new object?[] { testLiteral.AsMemory(), CultureInfo.InvariantCulture })!;
			deserialized.Should().Be(expectedResult);
		}
	}
}
