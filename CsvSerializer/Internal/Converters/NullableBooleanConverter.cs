using Missil;
using System;
using System.Reflection.Emit;
using System.Text;

namespace Csv.Converters {
	internal class NullableBooleanConverter : INativeConverter<bool?> {
		public void AppendToStringBuilder(StringBuilder stringBuilder, IFormatProvider provider, bool? value, CsvColumnAttribute? attribute, char delimiter) {
			if (value.HasValue) {
				stringBuilder.Append(value.Value);
			}
		}

		public bool? Deserialize(ReadOnlyMemory<char> text, IFormatProvider provider, CsvColumnAttribute? attribute) {
			if (text.Length == 0) {
				return null;
			} else {
				return bool.Parse(text.Span);
			}
		}

		/// <param name="local">A local of type <see cref="Nullable{Boolean}"/></param>
		[ConverterEmitter(typeof(bool?))]
		public void EmitAppendToStringBuilder(ILGenerator gen, LocalBuilder? local, LocalBuilder? _, CsvColumnAttribute? attribute) => gen
			.Stloc(local!)
			.Ldloca(local!)
			.CallvirtPropertyGet<bool?>("HasValue")
			.Brfalse_S(out Label @endif)
				.Ldloca(local!)
				.CallPropertyGet<bool?>("Value")
				.Callvirt<StringBuilder>("Append", typeof(bool))
			.Label(@endif);

		/// <param name="local">A local of type <see cref="Nullable{Boolean}"/></param>
		[ConverterEmitter(typeof(bool?))]
		public void EmitDeserialize(ILGenerator gen, LocalBuilder? local, LocalBuilder? _, CsvColumnAttribute? attribute) => gen
			.Dup()
			.CallPropertyGet<ReadOnlyMemory<char>>("Length")
			.Brfalse_S(out Label @else)
				.CallPropertyGet<ReadOnlyMemory<char>>("Span")
				.Call<bool>("Parse", typeof(ReadOnlySpan<char>))
				.Newobj<bool?>(typeof(bool))
				.Stloc(local!)
				.Br_S(out Label @endif)
			.Label(@else)
				.Pop()
				.Ldloca(local!)
				.Initobj<bool?>()
			.Label(@endif)
			.Ldloc(local!);
	}
}
