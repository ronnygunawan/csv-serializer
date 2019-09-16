using Csv.Internal.Converters;
using FluentAssertions;
using Missil;
using System;
using System.Globalization;
using System.Net;
using System.Reflection.Emit;
using System.Text;
using Xunit;

namespace Tests.ConverterTests {
	public class EnumConverterTests {
		[Fact]
		public void EnumSerializerIsValid() {
			EnumConverter<HttpStatusCode> converter = new EnumConverter<HttpStatusCode>();
			HttpStatusCode code = HttpStatusCode.NotFound;
			StringBuilder stringBuilder = new StringBuilder();
			converter.AppendToStringBuilder(stringBuilder, CultureInfo.InvariantCulture, code, null, ',');
			string serialized = stringBuilder.ToString();
			serialized.Should().Be("NotFound");
		}

		[Fact]
		public void EnumDeserializerIsValid() {
			EnumConverter<HttpStatusCode> converter = new EnumConverter<HttpStatusCode>();
			string text = "NotFound";
			HttpStatusCode code = converter.Deserialize(text.AsMemory(), CultureInfo.InvariantCulture, null);
			code.Should().Be(HttpStatusCode.NotFound);
		}

		[Fact]
		public void EmittedSerializerIsValid() {
			EnumConverter<HttpStatusCode> converter = new EnumConverter<HttpStatusCode>();
			HttpStatusCode code = HttpStatusCode.NotFound;
			DynamicMethod serialize = new DynamicMethod("Serialize", typeof(string), new Type[] { typeof(HttpStatusCode), typeof(IFormatProvider), typeof(char) }, typeof(EnumConverterTests));
			serialize.GetILGenerator()
				.Newobj<StringBuilder>()
				.Ldarg_0()
				.Emit(gen => converter.EmitAppendToStringBuilder(gen, null, null, null))
				.Callvirt<StringBuilder>("ToString")
				.Ret();
			string serialized = (string)serialize.Invoke(null, new object?[] { code, CultureInfo.InvariantCulture, ',' })!;
			serialized.Should().Be("NotFound");
		}

		[Fact]
		public void EmittedDeserializerIsValid() {
			EnumConverter<HttpStatusCode> converter = new EnumConverter<HttpStatusCode>();
			string text = "NotFound";
			DynamicMethod deserialize = new DynamicMethod("Deserialize", typeof(HttpStatusCode), new Type[] { typeof(ReadOnlyMemory<char>), typeof(IFormatProvider) }, typeof(UriConverterTests));
			deserialize.GetILGenerator()
				.DeclareLocal<string>(out LocalBuilder secondaryLocal)
				.Ldarga_S(0)
				.Emit(gen => converter.EmitDeserialize(gen, null, secondaryLocal, null))
				.Ret();
			HttpStatusCode deserialized = (HttpStatusCode)deserialize.Invoke(this, new object?[] { text.AsMemory(), CultureInfo.InvariantCulture })!;
			deserialized.Should().Be(HttpStatusCode.NotFound);
		}
	}
}
