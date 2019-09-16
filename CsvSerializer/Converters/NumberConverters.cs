using Missil;
using System;
using System.Globalization;
using System.Reflection.Emit;
using System.Text;

namespace Csv.Converters {
	/***************************************
	 | THIS CLASS WAS AUTO GENERATED USING |
	 | NumberConverters.tt TEXT TEMPLATE.  |
	 | DO NOT MODIFY THIS CLASS!!!         |
	 ***************************************/
	internal class ByteConverter : INativeConverter<Byte> {
		public void AppendToStringBuilder(StringBuilder stringBuilder, IFormatProvider provider, Byte value, CsvColumnAttribute? attribute, char delimiter) {
			stringBuilder.Append(value);
		}

		public Byte Deserialize(ReadOnlyMemory<char> text, IFormatProvider provider, CsvColumnAttribute? attribute) {
			return Byte.Parse(text.Span, provider: provider);
		}

		[ConverterEmitter]
		public void EmitAppendToStringBuilder(ILGenerator gen, LocalBuilder? _, LocalBuilder? __, CsvColumnAttribute? attribute) => gen
			.Call<StringBuilder>("Append", typeof(Byte));

		[ConverterEmitter]
		public void EmitDeserialize(ILGenerator gen, LocalBuilder? _, LocalBuilder? __, CsvColumnAttribute? attribute) => gen
			.CallPropertyGet<ReadOnlyMemory<char>>("Span")
			.Ldc_I4_X((int)NumberStyles.Integer)
			.Ldarg_1()
			.Call<Byte>("Parse", typeof(ReadOnlySpan<char>), typeof(NumberStyles), typeof(IFormatProvider));
	}

	/***************************************
	 | THIS CLASS WAS AUTO GENERATED USING |
	 | NumberConverters.tt TEXT TEMPLATE.  |
	 | DO NOT MODIFY THIS CLASS!!!         |
	 ***************************************/
	internal class NullableByteConverter : INativeConverter<Byte?> {
		public void AppendToStringBuilder(StringBuilder stringBuilder, IFormatProvider provider, Byte? value, CsvColumnAttribute? attribute, char delimiter) {
			if (value.HasValue) {
				stringBuilder.Append(value.Value);
			}
		}

		public Byte? Deserialize(ReadOnlyMemory<char> text, IFormatProvider provider, CsvColumnAttribute? attribute) {
			if (text.Length == 0) {
				return null;
			} else {
				return Byte.Parse(text.Span, provider: provider);
			}
		}


		/// <param name="local">A local of type <see cref="Nullable{Byte}"/></param>
		[ConverterEmitter(typeof(Byte?))]
		public void EmitAppendToStringBuilder(ILGenerator gen, LocalBuilder? local, LocalBuilder? secondaryLocal, CsvColumnAttribute? attribute) => gen
			.Stloc(local!)
			.Ldloca(local!)
			.CallvirtPropertyGet<Byte?>("HasValue")
			.Brfalse_S(out Label @endif)
				.Ldloca(local!)
				.CallPropertyGet<Byte?>("Value")
				.Callvirt<StringBuilder>("Append", typeof(Byte))
			.Label(@endif);

		/// <param name="local">A local of type <see cref="Nullable{Byte}"/></param>
		[ConverterEmitter(typeof(Byte?))]
		public void EmitDeserialize(ILGenerator gen, LocalBuilder? local, LocalBuilder? _, CsvColumnAttribute? attribute) => gen
			.Dup()
			.CallPropertyGet<ReadOnlyMemory<char>>("Length")
			.Brfalse_S(out Label @else)
				.CallPropertyGet<ReadOnlyMemory<char>>("Span")
				.Ldc_I4_X((int)NumberStyles.Integer)
				.Ldarg_1()
				.Call<Byte>("Parse", typeof(ReadOnlySpan<char>), typeof(NumberStyles), typeof(IFormatProvider))
				.Newobj<Byte?>(typeof(Byte))
				.Stloc(local!)
				.Br_S(out Label @endif)
			.Label(@else)
				.Pop()
				.Ldloca(local!)
				.Initobj<Byte?>()
			.Label(@endif)
			.Ldloc(local!);
	}

	/***************************************
	 | THIS CLASS WAS AUTO GENERATED USING |
	 | NumberConverters.tt TEXT TEMPLATE.  |
	 | DO NOT MODIFY THIS CLASS!!!         |
	 ***************************************/
	internal class SByteConverter : INativeConverter<SByte> {
		public void AppendToStringBuilder(StringBuilder stringBuilder, IFormatProvider provider, SByte value, CsvColumnAttribute? attribute, char delimiter) {
			stringBuilder.Append(value);
		}

		public SByte Deserialize(ReadOnlyMemory<char> text, IFormatProvider provider, CsvColumnAttribute? attribute) {
			return SByte.Parse(text.Span, provider: provider);
		}

		[ConverterEmitter]
		public void EmitAppendToStringBuilder(ILGenerator gen, LocalBuilder? _, LocalBuilder? __, CsvColumnAttribute? attribute) => gen
			.Call<StringBuilder>("Append", typeof(SByte));

		[ConverterEmitter]
		public void EmitDeserialize(ILGenerator gen, LocalBuilder? _, LocalBuilder? __, CsvColumnAttribute? attribute) => gen
			.CallPropertyGet<ReadOnlyMemory<char>>("Span")
			.Ldc_I4_X((int)NumberStyles.Integer)
			.Ldarg_1()
			.Call<SByte>("Parse", typeof(ReadOnlySpan<char>), typeof(NumberStyles), typeof(IFormatProvider));
	}

	/***************************************
	 | THIS CLASS WAS AUTO GENERATED USING |
	 | NumberConverters.tt TEXT TEMPLATE.  |
	 | DO NOT MODIFY THIS CLASS!!!         |
	 ***************************************/
	internal class NullableSByteConverter : INativeConverter<SByte?> {
		public void AppendToStringBuilder(StringBuilder stringBuilder, IFormatProvider provider, SByte? value, CsvColumnAttribute? attribute, char delimiter) {
			if (value.HasValue) {
				stringBuilder.Append(value.Value);
			}
		}

