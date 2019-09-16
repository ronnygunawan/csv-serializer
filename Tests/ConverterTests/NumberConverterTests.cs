using Csv;
using Csv.Converters;
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
		public void AppendToStringBuilderIsValid(Type converterType, Type valueType, object testValue, string expectedResult) {
			if (valueType == typeof(decimal)) {
				testValue = (decimal)(double)testValue;
			} else if (valueType == typeof(decimal?)) {
				testValue = (decimal?)(double?)testValue;
			}
			object converterObj = Activator.CreateInstance(converterType)!;
			MethodInfo serialize = converterType.GetMethod("AppendToStringBuilder", new Type[] { typeof(StringBuilder), typeof(IFormatProvider), valueType, typeof(CsvColumnAttribute) })!;
			StringBuilder stringBuilder = new StringBuilder();
			serialize.Invoke(converterObj, new[] { stringBuilder, CultureInfo.InvariantCulture, testValue, null });
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
		public void EmittedAppendToStringBuilderIsValid(Type emitterType, Type valueType, object testValue, string expectedResult) {
			if (valueType == typeof(decimal)) {
				testValue = (decimal)(double)testValue;
			} else if (valueType == typeof(decimal?)) {
				testValue = (decimal?)(double?)testValue;
			}
			IConverterEmitter emitter = (IConverterEmitter)Activator.CreateInstance(emitterType)!;
			DynamicMethod serialize = new DynamicMethod("Serialize", typeof(string), new Type[] { valueType, typeof(IFormatProvider) }, typeof(NumberConverterTests));
			LocalBuilder? local = null;
			serialize.GetILGenerator()
				.Emit(gen => Nullable.GetUnderlyingType(valueType) switch {
					{ } => gen.DeclareLocal(valueType, out local),
					_ => gen
				})
				.Newobj<StringBuilder>()
				.Ldarg_0()
				.Emit(gen => emitter.EmitAppendToStringBuilder(gen, local, null, null))
				.Callvirt<StringBuilder>("ToString")
				.Ret();
			string serialized = (string)serialize.Invoke(null, new[] { testValue, CultureInfo.InvariantCulture })!;
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
