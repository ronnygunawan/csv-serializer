using Missil;
using System;
using System.Reflection.Emit;
using System.Text;

namespace Csv.Internal.Converters {
	internal class NullableEnumConverter<TEnum> : INativeConverter<TEnum?> where TEnum : struct, Enum {
		public void AppendToStringBuilder(StringBuilder stringBuilder, IFormatProvider provider, TEnum? value, CsvColumnAttribute? attribute, char delimiter) {
			if (value.HasValue) {
				stringBuilder.Append(value.Value);
			}
		}

		public TEnum? Deserialize(ReadOnlyMemory<char> text, IFormatProvider provider, CsvColumnAttribute? attribute) {
			if (text.Length == 0) return null;
			return Enum.Parse<TEnum>(text.ToString());
		}

		/// <param name="nullableLocal">A local of type <see cref="Nullable{TEnum}"/></param>
		[ConverterEmitter(nullableOfGenericParameterIsPrimaryLocalType: true)]
		public void EmitAppendToStringBuilder(ILGenerator gen, LocalBuilder? nullableLocal, LocalBuilder? _, CsvColumnAttribute? attribute) => gen
			.Stloc(nullableLocal!)
			.Ldloca(nullableLocal!)
			.CallvirtPropertyGet<TEnum?>("HasValue")
			.Brfalse_S(out Label end)
				.Ldloca(nullableLocal!)
				.CallPropertyGet<TEnum?>("Value")
				.Box<TEnum>()
				.Callvirt<StringBuilder>("Append", typeof(object))
			.Label(end);

		/// <param name="nullableLocal">A local of type <see cref="Nullable{TEnum}"/></param>
		/// <param name="secondaryLocal">A local of type <see cref="String"/></param>
		[ConverterEmitter(nullableOfGenericParameterIsPrimaryLocalType: true, secondaryLocalType: typeof(string))]
		public void EmitDeserialize(ILGenerator gen, LocalBuilder? nullableLocal, LocalBuilder? secondaryLocal, CsvColumnAttribute? attribute) => gen
			.Dup()
			.CallPropertyGet<ReadOnlyMemory<char>>("Length")
			.Brfalse_S(out Label @else)
				.Callvirt<ReadOnlyMemory<char>>("ToString")
				.Stloc(secondaryLocal!)
				.Ldtoken<TEnum>()
				.Ldloc(secondaryLocal!)
				.Call<Enum>("Parse", typeof(Type), typeof(string))
				.Unbox_Any<TEnum>()
				.Newobj<TEnum?>(typeof(TEnum))
				.Stloc(nullableLocal!)
				.Br_S(out Label endif)
			.Label(@else)
				.Pop()
				.Ldloca(nullableLocal!)
				.Initobj<TEnum?>()
			.Label(endif)
			.Ldloc(nullableLocal!);
	}
}
