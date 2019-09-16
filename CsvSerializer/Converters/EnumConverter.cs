using Missil;
using System;
using System.Reflection.Emit;
using System.Text;

namespace Csv.Converters {
	public class EnumConverter<TEnum> : INativeConverter<TEnum> where TEnum : struct, Enum {
		public void AppendToStringBuilder(StringBuilder stringBuilder, IFormatProvider provider, TEnum value, CsvColumnAttribute? attribute, char delimiter) {
			stringBuilder.Append(value);
		}

		public TEnum Deserialize(ReadOnlyMemory<char> text, IFormatProvider provider, CsvColumnAttribute? attribute) {
			return Enum.Parse<TEnum>(text.ToString());
		}

		[ConverterEmitter]
		public void EmitAppendToStringBuilder(ILGenerator gen, LocalBuilder? _, LocalBuilder? __, CsvColumnAttribute? attribute) => gen
			.Box<TEnum>()
			.Callvirt<StringBuilder>("Append", typeof(object));

		/// <param name="secondaryLocal">A local of type <see cref="String"/></param>
		[ConverterEmitter(secondaryLocalType: typeof(string))]
		public void EmitDeserialize(ILGenerator gen, LocalBuilder? _, LocalBuilder? secondaryLocal, CsvColumnAttribute? attribute) => gen
			.Callvirt<ReadOnlyMemory<char>>("ToString")
			.Stloc(secondaryLocal!)
			.Ldtoken<TEnum>()
			.Ldloc(secondaryLocal!)
			.Call<Enum>("Parse", typeof(Type), typeof(string))
			.Unbox_Any<TEnum>();
	}
}
