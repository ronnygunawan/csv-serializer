using Csv.Internal.Converters;
using FluentAssertions;
using System;
using System.Globalization;
using System.Reflection.Emit;
using System.Text;
using Xunit;
using Missil;

namespace Tests.ConverterTests {
	public class StringConverterTests {
		[Fact]
		public void StringSerializerIsValid() {
			StringConverter converter = new StringConverter();
			string s = "Hello \" world";
			StringBuilder stringBuilder = new StringBuilder();
			converter.AppendToStringBuilder(stringBuilder, CultureInfo.InvariantCulture, s, null, ',');
			string serialized = stringBuilder.ToString();
			serialized.Should().Be("\"Hello \"\" world\"");
		}

		[Fact]
		public void NullStringIsSerializedIntoEmptyString() {
			StringConverter converter = new StringConverter();
			string? s = null;
			StringBuilder stringBuilder = new StringBuilder();
			converter.AppendToStringBuilder(stringBuilder, CultureInfo.InvariantCulture, s, null, ',');
			string serialized = stringBuilder.ToString();
			serialized.Should().Be("");
		}

		[Fact]
		public void EmptyStringIsSerializedIntoEmptyQuotedString() {
			StringConverter converter = new StringConverter();
			string s = string.Empty;
			StringBuilder stringBuilder = new StringBuilder();
			converter.AppendToStringBuilder(stringBuilder, CultureInfo.InvariantCulture, s, null, ',');
			string serialized = stringBuilder.ToString();
			serialized.Should().Be("\"\"");
		}

		[Fact]
		public void StringDeserializerIsValid() {
			StringConverter converter = new StringConverter();
			string text = "Hello \" world";
			string? deserialized = converter.Deserialize(text.AsMemory(), CultureInfo.InvariantCulture, null);
			deserialized.Should().Be("Hello \" world");
		}

		[Fact]
		public void EmptyStringIsDeserializedIntoEmptyString() {
			StringConverter converter = new StringConverter();
			string text = "";
			string? deserialized = converter.Deserialize(text.AsMemory(), CultureInfo.InvariantCulture, null);
			deserialized.Should().Be("");
		}

		[Fact]
		public void EmittedSerializerIsValid() {
			StringConverter converter = new StringConverter();
			string s = "Hello \" world";
			DynamicMethod serialize = new DynamicMethod("Serialize", typeof(string), new Type[] { typeof(string), typeof(IFormatProvider), typeof(char) }, typeof(StringConverterTests));
			serialize.GetILGenerator()
				.DeclareLocal<string>(out LocalBuilder local)
				.Newobj<StringBuilder>()
				.Ldarg_0()
				.Emit(gen => converter.EmitAppendToStringBuilder(gen, local, null, null))
				.Callvirt<StringBuilder>("ToString")
				.Ret();
			string? serialized = (string?)serialize.Invoke(null, new object?[] { s, CultureInfo.InvariantCulture, ',' })!;
			serialized.Should().Be("\"Hello \"\" world\"");
		}

		[Fact]
		public void EmittedSerializerSerializesNullIntoEmptyString() {
			StringConverter converter = new StringConverter();
			string? s = null;
			DynamicMethod serialize = new DynamicMethod("Serialize", typeof(string), new Type[] { typeof(string), typeof(IFormatProvider), typeof(char) }, typeof(StringConverterTests));
			serialize.GetILGenerator()
				.DeclareLocal<string>(out LocalBuilder local)
				.Newobj<StringBuilder>()
				.Ldarg_0()
				.Emit(gen => converter.EmitAppendToStringBuilder(gen, local, null, null))
				.Callvirt<StringBuilder>("ToString")
				.Ret();
			string? serialized = (string?)serialize.Invoke(null, new object?[] { s, CultureInfo.InvariantCulture, ',' })!;
			serialized.Should().Be("");
		}

		[Fact]
		public void EmittedSerializerSerializesEmptyStringIntoEmptyQuotedString() {
			StringConverter converter = new StringConverter();
			string s = string.Empty;
			DynamicMethod serialize = new DynamicMethod("Serialize", typeof(string), new Type[] { typeof(string), typeof(IFormatProvider), typeof(char) }, typeof(StringConverterTests));
			serialize.GetILGenerator()
				.DeclareLocal<string>(out LocalBuilder local)
				.Newobj<StringBuilder>()
				.Ldarg_0()
				.Emit(gen => converter.EmitAppendToStringBuilder(gen, local, null, null))
				.Callvirt<StringBuilder>("ToString")
				.Ret();
			string? serialized = (string?)serialize.Invoke(null, new object?[] { s, CultureInfo.InvariantCulture, ',' })!;
			serialized.Should().Be("\"\"");
		}

		[Fact]
		public void EmittedDeserializerIsValid() {
			StringConverter converter = new StringConverter();
			string text = "Hello \" world";
			DynamicMethod deserialize = new DynamicMethod("Deserialize", typeof(string), new Type[] { typeof(ReadOnlyMemory<char>), typeof(IFormatProvider), typeof(char) }, typeof(StringConverterTests));
			deserialize.GetILGenerator()
				.Ldarga_S(0)
				.Emit(gen => converter.EmitDeserialize(gen, null, null, null))
				.Ret();
			string? deserialized = (string?)deserialize.Invoke(this, new object?[] { text.AsMemory(), CultureInfo.InvariantCulture, ',' })!;
			deserialized.Should().Be("Hello \" world");
		}

		[Fact]
		public void EmittedDeserializerDeserializesEmptyStringIntoEmptyString() {
			StringConverter converter = new StringConverter();
			string text = "";
			DynamicMethod deserialize = new DynamicMethod("Deserialize", typeof(string), new Type[] { typeof(ReadOnlyMemory<char>), typeof(IFormatProvider), typeof(char) }, typeof(StringConverterTests));
			deserialize.GetILGenerator()
				.Ldarga_S(0)
				.Emit(gen => converter.EmitDeserialize(gen, null, null, null))
				.Ret();
			string? deserialized = (string?)deserialize.Invoke(this, new object?[] { text.AsMemory(), CultureInfo.InvariantCulture, ',' })!;
			deserialized.Should().Be("");
		}
	}
}
