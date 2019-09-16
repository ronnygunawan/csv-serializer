using Missil;
using System;
using System.Globalization;
using System.Reflection.Emit;
using System.Text;

namespace Csv.Converters {
	internal class NullableDateTimeConverter : INativeConverter<DateTime?> {
		public void AppendToStringBuilder(StringBuilder stringBuilder, IFormatProvider provider, DateTime? value, CsvColumnAttribute? attribute, char delimiter) {
			if (value.HasValue) {
				string text = attribute?.DateFormat switch {
					string dateFormat => value.Value.ToString(dateFormat, provider),
					_ => value.Value.ToString(provider)
				};
				bool containsQuote = text.Contains('\"');
				if (containsQuote) {
					text = text.Replace("\"", "\"\"");
				}
				if (containsQuote || text.Contains(delimiter)) {
					stringBuilder.Append('"').Append(text).Append('"');
				} else {
					stringBuilder.Append(text);
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

		/// <param name="nullableLocal">A local of type <see cref="Nullable{DateTime}"/></param>
		/// <param name="local">A local of type <see cref="DateTime"/></param>
		[ConverterEmitter(typeof(DateTime?), typeof(DateTime))]
		public void EmitAppendToStringBuilder(ILGenerator gen, LocalBuilder? nullableLocal, LocalBuilder? local, CsvColumnAttribute? attribute) => gen
			.Stloc(nullableLocal!)
			.Ldloca(nullableLocal!)
			.CallvirtPropertyGet<DateTime?>("HasValue")
			.Brfalse_S(out Label end)
				.Ldloca(nullableLocal!)
				.CallPropertyGet<DateTime?>("Value")
				.Stloc(local!)
				.Emit(gen => attribute?.DateFormat switch {
					string dateFormat => gen
						.Ldloca(local!)
						.Ldstr(dateFormat)
						.Ldarg_1()
						.Call<DateTime>("ToString", typeof(string), typeof(IFormatProvider)),
					_ => gen
						.Ldloca(local!)
						.Ldarg_1()
						.Call<DateTime>("ToString", typeof(IFormatProvider))
				})
				.Dup()
				.Ldc_I4_X((int)'"')
				.Callvirt<string>("Contains", typeof(char))
				.Brfalse_S(out Label doesNotContainQuote)
					.Ldstr("\"")
					.Ldstr("\"\"")
					.Call<string>("Replace", typeof(string), typeof(string))
					.Br_S(out Label appendQuotedString)
				.Label(doesNotContainQuote)
				.Dup()
				.Ldarg_2()
				.Callvirt<string>("Contains", typeof(char))
				.Brfalse_S(out Label doesNotContainDelimiter)
					.Label(appendQuotedString)
					.Ldc_I4_X((int)'"')
					.Call<StringBuilder>("Append", typeof(char))
					.Call<StringBuilder>("Append", typeof(string))
					.Ldc_I4_X((int)'"')
					.Call<StringBuilder>("Append", typeof(char))
					.Br_S(out Label endAppend)
				.Label(doesNotContainDelimiter)
					.Call<StringBuilder>("Append", typeof(string))
				.Label(endAppend)
			.Label(end);

		/// <param name="local">A local of type <see cref="Nullable{DateTime}"/></param>
		[ConverterEmitter(typeof(DateTime?))]
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
