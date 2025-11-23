using Shouldly;
using System;
using System.Diagnostics;

namespace Tests.Utilities {
	internal static class ShouldlyExtensions {
		public static void ShouldBeSimilarTo(this string actual, string expected, string? customMessage = null) {
			actual.Replace("\r\n", "\n").Replace("\r", "\n")
				.ShouldBe(expected.Replace("\r\n", "\n").Replace("\r", "\n"), customMessage);
		}

		public static TimeSpan ExecutionTime(this Action action) {
			Stopwatch sw = Stopwatch.StartNew();
			action();
			sw.Stop();
			return sw.Elapsed;
		}
	}
}
