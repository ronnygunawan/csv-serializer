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

		public void EmitAppendToStringBuilder(ILGenerator gen, LocalBuilder? local, CsvColumnAttribute? attribute) => gen
			.Emit(gen => attribute?.DateFormat switch {
				string dateFormat => gen
					.Stloc(local!)
					.Ldloca(local!)
					.Ldstr(dateFormat)
					.Ldarg_1()
					.Call<DateTime>("ToString", typeof(string), typeof(IFormatProvider))
					.Call<StringBuilder>("Append", typeof(string)),
				_ => gen
					.Stloc(local!)
					.Ldloca(local!)
					.Ldarg_1()
					.Call<DateTime>("ToString", typeof(IFormatProvider))
					.Call<StringBuilder>("Append", typeof(string))
			});

		public void EmitDeserialize(ILGenerator gen, LocalBuilder? local, CsvColumnAttribute? attribute) => throw new NotImplementedException();
	}
}
