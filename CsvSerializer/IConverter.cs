﻿using System;
using System.Text;

namespace Csv {
	public interface IConverter<T> {
		void AppendToStringBuilder(StringBuilder stringBuilder, IFormatProvider provider, T value, CsvColumnAttribute? attribute, char delimiter);
		T Deserialize(ReadOnlyMemory<char> text, IFormatProvider provider, CsvColumnAttribute? attribute);
	}
}