		public SByte? Deserialize(ReadOnlyMemory<char> text, IFormatProvider provider, CsvColumnAttribute? attribute) {
			if (text.Length == 0) {
				return null;
			} else {
				return SByte.Parse(text.Span, provider: provider);
			}
		}


		/// <param name="local">A local of type <see cref="Nullable{SByte}"/></param>
		[ConverterEmitter(typeof(SByte?))]
		public void EmitAppendToStringBuilder(ILGenerator gen, LocalBuilder? local, LocalBuilder? secondaryLocal, CsvColumnAttribute? attribute) => gen
			.Stloc(local!)
			.Ldloca(local!)
			.CallvirtPropertyGet<SByte?>("HasValue")
			.Brfalse_S(out Label @endif)
				.Ldloca(local!)
				.CallPropertyGet<SByte?>("Value")
				.Callvirt<StringBuilder>("Append", typeof(SByte))
			.Label(@endif);

		/// <param name="local">A local of type <see cref="Nullable{SByte}"/></param>
		[ConverterEmitter(typeof(SByte?))]
		public void EmitDeserialize(ILGenerator gen, LocalBuilder? local, LocalBuilder? _, CsvColumnAttribute? attribute) => gen
			.Dup()
			.CallPropertyGet<ReadOnlyMemory<char>>("Length")
			.Brfalse_S(out Label @else)
				.CallPropertyGet<ReadOnlyMemory<char>>("Span")
				.Ldc_I4_X((int)NumberStyles.Integer)
				.Ldarg_1()
				.Call<SByte>("Parse", typeof(ReadOnlySpan<char>), typeof(NumberStyles), typeof(IFormatProvider))
				.Newobj<SByte?>(typeof(SByte))
				.Stloc(local!)
				.Br_S(out Label @endif)
			.Label(@else)
				.Pop()
				.Ldloca(local!)
				.Initobj<SByte?>()
			.Label(@endif)
			.Ldloc(local!);
	}

	/***************************************
	 | THIS CLASS WAS AUTO GENERATED USING |
	 | NumberConverters.tt TEXT TEMPLATE.  |
	 | DO NOT MODIFY THIS CLASS!!!         |
	 ***************************************/
	internal class Int16Converter : INativeConverter<Int16> {
		public void AppendToStringBuilder(StringBuilder stringBuilder, IFormatProvider provider, Int16 value, CsvColumnAttribute? attribute, char delimiter) {
			stringBuilder.Append(value);
		}

		public Int16 Deserialize(ReadOnlyMemory<char> text, IFormatProvider provider, CsvColumnAttribute? attribute) {
			return Int16.Parse(text.Span, provider: provider);
		}

		[ConverterEmitter]
		public void EmitAppendToStringBuilder(ILGenerator gen, LocalBuilder? _, LocalBuilder? __, CsvColumnAttribute? attribute) => gen
			.Call<StringBuilder>("Append", typeof(Int16));

		[ConverterEmitter]
		public void EmitDeserialize(ILGenerator gen, LocalBuilder? _, LocalBuilder? __, CsvColumnAttribute? attribute) => gen
			.CallPropertyGet<ReadOnlyMemory<char>>("Span")
			.Ldc_I4_X((int)NumberStyles.Integer)
			.Ldarg_1()
			.Call<Int16>("Parse", typeof(ReadOnlySpan<char>), typeof(NumberStyles), typeof(IFormatProvider));
	}

	/***************************************
	 | THIS CLASS WAS AUTO GENERATED USING |
	 | NumberConverters.tt TEXT TEMPLATE.  |
	 | DO NOT MODIFY THIS CLASS!!!         |
	 ***************************************/
	internal class NullableInt16Converter : INativeConverter<Int16?> {
		public void AppendToStringBuilder(StringBuilder stringBuilder, IFormatProvider provider, Int16? value, CsvColumnAttribute? attribute, char delimiter) {
			if (value.HasValue) {
				stringBuilder.Append(value.Value);
			}
		}

		public Int16? Deserialize(ReadOnlyMemory<char> text, IFormatProvider provider, CsvColumnAttribute? attribute) {
			if (text.Length == 0) {
				return null;
			} else {
				return Int16.Parse(text.Span, provider: provider);
			}
		}


		/// <param name="local">A local of type <see cref="Nullable{Int16}"/></param>
		[ConverterEmitter(typeof(Int16?))]
		public void EmitAppendToStringBuilder(ILGenerator gen, LocalBuilder? local, LocalBuilder? secondaryLocal, CsvColumnAttribute? attribute) => gen
			.Stloc(local!)
			.Ldloca(local!)
			.CallvirtPropertyGet<Int16?>("HasValue")
			.Brfalse_S(out Label @endif)
				.Ldloca(local!)
				.CallPropertyGet<Int16?>("Value")
				.Callvirt<StringBuilder>("Append", typeof(Int16))
			.Label(@endif);

		/// <param name="local">A local of type <see cref="Nullable{Int16}"/></param>
		[ConverterEmitter(typeof(Int16?))]
		public void EmitDeserialize(ILGenerator gen, LocalBuilder? local, LocalBuilder? _, CsvColumnAttribute? attribute) => gen
			.Dup()
			.CallPropertyGet<ReadOnlyMemory<char>>("Length")
			.Brfalse_S(out Label @else)
				.CallPropertyGet<ReadOnlyMemory<char>>("Span")
				.Ldc_I4_X((int)NumberStyles.Integer)
				.Ldarg_1()
				.Call<Int16>("Parse", typeof(ReadOnlySpan<char>), typeof(NumberStyles), typeof(IFormatProvider))
				.Newobj<Int16?>(typeof(Int16))
				.Stloc(local!)
				.Br_S(out Label @endif)
			.Label(@else)
				.Pop()
				.Ldloca(local!)
				.Initobj<Int16?>()
			.Label(@endif)
			.Ldloc(local!);
	}

