using System;

namespace Csv {
	public class CsvFormatException : CsvException {
		public Type? Type { get; }
		public string? PropertyName { get; }
		public string? Value { get; }

		public CsvFormatException(string value, string? message) : base($"Cannot deserialize '{value}'. {message}") {
			Value = value;
		}

		public CsvFormatException(Type type, string value, string? message) : base($"Cannot deserialize '{value}' into {type.Name}. {message}") {
			Type = type;
			Value = value;
		}

		public CsvFormatException(Type type, string propertyName, string value, string? message) : base($"Cannot deserialize '{value}' into {type.Name}.{propertyName}. {message}") {
			Type = type;
			PropertyName = propertyName;
			Value = value;
		}

		public CsvFormatException(Type type, string propertyName, string value, string? message, Exception? innerException) : base($"Cannot deserialize '{value}' into {type.Name}.{propertyName}. {message}", innerException) {
			Type = type;
			PropertyName = propertyName;
			Value = value;
		}
	}
}
