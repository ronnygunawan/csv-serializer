using Missil;
using System;
using System.Globalization;
using System.Reflection.Emit;
using System.Text;

namespace Csv.Converters {
	public class ByteConverter : INativeConverter<Byte> {
		public void AppendToStringBuilder(StringBuilder stringBuilder, IFormatProvider formatProvider, Byte value, CsvColumnAttribute? attribute) {
			stringBuilder.Append(value);
		}

		public Byte Deserialize(ReadOnlyMemory<char> text, IFormatProvider provider, CsvColumnAttribute? attribute) {
			return Byte.Parse(text.Span, provider: provider);
		}

		public void EmitAppendToStringBuilder(ILGenerator gen, LocalBuilder? local, CsvColumnAttribute? attribute) => gen
			.Call<StringBuilder>("Append", typeof(Byte));

		public void EmitDeserialize(ILGenerator gen, LocalBuilder? local, CsvColumnAttribute? attribute) => gen
			.Call(typeof(ReadOnlyMemory<char>).GetProperty("Span")!.GetGetMethod()!)
			.Ldc_I4_X((int)NumberStyles.Integer)
			.Ldarg_1()
			.Call<Byte>("Parse", typeof(ReadOnlySpan<char>), typeof(NumberStyles), typeof(IFormatProvider));
	}