	/***************************************
	 | THIS CLASS WAS AUTO GENERATED USING |
	 | NumberConverters.tt TEXT TEMPLATE.  |
	 | DO NOT MODIFY THIS CLASS!!!         |
	 ***************************************/
	internal class UInt16Converter : INativeConverter<UInt16> {
		public void AppendToStringBuilder(StringBuilder stringBuilder, IFormatProvider provider, UInt16 value, CsvColumnAttribute? attribute, char delimiter) {
			stringBuilder.Append(value);
		}

		public UInt16 Deserialize(ReadOnlyMemory<char> text, IFormatProvider provider, CsvColumnAttribute? attribute) {
			return UInt16.Parse(text.Span, provider: provider);
		}

		[ConverterEmitter]
		public void EmitAppendToStringBuilder(ILGenerator gen, LocalBuilder? _, LocalBuilder? __, CsvColumnAttribute? attribute) => gen
			.Call<StringBuilder>("Append", typeof(UInt16));

		[ConverterEmitter]
		public void EmitDeserialize(ILGenerator gen, LocalBuilder? _, LocalBuilder? __, CsvColumnAttribute? attribute) => gen
			.CallPropertyGet<ReadOnlyMemory<char>>("Span")
			.Ldc_I4_X((int)NumberStyles.Integer)
			.Ldarg_1()
			.Call<UInt16>("Parse", typeof(ReadOnlySpan<char>), typeof(NumberStyles), typeof(IFormatProvider));
	}

	/***************************************
	 | THIS CLASS WAS AUTO GENERATED USING |
	 | NumberConverters.tt TEXT TEMPLATE.  |
	 | DO NOT MODIFY THIS CLASS!!!         |
	 ***************************************/
	internal class NullableUInt16Converter : INativeConverter<UInt16?> {
		public void AppendToStringBuilder(StringBuilder stringBuilder, IFormatProvider provider, UInt16? value, CsvColumnAttribute? attribute, char delimiter) {
			if (value.HasValue) {
				stringBuilder.Append(value.Value);
			}
		}

		public UInt16? Deserialize(ReadOnlyMemory<char> text, IFormatProvider provider, CsvColumnAttribute? attribute) {
			if (text.Length == 0) {
				return null;
			} else {
				return UInt16.Parse(text.Span, provider: provider);
			}
		}


		/// <param name="local">A local of type <see cref="Nullable{UInt16}"/></param>
		[ConverterEmitter(typeof(UInt16?))]
		public void EmitAppendToStringBuilder(ILGenerator gen, LocalBuilder? local, LocalBuilder? secondaryLocal, CsvColumnAttribute? attribute) => gen
			.Stloc(local!)
			.Ldloca(local!)
			.CallvirtPropertyGet<UInt16?>("HasValue")
			.Brfalse_S(out Label @endif)
				.Ldloca(local!)
				.CallPropertyGet<UInt16?>("Value")
				.Callvirt<StringBuilder>("Append", typeof(UInt16))
			.Label(@endif);

		/// <param name="local">A local of type <see cref="Nullable{UInt16}"/></param>
		[ConverterEmitter(typeof(UInt16?))]
		public void EmitDeserialize(ILGenerator gen, LocalBuilder? local, LocalBuilder? _, CsvColumnAttribute? attribute) => gen
			.Dup()
			.CallPropertyGet<ReadOnlyMemory<char>>("Length")
			.Brfalse_S(out Label @else)
				.CallPropertyGet<ReadOnlyMemory<char>>("Span")
				.Ldc_I4_X((int)NumberStyles.Integer)
				.Ldarg_1()
				.Call<UInt16>("Parse", typeof(ReadOnlySpan<char>), typeof(NumberStyles), typeof(IFormatProvider))
				.Newobj<UInt16?>(typeof(UInt16))
				.Stloc(local!)
				.Br_S(out Label @endif)
			.Label(@else)
				.Pop()
				.Ldloca(local!)
				.Initobj<UInt16?>()
			.Label(@endif)
			.Ldloc(local!);
	}

	/***************************************
	 | THIS CLASS WAS AUTO GENERATED USING |
	 | NumberConverters.tt TEXT TEMPLATE.  |
	 | DO NOT MODIFY THIS CLASS!!!         |
	 ***************************************/
	internal class Int32Converter : INativeConverter<Int32> {
		public void AppendToStringBuilder(StringBuilder stringBuilder, IFormatProvider provider, Int32 value, CsvColumnAttribute? attribute, char delimiter) {
			stringBuilder.Append(value);
		}

		public Int32 Deserialize(ReadOnlyMemory<char> text, IFormatProvider provider, CsvColumnAttribute? attribute) {
			return Int32.Parse(text.Span, provider: provider);
		}

		[ConverterEmitter]
		public void EmitAppendToStringBuilder(ILGenerator gen, LocalBuilder? _, LocalBuilder? __, CsvColumnAttribute? attribute) => gen
			.Call<StringBuilder>("Append", typeof(Int32));

		[ConverterEmitter]
		public void EmitDeserialize(ILGenerator gen, LocalBuilder? _, LocalBuilder? __, CsvColumnAttribute? attribute) => gen
			.CallPropertyGet<ReadOnlyMemory<char>>("Span")
			.Ldc_I4_X((int)NumberStyles.Integer)
			.Ldarg_1()
			.Call<Int32>("Parse", typeof(ReadOnlySpan<char>), typeof(NumberStyles), typeof(IFormatProvider));
	}

	/***************************************
	 | THIS CLASS WAS AUTO GENERATED USING |
	 | NumberConverters.tt TEXT TEMPLATE.  |
	 | DO NOT MODIFY THIS CLASS!!!         |
	 ***************************************/
	internal class NullableInt32Converter : INativeConverter<Int32?> {
		public void AppendToStringBuilder(StringBuilder stringBuilder, IFormatProvider provider, Int32? value, CsvColumnAttribute? attribute, char delimiter) {
			if (value.HasValue) {
				stringBuilder.Append(value.Value);
			}
		}

		public Int32? Deserialize(ReadOnlyMemory<char> text, IFormatProvider provider, CsvColumnAttribute? attribute) {
			if (text.Length == 0) {
				return null;
			} else {
				return Int32.Parse(text.Span, provider: provider);
			}
		}


