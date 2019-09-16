using System;
using System.Globalization;
using System.Reflection.Emit;
using System.Text;
using Missil;

namespace Csv.Converters {
	public class DateTimeConverter : INativeConverter<DateTime> {
		public void AppendToStringBuilder(StringBuilder stringBuilder, IFormatProvider provider, DateTime value, CsvColumnAttribute? attribute) {
			if (attribute?.DateFormat is string dateFormat) {
				stringBuilder.Append(value.ToString(dateFormat, provider));
			} else {
				stringBuilder.Append(value.ToString(provider));
			}
		}

		public DateTime Deserialize(ReadOnlyMemory<char> text, IFormatProvider provider, CsvColumnAttribute? attribute) {
			if (attribute?.DateFormat is string dateFormat) {
				return DateTime.ParseExact(text.Span, dateFormat.AsSpan(), provider, DateTimeStyles.AssumeLocal);
			} else {
				return DateTime.Parse(text.Span, provider, DateTimeStyles.AssumeLocal);
			}
		}

		public void EmitAppendToStringBuilder(ILGenerator gen, LocalBuilder? local, LocalBuilder? _, CsvColumnAttribute? attribute) => gen
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
			});

		public void EmitDeserialize(ILGenerator gen, LocalBuilder? local, LocalBuilder? _, CsvColumnAttribute? attribute) => gen
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
			});
	}
}