	public class NullableByteConverter : INativeConverter<Byte?> {
		public void AppendToStringBuilder(StringBuilder stringBuilder, IFormatProvider formatProvider, Byte? value, CsvColumnAttribute? attribute) {
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

		public void EmitAppendToStringBuilder(ILGenerator gen, LocalBuilder? local, CsvColumnAttribute? attribute) => gen
			.Stloc(local!)
			.Ldloca(local!)
			.Callvirt(typeof(Byte?).GetProperty("HasValue")!.GetGetMethod()!)
			.Brfalse_S(out Label @endif)
				.Ldloca(local!)
				.Call(typeof(Byte?).GetProperty("Value")!.GetGetMethod()!)
				.Callvirt<StringBuilder>("Append", typeof(Byte))
			.Label(@endif);

		public void EmitDeserialize(ILGenerator gen, LocalBuilder? local, CsvColumnAttribute? attribute) => gen
			.Dup()
			.Call(typeof(ReadOnlyMemory<char>).GetProperty("Length")!.GetGetMethod()!)
			.Brfalse_S(out Label @else)
				.Call(typeof(ReadOnlyMemory<char>).GetProperty("Span")!.GetGetMethod()!)
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

	public class SByteConverter : INativeConverter<SByte> {
		public void AppendToStringBuilder(StringBuilder stringBuilder, IFormatProvider formatProvider, SByte value, CsvColumnAttribute? attribute) {
			stringBuilder.Append(value);
		}

		public SByte Deserialize(ReadOnlyMemory<char> text, IFormatProvider provider, CsvColumnAttribute? attribute) {
			return SByte.Parse(text.Span, provider: provider);
		}

		public void EmitAppendToStringBuilder(ILGenerator gen, LocalBuilder? local, CsvColumnAttribute? attribute) => gen
			.Call<StringBuilder>("Append", typeof(SByte));

		public void EmitDeserialize(ILGenerator gen, LocalBuilder? local, CsvColumnAttribute? attribute) => gen
			.Call(typeof(ReadOnlyMemory<char>).GetProperty("Span")!.GetGetMethod()!)
			.Ldc_I4_X((int)NumberStyles.Integer)
			.Ldarg_1()
			.Call<SByte>("Parse", typeof(ReadOnlySpan<char>), typeof(NumberStyles), typeof(IFormatProvider));
	}

	public class NullableSByteConverter : INativeConverter<SByte?> {
		public void AppendToStringBuilder(StringBuilder stringBuilder, IFormatProvider formatProvider, SByte? value, CsvColumnAttribute? attribute) {
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

		public void EmitAppendToStringBuilder(ILGenerator gen, LocalBuilder? local, CsvColumnAttribute? attribute) => gen
			.Stloc(local!)
			.Ldloca(local!)
			.Callvirt(typeof(SByte?).GetProperty("HasValue")!.GetGetMethod()!)
			.Brfalse_S(out Label @endif)
				.Ldloca(local!)
				.Call(typeof(SByte?).GetProperty("Value")!.GetGetMethod()!)
				.Callvirt<StringBuilder>("Append", typeof(SByte))
			.Label(@endif);

		public void EmitDeserialize(ILGenerator gen, LocalBuilder? local, CsvColumnAttribute? attribute) => gen
			.Dup()
			.Call(typeof(ReadOnlyMemory<char>).GetProperty("Length")!.GetGetMethod()!)
			.Brfalse_S(out Label @else)
				.Call(typeof(ReadOnlyMemory<char>).GetProperty("Span")!.GetGetMethod()!)
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

	public class Int16Converter : INativeConverter<Int16> {
		public void AppendToStringBuilder(StringBuilder stringBuilder, IFormatProvider formatProvider, Int16 value, CsvColumnAttribute? attribute) {
			stringBuilder.Append(value);
		}

		public Int16 Deserialize(ReadOnlyMemory<char> text, IFormatProvider provider, CsvColumnAttribute? attribute) {
			return Int16.Parse(text.Span, provider: provider);
		}

		public void EmitAppendToStringBuilder(ILGenerator gen, LocalBuilder? local, CsvColumnAttribute? attribute) => gen
			.Call<StringBuilder>("Append", typeof(Int16));

		public void EmitDeserialize(ILGenerator gen, LocalBuilder? local, CsvColumnAttribute? attribute) => gen
			.Call(typeof(ReadOnlyMemory<char>).GetProperty("Span")!.GetGetMethod()!)
			.Ldc_I4_X((int)NumberStyles.Integer)
			.Ldarg_1()
			.Call<Int16>("Parse", typeof(ReadOnlySpan<char>), typeof(NumberStyles), typeof(IFormatProvider));
	}

	public class NullableInt16Converter : INativeConverter<Int16?> {
		public void AppendToStringBuilder(StringBuilder stringBuilder, IFormatProvider formatProvider, Int16? value, CsvColumnAttribute? attribute) {
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

		public void EmitAppendToStringBuilder(ILGenerator gen, LocalBuilder? local, CsvColumnAttribute? attribute) => gen
			.Stloc(local!)
			.Ldloca(local!)
			.Callvirt(typeof(Int16?).GetProperty("HasValue")!.GetGetMethod()!)
			.Brfalse_S(out Label @endif)
				.Ldloca(local!)
				.Call(typeof(Int16?).GetProperty("Value")!.GetGetMethod()!)
				.Callvirt<StringBuilder>("Append", typeof(Int16))
			.Label(@endif);

		public void EmitDeserialize(ILGenerator gen, LocalBuilder? local, CsvColumnAttribute? attribute) => gen
			.Dup()
			.Call(typeof(ReadOnlyMemory<char>).GetProperty("Length")!.GetGetMethod()!)
			.Brfalse_S(out Label @else)
				.Call(typeof(ReadOnlyMemory<char>).GetProperty("Span")!.GetGetMethod()!)
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

	public class UInt16Converter : INativeConverter<UInt16> {
		public void AppendToStringBuilder(StringBuilder stringBuilder, IFormatProvider formatProvider, UInt16 value, CsvColumnAttribute? attribute) {
			stringBuilder.Append(value);
		}

		public UInt16 Deserialize(ReadOnlyMemory<char> text, IFormatProvider provider, CsvColumnAttribute? attribute) {
			return UInt16.Parse(text.Span, provider: provider);
		}

		public void EmitAppendToStringBuilder(ILGenerator gen, LocalBuilder? local, CsvColumnAttribute? attribute) => gen
			.Call<StringBuilder>("Append", typeof(UInt16));

		public void EmitDeserialize(ILGenerator gen, LocalBuilder? local, CsvColumnAttribute? attribute) => gen
			.Call(typeof(ReadOnlyMemory<char>).GetProperty("Span")!.GetGetMethod()!)
			.Ldc_I4_X((int)NumberStyles.Integer)
			.Ldarg_1()
			.Call<UInt16>("Parse", typeof(ReadOnlySpan<char>), typeof(NumberStyles), typeof(IFormatProvider));
	}

	public class NullableUInt16Converter : INativeConverter<UInt16?> {
		public void AppendToStringBuilder(StringBuilder stringBuilder, IFormatProvider formatProvider, UInt16? value, CsvColumnAttribute? attribute) {
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

		public void EmitAppendToStringBuilder(ILGenerator gen, LocalBuilder? local, CsvColumnAttribute? attribute) => gen
			.Stloc(local!)
			.Ldloca(local!)
			.Callvirt(typeof(UInt16?).GetProperty("HasValue")!.GetGetMethod()!)
			.Brfalse_S(out Label @endif)
				.Ldloca(local!)
				.Call(typeof(UInt16?).GetProperty("Value")!.GetGetMethod()!)
				.Callvirt<StringBuilder>("Append", typeof(UInt16))
			.Label(@endif);

		public void EmitDeserialize(ILGenerator gen, LocalBuilder? local, CsvColumnAttribute? attribute) => gen
			.Dup()
			.Call(typeof(ReadOnlyMemory<char>).GetProperty("Length")!.GetGetMethod()!)
			.Brfalse_S(out Label @else)
				.Call(typeof(ReadOnlyMemory<char>).GetProperty("Span")!.GetGetMethod()!)
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

	public class Int32Converter : INativeConverter<Int32> {
		public void AppendToStringBuilder(StringBuilder stringBuilder, IFormatProvider formatProvider, Int32 value, CsvColumnAttribute? attribute) {
			stringBuilder.Append(value);
		}

		public Int32 Deserialize(ReadOnlyMemory<char> text, IFormatProvider provider, CsvColumnAttribute? attribute) {
			return Int32.Parse(text.Span, provider: provider);
		}

		public void EmitAppendToStringBuilder(ILGenerator gen, LocalBuilder? local, CsvColumnAttribute? attribute) => gen
			.Call<StringBuilder>("Append", typeof(Int32));

		public void EmitDeserialize(ILGenerator gen, LocalBuilder? local, CsvColumnAttribute? attribute) => gen
			.Call(typeof(ReadOnlyMemory<char>).GetProperty("Span")!.GetGetMethod()!)
			.Ldc_I4_X((int)NumberStyles.Integer)
			.Ldarg_1()
			.Call<Int32>("Parse", typeof(ReadOnlySpan<char>), typeof(NumberStyles), typeof(IFormatProvider));
	}

	public class NullableInt32Converter : INativeConverter<Int32?> {
		public void AppendToStringBuilder(StringBuilder stringBuilder, IFormatProvider formatProvider, Int32? value, CsvColumnAttribute? attribute) {
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

		public void EmitAppendToStringBuilder(ILGenerator gen, LocalBuilder? local, CsvColumnAttribute? attribute) => gen
			.Stloc(local!)
			.Ldloca(local!)
			.Callvirt(typeof(Int32?).GetProperty("HasValue")!.GetGetMethod()!)
			.Brfalse_S(out Label @endif)
				.Ldloca(local!)
				.Call(typeof(Int32?).GetProperty("Value")!.GetGetMethod()!)
				.Callvirt<StringBuilder>("Append", typeof(Int32))
			.Label(@endif);

		public void EmitDeserialize(ILGenerator gen, LocalBuilder? local, CsvColumnAttribute? attribute) => gen
			.Dup()
			.Call(typeof(ReadOnlyMemory<char>).GetProperty("Length")!.GetGetMethod()!)
			.Brfalse_S(out Label @else)
				.Call(typeof(ReadOnlyMemory<char>).GetProperty("Span")!.GetGetMethod()!)
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

	public class UInt32Converter : INativeConverter<UInt32> {
		public void AppendToStringBuilder(StringBuilder stringBuilder, IFormatProvider formatProvider, UInt32 value, CsvColumnAttribute? attribute) {
			stringBuilder.Append(value);
		}

		public UInt32 Deserialize(ReadOnlyMemory<char> text, IFormatProvider provider, CsvColumnAttribute? attribute) {
			return UInt32.Parse(text.Span, provider: provider);
		}

		public void EmitAppendToStringBuilder(ILGenerator gen, LocalBuilder? local, CsvColumnAttribute? attribute) => gen
			.Call<StringBuilder>("Append", typeof(UInt32));

		public void EmitDeserialize(ILGenerator gen, LocalBuilder? local, CsvColumnAttribute? attribute) => gen
			.Call(typeof(ReadOnlyMemory<char>).GetProperty("Span")!.GetGetMethod()!)
			.Ldc_I4_X((int)NumberStyles.Integer)
			.Ldarg_1()
			.Call<UInt32>("Parse", typeof(ReadOnlySpan<char>), typeof(NumberStyles), typeof(IFormatProvider));
	}

	public class NullableUInt32Converter : INativeConverter<UInt32?> {
		public void AppendToStringBuilder(StringBuilder stringBuilder, IFormatProvider formatProvider, UInt32? value, CsvColumnAttribute? attribute) {
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

		public void EmitAppendToStringBuilder(ILGenerator gen, LocalBuilder? local, CsvColumnAttribute? attribute) => gen
			.Stloc(local!)
			.Ldloca(local!)
			.Callvirt(typeof(UInt32?).GetProperty("HasValue")!.GetGetMethod()!)
			.Brfalse_S(out Label @endif)
				.Ldloca(local!)
				.Call(typeof(UInt32?).GetProperty("Value")!.GetGetMethod()!)
				.Callvirt<StringBuilder>("Append", typeof(UInt32))
			.Label(@endif);

		public void EmitDeserialize(ILGenerator gen, LocalBuilder? local, CsvColumnAttribute? attribute) => gen
			.Dup()
			.Call(typeof(ReadOnlyMemory<char>).GetProperty("Length")!.GetGetMethod()!)
			.Brfalse_S(out Label @else)
				.Call(typeof(ReadOnlyMemory<char>).GetProperty("Span")!.GetGetMethod()!)
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

	public class Int64Converter : INativeConverter<Int64> {
		public void AppendToStringBuilder(StringBuilder stringBuilder, IFormatProvider formatProvider, Int64 value, CsvColumnAttribute? attribute) {
			stringBuilder.Append(value);
		}

		public Int64 Deserialize(ReadOnlyMemory<char> text, IFormatProvider provider, CsvColumnAttribute? attribute) {
			return Int64.Parse(text.Span, provider: provider);
		}

		public void EmitAppendToStringBuilder(ILGenerator gen, LocalBuilder? local, CsvColumnAttribute? attribute) => gen
			.Call<StringBuilder>("Append", typeof(Int64));

		public void EmitDeserialize(ILGenerator gen, LocalBuilder? local, CsvColumnAttribute? attribute) => gen
			.Call(typeof(ReadOnlyMemory<char>).GetProperty("Span")!.GetGetMethod()!)
			.Ldc_I4_X((int)NumberStyles.Integer)
			.Ldarg_1()
			.Call<Int64>("Parse", typeof(ReadOnlySpan<char>), typeof(NumberStyles), typeof(IFormatProvider));
	}

	public class NullableInt64Converter : INativeConverter<Int64?> {
		public void AppendToStringBuilder(StringBuilder stringBuilder, IFormatProvider formatProvider, Int64? value, CsvColumnAttribute? attribute) {
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

		public void EmitAppendToStringBuilder(ILGenerator gen, LocalBuilder? local, CsvColumnAttribute? attribute) => gen
			.Stloc(local!)
			.Ldloca(local!)
			.Callvirt(typeof(Int64?).GetProperty("HasValue")!.GetGetMethod()!)
			.Brfalse_S(out Label @endif)
				.Ldloca(local!)
				.Call(typeof(Int64?).GetProperty("Value")!.GetGetMethod()!)
				.Callvirt<StringBuilder>("Append", typeof(Int64))
			.Label(@endif);

		public void EmitDeserialize(ILGenerator gen, LocalBuilder? local, CsvColumnAttribute? attribute) => gen
			.Dup()
			.Call(typeof(ReadOnlyMemory<char>).GetProperty("Length")!.GetGetMethod()!)
			.Brfalse_S(out Label @else)
				.Call(typeof(ReadOnlyMemory<char>).GetProperty("Span")!.GetGetMethod()!)
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

	public class UInt64Converter : INativeConverter<UInt64> {
		public void AppendToStringBuilder(StringBuilder stringBuilder, IFormatProvider formatProvider, UInt64 value, CsvColumnAttribute? attribute) {
			stringBuilder.Append(value);
		}

		public UInt64 Deserialize(ReadOnlyMemory<char> text, IFormatProvider provider, CsvColumnAttribute? attribute) {
			return UInt64.Parse(text.Span, provider: provider);
		}

		public void EmitAppendToStringBuilder(ILGenerator gen, LocalBuilder? local, CsvColumnAttribute? attribute) => gen
			.Call<StringBuilder>("Append", typeof(UInt64));

		public void EmitDeserialize(ILGenerator gen, LocalBuilder? local, CsvColumnAttribute? attribute) => gen
			.Call(typeof(ReadOnlyMemory<char>).GetProperty("Span")!.GetGetMethod()!)
			.Ldc_I4_X((int)NumberStyles.Integer)
			.Ldarg_1()
			.Call<UInt64>("Parse", typeof(ReadOnlySpan<char>), typeof(NumberStyles), typeof(IFormatProvider));
	}

	public class NullableUInt64Converter : INativeConverter<UInt64?> {
		public void AppendToStringBuilder(StringBuilder stringBuilder, IFormatProvider formatProvider, UInt64? value, CsvColumnAttribute? attribute) {
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

		public void EmitAppendToStringBuilder(ILGenerator gen, LocalBuilder? local, CsvColumnAttribute? attribute) => gen
			.Stloc(local!)
			.Ldloca(local!)
			.Callvirt(typeof(UInt64?).GetProperty("HasValue")!.GetGetMethod()!)
			.Brfalse_S(out Label @endif)
				.Ldloca(local!)
				.Call(typeof(UInt64?).GetProperty("Value")!.GetGetMethod()!)
				.Callvirt<StringBuilder>("Append", typeof(UInt64))
			.Label(@endif);

		public void EmitDeserialize(ILGenerator gen, LocalBuilder? local, CsvColumnAttribute? attribute) => gen
			.Dup()
			.Call(typeof(ReadOnlyMemory<char>).GetProperty("Length")!.GetGetMethod()!)
			.Brfalse_S(out Label @else)
				.Call(typeof(ReadOnlyMemory<char>).GetProperty("Span")!.GetGetMethod()!)
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

	public class SingleConverter : INativeConverter<Single> {
		public void AppendToStringBuilder(StringBuilder stringBuilder, IFormatProvider formatProvider, Single value, CsvColumnAttribute? attribute) {
			stringBuilder.Append(value);
		}

		public Single Deserialize(ReadOnlyMemory<char> text, IFormatProvider provider, CsvColumnAttribute? attribute) {
			return Single.Parse(text.Span, provider: provider);
		}

		public void EmitAppendToStringBuilder(ILGenerator gen, LocalBuilder? local, CsvColumnAttribute? attribute) => gen
			.Call<StringBuilder>("Append", typeof(Single));

		public void EmitDeserialize(ILGenerator gen, LocalBuilder? local, CsvColumnAttribute? attribute) => gen
			.Call(typeof(ReadOnlyMemory<char>).GetProperty("Span")!.GetGetMethod()!)
			.Ldc_I4_X((int)NumberStyles.Float)
			.Ldarg_1()
			.Call<Single>("Parse", typeof(ReadOnlySpan<char>), typeof(NumberStyles), typeof(IFormatProvider));
	}

	public class NullableSingleConverter : INativeConverter<Single?> {
		public void AppendToStringBuilder(StringBuilder stringBuilder, IFormatProvider formatProvider, Single? value, CsvColumnAttribute? attribute) {
			if (value.HasValue) {
				stringBuilder.Append(value.Value);
			}
		}

		public Single? Deserialize(ReadOnlyMemory<char> text, IFormatProvider provider, CsvColumnAttribute? attribute) {
			if (text.Length == 0) {
				return null;
			} else {
				return Single.Parse(text.Span, provider: provider);
			}
		}

		public void EmitAppendToStringBuilder(ILGenerator gen, LocalBuilder? local, CsvColumnAttribute? attribute) => gen
			.Stloc(local!)
			.Ldloca(local!)
			.Callvirt(typeof(Single?).GetProperty("HasValue")!.GetGetMethod()!)
			.Brfalse_S(out Label @endif)
				.Ldloca(local!)
				.Call(typeof(Single?).GetProperty("Value")!.GetGetMethod()!)
				.Callvirt<StringBuilder>("Append", typeof(Single))
			.Label(@endif);

		public void EmitDeserialize(ILGenerator gen, LocalBuilder? local, CsvColumnAttribute? attribute) => gen
			.Dup()
			.Call(typeof(ReadOnlyMemory<char>).GetProperty("Length")!.GetGetMethod()!)
			.Brfalse_S(out Label @else)
				.Call(typeof(ReadOnlyMemory<char>).GetProperty("Span")!.GetGetMethod()!)
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

	public class DoubleConverter : INativeConverter<Double> {
		public void AppendToStringBuilder(StringBuilder stringBuilder, IFormatProvider formatProvider, Double value, CsvColumnAttribute? attribute) {
			stringBuilder.Append(value);
		}

		public Double Deserialize(ReadOnlyMemory<char> text, IFormatProvider provider, CsvColumnAttribute? attribute) {
			return Double.Parse(text.Span, provider: provider);
		}

		public void EmitAppendToStringBuilder(ILGenerator gen, LocalBuilder? local, CsvColumnAttribute? attribute) => gen
			.Call<StringBuilder>("Append", typeof(Double));

		public void EmitDeserialize(ILGenerator gen, LocalBuilder? local, CsvColumnAttribute? attribute) => gen
			.Call(typeof(ReadOnlyMemory<char>).GetProperty("Span")!.GetGetMethod()!)
			.Ldc_I4_X((int)NumberStyles.Float)
			.Ldarg_1()
			.Call<Double>("Parse", typeof(ReadOnlySpan<char>), typeof(NumberStyles), typeof(IFormatProvider));
	}

	public class NullableDoubleConverter : INativeConverter<Double?> {
		public void AppendToStringBuilder(StringBuilder stringBuilder, IFormatProvider formatProvider, Double? value, CsvColumnAttribute? attribute) {
			if (value.HasValue) {
				stringBuilder.Append(value.Value);
			}
		}

		public Double? Deserialize(ReadOnlyMemory<char> text, IFormatProvider provider, CsvColumnAttribute? attribute) {
			if (text.Length == 0) {
				return null;
			} else {
				return Double.Parse(text.Span, provider: provider);
			}
		}

		public void EmitAppendToStringBuilder(ILGenerator gen, LocalBuilder? local, CsvColumnAttribute? attribute) => gen
			.Stloc(local!)
			.Ldloca(local!)
			.Callvirt(typeof(Double?).GetProperty("HasValue")!.GetGetMethod()!)
			.Brfalse_S(out Label @endif)
				.Ldloca(local!)
				.Call(typeof(Double?).GetProperty("Value")!.GetGetMethod()!)
				.Callvirt<StringBuilder>("Append", typeof(Double))
			.Label(@endif);

		public void EmitDeserialize(ILGenerator gen, LocalBuilder? local, CsvColumnAttribute? attribute) => gen
			.Dup()
			.Call(typeof(ReadOnlyMemory<char>).GetProperty("Length")!.GetGetMethod()!)
			.Brfalse_S(out Label @else)
				.Call(typeof(ReadOnlyMemory<char>).GetProperty("Span")!.GetGetMethod()!)
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

	public class DecimalConverter : INativeConverter<Decimal> {
		public void AppendToStringBuilder(StringBuilder stringBuilder, IFormatProvider formatProvider, Decimal value, CsvColumnAttribute? attribute) {
			stringBuilder.Append(value);
		}

		public Decimal Deserialize(ReadOnlyMemory<char> text, IFormatProvider provider, CsvColumnAttribute? attribute) {
			return Decimal.Parse(text.Span, provider: provider);
		}

		public void EmitAppendToStringBuilder(ILGenerator gen, LocalBuilder? local, CsvColumnAttribute? attribute) => gen
			.Call<StringBuilder>("Append", typeof(Decimal));

		public void EmitDeserialize(ILGenerator gen, LocalBuilder? local, CsvColumnAttribute? attribute) => gen
			.Call(typeof(ReadOnlyMemory<char>).GetProperty("Span")!.GetGetMethod()!)
			.Ldc_I4_X((int)NumberStyles.Currency)
			.Ldarg_1()
			.Call<Decimal>("Parse", typeof(ReadOnlySpan<char>), typeof(NumberStyles), typeof(IFormatProvider));
	}

	public class NullableDecimalConverter : INativeConverter<Decimal?> {
		public void AppendToStringBuilder(StringBuilder stringBuilder, IFormatProvider formatProvider, Decimal? value, CsvColumnAttribute? attribute) {
			if (value.HasValue) {
				stringBuilder.Append(value.Value);
			}
		}

		public Decimal? Deserialize(ReadOnlyMemory<char> text, IFormatProvider provider, CsvColumnAttribute? attribute) {
			if (text.Length == 0) {
				return null;
			} else {
				return Decimal.Parse(text.Span, provider: provider);
			}
		}

		public void EmitAppendToStringBuilder(ILGenerator gen, LocalBuilder? local, CsvColumnAttribute? attribute) => gen
			.Stloc(local!)
			.Ldloca(local!)
			.Callvirt(typeof(Decimal?).GetProperty("HasValue")!.GetGetMethod()!)
			.Brfalse_S(out Label @endif)
				.Ldloca(local!)
				.Call(typeof(Decimal?).GetProperty("Value")!.GetGetMethod()!)
				.Callvirt<StringBuilder>("Append", typeof(Decimal))
			.Label(@endif);

		public void EmitDeserialize(ILGenerator gen, LocalBuilder? local, CsvColumnAttribute? attribute) => gen
			.Dup()
			.Call(typeof(ReadOnlyMemory<char>).GetProperty("Length")!.GetGetMethod()!)
			.Brfalse_S(out Label @else)
				.Call(typeof(ReadOnlyMemory<char>).GetProperty("Span")!.GetGetMethod()!)
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