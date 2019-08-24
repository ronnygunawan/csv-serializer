using System;

namespace Csv {
	public class CsvPropertyTypeException : CsvException {
		public Type PropertyType { get; }

		public CsvPropertyTypeException(Type propertyType) : base($"Property of type {propertyType.Name} cannot be used in serialization or deserialization.") {
			PropertyType = propertyType;
		}

		public CsvPropertyTypeException(Type propertyType, string? message) : base($"Property of type {propertyType.Name} cannot be used in serialization or deserialization. {message}") {
			PropertyType = propertyType;
		}

		public CsvPropertyTypeException(Type propertyType, string? message, Exception? innerException) : base($"Property of type {propertyType.Name} cannot be used in serialization or deserialization. {message}", innerException) {
			PropertyType = propertyType;
		}
	}
}