		/// <param name="local">A local of type <see cref="Nullable{Int32}"/></param>
		[ConverterEmitter(typeof(Int32?))]
		public void EmitAppendToStringBuilder(ILGenerator gen, LocalBuilder? local, LocalBuilder? secondaryLocal, CsvColumnAttribute? attribute) => gen
			.Stloc(local!)
			.Ldloca(local!)
			.CallvirtPropertyGet<Int32?>("HasValue")
			.Brfalse_S(out Label @endif)
				.Ldloca(local!)
				.CallPropertyGet<Int32?>("Value")
				.Callvirt<StringBuilder>("Append", typeof(Int32))
			.Label(@endif);

		/// <param name="local">A local of type <see cref="Nullable{Int32}"/></param>
		[ConverterEmitter(typeof(Int32?))]
		public void EmitDeserialize(ILGenerator gen, LocalBuilder? local, LocalBuilder? _, CsvColumnAttribute? attribute) => gen
			.Dup()
			.CallPropertyGet<ReadOnlyMemory<char>>("Length")
			.Brfalse_S(out Label @else)
				.CallPropertyGet<ReadOnlyMemory<char>>("Span")
				.Ldc_I4_X((int)NumberStyles.Integer)
				.Ldarg_1()
				.Call<Int32>("Parse", typeof(ReadOnlySpan<char>), typeof(NumberStyles), typeof(IFormatProvider))
				.Newobj<Int32?>(typeof(Int32))
				.Stloc(local!)
				.Br_S(out Label @endif)
			.Label(@else)
				.Pop()
				.Ldloca(local!)
				.Initobj<Int32?>()
			.Label(@endif)
			.Ldloc(local!);
	}

	/***************************************
	 | THIS CLASS WAS AUTO GENERATED USING |
	 | NumberConverters.tt TEXT TEMPLATE.  |
	 | DO NOT MODIFY THIS CLASS!!!         |
	 ***************************************/
	internal class UInt32Converter : INativeConverter<UInt32> {
		public void AppendToStringBuilder(StringBuilder stringBuilder, IFormatProvider provider, UInt32 value, CsvColumnAttribute? attribute, char delimiter) {
			stringBuilder.Append(value);
		}

		public UInt32 Deserialize(ReadOnlyMemory<char> text, IFormatProvider provider, CsvColumnAttribute? attribute) {
			return UInt32.Parse(text.Span, provider: provider);
		}

		[ConverterEmitter]
		public void EmitAppendToStringBuilder(ILGenerator gen, LocalBuilder? _, LocalBuilder? __, CsvColumnAttribute? attribute) => gen
			.Call<StringBuilder>("Append", typeof(UInt32));

		[ConverterEmitter]
		public void EmitDeserialize(ILGenerator gen, LocalBuilder? _, LocalBuilder? __, CsvColumnAttribute? attribute) => gen
			.CallPropertyGet<ReadOnlyMemory<char>>("Span")
			.Ldc_I4_X((int)NumberStyles.Integer)
			.Ldarg_1()
			.Call<UInt32>("Parse", typeof(ReadOnlySpan<char>), typeof(NumberStyles), typeof(IFormatProvider));
	}

	/***************************************
	 | THIS CLASS WAS AUTO GENERATED USING |
	 | NumberConverters.tt TEXT TEMPLATE.  |
	 | DO NOT MODIFY THIS CLASS!!!         |
	 ***************************************/
	internal class NullableUInt32Converter : INativeConverter<UInt32?> {
		public void AppendToStringBuilder(StringBuilder stringBuilder, IFormatProvider provider, UInt32? value, CsvColumnAttribute? attribute, char delimiter) {
			if (value.HasValue) {
				stringBuilder.Append(value.Value);
			}
		}

		public UInt32? Deserialize(ReadOnlyMemory<char> text, IFormatProvider provider, CsvColumnAttribute? attribute) {
			if (text.Length == 0) {
				return null;
			} else {
				return UInt32.Parse(text.Span, provider: provider);
			}
		}


		/// <param name="local">A local of type <see cref="Nullable{UInt32}"/></param>
		[ConverterEmitter(typeof(UInt32?))]
		public void EmitAppendToStringBuilder(ILGenerator gen, LocalBuilder? local, LocalBuilder? secondaryLocal, CsvColumnAttribute? attribute) => gen
			.Stloc(local!)
			.Ldloca(local!)
			.CallvirtPropertyGet<UInt32?>("HasValue")
			.Brfalse_S(out Label @endif)
				.Ldloca(local!)
				.CallPropertyGet<UInt32?>("Value")
				.Callvirt<StringBuilder>("Append", typeof(UInt32))
			.Label(@endif);

		/// <param name="local">A local of type <see cref="Nullable{UInt32}"/></param>
		[ConverterEmitter(typeof(UInt32?))]
		public void EmitDeserialize(ILGenerator gen, LocalBuilder? local, LocalBuilder? _, CsvColumnAttribute? attribute) => gen
			.Dup()
			.CallPropertyGet<ReadOnlyMemory<char>>("Length")
			.Brfalse_S(out Label @else)
				.CallPropertyGet<ReadOnlyMemory<char>>("Span")
				.Ldc_I4_X((int)NumberStyles.Integer)
				.Ldarg_1()
				.Call<UInt32>("Parse", typeof(ReadOnlySpan<char>), typeof(NumberStyles), typeof(IFormatProvider))
				.Newobj<UInt32?>(typeof(UInt32))
				.Stloc(local!)
				.Br_S(out Label @endif)
			.Label(@else)
				.Pop()
				.Ldloca(local!)
				.Initobj<UInt32?>()
			.Label(@endif)
			.Ldloc(local!);
	}

	/***************************************
	 | THIS CLASS WAS AUTO GENERATED USING |
	 | NumberConverters.tt TEXT TEMPLATE.  |
	 | DO NOT MODIFY THIS CLASS!!!         |
	 ***************************************/
	internal class Int64Converter : INativeConverter<Int64> {
		public void AppendToStringBuilder(StringBuilder stringBuilder, IFormatProvider provider, Int64 value, CsvColumnAttribute? attribute, char delimiter) {
			stringBuilder.Append(value);
		}

