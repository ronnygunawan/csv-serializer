using Csv.Converters;
using FluentAssertions;
using Missil;
using System;
using System.Globalization;
using System.Net;
using System.Reflection.Emit;
using System.Text;
using Xunit;

namespace Tests.ConverterTests {
	public class NullableEnumConverterTests {
		[Fact]
		public void NullableEnumSerializerIsValid() {
			NullableEnumConverter<HttpStatusCode> converter = new NullableEnumConverter<HttpStatusCode>();
			HttpStatusCode? code = HttpStatusCode.NotFound;
			StringBuilder stringBuilder = new StringBuilder();
			converter.AppendToStringBuilder(stringBuilder, CultureInfo.InvariantCulture, code, null, ',');
			string serialized = stringBuilder.ToString();
			serialized.Should().Be("NotFound");
		}

		[Fact]
		public void NullValueIsSerializedIntoEmptyString() {
			NullableEnumConverter<HttpStatusCode> converter = new NullableEnumConverter<HttpStatusCode>();
			HttpStatusCode? code = null;
			StringBuilder stringBuilder = new StringBuilder();
			converter.AppendToStringBuilder(stringBuilder, CultureInfo.InvariantCulture, code, null, ',');
			string serialized = stringBuilder.ToString();
			serialized.Should().Be("");
		}

		[Fact]
		public void NullableEnumDeserializerIsValid() {
			NullableEnumConverter<HttpStatusCode> converter = new NullableEnumConverter<HttpStatusCode>();
			string text = "NotFound";
			HttpStatusCode? code = converter.Deserialize(text.AsMemory(), CultureInfo.InvariantCulture, null);
			code.Should().Be(HttpStatusCode.NotFound);
		}

		[Fact]
		public void EmptyStringIsDeserializedIntoNullValue() {
			NullableEnumConverter<HttpStatusCode> converter = new NullableEnumConverter<HttpStatusCode>();
			string text = "";
			HttpStatusCode? code = converter.Deserialize(text.AsMemory(), CultureInfo.InvariantCulture, null);
			code.Should().BeNull();
		}

		[Fact]
		public void EmittedSerializerIsValid() {
			NullableEnumConverter<HttpStatusCode> converter = new NullableEnumConverter<HttpStatusCode>();
			HttpStatusCode? code = HttpStatusCode.NotFound;
			DynamicMethod serialize = new DynamicMethod("Serialize", typeof(string), new Type[] { typeof(HttpStatusCode?), typeof(IFormatProvider), typeof(char) }, typeof(NullableEnumConverterTests));
			serialize.GetILGenerator()
				.DeclareLocal<HttpStatusCode?>(out LocalBuilder nullableLocal)
				.Newobj<StringBuilder>()
				.Ldarg_0()
				.Emit(gen => converter.EmitAppendToStringBuilder(gen, nullableLocal, null, null))
				.Callvirt<StringBuilder>("ToString")
				.Ret();
			string serialized = (string)serialize.Invoke(null, new object?[] { code, CultureInfo.InvariantCulture, ',' })!;
			serialized.Should().Be("NotFound");
		}

		[Fact]
		public void EmittedSerializerSerializesNullValueIntoEmptyString() {
			NullableEnumConverter<HttpStatusCode> converter = new NullableEnumConverter<HttpStatusCode>();
			HttpStatusCode? code = null;
			DynamicMethod serialize = new DynamicMethod("Serialize", typeof(string), new Type[] { typeof(HttpStatusCode?), typeof(IFormatProvider), typeof(char) }, typeof(NullableEnumConverterTests));
			serialize.GetILGenerator()
				.DeclareLocal<HttpStatusCode?>(out LocalBuilder nullableLocal)
				.Newobj<StringBuilder>()
				.Ldarg_0()
				.Emit(gen => converter.EmitAppendToStringBuilder(gen, nullableLocal, null, null))
				.Callvirt<StringBuilder>("ToString")
				.Ret();
			string serialized = (string)serialize.Invoke(null, new object?[] { code, CultureInfo.InvariantCulture, ',' })!;
			serialized.Should().Be("");
		}

		[Fact]
		public void EmittedDeserializerIsValid() {
			NullableEnumConverter<HttpStatusCode> converter = new NullableEnumConverter<HttpStatusCode>();
			string text = "NotFound";
			DynamicMethod deserialize = new DynamicMethod("Deserialize", typeof(HttpStatusCode?), new Type[] { typeof(ReadOnlyMemory<char>), typeof(IFormatProvider) }, typeof(NullableEnumConverterTests));
			deserialize.GetILGenerator()
				.DeclareLocal<HttpStatusCode?>(out LocalBuilder nullableLocal)
				.DeclareLocal<string>(out LocalBuilder secondaryLocal)
				.Ldarga_S(0)
				.Emit(gen => converter.EmitDeserialize(gen, nullableLocal, secondaryLocal, null))
				.Ret();
			HttpStatusCode? code = (HttpStatusCode?)deserialize.Invoke(this, new object?[] { text.AsMemory(), CultureInfo.InvariantCulture })!;
			code.Should().Be(HttpStatusCode.NotFound);
		}

		[Fact]
		public void EmittedDeserializerDeserializesEmptyStringIntoNull() {
			NullableEnumConverter<HttpStatusCode> converter = new NullableEnumConverter<HttpStatusCode>();
			string text = "";
			DynamicMethod deserialize = new DynamicMethod("Deserialize", typeof(HttpStatusCode?), new Type[] { typeof(ReadOnlyMemory<char>), typeof(IFormatProvider) }, typeof(NullableEnumConverterTests));
			deserialize.GetILGenerator()
				.DeclareLocal<HttpStatusCode?>(out LocalBuilder nullableLocal)
				.DeclareLocal<string>(out LocalBuilder secondaryLocal)
				.Ldarga_S(0)
				.Emit(gen => converter.EmitDeserialize(gen, nullableLocal, secondaryLocal, null))
				.Ret();
			HttpStatusCode? code = (HttpStatusCode?)deserialize.Invoke(this, new object?[] { text.AsMemory(), CultureInfo.InvariantCulture })!;
			code.Should().BeNull();
		}
	}
}
