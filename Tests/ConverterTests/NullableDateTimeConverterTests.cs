using Csv;
using Csv.Converters;
using FluentAssertions;
using Missil;
using System;
using System.Globalization;
using System.Reflection.Emit;
using System.Text;
using Xunit;

namespace Tests.ConverterTests {
	public class NullableDateTimeConverterTests {
		[Fact]
		public void DateTimeSerializerIsValid() {
			NullableDateTimeConverter converter = new NullableDateTimeConverter();
			DateTime? date = new DateTime(2019, 5, 4, 3, 2, 1);
			StringBuilder stringBuilder = new StringBuilder();
			converter.AppendToStringBuilder(stringBuilder, CultureInfo.InvariantCulture, date, null);
			string serialized = stringBuilder.ToString();
			serialized.Should().Be("05/04/2019 03:02:01");
		}

		[Fact]
		public void NullIsSerializedIntoEmptyString() {
			NullableDateTimeConverter converter = new NullableDateTimeConverter();
			DateTime? date = null;
			StringBuilder stringBuilder = new StringBuilder();
			converter.AppendToStringBuilder(stringBuilder, CultureInfo.InvariantCulture, date, null);
			string serialized = stringBuilder.ToString();
			serialized.Should().Be("");
		}

		[Fact]
		public void CanSerializeUsingCustomDateFormat() {
			NullableDateTimeConverter converter = new NullableDateTimeConverter();
			DateTime? date = new DateTime(2019, 5, 4, 3, 2, 1);
			StringBuilder stringBuilder = new StringBuilder();
			CsvColumnAttribute attribute = new CsvColumnAttribute("Date") { DateFormat = "yyyy/MMM/dd H:mm:ss" };
			converter.AppendToStringBuilder(stringBuilder, CultureInfo.InvariantCulture, date, attribute);
			string serialized = stringBuilder.ToString();
			serialized.Should().Be("2019/May/04 3:02:01");
		}

		[Fact]
		public void DateTimeDeserializerIsValid() {
			NullableDateTimeConverter converter = new NullableDateTimeConverter();
			string text = "05/04/2019 03:02:01";
			DateTime? date = converter.Deserialize(text.AsMemory(), CultureInfo.InvariantCulture, null);
			date.Should().Be(new DateTime(2019, 5, 4, 3, 2, 1));
		}

		[Fact]
		public void EmptyStringIsDeserializedIntoNull() {
			NullableDateTimeConverter converter = new NullableDateTimeConverter();
			string text = "";
			DateTime? date = converter.Deserialize(text.AsMemory(), CultureInfo.InvariantCulture, null);
			date.Should().Be(null);
		}

		[Fact]
		public void CanDeserializeUsingCustomDateFormat() {
			NullableDateTimeConverter converter = new NullableDateTimeConverter();
			string text = "2019/May/04 3:02:01";
			CsvColumnAttribute attribute = new CsvColumnAttribute("Date") { DateFormat = "yyyy/MMM/dd H:mm:ss" };
			DateTime? date = converter.Deserialize(text.AsMemory(), CultureInfo.InvariantCulture, attribute);
			date.Should().Be(new DateTime(2019, 5, 4, 3, 2, 1));
		}

		[Fact]
		public void EmittedSerializerIsValid() {
			NullableDateTimeConverter converter = new NullableDateTimeConverter();
			DateTime? date = new DateTime(2019, 5, 4, 3, 2, 1);
			DynamicMethod serialize = new DynamicMethod("Serialize", typeof(string), new Type[] { typeof(DateTime?), typeof(IFormatProvider) }, typeof(DateTimeConverterTests));
			serialize.GetILGenerator()
				.DeclareLocal<DateTime?>(out LocalBuilder nullableLocal)
				.DeclareLocal<DateTime>(out LocalBuilder local)
				.Newobj<StringBuilder>()
				.Ldarg_0()
				.Emit(gen => converter.EmitAppendToStringBuilder(gen, nullableLocal, local, null))
				.Callvirt<StringBuilder>("ToString")
				.Ret();
			string serialized = (string)serialize.Invoke(null, new object?[] { date, CultureInfo.InvariantCulture })!;
			serialized.Should().Be("05/04/2019 03:02:01");
		}

		[Fact]
		public void EmitterSerializerSerializesNullIntoEmptyString() {
			NullableDateTimeConverter converter = new NullableDateTimeConverter();
			DateTime? date = null;
			DynamicMethod serialize = new DynamicMethod("Serialize", typeof(string), new Type[] { typeof(DateTime?), typeof(IFormatProvider) }, typeof(DateTimeConverterTests));
			serialize.GetILGenerator()
				.DeclareLocal<DateTime?>(out LocalBuilder nullableLocal)
				.DeclareLocal<DateTime>(out LocalBuilder local)
				.Newobj<StringBuilder>()
				.Ldarg_0()
				.Emit(gen => converter.EmitAppendToStringBuilder(gen, nullableLocal, local, null))
				.Callvirt<StringBuilder>("ToString")
				.Ret();
			string serialized = (string)serialize.Invoke(null, new object?[] { date, CultureInfo.InvariantCulture })!;
			serialized.Should().Be("");
		}