		public Int64 Deserialize(ReadOnlyMemory<char> text, IFormatProvider provider, CsvColumnAttribute? attribute) {
			return Int64.Parse(text.Span, provider: provider);
		}

		[ConverterEmitter]
		public void EmitAppendToStringBuilder(ILGenerator gen, LocalBuilder? _, LocalBuilder? __, CsvColumnAttribute? attribute) => gen
			.Call<StringBuilder>("Append", typeof(Int64));

		[ConverterEmitter]
		public void EmitDeserialize(ILGenerator gen, LocalBuilder? _, LocalBuilder? __, CsvColumnAttribute? attribute) => gen
			.CallPropertyGet<ReadOnlyMemory<char>>("Span")
			.Ldc_I4_X((int)NumberStyles.Integer)
			.Ldarg_1()
			.Call<Int64>("Parse", typeof(ReadOnlySpan<char>), typeof(NumberStyles), typeof(IFormatProvider));
	}

	/***************************************
	 | THIS CLASS WAS AUTO GENERATED USING |
	 | NumberConverters.tt TEXT TEMPLATE.  |
	 | DO NOT MODIFY THIS CLASS!!!         |
	 ***************************************/
	internal class NullableInt64Converter : INativeConverter<Int64?> {
		public void AppendToStringBuilder(StringBuilder stringBuilder, IFormatProvider provider, Int64? value, CsvColumnAttribute? attribute, char delimiter) {
			if (value.HasValue) {
				stringBuilder.Append(value.Value);
			}
		}

		public Int64? Deserialize(ReadOnlyMemory<char> text, IFormatProvider provider, CsvColumnAttribute? attribute) {
			if (text.Length == 0) {
				return null;
			} else {
				return Int64.Parse(text.Span, provider: provider);
			}
		}


		/// <param name="local">A local of type <see cref="Nullable{Int64}"/></param>
		[ConverterEmitter(typeof(Int64?))]
		public void EmitAppendToStringBuilder(ILGenerator gen, LocalBuilder? local, LocalBuilder? secondaryLocal, CsvColumnAttribute? attribute) => gen
			.Stloc(local!)
			.Ldloca(local!)
			.CallvirtPropertyGet<Int64?>("HasValue")
			.Brfalse_S(out Label @endif)
				.Ldloca(local!)
				.CallPropertyGet<Int64?>("Value")
				.Callvirt<StringBuilder>("Append", typeof(Int64))
			.Label(@endif);

		/// <param name="local">A local of type <see cref="Nullable{Int64}"/></param>
		[ConverterEmitter(typeof(Int64?))]
		public void EmitDeserialize(ILGenerator gen, LocalBuilder? local, LocalBuilder? _, CsvColumnAttribute? attribute) => gen
			.Dup()
			.CallPropertyGet<ReadOnlyMemory<char>>("Length")
			.Brfalse_S(out Label @else)
				.CallPropertyGet<ReadOnlyMemory<char>>("Span")
				.Ldc_I4_X((int)NumberStyles.Integer)
				.Ldarg_1()
				.Call<Int64>("Parse", typeof(ReadOnlySpan<char>), typeof(NumberStyles), typeof(IFormatProvider))
				.Newobj<Int64?>(typeof(Int64))
				.Stloc(local!)
				.Br_S(out Label @endif)
			.Label(@else)
				.Pop()
				.Ldloca(local!)
				.Initobj<Int64?>()
			.Label(@endif)
			.Ldloc(local!);
	}

	/***************************************
	 | THIS CLASS WAS AUTO GENERATED USING |
	 | NumberConverters.tt TEXT TEMPLATE.  |
	 | DO NOT MODIFY THIS CLASS!!!         |
	 ***************************************/
	internal class UInt64Converter : INativeConverter<UInt64> {
		public void AppendToStringBuilder(StringBuilder stringBuilder, IFormatProvider provider, UInt64 value, CsvColumnAttribute? attribute, char delimiter) {
			stringBuilder.Append(value);
		}

		public UInt64 Deserialize(ReadOnlyMemory<char> text, IFormatProvider provider, CsvColumnAttribute? attribute) {
			return UInt64.Parse(text.Span, provider: provider);
		}

		[ConverterEmitter]
		public void EmitAppendToStringBuilder(ILGenerator gen, LocalBuilder? _, LocalBuilder? __, CsvColumnAttribute? attribute) => gen
			.Call<StringBuilder>("Append", typeof(UInt64));

		[ConverterEmitter]
		public void EmitDeserialize(ILGenerator gen, LocalBuilder? _, LocalBuilder? __, CsvColumnAttribute? attribute) => gen
			.CallPropertyGet<ReadOnlyMemory<char>>("Span")
			.Ldc_I4_X((int)NumberStyles.Integer)
			.Ldarg_1()
			.Call<UInt64>("Parse", typeof(ReadOnlySpan<char>), typeof(NumberStyles), typeof(IFormatProvider));
	}

	/***************************************
	 | THIS CLASS WAS AUTO GENERATED USING |
	 | NumberConverters.tt TEXT TEMPLATE.  |
	 | DO NOT MODIFY THIS CLASS!!!         |
	 ***************************************/
	internal class NullableUInt64Converter : INativeConverter<UInt64?> {
		public void AppendToStringBuilder(StringBuilder stringBuilder, IFormatProvider provider, UInt64? value, CsvColumnAttribute? attribute, char delimiter) {
			if (value.HasValue) {
				stringBuilder.Append(value.Value);
			}
		}

		public UInt64? Deserialize(ReadOnlyMemory<char> text, IFormatProvider provider, CsvColumnAttribute? attribute) {
			if (text.Length == 0) {
				return null;
			} else {
				return UInt64.Parse(text.Span, provider: provider);
			}
		}


		/// <param name="local">A local of type <see cref="Nullable{UInt64}"/></param>
		[ConverterEmitter(typeof(UInt64?))]
		public void EmitAppendToStringBuilder(ILGenerator gen, LocalBuilder? local, LocalBuilder? secondaryLocal, CsvColumnAttribute? attribute) => gen
			.Stloc(local!)
			.Ldloca(local!)
			.CallvirtPropertyGet<UInt64?>("HasValue")
			.Brfalse_S(out Label @endif)
				.Ldloca(local!)
				.CallPropertyGet<UInt64?>("Value")
				.Callvirt<StringBuilder>("Append", typeof(UInt64))
			.Label(@endif);

