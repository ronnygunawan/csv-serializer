using System;

namespace Csv {
	public abstract class CsvException : Exception {
		public CsvException() {
		}

		public CsvException(string? message) : base(message) {
		}

		public CsvException(string? message, Exception? innerException) : base(message, innerException) {
		}
	}
}
