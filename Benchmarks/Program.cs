using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Benchmarks {
	public static class Program {
		public static void Main(string[] args) {
			BenchmarkRunner.Run<Serialize>();
		}
	}

	[RPlotExporter, RankColumn, MemoryDiagnoser]
	public class Serialize {
		private Model[] _data;

		[Params(1000, 10000, 100000)]
		public int N;

		[GlobalSetup]
		public void Setup() {
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
				DateTime = new DateTime(2019, 8, 23)
			};
			_data = Enumerable.Repeat(item, N).ToArray();
		}

		[Benchmark]
		public string CsvSerializerSerialize() => Csv.CsvSerializer.Serialize(_data);

		[Benchmark]
		public string CsvHelperSerialize() {
			using MemoryStream memoryStream = new();
			using StreamWriter streamWriter = new(memoryStream);
			using CsvHelper.CsvWriter csvWriter = new(streamWriter, CultureInfo.InvariantCulture);
			csvWriter.WriteRecords(_data);
			return Encoding.UTF8.GetString(memoryStream.ToArray());
		}

		[Benchmark]
		public byte[] MessagePackSerialize() => MessagePack.MessagePackSerializer.Serialize(_data);

		[Benchmark]
		public string MessagePackSerializeToBase64() => Convert.ToBase64String(MessagePack.MessagePackSerializer.Serialize(_data));

		[Benchmark]
		public string MessagePackSerializeToJson() => MessagePack.MessagePackSerializer.SerializeToJson(_data);

		[Benchmark]
		public string NewtonsoftJsonSerializeObject() => Newtonsoft.Json.JsonConvert.SerializeObject(_data);

		[Benchmark]
		public byte[] Utf8JsonSerialize() => Utf8Json.JsonSerializer.Serialize(_data);

		[Benchmark]
		public string SystemTextJsonSerialize() => System.Text.Json.JsonSerializer.Serialize(_data);
	}

	[MessagePack.MessagePackObject]
	public class Model {

		[MessagePack.Key(0)]
		public bool? Bool { get; set; }

		[MessagePack.Key(1)]
		public byte? Byte { get; set; }

		[MessagePack.Key(2)]
		public sbyte? SByte { get; set; }

		[MessagePack.Key(3)]
		public short? Short { get; set; }

		[MessagePack.Key(4)]
		public ushort? UShort { get; set; }

		[MessagePack.Key(5)]
		public int? Int { get; set; }

		[MessagePack.Key(6)]
		public uint? UInt { get; set; }

		[MessagePack.Key(7)]
		public long? Long { get; set; }

		[MessagePack.Key(8)]
		public ulong? ULong { get; set; }

		[MessagePack.Key(9)]
		public float? Float { get; set; }

		[MessagePack.Key(10)]
		public double? Double { get; set; }

		[MessagePack.Key(11)]
		public decimal? Decimal { get; set; }

		[MessagePack.Key(12)]
		public string String { get; set; }

		[MessagePack.Key(13)]
		public DateTime? DateTime { get; set; }
	}
}
