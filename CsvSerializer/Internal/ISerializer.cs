using System;
using System.Text;

namespace Csv {
	internal interface ISerializer {
		void SerializeHeader(char delimiter, StringBuilder stringBuilder);
		void SerializeItem(IFormatProvider provider, char delimiter, StringBuilder stringBuilder, object item);
	}
}
