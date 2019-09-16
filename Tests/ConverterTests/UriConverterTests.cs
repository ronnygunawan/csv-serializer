using Csv.Internal.Converters;
using FluentAssertions;
using Missil;
using System;
using System.Globalization;
using System.Reflection.Emit;
using System.Text;
using Xunit;

namespace Tests.ConverterTests {
	public class UriConverterTests {
		[Fact]
		public void UriSerializerIsValid() {
			UriConverter converter = new UriConverter();
			Uri uri = new Uri("http://localhost:3000/");
			StringBuilder stringBuilder = new StringBuilder();
			converter.AppendToStringBuilder(stringBuilder, CultureInfo.InvariantCulture, uri, null, ',');
			string serialized = stringBuilder.ToString();
			serialized.Should().Be("\"http://localhost:3000/\"");
		}

		[Fact]
		public void NullUriIsSerializedIntoEmptyString() {
			UriConverter converter = new UriConverter();
			Uri? uri = null;
			StringBuilder stringBuilder = new StringBuilder();
			converter.AppendToStringBuilder(stringBuilder, CultureInfo.InvariantCulture, uri, null, ',');
			string serialized = stringBuilder.ToString();
			serialized.Should().Be("");
		}

		[Fact]
		public void UriWithQuotesIsSerializedUsingEscapeCharacters() {
			UriConverter converter = new UriConverter();
			Uri uri = new Uri("http://localhost:3000/?x=\"");
			StringBuilder stringBuilder = new StringBuilder();
			converter.AppendToStringBuilder(stringBuilder, CultureInfo.InvariantCulture, uri, null, ',');
			string serialized = stringBuilder.ToString();
			serialized.Should().Be("\"http://localhost:3000/?x=\"\"\"");
		}

		[Fact]
		public void UriDeserializerIsValid() {
			UriConverter converter = new UriConverter();
			string text = "http://localhost:3000/";
			Uri? uri = converter.Deserialize(text.AsMemory(), CultureInfo.InvariantCulture, null);
			uri.Should().Be(new Uri("http://localhost:3000/"));
		}

		[Fact]
		public void EmptyStringIsDeserializedIntoNull() {
			UriConverter converter = new UriConverter();
			string text = "";
			Uri? uri = converter.Deserialize(text.AsMemory(), CultureInfo.InvariantCulture, null);
			uri.Should().BeNull();
		}

		[Fact]
		public void EmittedSerializerIsValid() {
			UriConverter converter = new UriConverter();
			Uri uri = new Uri("http://localhost:3000/");
			DynamicMethod serialize = new DynamicMethod("Serialize", typeof(string), new Type[] { typeof(Uri), typeof(IFormatProvider), typeof(char) }, typeof(UriConverterTests));
			serialize.GetILGenerator()
				.DeclareLocal<Uri>(out LocalBuilder local)
				.Newobj<StringBuilder>()
				.Ldarg_0()
				.Emit(gen => converter.EmitAppendToStringBuilder(gen, local, null, null))
				.Callvirt<StringBuilder>("ToString")
				.Ret();
			string serialized = (string)serialize.Invoke(null, new object?[] { uri, CultureInfo.InvariantCulture, ',' })!;
			serialized.Should().Be("\"http://localhost:3000/\"");
		}

		[Fact]
		public void EmittedSerializerSerializesNullUriIntoEmptyString() {
			UriConverter converter = new UriConverter();
			Uri? uri = null;
			DynamicMethod serialize = new DynamicMethod("Serialize", typeof(string), new Type[] { typeof(Uri), typeof(IFormatProvider), typeof(char) }, typeof(UriConverterTests));
			serialize.GetILGenerator()
				.DeclareLocal<Uri>(out LocalBuilder local)
				.Newobj<StringBuilder>()
				.Ldarg_0()
				.Emit(gen => converter.EmitAppendToStringBuilder(gen, local, null, null))
				.Callvirt<StringBuilder>("ToString")
				.Ret();
			string serialized = (string)serialize.Invoke(null, new object?[] { uri, CultureInfo.InvariantCulture, ',' })!;
			serialized.Should().Be("");
		}

		[Fact]
		public void EmittedSerializerSerializesQuotesIntoEscapeCharacters() {
			UriConverter converter = new UriConverter();
			Uri uri = new Uri("http://localhost:3000/?x=\"");
			DynamicMethod serialize = new DynamicMethod("Serialize", typeof(string), new Type[] { typeof(Uri), typeof(IFormatProvider), typeof(char) }, typeof(UriConverterTests));
			serialize.GetILGenerator()
				.DeclareLocal<Uri>(out LocalBuilder local)
				.Newobj<StringBuilder>()
				.Ldarg_0()
				.Emit(gen => converter.EmitAppendToStringBuilder(gen, local, null, null))
				.Callvirt<StringBuilder>("ToString")
				.Ret();
			string serialized = (string)serialize.Invoke(null, new object?[] { uri, CultureInfo.InvariantCulture, ',' })!;
			serialized.Should().Be("\"http://localhost:3000/?x=\"\"\"");
		}

		[Fact]
		public void EmittedDeserializerIsValid() {
			UriConverter converter = new UriConverter();
			string text = "http://localhost:3000/";
			DynamicMethod deserialize = new DynamicMethod("Deserialize", typeof(Uri), new Type[] { typeof(ReadOnlyMemory<char>), typeof(IFormatProvider) }, typeof(UriConverterTests));
			deserialize.GetILGenerator()
				.Ldarga_S(0)
				.Emit(gen => converter.EmitDeserialize(gen, null, null, null))
				.Ret();
			Uri? deserialized = (Uri?)deserialize.Invoke(this, new object?[] { text.AsMemory(), CultureInfo.InvariantCulture })!;
			deserialized.Should().Be(new Uri("http://localhost:3000/"));
		}

		[Fact]
		public void EmittedDeserializerDeserializesEmptyStringIntoNull() {
			UriConverter converter = new UriConverter();
			string text = "";
			DynamicMethod deserialize = new DynamicMethod("Deserialize", typeof(Uri), new Type[] { typeof(ReadOnlyMemory<char>), typeof(IFormatProvider) }, typeof(UriConverterTests));
			deserialize.GetILGenerator()
				.Ldarga_S(0)
				.Emit(gen => converter.EmitDeserialize(gen, null, null, null))
				.Ret();
			Uri? deserialized = (Uri?)deserialize.Invoke(this, new object?[] { text.AsMemory(), CultureInfo.InvariantCulture })!;
			deserialized.Should().BeNull();
		}
	}
}
