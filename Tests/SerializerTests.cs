using Csv;
using FluentAssertions;
using System;
using System.Globalization;
using System.Linq;
using System.Net;
using Tests.Utilities;
using Xunit;

namespace Tests {
	public class SerializeTests {
		[Fact]
		public void AnonymousTypesAreSerializedUsingNaiveSerializer() {
			var item = new {
				Bool = true,
				Byte = (byte)0x66,
				SByte = (sbyte)-100,
				Short = (short)-200,
				UShort = (ushort)200,
				Int = -3000,
				UInt = 3000,
				Long = -40000L,
				ULong = 40000UL,
				Float = 100000000000000.0f,
				Double = 17837193718273812973.0,
				Decimal = 989898989898m,
				String = "CSV Serializer",
				DateTime = new DateTime(2019, 8, 23),
				Uri = new Uri("http://localhost:5000/"),
				StatusCode = HttpStatusCode.OK
			};

			string csv = CsvSerializer.Serialize(new[] { item }, withHeaders: true, provider: CultureInfo.GetCultureInfo("en-US"));
			csv.Should().BeSimilarTo("""
				"Bool","Byte","SByte","Short","UShort","Int","UInt","Long","ULong","Float","Double","Decimal","String","DateTime","Uri","StatusCode"
				True,102,-100,-200,200,-3000,3000,-40000,40000,1E+14,1.7837193718273812E+19,989898989898,"CSV Serializer","8/23/2019 12:00:00 AM","http://localhost:5000/",OK
				""");
		}

		[Fact]
		public void PublicTypesAreSerializedUsingDynamicSerializer() {
			Model item = new() {
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
				DateTime = new DateTime(2019, 8, 23),
				Uri = new Uri("http://localhost:5000/"),
				StatusCode = HttpStatusCode.OK
			};
			string csv = CsvSerializer.Serialize(new[] { item }, withHeaders: true, provider: CultureInfo.GetCultureInfo("en-US"));
			csv.Should().BeSimilarTo("""
				"Bool","Byte","SByte","Short","UShort","Int","UInt","Long","ULong","Float","Double","Decimal","String","DateTime","Uri","StatusCode"
				True,102,-100,-200,200,-3000,3000,-40000,40000,1E+14,1.7837193718273812E+19,989898989898,"CSV Serializer","8/23/2019 12:00:00 AM","http://localhost:5000/",OK
				""");
		}

		[Fact]
		public void PrivateTypesAreSerializedUsingNaiveSerializer() {
			PrivateModel item = new() {
				Name = "CSV Serializer"
			};
			string csv = CsvSerializer.Serialize(new[] { item }, withHeaders: true);
			csv.Should().BeSimilarTo("""
				"Name"
				"CSV Serializer"
				""");
		}

		[Fact]
		public void Serializing1MillionRowsOfAnonymousTypeCompletesIn10Seconds() {
			var item = new {
				Bool = true,
				Byte = (byte)0x66,
				SByte = (sbyte)-100,
				Short = (short)-200,
				UShort = (ushort)200,
				Int = -3000,
				UInt = 3000,
				Long = -40000L,
				ULong = 40000UL,
				Float = 100000000000000.0f,
				Double = 17837193718273812973.0,
				Decimal = 989898989898m,
				String = "CSV Serializer",
				DateTime = new DateTime(2019, 8, 23),
				Uri = new Uri("http://localhost:5000/")
			};
			new Action(() => CsvSerializer.Serialize(Enumerable.Repeat(item, 1_000_000))).ExecutionTime().Should().BeLessThan(TimeSpan.FromSeconds(10));
		}

		[Fact]
		public void Serializing1MillionRowsOfPublicTypeCompletesIn10Seconds() {
			Model item = new() {
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
				DateTime = new DateTime(2019, 8, 23),
				Uri = new Uri("http://localhost:5000/")
			};
			new Action(() => CsvSerializer.Serialize(Enumerable.Repeat(item, 1_000_000))).ExecutionTime().Should().BeLessThan(TimeSpan.FromSeconds(10));
		}

