using System;
using System.Collections.Generic;

namespace Csv {
	public interface IDeserializer {
		List<object> Deserialize(ReadOnlySpan<char> csv, char separator, bool skipHeader);
	}
}
