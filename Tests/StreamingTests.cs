using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Csv;
using Shouldly;
using Tests.Utilities;
using Xunit;

namespace Tests {
	public sealed class StreamingTests {
		private static readonly Encoding UTF8WithoutBOM = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

		[Fact]
		public void NullValuesAreSerializedIntoEmptyColumn() {
			typeof(ModelWithNullableValues).IsPublic.ShouldBeFalse();

			ModelWithNullableValues obj = new() {
				Bool = null,
				Byte = null,
				SByte = null,
				Short = null,
				UShort = null,
				Int = null,
				UInt = null,
				Long = null,
				ULong = null,
				Float = null,
				Double = null,
				Decimal = null,
				String = null,
				DateTime = null
			};
			using MemoryStream serializeStream1 = new();
			using StreamWriter streamWriter1 = new(serializeStream1, UTF8WithoutBOM);
			CsvSerializer.Serialize(streamWriter1, [obj], withHeaders: true, provider: CultureInfo.GetCultureInfo("en-US"));
			string csv = UTF8WithoutBOM.GetString(serializeStream1.ToArray());
			csv.ShouldBeSimilarTo("""
				"Bool","Byte","SByte","Short","UShort","Int","UInt","Long","ULong","Float","Double","Decimal","String","DateTime"
				,,,,,,,,,,,,,

				""");

			obj = new ModelWithNullableValues {
				Bool = true,
				Byte = 0x66,
				SByte = -100,
				Short = -200,
				UShort = 200,
				Int = -3000,
				UInt = 3000,
				Long = -40000L,
				ULong = 40000UL,
				Float = 100000000000000.0f,
				Double = 17837193718273812973.0,
				Decimal = 989898989898m,
				String = "CSV Serializer",
				DateTime = new DateTime(2019, 8, 23)
			};
			using MemoryStream serializeStream2 = new();
			using StreamWriter streamWriter2 = new(serializeStream2, UTF8WithoutBOM);
			CsvSerializer.Serialize(streamWriter2, [obj], withHeaders: true, provider: CultureInfo.GetCultureInfo("en-US"));
			csv = UTF8WithoutBOM.GetString(serializeStream2.ToArray());
			csv.ShouldBeSimilarTo("""
				"Bool","Byte","SByte","Short","UShort","Int","UInt","Long","ULong","Float","Double","Decimal","String","DateTime"
				True,102,-100,-200,200,-3000,3000,-40000,40000,1E+14,1.7837193718273812E+19,989898989898,"CSV Serializer","8/23/2019 12:00:00 AM"

				""");
		}

		[Fact]
		public void EmptyColumnsAreDeserializedIntoNull() {
			typeof(ModelWithNullableValues).IsPublic.ShouldBeFalse();

			string csv = ",,,,,,,,,,,,,";
			using MemoryStream deserializeStream1 = new(UTF8WithoutBOM.GetBytes(csv));
			using StreamReader streamReader1 = new(deserializeStream1, UTF8WithoutBOM);
			ModelWithNullableValues[] items = CsvSerializer.Deserialize<ModelWithNullableValues>(streamReader1, provider: CultureInfo.GetCultureInfo("en-US")).ToArray();
			items.Length.ShouldBe(1);
			ModelWithNullableValues item = items[0];
			item.Bool.ShouldBeNull();
			item.Byte.ShouldBeNull();
			item.SByte.ShouldBeNull();
			item.Short.ShouldBeNull();
			item.UShort.ShouldBeNull();
			item.Int.ShouldBeNull();
			item.UInt.ShouldBeNull();
			item.Long.ShouldBeNull();
			item.ULong.ShouldBeNull();
			item.Float.ShouldBeNull();
			item.Double.ShouldBeNull();
			item.Decimal.ShouldBeNull();
			item.String.ShouldBeEmpty();
			item.DateTime.ShouldBeNull();

			csv = """
				"Bool","Byte","SByte","Short","UShort","Int","UInt","Long","ULong","Float","Double","Decimal","String","DateTime"
				True,102,-100,-200,200,-3000,3000,-40000,40000,1E+14,1.7837193718273812E+19,989898989898,"CSV Serializer","08/23/2019 00:00:00"
				""";
			using MemoryStream deserializeStream2 = new(UTF8WithoutBOM.GetBytes(csv));
			using StreamReader streamReader2 = new(deserializeStream2, UTF8WithoutBOM);
			items = CsvSerializer.Deserialize<ModelWithNullableValues>(streamReader2, hasHeaders: true, provider: CultureInfo.GetCultureInfo("en-US")).ToArray();
			items.Length.ShouldBe(1);
			item = items.Single();
			item.Bool.ShouldBe(true);
			item.Byte.ShouldBe((byte?)0x66);
			item.SByte.ShouldBe((sbyte?)-100);
			item.Short.ShouldBe((short?)-200);
			item.UShort.ShouldBe((ushort?)200);
			item.Int.ShouldBe((int?)-3000);
			item.UInt.ShouldBe((uint?)3000);
			item.Long.ShouldBe((long?)-40000L);
			item.ULong.ShouldBe((ulong?)40000UL);
			item.Float.ShouldBe((float?)100000000000000.0f);
			item.Double.ShouldBe((double?)17837193718273812973.0);
			item.Decimal.ShouldBe((decimal?)989898989898m);
			item.String.ShouldBe("CSV Serializer");
			item.DateTime.ShouldBe((DateTime?)new DateTime(2019, 8, 23));
		}

