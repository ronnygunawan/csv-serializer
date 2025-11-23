using Csv;
using FluentAssertions;
using System;
using System.Globalization;
using System.Linq;
using Tests.Utilities;
using Xunit;

namespace Tests {
	public sealed class NaiveSerializerTests {
		[Fact]
		public void NullValuesAreSerializedIntoEmptyColumn() {
			typeof(ModelWithNullableValues).IsPublic.Should().BeFalse();

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
			string csv = CsvSerializer.Serialize([obj], withHeaders: true, provider: CultureInfo.GetCultureInfo("en-US"));
			csv.Should().BeSimilarTo("""
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
			csv = CsvSerializer.Serialize([obj], withHeaders: true, provider: CultureInfo.GetCultureInfo("en-US"));
			csv.Should().BeSimilarTo("""
				"Bool","Byte","SByte","Short","UShort","Int","UInt","Long","ULong","Float","Double","Decimal","String","DateTime"
				True,102,-100,-200,200,-3000,3000,-40000,40000,1E+14,1.7837193718273812E+19,989898989898,"CSV Serializer","8/23/2019 12:00:00 AM"
				""");
		}

		[Fact]
		public void EmptyColumnsAreDeserializedIntoNull() {
			typeof(ModelWithNullableValues).IsPublic.Should().BeFalse();

			string csv = ",,,,,,,,,,,,,";
			ModelWithNullableValues[] items = CsvSerializer.Deserialize<ModelWithNullableValues>(csv, provider: CultureInfo.GetCultureInfo("en-US"));
			items.Length.Should().Be(1);
			ModelWithNullableValues item = items[0];
			item.Bool.Should().BeNull();
			item.Byte.Should().BeNull();
			item.SByte.Should().BeNull();
			item.Short.Should().BeNull();
			item.UShort.Should().BeNull();
			item.Int.Should().BeNull();
			item.UInt.Should().BeNull();
			item.Long.Should().BeNull();
			item.ULong.Should().BeNull();
			item.Float.Should().BeNull();
			item.Double.Should().BeNull();
			item.Decimal.Should().BeNull();
			item.String.Should().BeEmpty();
			item.DateTime.Should().BeNull();

			csv = """
				"Bool","Byte","SByte","Short","UShort","Int","UInt","Long","ULong","Float","Double","Decimal","String","DateTime"
				True,102,-100,-200,200,-3000,3000,-40000,40000,1E+14,1.7837193718273812E+19,989898989898,"CSV Serializer","08/23/2019 00:00:00"
				""";
			items = CsvSerializer.Deserialize<ModelWithNullableValues>(csv, hasHeaders: true, provider: CultureInfo.GetCultureInfo("en-US"));
			items.Length.Should().Be(1);
			item = items.Single();
			item.Bool.Should().BeTrue();
			item.Byte.Should().Be(0x66);
			item.SByte.Should().Be(-100);
			item.Short.Should().Be(-200);
			item.UShort.Should().Be(200);
			item.Int.Should().Be(-3000);
			item.UInt.Should().Be(3000);
			item.Long.Should().Be(-40000L);
			item.ULong.Should().Be(40000UL);
			item.Float.Should().Be(100000000000000.0f);
			item.Double.Should().Be(17837193718273812973.0);
			item.Decimal.Should().Be(989898989898m);
			item.String.Should().Be("CSV Serializer");
			item.DateTime.Should().Be(new DateTime(2019, 8, 23));
		}

		[Fact]
		public void DoubleQuotesAreEscapedOnSerializing() {
			var obj = new {
				Name = "Tony \"Iron Man\" Stark"
			};
			string csv = CsvSerializer.Serialize([obj]);
			csv.Should().Be("""
				"Tony ""Iron Man"" Stark"
				""");
		}

		public sealed record EscapeTest {
			public string? Name { get; set; }
		}
		[Fact]
		public void DoubleQuotesAreUnescapedOnDeserializing() {
			typeof(EscapeTest).IsPublic.Should().BeFalse();
			string csv = """
				"Tony ""Iron Man"" Stark"
				""";
			EscapeTest[] items = CsvSerializer.Deserialize<EscapeTest>(csv, provider: CultureInfo.GetCultureInfo("en-US"));
			items.Length.Should().Be(1);
			EscapeTest item = items[0];
			item.Name.Should().Be("Tony \"Iron Man\" Stark");
		}

		public sealed record CommaTest {
			public string? Name { get; set; }
			public string? LastName { get; set; }
		}
		[Fact]
		public void CommasInStringDontSplitString() {
			typeof(CommaTest).IsPublic.Should().BeFalse();
			string csv = """
				"Stark, Tony","Stark"
				"Banner, Bruce","Banner"
				""";
			CommaTest[] items = CsvSerializer.Deserialize<CommaTest>(csv, provider: CultureInfo.GetCultureInfo("en-US"));
			items.Length.Should().Be(2);
			items[0].Name.Should().Be("Stark, Tony");
			items[0].LastName.Should().Be("Stark");
			items[1].Name.Should().Be("Banner, Bruce");
			items[1].LastName.Should().Be("Banner");
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

		public sealed record ModelWithIgnoredProperties {
			[CsvIgnore]
			public int Id { get; set; }

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

			[CsvIgnore]
			public string? String { get; set; }

			public DateTime? DateTime { get; set; }
		}
	}
}
