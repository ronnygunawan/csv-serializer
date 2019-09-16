using Missil;
using System;
using System.Reflection.Emit;
using System.Text;

namespace Csv.Internal.Converters {
	internal class BooleanConverter : INativeConverter<bool> {
		public void AppendToStringBuilder(StringBuilder stringBuilder, IFormatProvider provider, bool value, CsvColumnAttribute? attribute, char delimiter) {
			stringBuilder.Append(value);
		}

		public bool Deserialize(ReadOnlyMemory<char> text, IFormatProvider provider, CsvColumnAttribute? attribute) {
			return bool.Parse(text.Span);
		}

		[ConverterEmitter]
		public void EmitAppendToStringBuilder(ILGenerator gen, LocalBuilder? _, LocalBuilder? __, CsvColumnAttribute? attribute) => gen
			.Call<StringBuilder>("Append", typeof(bool));

		[ConverterEmitter]
		public void EmitDeserialize(ILGenerator gen, LocalBuilder? _, LocalBuilder? __, CsvColumnAttribute? attribute) => gen
			.CallPropertyGet<ReadOnlyMemory<char>>("Span")
			.Call<bool>("Parse", typeof(ReadOnlySpan<char>));
	}
}
