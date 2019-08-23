using Csv;
using FluentAssertions;
using System;
using System.Linq;
using Xunit;

namespace Tests {
	public class SerializeTests {
		[Fact]
		public void AnonymousTypesAreSerializedUsingNaiveSerializer() {
			var obj = new {
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
				DateTime = new DateTime(2019, 8, 23)
			};
			string csv = CsvSerializer.Serialize(new[] { obj }, withHeaders: true);
			csv.Should().Be("\"Bool\",\"Byte\",\"SByte\",\"Short\",\"UShort\",\"Int\",\"UInt\",\"Long\",\"ULong\",\"Float\",\"Double\",\"Decimal\",\"String\",\"DateTime\"\r\nTrue,102,-100,-200,200,-3000,3000,-40000,40000,1E+14,1.7837193718273812E+19,989898989898,\"CSV Serializer\",\"08/23/2019 00:00:00\"");
		}

		[Fact]
		public void PublicTypesAreSerializedUsingDynamicSerializer() {
			Model obj = new Model {
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
			string csv = CsvSerializer.Serialize(new[] { obj }, withHeaders: true);
			csv.Should().Be("\"Bool\",\"Byte\",\"SByte\",\"Short\",\"UShort\",\"Int\",\"UInt\",\"Long\",\"ULong\",\"Float\",\"Double\",\"Decimal\",\"String\",\"DateTime\"\r\nTrue,102,-100,-200,200,-3000,3000,-40000,40000,1E+14,1.7837193718273812E+19,989898989898,\"CSV Serializer\",\"08/23/2019 00:00:00\"");
		}

		[Fact]
		public void PrivateTypesAreSerializedUsingNaiveSerializer() {
			PrivateModel obj = new PrivateModel {
				Name = "CSV Serializer"
			};
			string csv = CsvSerializer.Serialize(new[] { obj }, withHeaders: true);
			csv.Should().Be("\"Name\"\r\n\"CSV Serializer\"");
		}

		[Fact]
		public void Serializing1MillionRowsOfAnonymousTypeCompletesIn10Seconds() {
			var obj = new {
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
				DateTime = new DateTime(2019, 8, 23)
			};
			new Action(() => CsvSerializer.Serialize(Enumerable.Repeat(obj, 1_000_000))).ExecutionTime().Should().BeLessThan(TimeSpan.FromSeconds(10));
		}

		[Fact]
		public void Serializing1MillionRowsOfPublicTypeCompletesIn10Seconds() {
			Model obj = new Model {
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
			new Action(() => CsvSerializer.Serialize(Enumerable.Repeat(obj, 1_000_000))).ExecutionTime().Should().BeLessThan(TimeSpan.FromSeconds(10));
		}

		[Fact]
		public void Deserializing1MillionRowsOfPublicTypeCompletesIn10Seconds() {
			Model obj = new Model {
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
			string csv = CsvSerializer.Serialize(Enumerable.Repeat(obj, 1_000_000));
			new Action(() => CsvSerializer.Deserialize<Model>(csv)).ExecutionTime().Should().BeLessThan(TimeSpan.FromSeconds(10));
		}

		[Fact]
		public void CsvCanBeDeserializedToPublicType() {
			string csv = "\"Bool\",\"Byte\",\"SByte\",\"Short\",\"UShort\",\"Int\",\"UInt\",\"Long\",\"ULong\",\"Float\",\"Double\",\"Decimal\",\"String\",\"DateTime\"\r\nTrue,102,-100,-200,200,-3000,3000,-40000,40000,1E+14,1.7837193718273812E+19,989898989898,\"CSV Serializer\",\"08/23/2019 00:00:00\"";
			Model[] models = CsvSerializer.Deserialize<Model>(csv, hasHeaders: true);
			models.Length.Should().Be(1);
			Model model = models.Single();
			model.Bool.Should().BeTrue();
			model.Byte.Should().Be(0x66);
			model.SByte.Should().Be(-100);
			model.Short.Should().Be(-200);
			model.UShort.Should().Be(200);
			model.Int.Should().Be(-3000);
			model.UInt.Should().Be(3000);
			model.Long.Should().Be(-40000L);
			model.ULong.Should().Be(40000UL);
			model.Float.Should().Be(100000000000000.0f);
			model.Double.Should().Be(17837193718273812973.0);
			model.Decimal.Should().Be(989898989898m);
			model.String.Should().Be("CSV Serializer");
			model.DateTime.Should().Be(new DateTime(2019, 8, 23));
		}

		[Fact]
		public void CsvCanBeDeserializedToPrivateType() {
			string csv = "\"Name\"\r\n\"CSV Serializer\"";
			PrivateModel[] models = CsvSerializer.Deserialize<PrivateModel>(csv, hasHeaders: true);
			models.Length.Should().Be(1);
			PrivateModel model = models.Single();
			model.Name.Should().Be("CSV Serializer");
		}

		[Fact]
		public void HeaderNameCanBeRenamedUsingAttribute() {
			ModelWithColumnNames[] models = {
				new ModelWithColumnNames { ItemName = "Coconut", WeightInGrams = 900 },
				new ModelWithColumnNames { ItemName = "Jackfruit", WeightInGrams = 1200 }
			};
			string csv = CsvSerializer.Serialize(models, withHeaders: true);
			string[] lines = csv.Split("\r\n");
			lines.Length.Should().Be(3);
			string header = lines[0];
			header.Should().Be("\"Name\",\"Weight\"");
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
		}

		class PrivateModel {
			public string? Name { get; set; }
		}

		public class ModelWithColumnNames {
			[CsvColumn("Name")]
			public string? ItemName { get; set; }
			[CsvColumn("Weight")]
			public int WeightInGrams { get; set; }
		}
	}
}