		[Fact]
		public void DoubleQuotesAreEscapedOnSerializing() {
			var obj = new {
				Name = "Tony \"Iron Man\" Stark"
			};
			using MemoryStream serializeStream = new();
			using StreamWriter streamWriter = new(serializeStream, UTF8WithoutBOM);
			CsvSerializer.Serialize(streamWriter, [obj]);
			string csv = UTF8WithoutBOM.GetString(serializeStream.ToArray());
			csv.ShouldBeSimilarTo("""
				"Tony ""Iron Man"" Stark"

				""");
		}

		public sealed record EscapeTest {
			public string? Name { get; set; }
		}
		[Fact]
		public void DoubleQuotesAreUnescapedOnDeserializing() {
			typeof(EscapeTest).IsPublic.ShouldBeFalse();
			string csv = """
				"Tony ""Iron Man"" Stark"
				""";
			using MemoryStream deserializeStream = new(UTF8WithoutBOM.GetBytes(csv));
			using StreamReader streamReader = new(deserializeStream, UTF8WithoutBOM);
			EscapeTest[] items = CsvSerializer.Deserialize<EscapeTest>(streamReader, provider: CultureInfo.GetCultureInfo("en-US")).ToArray();
			items.Length.ShouldBe(1);
			EscapeTest item = items[0];
			item.Name.ShouldBe("Tony \"Iron Man\" Stark");
		}

		public sealed record CommaTest {
			public string? Name { get; set; }
			public string? LastName { get; set; }
		}
		[Fact]
		public void CommasInStringDontSplitString() {
			typeof(CommaTest).IsPublic.ShouldBeFalse();
			string csv = """
				"Stark, Tony","Stark"
				"Banner, Bruce","Banner"
				""";
			using MemoryStream deserializeStream = new(UTF8WithoutBOM.GetBytes(csv));
			using StreamReader streamReader = new(deserializeStream, UTF8WithoutBOM);
			CommaTest[] items = CsvSerializer.Deserialize<CommaTest>(streamReader, provider: CultureInfo.GetCultureInfo("en-US")).ToArray();
			items.Length.ShouldBe(2);
			items[0].Name.ShouldBe("Stark, Tony");
			items[0].LastName.ShouldBe("Stark");
			items[1].Name.ShouldBe("Banner, Bruce");
			items[1].LastName.ShouldBe("Banner");
		}

		public sealed record ModelWithNullableValues {
			public bool? Bool { get; set; }
			public byte? Byte { get; set; }
			public sbyte? SByte { get; set; }
			public short? Short { get; set; }
			public ushort? UShort { get; set; }
			public int? Int { get; set; }
			public uint? UInt { get; set; }
			public long? Long { get; set; }
			public ulong? ULong { get; set; }
			public float? Float { get; set; }
			public double? Double { get; set; }
			public decimal? Decimal { get; set; }
			public string? String { get; set; }
			public DateTime? DateTime { get; set; }
		}
	}
}
