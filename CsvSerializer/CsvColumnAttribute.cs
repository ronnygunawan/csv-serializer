using System;

namespace Csv {
	[AttributeUsage(AttributeTargets.Property)]
	public class CsvColumnAttribute : Attribute {
		public string Name { get; }
		public string? DateFormat { get; set; }

		public CsvColumnAttribute(string name) {
			Name = name;
		}
	}
}