		/// <param name="local">A local of type <see cref="Nullable{UInt64}"/></param>
		[ConverterEmitter(typeof(UInt64?))]
		public void EmitDeserialize(ILGenerator gen, LocalBuilder? local, LocalBuilder? _, CsvColumnAttribute? attribute) => gen
			.Dup()
			.CallPropertyGet<ReadOnlyMemory<char>>("Length")
			.Brfalse_S(out Label @else)
				.CallPropertyGet<ReadOnlyMemory<char>>("Span")
				.Ldc_I4_X((int)NumberStyles.Integer)
				.Ldarg_1()
				.Call<UInt64>("Parse", typeof(ReadOnlySpan<char>), typeof(NumberStyles), typeof(IFormatProvider))
				.Newobj<UInt64?>(typeof(UInt64))
				.Stloc(local!)
				.Br_S(out Label @endif)
			.Label(@else)
				.Pop()
				.Ldloca(local!)
				.Initobj<UInt64?>()
			.Label(@endif)
			.Ldloc(local!);
	}

	/***************************************
	 | THIS CLASS WAS AUTO GENERATED USING |
	 | NumberConverters.tt TEXT TEMPLATE.  |
	 | DO NOT MODIFY THIS CLASS!!!         |
	 ***************************************/
	internal class SingleConverter : INativeConverter<Single> {
		public void AppendToStringBuilder(StringBuilder stringBuilder, IFormatProvider provider, Single value, CsvColumnAttribute? attribute, char delimiter) {
			string text = value.ToString(provider);
			if (text.Contains(delimiter)) {
				stringBuilder.Append('"').Append(text).Append('"');
			} else {
				stringBuilder.Append(text);
			}
		}

		public Single Deserialize(ReadOnlyMemory<char> text, IFormatProvider provider, CsvColumnAttribute? attribute) {
			return Single.Parse(text.Span, provider: provider);
		}

		/// <param name="local">A local of type <see cref="Single"/></param>
		[ConverterEmitter(typeof(Single))]
		public void EmitAppendToStringBuilder(ILGenerator gen, LocalBuilder? local, LocalBuilder? _, CsvColumnAttribute? attribute) => gen
			.Stloc(local!)
			.Ldloca(local!)
			.Ldarg_1()
			.Call<Single>("ToString", typeof(IFormatProvider))
			.Dup()
			.Ldarg_2()
			.Callvirt<string>("Contains", typeof(char))
			.Brfalse_S(out Label @else)
				.Ldc_I4_X((int)'"')
				.Call<StringBuilder>("Append", typeof(char))
				.Call<StringBuilder>("Append", typeof(string))
				.Ldc_I4_X((int)'"')
				.Call<StringBuilder>("Append", typeof(char))
				.Br_S(out Label endif)
			.Label(@else)
				.Call<StringBuilder>("Append", typeof(string))
			.Label(endif);

		[ConverterEmitter]
		public void EmitDeserialize(ILGenerator gen, LocalBuilder? _, LocalBuilder? __, CsvColumnAttribute? attribute) => gen
			.CallPropertyGet<ReadOnlyMemory<char>>("Span")
			.Ldc_I4_X((int)NumberStyles.Float)
			.Ldarg_1()
			.Call<Single>("Parse", typeof(ReadOnlySpan<char>), typeof(NumberStyles), typeof(IFormatProvider));
	}

	/***************************************
	 | THIS CLASS WAS AUTO GENERATED USING |
	 | NumberConverters.tt TEXT TEMPLATE.  |
	 | DO NOT MODIFY THIS CLASS!!!         |
	 ***************************************/
	internal class NullableSingleConverter : INativeConverter<Single?> {
		public void AppendToStringBuilder(StringBuilder stringBuilder, IFormatProvider provider, Single? value, CsvColumnAttribute? attribute, char delimiter) {
			if (value.HasValue) {
				string text = value.Value.ToString(provider);
				if (text.Contains(delimiter)) {
					stringBuilder.Append('"').Append(text).Append('"');
				} else {
					stringBuilder.Append(text);
				}
			}
		}

		public Single? Deserialize(ReadOnlyMemory<char> text, IFormatProvider provider, CsvColumnAttribute? attribute) {
			if (text.Length == 0) {
				return null;
			} else {
				return Single.Parse(text.Span, provider: provider);
			}
		}


		/// <param name="local">A local of type <see cref="Nullable{Single}"/></param>
		/// <param name="secondaryLocal">A local of type <see cref="Single"/></param>
		[ConverterEmitter(typeof(Single?), typeof(Single))]
		public void EmitAppendToStringBuilder(ILGenerator gen, LocalBuilder? local, LocalBuilder? secondaryLocal, CsvColumnAttribute? attribute) => gen
			.Stloc(local!)
			.Ldloca(local!)
			.CallvirtPropertyGet<Single?>("HasValue")
			.Brfalse_S(out Label @endif)
				.Ldloca(local!)
				.CallPropertyGet<Single?>("Value")
				.Stloc(secondaryLocal!)
				.Ldloca(secondaryLocal!)
				.Ldarg_1()
				.Call<Single>("ToString", typeof(IFormatProvider))
				.Dup()
				.Ldarg_2()
				.Callvirt<string>("Contains", typeof(char))
				.Brfalse_S(out Label @else)
					.Ldc_I4_X((int)'"')
					.Call<StringBuilder>("Append", typeof(char))
					.Call<StringBuilder>("Append", typeof(string))
					.Ldc_I4_X((int)'"')
					.Call<StringBuilder>("Append", typeof(char))
					.Br_S(@endif)
				.Label(@else)
					.Call<StringBuilder>("Append", typeof(string))
			.Label(@endif);

