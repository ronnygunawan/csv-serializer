using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace Benchmarks {
	public static class Program {
		public static void Main(string[] args) {
			BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run();
		}
	}

	[RPlotExporter, RankColumn, MemoryDiagnoser]
	[SimpleJob(launchCount: 1, warmupCount: 3, iterationCount: 10)]
	public class Serialize {
		private static readonly Csv.Internal.NaiveImpl.NaiveSerializer<Model> V1Serializer = new();

		private static readonly RecordParser.Parsers.IVariableLengthWriter<Model> RecordParserWriter = new RecordParser.Builders.Writer.VariableLengthWriterSequentialBuilder<Model>()
			.Map(x => x.Bool)
			.Skip(1)
			.Map(x => x.Byte)
			.Skip(1)
			.Map(x => x.SByte)
			.Skip(1)
			.Map(x => x.Short)
			.Skip(1)
			.Map(x => x.UShort)
			.Skip(1)
			.Map(x => x.Int)
			.Skip(1)
			.Map(x => x.UInt)
			.Skip(1)
			.Map(x => x.Long)
			.Skip(1)
			.Map(x => x.ULong)
			.Skip(1)
			.Map(x => x.Float)
			.Skip(1)
			.Map(x => x.Double)
			.Skip(1)
			.Map(x => x.Decimal)
			.Skip(1)
			.Map(x => x.String)
			.Skip(1)
			.Map(x => x.DateTime)
			.Skip(1)
			.Build(",");

		private Model[] _data;

		[Params(1000, 10000, 100000)]
		// ReSharper disable once UnassignedField.Global
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
		public string CsvSerializerV1Serialize() {
			StringBuilder stringBuilder = new();
			foreach (Model item in _data) {
				V1Serializer.SerializeItem(null, ',', stringBuilder, item);
			}
			return stringBuilder.ToString().TrimEnd();
		}

		[Benchmark]
		public string CsvSerializerV2Serialize() {
			return Csv.CsvSerializer.Serialize(_data);
		}

		[Benchmark]
		public string CsvSerializerStreamingSerialize() {
			using MemoryStream memoryStream = new();
			using StreamWriter streamWriter = new(memoryStream, Encoding.UTF8);
			Csv.CsvSerializer.Serialize(streamWriter, _data);
			memoryStream.Position = 0;
			using StreamReader streamReader = new(memoryStream, Encoding.UTF8);
			return streamReader.ReadToEnd();
		}

		[Benchmark]
		public string CsvHelperSerialize() {
			using MemoryStream memoryStream = new();
			using StreamWriter streamWriter = new(memoryStream);
			using CsvHelper.CsvWriter csvWriter = new(streamWriter, CultureInfo.InvariantCulture);
			csvWriter.WriteRecords(_data);
			return Encoding.UTF8.GetString(memoryStream.ToArray());
		}

		[Benchmark]
		public string MessagePackSerializeToJson() => MessagePack.MessagePackSerializer.SerializeToJson(_data);

		[Benchmark]
		public string NewtonsoftJsonSerializeObject() => Newtonsoft.Json.JsonConvert.SerializeObject(_data);

		[Benchmark]
		public byte[] Utf8JsonSerialize() => Utf8Json.JsonSerializer.Serialize(_data);

		[Benchmark]
		public string RecordParserWrite() {
			Span<char> buffer = stackalloc char[2048];
			StringBuilder stringBuilder = new();
			foreach (Model item in _data) {
				RecordParserWriter.TryFormat(item, buffer, out int charsWritten);
				string s = new(buffer[..charsWritten]);
				stringBuilder.AppendLine(s);
			}
			return stringBuilder.ToString().TrimEnd();
		}

		[Benchmark(Baseline = true)]
		public string SystemTextJsonSerialize() => System.Text.Json.JsonSerializer.Serialize(_data);

		[Benchmark]
		public string StringJoin() => string.Join("\r\n",
			from item in _data
			select string.Join(',',
				from v in new object[] {
					item.Bool,
					item.Byte,
					item.SByte,
					item.Short,
					item.UShort,
					item.Int,
					item.UInt,
					item.Long,
					item.ULong,
					item.Float,
					item.Double,
					item.Decimal,
					item.String,
					item.DateTime
				}
				select v.ToString()
			)
		);
	}

	[RPlotExporter, RankColumn, MemoryDiagnoser]
	[SimpleJob(launchCount: 1, warmupCount: 3, iterationCount: 10)]
	public class Deserialize {
		private static readonly Csv.Internal.NaiveImpl.NaiveDeserializer<Model> V1Deserializer = new();

		private static readonly RecordParser.Parsers.IVariableLengthReader<Model> RecordParserReader = new RecordParser.Builders.Reader.VariableLengthReaderBuilder<Model>()
			.Map(x => x.Bool, 0)
			.Map(x => x.Byte, 1)
			.Map(x => x.SByte, 2)
			.Map(x => x.Short, 3)
			.Map(x => x.UShort, 4)
			.Map(x => x.Int, 5)
			.Map(x => x.UInt, 6)
			.Map(x => x.Long, 7)
			.Map(x => x.ULong, 8)
			.Map(x => x.Float, 9)
			.Map(x => x.Double, 10)
			.Map(x => x.Decimal, 11)
			.Map(x => x.String, 12)
			.Map(x => x.DateTime, 13)
			.Build(",");

		private string _csv;

		[Params(1000, 10000, 100000)]
		// ReSharper disable once UnassignedField.Global
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
			Model[] data = Enumerable.Repeat(item, N).ToArray();
			_csv = Csv.CsvSerializer.Serialize(data, withHeaders: true);
		}

		[Benchmark]
		public List<Model> CsvSerializerV1Deserialize() {
			return V1Deserializer.Deserialize(CultureInfo.InvariantCulture, ',', skipHeader: true, _csv.AsMemory()).Cast<Model>().ToList();
		}

		[Benchmark]
		public Model[] CsvSerializerV2Deserialize() {
			return Csv.CsvSerializer.Deserialize<Model>(_csv, hasHeaders: true);
		}

		[Benchmark]
		public void CsvSerializerStreamingDeserialize() {
			using MemoryStream memoryStream = new(Encoding.UTF8.GetBytes(_csv));
			using StreamReader streamReader = new(memoryStream);
			foreach (Model _ in Csv.CsvSerializer.Deserialize<Model>(streamReader, hasHeaders: true)) { }
		}

		[Benchmark]
		public void CsvHelperDeserialize() {
			using MemoryStream memoryStream = new(Encoding.UTF8.GetBytes(_csv));
			using StreamReader streamReader = new(memoryStream);
			using CsvHelper.CsvReader csvReader = new(streamReader, CultureInfo.InvariantCulture);
			foreach (Model _ in csvReader.GetRecords<Model>()) { }
		}

		[Benchmark]
		public void RecordParserRead() {
			foreach (string line in _csv.Split("\r\n").Skip(1)) {
				_ = RecordParserReader.Parse(line.AsSpan());
			}
		}
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
