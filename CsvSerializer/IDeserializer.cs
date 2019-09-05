using System;
using System.Collections.Generic;

namespace Csv {
	public interface IDeserializer {
		List<object> Deserialize(ReadOnlyMemory<char> csv, char separator, bool skipHeader);
	}
}
