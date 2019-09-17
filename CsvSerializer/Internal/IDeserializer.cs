using System;
using System.Collections.Generic;

namespace Csv {
	internal interface IDeserializer {
		List<object> Deserialize(IFormatProvider provider, char delimiter, bool skipHeader, ReadOnlyMemory<char> csv);
	}
}
