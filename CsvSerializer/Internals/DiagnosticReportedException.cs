using System;
using System.Runtime.Serialization;

namespace Csv.Internals {
	internal class DiagnosticReportedException : Exception {
		public DiagnosticReportedException() { }
		public DiagnosticReportedException(string message) : base(message) { }
		public DiagnosticReportedException(string message, Exception innerException) : base(message, innerException) { }
		protected DiagnosticReportedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}
