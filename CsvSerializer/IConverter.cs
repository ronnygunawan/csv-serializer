using System;
using System.Globalization;
using System.Text;

namespace Csv {
	public interface IConverter<T> {
		void AppendToStringBuilder(StringBuilder stringBuilder, IFormatProvider provider, T value, CsvColumnAttribute? attribute);
		T Deserialize(ReadOnlyMemory<char> text, IFormatProvider provider, CsvColumnAttribute? attribute);
	}
}
