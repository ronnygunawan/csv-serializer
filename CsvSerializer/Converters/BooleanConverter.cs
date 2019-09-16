using Missil;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;

namespace Csv.Converters {
	public class BooleanConverter : INativeConverter<bool> {
		public void AppendToStringBuilder(StringBuilder stringBuilder, IFormatProvider provider, bool value, CsvColumnAttribute? attribute, char delimiter) {
			stringBuilder.Append(value);
		}

		public bool Deserialize(ReadOnlyMemory<char> text, IFormatProvider provider, CsvColumnAttribute? attribute) {
			return bool.Parse(text.Span);
		}

		public void EmitAppendToStringBuilder(ILGenerator gen, LocalBuilder? local, LocalBuilder? secondaryLocal, CsvColumnAttribute? attribute) => gen
			.Call<StringBuilder>("Append", typeof(bool));

		public void EmitDeserialize(ILGenerator gen, LocalBuilder? local, LocalBuilder? secondaryLocal, CsvColumnAttribute? attribute) => gen
			.CallPropertyGet<ReadOnlyMemory<char>>("Span")
			.Call<bool>("Parse", typeof(ReadOnlySpan<char>));
	}
}