		[Fact]
		public void Deserializing1MillionRowsOfPublicTypeCompletesIn20Seconds() {
			Model item = new() {
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
				DateTime = new DateTime(2019, 8, 23),
				Uri = new Uri("http://localhost:5000/"),
				StatusCode = HttpStatusCode.OK
			};
			string csv = CsvSerializer.Serialize(Enumerable.Repeat(item, 1_000_000));
			new Action(() => CsvSerializer.Deserialize<Model>(csv)).ExecutionTime().Should().BeLessThan(TimeSpan.FromSeconds(20));
		}

		[Fact]
		public void CsvCanBeDeserializedToPublicType() {
			string csv = """
				"Bool","Byte","SByte","Short","UShort","Int","UInt","Long","ULong","Float","Double","Decimal","String","DateTime","Uri","StatusCode"
				True,102,-100,-200,200,-3000,3000,-40000,40000,1E+14,1.7837193718273812E+19,989898989898,"CSV Serializer","08/23/2019 00:00:00","http://localhost:5000/",OK
				""";
			Model[] items = CsvSerializer.Deserialize<Model>(csv, hasHeaders: true, provider: CultureInfo.InvariantCulture);
			items.Length.Should().Be(1);
			Model item = items.Single();
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
			item.Uri.Should().Be(new Uri("http://localhost:5000/"));
		}

		[Fact]
		public void CsvCanBeDeserializedToPrivateType() {
			string csv = "\"Name\"\r\n\"CSV Serializer\"";
			PrivateModel[] items = CsvSerializer.Deserialize<PrivateModel>(csv, hasHeaders: true);
			items.Length.Should().Be(1);
			PrivateModel item = items.Single();
			item.Name.Should().Be("CSV Serializer");
		}

		[Fact]
		public void HeaderNameCanBeRenamedUsingAttribute() {
			ModelWithColumnNames[] items = {
				new ModelWithColumnNames { ItemName = "Coconut", WeightInGrams = 900 },
				new ModelWithColumnNames { ItemName = "Jackfruit", WeightInGrams = 1200 }
			};
			string csv = CsvSerializer.Serialize(items, withHeaders: true);
			string[] lines = csv.Split("\r\n");
			lines.Length.Should().Be(3);
			string header = lines[0];
			header.Should().Be("\"Name\",\"Weight\"");
		}

		[Fact]
		public void CanDeserializeCsvFromExcel() {
			string csv = """
				No;Name;Price;Weight;CreatedDate;IsSuspended
				10;Deflector, Dust (For Rear Differential);200000;20,5;13/12/2019;FALSE
				13;"Deflector; Tire; Filter";150000;15,5;20/11/2019;TRUE
				""";
			ExcelModel[] models = CsvSerializer.Deserialize<ExcelModel>(csv, hasHeaders: true, delimiter: ';', provider: CultureInfo.GetCultureInfo("id-ID"));
			models.Count().Should().Be(2);
		}
	}

	public class ExcelModel {
		public int No { get; set; }
		public required string Name { get; set; }
		public decimal Price { get; set; }
		public double Weight { get; set; }
		[CsvColumn("CreatedDate", DateFormat = "dd/MM/yyyy")]
		public DateTime CreatedDate { get; set; }
		public bool IsSuspended { get; set; }
	}

	public class Model {
		public bool Bool { get; set; }
		public byte Byte { get; set; }
		public sbyte SByte { get; set; }
		public short Short { get; set; }
		public ushort UShort { get; set; }
		public int Int { get; set; }
		public uint UInt { get; set; }
		public long Long { get; set; }
		public ulong ULong { get; set; }
		public float Float { get; set; }
		public double Double { get; set; }
		public decimal Decimal { get; set; }
		public string? String { get; set; }
		public DateTime DateTime { get; set; }
		public Uri? Uri { get; set; }
		public HttpStatusCode? StatusCode { get; set; }
	}

	internal sealed class PrivateModel {
		public string? Name { get; set; }
	}

	public class ModelWithColumnNames {
		[CsvColumn("Name")]
		public string? ItemName { get; set; }
		[CsvColumn("Weight")]
		public int WeightInGrams { get; set; }
	}
}
