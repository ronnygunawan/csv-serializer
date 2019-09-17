using Missil;
using System;
using System.Reflection.Emit;
using System.Text;

namespace Csv.Internal.Converters {
	internal class StringConverter : INativeConverter<string?> {
		public void AppendToStringBuilder(StringBuilder stringBuilder, IFormatProvider provider, string? value, CsvColumnAttribute? attribute, char delimiter) {
			if (value is { }) {
				stringBuilder.Append('"').Append(value.Replace("\"", "\"\"")).Append('"');
			}
		}

		public string? Deserialize(ReadOnlyMemory<char> text, IFormatProvider provider, CsvColumnAttribute? attribute) {
			if (text.Length == 0) {
				return string.Empty;
			} else {
				return text.ToString();
			}
		}

		/// <param name="local">A local of type <see cref="String"/></param>
		[ConverterEmitter(typeof(string))]
		public void EmitAppendToStringBuilder(ILGenerator gen, LocalBuilder? local, LocalBuilder? _, CsvColumnAttribute? attribute) => gen
			.Dup()
			.Ldnull()
			.Cgt_Un()
			.Brfalse_S(out Label @else)
				.Stloc(local!)
				.Ldc_I4_X((int)'"')
				.Callvirt<StringBuilder>("Append", typeof(char))
				.Ldloc(local!)
				.Callvirt<Uri>("ToString")
				.Ldstr("\"")
				.Ldstr("\"\"")
				.Callvirt<string>("Replace", typeof(string), typeof(string))
				.Callvirt<StringBuilder>("Append", typeof(string))
				.Ldc_I4_X((int)'"')
				.Callvirt<StringBuilder>("Append", typeof(char))
				.Br_S(out Label @endif)
			.Label(@else)
				.Pop()
			.Label(@endif);

		[ConverterEmitter]
		public void EmitDeserialize(ILGenerator gen, LocalBuilder? _, LocalBuilder? __, CsvColumnAttribute? attribute) => gen
			.Dup()
			.CallPropertyGet<ReadOnlyMemory<char>>("Length")
			.Brfalse_S(out Label @else)
				.Callvirt<ReadOnlyMemory<char>>("ToString")
				.Br_S(out Label @endif)
			.Label(@else)
				.Pop()
				.Ldsfld(typeof(string).GetField("Empty")!)
			.Label(@endif);
	}
}