		/// <param name="local">A local of type <see cref="Nullable{Single}"/></param>
		[ConverterEmitter(typeof(Single?))]
		public void EmitDeserialize(ILGenerator gen, LocalBuilder? local, LocalBuilder? _, CsvColumnAttribute? attribute) => gen
			.Dup()
			.CallPropertyGet<ReadOnlyMemory<char>>("Length")
			.Brfalse_S(out Label @else)
				.CallPropertyGet<ReadOnlyMemory<char>>("Span")
				.Ldc_I4_X((int)NumberStyles.Float)
				.Ldarg_1()
				.Call<Single>("Parse", typeof(ReadOnlySpan<char>), typeof(NumberStyles), typeof(IFormatProvider))
				.Newobj<Single?>(typeof(Single))
				.Stloc(local!)
				.Br_S(out Label @endif)
			.Label(@else)
				.Pop()
				.Ldloca(local!)
				.Initobj<Single?>()
			.Label(@endif)
			.Ldloc(local!);
	}

	/***************************************
	 | THIS CLASS WAS AUTO GENERATED USING |
	 | NumberConverters.tt TEXT TEMPLATE.  |
	 | DO NOT MODIFY THIS CLASS!!!         |
	 ***************************************/
	internal class DoubleConverter : INativeConverter<Double> {
		public void AppendToStringBuilder(StringBuilder stringBuilder, IFormatProvider provider, Double value, CsvColumnAttribute? attribute, char delimiter) {
			string text = value.ToString(provider);
			if (text.Contains(delimiter)) {
				stringBuilder.Append('"').Append(text).Append('"');
			} else {
				stringBuilder.Append(text);
			}
		}

		public Double Deserialize(ReadOnlyMemory<char> text, IFormatProvider provider, CsvColumnAttribute? attribute) {
			return Double.Parse(text.Span, provider: provider);
		}

		/// <param name="local">A local of type <see cref="Double"/></param>
		[ConverterEmitter(typeof(Double))]
		public void EmitAppendToStringBuilder(ILGenerator gen, LocalBuilder? local, LocalBuilder? _, CsvColumnAttribute? attribute) => gen
			.Stloc(local!)
			.Ldloca(local!)
			.Ldarg_1()
			.Call<Double>("ToString", typeof(IFormatProvider))
			.Dup()
			.Ldarg_2()
			.Callvirt<string>("Contains", typeof(char))
			.Brfalse_S(out Label @else)
				.Ldc_I4_X((int)'"')
				.Call<StringBuilder>("Append", typeof(char))
				.Call<StringBuilder>("Append", typeof(string))
				.Ldc_I4_X((int)'"')
				.Call<StringBuilder>("Append", typeof(char))
				.Br_S(out Label endif)
			.Label(@else)
				.Call<StringBuilder>("Append", typeof(string))
			.Label(endif);

		[ConverterEmitter]
		public void EmitDeserialize(ILGenerator gen, LocalBuilder? _, LocalBuilder? __, CsvColumnAttribute? attribute) => gen
			.CallPropertyGet<ReadOnlyMemory<char>>("Span")
			.Ldc_I4_X((int)NumberStyles.Float)
			.Ldarg_1()
			.Call<Double>("Parse", typeof(ReadOnlySpan<char>), typeof(NumberStyles), typeof(IFormatProvider));
	}

	/***************************************
	 | THIS CLASS WAS AUTO GENERATED USING |
	 | NumberConverters.tt TEXT TEMPLATE.  |
	 | DO NOT MODIFY THIS CLASS!!!         |
	 ***************************************/
	internal class NullableDoubleConverter : INativeConverter<Double?> {
		public void AppendToStringBuilder(StringBuilder stringBuilder, IFormatProvider provider, Double? value, CsvColumnAttribute? attribute, char delimiter) {
			if (value.HasValue) {
				string text = value.Value.ToString(provider);
				if (text.Contains(delimiter)) {
					stringBuilder.Append('"').Append(text).Append('"');
				} else {
					stringBuilder.Append(text);
				}
			}
		}

		public Double? Deserialize(ReadOnlyMemory<char> text, IFormatProvider provider, CsvColumnAttribute? attribute) {
			if (text.Length == 0) {
				return null;
			} else {
				return Double.Parse(text.Span, provider: provider);
			}
		}


		/// <param name="local">A local of type <see cref="Nullable{Double}"/></param>
		/// <param name="secondaryLocal">A local of type <see cref="Double"/></param>
		[ConverterEmitter(typeof(Double?), typeof(Double))]
		public void EmitAppendToStringBuilder(ILGenerator gen, LocalBuilder? local, LocalBuilder? secondaryLocal, CsvColumnAttribute? attribute) => gen
			.Stloc(local!)
			.Ldloca(local!)
			.CallvirtPropertyGet<Double?>("HasValue")
			.Brfalse_S(out Label @endif)
				.Ldloca(local!)
				.CallPropertyGet<Double?>("Value")
				.Stloc(secondaryLocal!)
				.Ldloca(secondaryLocal!)
				.Ldarg_1()
				.Call<Double>("ToString", typeof(IFormatProvider))
				.Dup()
				.Ldarg_2()
				.Callvirt<string>("Contains", typeof(char))
				.Brfalse_S(out Label @else)
					.Ldc_I4_X((int)'"')
					.Call<StringBuilder>("Append", typeof(char))
					.Call<StringBuilder>("Append", typeof(string))
					.Ldc_I4_X((int)'"')
					.Call<StringBuilder>("Append", typeof(char))
					.Br_S(@endif)
				.Label(@else)
					.Call<StringBuilder>("Append", typeof(string))
			.Label(@endif);

		/// <param name="local">A local of type <see cref="Nullable{Double}"/></param>
		[ConverterEmitter(typeof(Double?))]
		public void EmitDeserialize(ILGenerator gen, LocalBuilder? local, LocalBuilder? _, CsvColumnAttribute? attribute) => gen
			.Dup()
			.CallPropertyGet<ReadOnlyMemory<char>>("Length")
			.Brfalse_S(out Label @else)
				.CallPropertyGet<ReadOnlyMemory<char>>("Span")
				.Ldc_I4_X((int)NumberStyles.Float)
				.Ldarg_1()
				.Call<Double>("Parse", typeof(ReadOnlySpan<char>), typeof(NumberStyles), typeof(IFormatProvider))
				.Newobj<Double?>(typeof(Double))
				.Stloc(local!)
				.Br_S(out Label @endif)
			.Label(@else)
				.Pop()
				.Ldloca(local!)
				.Initobj<Double?>()
			.Label(@endif)
			.Ldloc(local!);
	}

