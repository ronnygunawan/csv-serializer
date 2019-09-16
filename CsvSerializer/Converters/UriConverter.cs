using Missil;
using System;
using System.Reflection.Emit;
using System.Text;

namespace Csv.Converters {
	internal class UriConverter : INativeConverter<Uri?> {
		public void AppendToStringBuilder(StringBuilder stringBuilder, IFormatProvider provider, Uri? value, CsvColumnAttribute? attribute, char delimiter) {
			if (value?.ToString() is string address) {
				stringBuilder
					.Append('"')
					.Append(address.Replace("\"", "\"\""))
					.Append('"');
			}
		}

		public Uri? Deserialize(ReadOnlyMemory<char> text, IFormatProvider provider, CsvColumnAttribute? attribute) {
			if (text.Length == 0) {
				return null;
			} else {
				return new Uri(text.ToString());
			}
		}

		/// <param name="local">A local of type <see cref="Uri"/></param>
		[ConverterEmitter(typeof(Uri))]
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
				.Newobj<Uri>(typeof(string))
				.Br_S(out Label @endif)
			.Label(@else)
				.Pop()
				.Ldnull()
			.Label(@endif);
	}
}
