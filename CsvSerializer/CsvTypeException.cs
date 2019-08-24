using System;

namespace Csv {
	public class CsvTypeException : CsvException {
		public Type Type { get; }
		public string? PropertyName { get; }

		public CsvTypeException(Type type) : base($"{type.Name} cannot be used in serialization or deserialization.") {
			Type = type;
		}

		public CsvTypeException(Type type, string? message) : base($"{type.Name} cannot be used in serialization or deserialization. {message}") {
			Type = type;
		}

		public CsvTypeException(Type type, string propertyName, string? message) : base($"{type.Name}.{propertyName} cannot be used in serialization or deserialization. {message}") {
			Type = type;
			PropertyName = propertyName;
		}

		public CsvTypeException(Type type, string propertyName, string? message, Exception? innerException) : base($"{type.Name}.{propertyName} cannot be used in serialization or deserialization. {message}", innerException) {
			Type = type;
			PropertyName = propertyName;
		}
	}
}