	/***************************************
	 | THIS CLASS WAS AUTO GENERATED USING |
	 | NumberConverters.tt TEXT TEMPLATE.  |
	 | DO NOT MODIFY THIS CLASS!!!         |
	 ***************************************/
	internal class DecimalConverter : INativeConverter<Decimal> {
		public void AppendToStringBuilder(StringBuilder stringBuilder, IFormatProvider provider, Decimal value, CsvColumnAttribute? attribute, char delimiter) {
			string text = value.ToString(provider);
			if (text.Contains(delimiter)) {
				stringBuilder.Append('"').Append(text).Append('"');
			} else {
				stringBuilder.Append(text);
			}
		}

		public Decimal Deserialize(ReadOnlyMemory<char> text, IFormatProvider provider, CsvColumnAttribute? attribute) {
			return Decimal.Parse(text.Span, provider: provider);
		}

		/// <param name="local">A local of type <see cref="Decimal"/></param>
		[ConverterEmitter(typeof(Decimal))]
		public void EmitAppendToStringBuilder(ILGenerator gen, LocalBuilder? local, LocalBuilder? _, CsvColumnAttribute? attribute) => gen
			.Stloc(local!)
			.Ldloca(local!)
			.Ldarg_1()
			.Call<Decimal>("ToString", typeof(IFormatProvider))
			.Dup()
			.Ldarg_2()
			.Callvirt<string>("Contains", typeof(char))
			.Brfalse_S(out Label @else)
				.Ldc_I4_X((int)'"')
				.Call<StringBuilder>("Append", typeof(char))
				.Call<StringBuilder>("Append", typeof(string))
				.Ldc_I4_X((int)'"')
				.Call<StringBuilder>("Append", typeof(char))
				.Br_S(out Label endif)
			.Label(@else)
				.Call<StringBuilder>("Append", typeof(string))
			.Label(endif);

		[ConverterEmitter]
		public void EmitDeserialize(ILGenerator gen, LocalBuilder? _, LocalBuilder? __, CsvColumnAttribute? attribute) => gen
			.CallPropertyGet<ReadOnlyMemory<char>>("Span")
			.Ldc_I4_X((int)NumberStyles.Currency)
			.Ldarg_1()
			.Call<Decimal>("Parse", typeof(ReadOnlySpan<char>), typeof(NumberStyles), typeof(IFormatProvider));
	}

	/***************************************
	 | THIS CLASS WAS AUTO GENERATED USING |
	 | NumberConverters.tt TEXT TEMPLATE.  |
	 | DO NOT MODIFY THIS CLASS!!!         |
	 ***************************************/
	internal class NullableDecimalConverter : INativeConverter<Decimal?> {
		public void AppendToStringBuilder(StringBuilder stringBuilder, IFormatProvider provider, Decimal? value, CsvColumnAttribute? attribute, char delimiter) {
			if (value.HasValue) {
				string text = value.Value.ToString(provider);
				if (text.Contains(delimiter)) {
					stringBuilder.Append('"').Append(text).Append('"');
				} else {
					stringBuilder.Append(text);
				}
			}
		}

		public Decimal? Deserialize(ReadOnlyMemory<char> text, IFormatProvider provider, CsvColumnAttribute? attribute) {
			if (text.Length == 0) {
				return null;
			} else {
				return Decimal.Parse(text.Span, provider: provider);
			}
		}


		/// <param name="local">A local of type <see cref="Nullable{Decimal}"/></param>
		/// <param name="secondaryLocal">A local of type <see cref="Decimal"/></param>
		[ConverterEmitter(typeof(Decimal?), typeof(Decimal))]
		public void EmitAppendToStringBuilder(ILGenerator gen, LocalBuilder? local, LocalBuilder? secondaryLocal, CsvColumnAttribute? attribute) => gen
			.Stloc(local!)
			.Ldloca(local!)
			.CallvirtPropertyGet<Decimal?>("HasValue")
			.Brfalse_S(out Label @endif)
				.Ldloca(local!)
				.CallPropertyGet<Decimal?>("Value")
				.Stloc(secondaryLocal!)
				.Ldloca(secondaryLocal!)
				.Ldarg_1()
				.Call<Decimal>("ToString", typeof(IFormatProvider))
				.Dup()
				.Ldarg_2()
				.Callvirt<string>("Contains", typeof(char))
				.Brfalse_S(out Label @else)
					.Ldc_I4_X((int)'"')
					.Call<StringBuilder>("Append", typeof(char))
					.Call<StringBuilder>("Append", typeof(string))
					.Ldc_I4_X((int)'"')
					.Call<StringBuilder>("Append", typeof(char))
					.Br_S(@endif)
				.Label(@else)
					.Call<StringBuilder>("Append", typeof(string))
			.Label(@endif);

		/// <param name="local">A local of type <see cref="Nullable{Decimal}"/></param>
		[ConverterEmitter(typeof(Decimal?))]
		public void EmitDeserialize(ILGenerator gen, LocalBuilder? local, LocalBuilder? _, CsvColumnAttribute? attribute) => gen
			.Dup()
			.CallPropertyGet<ReadOnlyMemory<char>>("Length")
			.Brfalse_S(out Label @else)
				.CallPropertyGet<ReadOnlyMemory<char>>("Span")
				.Ldc_I4_X((int)NumberStyles.Currency)
				.Ldarg_1()
				.Call<Decimal>("Parse", typeof(ReadOnlySpan<char>), typeof(NumberStyles), typeof(IFormatProvider))
				.Newobj<Decimal?>(typeof(Decimal))
				.Stloc(local!)
				.Br_S(out Label @endif)
			.Label(@else)
				.Pop()
				.Ldloca(local!)
				.Initobj<Decimal?>()
			.Label(@endif)
			.Ldloc(local!);
	}

}