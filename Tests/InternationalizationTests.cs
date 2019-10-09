using Csv;
using FluentAssertions;
using System;
using System.Globalization;
using Xunit;

namespace Tests {
	public class InternationalizationTests {
		[Fact]
		public void CanSerializeAndDeserializeInEN_USLocale() {
			IFormatProvider provider = CultureInfo.GetCultureInfo("en-US");
			DecimalAndDouble model = new DecimalAndDouble {
				Decimal = 123_456.789m,
				Double = 123_456.789
			};
			string csv = CsvSerializer.Serialize(new[] { model }, provider: provider);
			DecimalAndDouble[] deserialized = CsvSerializer.Deserialize<DecimalAndDouble>(csv, provider: provider);
			deserialized.Length.Should().Be(1);
			model = deserialized[0];
			model.Decimal.Should().Be(123_456.789m);
			model.Double.Should().Be(123_456.789);
		}

		[Fact]
		public void CanSerializeAndDeserializeInEN_GBLocale() {
			IFormatProvider provider = CultureInfo.GetCultureInfo("en-GB");
			DecimalAndDouble model = new DecimalAndDouble {
				Decimal = 123_456.789m,
				Double = 123_456.789
			};
			string csv = CsvSerializer.Serialize(new[] { model }, provider: provider);
			DecimalAndDouble[] deserialized = CsvSerializer.Deserialize<DecimalAndDouble>(csv, provider: provider);
			deserialized.Length.Should().Be(1);
			model = deserialized[0];
			model.Decimal.Should().Be(123_456.789m);
			model.Double.Should().Be(123_456.789);
		}

		[Fact]
		public void CanSerializeAndDeserializeInID_IDLocale() {
			IFormatProvider provider = CultureInfo.GetCultureInfo("id-ID");
			DecimalAndDouble model = new DecimalAndDouble {
				Decimal = 123_456.789m,
				Double = 123_456.789
			};
			string csv = CsvSerializer.Serialize(new[] { model }, provider: provider);
			DecimalAndDouble[] deserialized = CsvSerializer.Deserialize<DecimalAndDouble>(csv, provider: provider);
			deserialized.Length.Should().Be(1);
			model = deserialized[0];
			model.Decimal.Should().Be(123_456.789m);
			model.Double.Should().Be(123_456.789);
		}

		[Fact]
		public void CanSerializeAndDeserializeInEN_INLocale() {
			IFormatProvider provider = CultureInfo.GetCultureInfo("en-IN");
			DecimalAndDouble model = new DecimalAndDouble {
				Decimal = 123_456.789m,
				Double = 123_456.789
			};
			string csv = CsvSerializer.Serialize(new[] { model }, provider: provider);
			DecimalAndDouble[] deserialized = CsvSerializer.Deserialize<DecimalAndDouble>(csv, provider: provider);
			deserialized.Length.Should().Be(1);
			model = deserialized[0];
			model.Decimal.Should().Be(123_456.789m);
			model.Double.Should().Be(123_456.789);
		}

		[Fact]
		public void CanSerializeAndDeserializeInFR_FRLocale() {
			IFormatProvider provider = CultureInfo.GetCultureInfo("fr-FR");
			DecimalAndDouble model = new DecimalAndDouble {
				Decimal = 123_456.789m,
				Double = 123_456.789
			};
			string csv = CsvSerializer.Serialize(new[] { model }, provider: provider);
			DecimalAndDouble[] deserialized = CsvSerializer.Deserialize<DecimalAndDouble>(csv, provider: provider);
			deserialized.Length.Should().Be(1);
			model = deserialized[0];
			model.Decimal.Should().Be(123_456.789m);
			model.Double.Should().Be(123_456.789);
		}

		[Fact]
		public void CanSerializeAndDeserializeInDE_DELocale() {
			IFormatProvider provider = CultureInfo.GetCultureInfo("de-DE");
			DecimalAndDouble model = new DecimalAndDouble {
				Decimal = 123_456.789m,
				Double = 123_456.789
			};
			string csv = CsvSerializer.Serialize(new[] { model }, provider: provider);
			DecimalAndDouble[] deserialized = CsvSerializer.Deserialize<DecimalAndDouble>(csv, provider: provider);
			deserialized.Length.Should().Be(1);
			model = deserialized[0];
			model.Decimal.Should().Be(123_456.789m);
			model.Double.Should().Be(123_456.789);
		}

		[Fact]
		public void CanSerializeAndDeserializeInZH_CNLocale() {
			IFormatProvider provider = CultureInfo.GetCultureInfo("zh-CN");
			DecimalAndDouble model = new DecimalAndDouble {
				Decimal = 123_456.789m,
				Double = 123_456.789
			};
			string csv = CsvSerializer.Serialize(new[] { model }, provider: provider);
			DecimalAndDouble[] deserialized = CsvSerializer.Deserialize<DecimalAndDouble>(csv, provider: provider);
			deserialized.Length.Should().Be(1);
			model = deserialized[0];
			model.Decimal.Should().Be(123_456.789m);
			model.Double.Should().Be(123_456.789);
		}

		[Fact]
		public void CanSerializeAndDeserializeInAR_SALocale() {
			IFormatProvider provider = CultureInfo.GetCultureInfo("ar-SA");
			DecimalAndDouble model = new DecimalAndDouble {
				Decimal = 123_456.789m,
				Double = 123_456.789
			};
			string csv = CsvSerializer.Serialize(new[] { model }, provider: provider);
			DecimalAndDouble[] deserialized = CsvSerializer.Deserialize<DecimalAndDouble>(csv, provider: provider);
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