		[Fact]
		public void CanEmitSerializerWithCustomDateFormat() {
			NullableDateTimeConverter converter = new NullableDateTimeConverter();
			DateTime? date = new DateTime(2019, 5, 4, 3, 2, 1);
			CsvColumnAttribute attribute = new CsvColumnAttribute("Date") { DateFormat = "yyyy/MMM/dd H:mm:ss" };
			DynamicMethod serialize = new DynamicMethod("Serialize", typeof(string), new Type[] { typeof(DateTime?), typeof(IFormatProvider) }, typeof(DateTimeConverterTests));
			serialize.GetILGenerator()
				.DeclareLocal<DateTime?>(out LocalBuilder nullableLocal)
				.DeclareLocal<DateTime>(out LocalBuilder local)
				.Newobj<StringBuilder>()
				.Ldarg_0()
				.Emit(gen => converter.EmitAppendToStringBuilder(gen, nullableLocal, local, attribute))
				.Callvirt<StringBuilder>("ToString")
				.Ret();
			string serialized = (string)serialize.Invoke(null, new object?[] { date, CultureInfo.InvariantCulture })!;
			serialized.Should().Be("2019/May/04 3:02:01");
		}

		[Fact]
		public void EmittedDeserializerIsValid() {
			NullableDateTimeConverter converter = new NullableDateTimeConverter();
			string text = "05/04/2019 03:02:01";
			DynamicMethod deserialize = new DynamicMethod("Deserialize", typeof(DateTime?), new Type[] { typeof(ReadOnlyMemory<char>), typeof(IFormatProvider) }, typeof(DateTimeConverterTests));
			deserialize.GetILGenerator()
				.DeclareLocal<DateTime?>(out LocalBuilder nullableLocal)
				.DeclareLocal<DateTime>(out LocalBuilder local)
				.Ldarga_S(0)
				.Emit(gen => converter.EmitDeserialize(gen, nullableLocal, local, null))
				.Ret();
			DateTime? deserialized = (DateTime?)deserialize.Invoke(this, new object?[] { text.AsMemory(), CultureInfo.InvariantCulture })!;
			deserialized.Should().Be(new DateTime(2019, 5, 4, 3, 2, 1));
		}

		[Fact]
		public void EmittedDeserializerDeserializesEmptyStringIntoNull() {
			NullableDateTimeConverter converter = new NullableDateTimeConverter();
			string text = "";
			DynamicMethod deserialize = new DynamicMethod("Deserialize", typeof(DateTime?), new Type[] { typeof(ReadOnlyMemory<char>), typeof(IFormatProvider) }, typeof(DateTimeConverterTests));
			deserialize.GetILGenerator()
				.DeclareLocal<DateTime?>(out LocalBuilder nullableLocal)
				.DeclareLocal<DateTime>(out LocalBuilder local)
				.Ldarga_S(0)
				.Emit(gen => converter.EmitDeserialize(gen, nullableLocal, local, null))
				.Ret();
			DateTime? deserialized = (DateTime?)deserialize.Invoke(this, new object?[] { text.AsMemory(), CultureInfo.InvariantCulture })!;
			deserialized.Should().Be(null);
		}

		[Fact]
		public void CanEmitDeserializerWithCustomDateFormat() {
			NullableDateTimeConverter converter = new NullableDateTimeConverter();
			string text = "2019/May/04 3:02:01";
			CsvColumnAttribute attribute = new CsvColumnAttribute("Date") { DateFormat = "yyyy/MMM/dd H:mm:ss" };
			DynamicMethod deserialize = new DynamicMethod("Deserialize", typeof(DateTime?), new Type[] { typeof(ReadOnlyMemory<char>), typeof(IFormatProvider) }, typeof(DateTimeConverterTests));
			deserialize.GetILGenerator()
				.DeclareLocal<DateTime?>(out LocalBuilder nullableLocal)
				.DeclareLocal<DateTime>(out LocalBuilder local)
				.Ldarga_S(0)
				.Emit(gen => converter.EmitDeserialize(gen, nullableLocal, local, attribute))
				.Ret();
			DateTime? deserialized = (DateTime?)deserialize.Invoke(this, new object?[] { text.AsMemory(), CultureInfo.InvariantCulture })!;
			deserialized.Should().Be(new DateTime(2019, 5, 4, 3, 2, 1));
		}
	}
}
