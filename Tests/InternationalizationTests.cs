using Csv;
using FluentAssertions;
using System.Globalization;
using System.Threading;
using Xunit;

namespace Tests {
	public class InternationalizationTests {
		[Fact]
		public void CanSerializeAndDeserializeInEN_USLocale() {
			Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
			DecimalAndDouble model = new DecimalAndDouble {
				Decimal = 123_456.789m,
				Double = 123_456.789
			};
			string csv = CsvSerializer.Serialize(new[] { model });
			DecimalAndDouble[] deserialized = CsvSerializer.Deserialize<DecimalAndDouble>(csv);
			deserialized.Length.Should().Be(1);
			model = deserialized[0];
			model.Decimal.Should().Be(123_456.789m);
			model.Double.Should().Be(123_456.789);
		}

		[Fact]
		public void CanSerializeAndDeserializeInEN_GBLocale() {
			Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-GB");
			DecimalAndDouble model = new DecimalAndDouble {
				Decimal = 123_456.789m,
				Double = 123_456.789
			};
			string csv = CsvSerializer.Serialize(new[] { model });
			DecimalAndDouble[] deserialized = CsvSerializer.Deserialize<DecimalAndDouble>(csv);
			deserialized.Length.Should().Be(1);
			model = deserialized[0];
			model.Decimal.Should().Be(123_456.789m);
			model.Double.Should().Be(123_456.789);
		}

		[Fact]
		public void CanSerializeAndDeserializeInID_IDLocale() {
			Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("id-ID");
			DecimalAndDouble model = new DecimalAndDouble {
				Decimal = 123_456.789m,
				Double = 123_456.789
			};
			string csv = CsvSerializer.Serialize(new[] { model });
			DecimalAndDouble[] deserialized = CsvSerializer.Deserialize<DecimalAndDouble>(csv);
			deserialized.Length.Should().Be(1);
			model = deserialized[0];
			model.Decimal.Should().Be(123_456.789m);
			model.Double.Should().Be(123_456.789);
		}

		[Fact]
		public void CanSerializeAndDeserializeInEN_INLocale() {
			Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-IN");
			DecimalAndDouble model = new DecimalAndDouble {
				Decimal = 123_456.789m,
				Double = 123_456.789
			};
			string csv = CsvSerializer.Serialize(new[] { model });
			DecimalAndDouble[] deserialized = CsvSerializer.Deserialize<DecimalAndDouble>(csv);
			deserialized.Length.Should().Be(1);
			model = deserialized[0];
			model.Decimal.Should().Be(123_456.789m);
			model.Double.Should().Be(123_456.789);
		}

		[Fact]
		public void CanSerializeAndDeserializeInFR_FRLocale() {
			Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("fr-FR");
			DecimalAndDouble model = new DecimalAndDouble {
				Decimal = 123_456.789m,
				Double = 123_456.789
			};
			string csv = CsvSerializer.Serialize(new[] { model });
			DecimalAndDouble[] deserialized = CsvSerializer.Deserialize<DecimalAndDouble>(csv);
			deserialized.Length.Should().Be(1);
			model = deserialized[0];
			model.Decimal.Should().Be(123_456.789m);
			model.Double.Should().Be(123_456.789);
		}

		[Fact]
		public void CanSerializeAndDeserializeInDE_DELocale() {
			Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");
			DecimalAndDouble model = new DecimalAndDouble {
				Decimal = 123_456.789m,
				Double = 123_456.789
			};
			string csv = CsvSerializer.Serialize(new[] { model });
			DecimalAndDouble[] deserialized = CsvSerializer.Deserialize<DecimalAndDouble>(csv);
			deserialized.Length.Should().Be(1);
			model = deserialized[0];
			model.Decimal.Should().Be(123_456.789m);
			model.Double.Should().Be(123_456.789);
		}

		[Fact]
		public void CanSerializeAndDeserializeInZH_CNLocale() {
			Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("zh-CN");
			DecimalAndDouble model = new DecimalAndDouble {
				Decimal = 123_456.789m,
				Double = 123_456.789
			};
			string csv = CsvSerializer.Serialize(new[] { model });
			DecimalAndDouble[] deserialized = CsvSerializer.Deserialize<DecimalAndDouble>(csv);
			deserialized.Length.Should().Be(1);
			model = deserialized[0];
			model.Decimal.Should().Be(123_456.789m);
			model.Double.Should().Be(123_456.789);
		}

		[Fact]
		public void CanSerializeAndDeserializeInAR_SALocale() {
			Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("ar-SA");
			DecimalAndDouble model = new DecimalAndDouble {
				Decimal = 123_456.789m,
				Double = 123_456.789
			};
			string csv = CsvSerializer.Serialize(new[] { model });
			DecimalAndDouble[] deserialized = CsvSerializer.Deserialize<DecimalAndDouble>(csv);
			deserialized.Length.Should().Be(1);
			model = deserialized[0];
			model.Decimal.Should().Be(123_456.789m);
			model.Double.Should().Be(123_456.789);
		}
	}

	public class DecimalAndDouble {
		public decimal Decimal { get; set; }
		public double Double { get; set; }
	}
}
