using System;

namespace Csv.Internal.Helpers {
	internal static class ReadOnlyMemoryHelper {
		public static int IndexOf(ReadOnlyMemory<char> memory, char c) => memory.Span.IndexOf(c);
	}
}
