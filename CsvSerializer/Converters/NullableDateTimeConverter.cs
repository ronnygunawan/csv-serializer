using Missil;
using System;
using System.Globalization;
using System.Reflection.Emit;
using System.Text;

namespace Csv.Converters {
	public class NullableDateTimeConverter : INativeConverter<DateTime?> {
		public void AppendToStringBuilder(StringBuilder stringBuilder, IFormatProvider provider, DateTime? value, CsvColumnAttribute? attribute) {
			if (value.HasValue) {
				if (attribute?.DateFormat is string dateFormat) {
					stringBuilder.Append(value.Value.ToString(dateFormat, provider));
				} else {
					stringBuilder.Append(value.Value.ToString(provider));
				}
			}
		}

		public DateTime? Deserialize(ReadOnlyMemory<char> text, IFormatProvider provider, CsvColumnAttribute? attribute) {
			if (text.Length == 0) return null;
			if (attribute?.DateFormat is string dateFormat) {
				return DateTime.ParseExact(text.Span, dateFormat.AsSpan(), provider, DateTimeStyles.AssumeLocal);
			} else {
				return DateTime.Parse(text.Span, provider, DateTimeStyles.AssumeLocal);
			}
		}

		public void EmitAppendToStringBuilder(ILGenerator gen, LocalBuilder? nullableLocal, LocalBuilder? local, CsvColumnAttribute? attribute) => gen
			.Stloc(nullableLocal!)
			.Ldloca(nullableLocal!)
			.CallvirtPropertyGet<DateTime?>("HasValue")
			.Brfalse_S(out Label endif)
				.Ldloca(nullableLocal!)
				.CallPropertyGet<DateTime?>("Value")
				.Stloc(local!)
				.Emit(gen => attribute?.DateFormat switch {
					string dateFormat => gen
						.Ldloca(local!)
						.Ldstr(dateFormat)
						.Ldarg_1()
						.Call<DateTime>("ToString", typeof(string), typeof(IFormatProvider))
						.Call<StringBuilder>("Append", typeof(string)),
					_ => gen
						.Ldloca(local!)
						.Ldarg_1()
						.Call<DateTime>("ToString", typeof(IFormatProvider))
						.Call<StringBuilder>("Append", typeof(string))
				})
			.Label(endif);

		public void EmitDeserialize(ILGenerator gen, LocalBuilder? local, LocalBuilder? _, CsvColumnAttribute? attribute) => gen
			.Dup()
			.CallPropertyGet<ReadOnlyMemory<char>>("Length")
			.Brfalse_S(out Label @else)
				.Emit(gen => attribute?.DateFormat switch {
					string dateFormat => gen
						.CallPropertyGet<ReadOnlyMemory<char>>("Span")
						.Ldstr(dateFormat)
						.Call(typeof(MemoryExtensions).GetMethod("AsSpan", new Type[] { typeof(string) })!)
						.Ldarg_1()
						.Ldc_I4_X((int)DateTimeStyles.AssumeLocal)
						.Call<DateTime>("ParseExact", typeof(ReadOnlySpan<char>), typeof(ReadOnlySpan<char>), typeof(IFormatProvider), typeof(DateTimeStyles)),
					_ => gen
						.CallPropertyGet<ReadOnlyMemory<char>>("Span")
						.Ldarg_1()
						.Ldc_I4_X((int)DateTimeStyles.AssumeLocal)
						.Call<DateTime>("Parse", typeof(ReadOnlySpan<char>), typeof(IFormatProvider), typeof(DateTimeStyles))
				})
				.Newobj<DateTime?>(typeof(DateTime))
				.Stloc(local!)
				.Br_S(out Label endif)
			.Label(@else)
				.Pop()
				.Ldloca(local!)
				.Initobj<DateTime?>()
			.Label(endif)
			.Ldloc(local!);
	}
}
