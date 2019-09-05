using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using Csv.Parser;

namespace Csv.Emitter {
	internal static class Methods {
		public static int IndexOf(ReadOnlySpan<char> span, char c) => span.IndexOf(c);

		public static MethodInfo StringSplitter_ReadNextLine = typeof(StringSplitter).GetMethod(nameof(StringSplitter.ReadNextLine), new Type[] { typeof(ReadOnlyMemory<char>).MakeByRefType(), typeof(char) })!;

		public static MethodInfo List_String_get_Count = typeof(List<string>).GetProperty(nameof(List<string>.Count), BindingFlags.Public | BindingFlags.Instance)!.GetGetMethod()!;
		public static MethodInfo List_String_get_Item = typeof(List<string>).GetProperties(BindingFlags.Public | BindingFlags.Instance).Single(prop => prop.GetIndexParameters().Length > 0).GetGetMethod()!;

		public static MethodInfo List_object_Add = typeof(List<object>).GetMethod(nameof(List<object>.Add), new Type[] { typeof(object) })!;

		public static MethodInfo ReadOnlySpan_Char_get_Length = typeof(ReadOnlySpan<char>).GetProperty(nameof(ReadOnlySpan<char>.Length), BindingFlags.Public | BindingFlags.Instance)!.GetGetMethod()!;
		public static MethodInfo ReadOnlySpan_Char_IndexOf = typeof(Methods).GetMethod(nameof(Methods.IndexOf), new Type[] { typeof(ReadOnlySpan<char>), typeof(char) })!;
		public static MethodInfo ReadOnlySpan_Char_ToString = typeof(ReadOnlySpan<char>).GetMethod(nameof(ReadOnlySpan<char>.ToString), Type.EmptyTypes)!;
		public static MethodInfo ReadOnlySpan_Char_Slice = typeof(ReadOnlySpan<char>).GetMethod(nameof(ReadOnlySpan<char>.Slice), new Type[] { typeof(int), typeof(int) })!;

		public static MethodInfo Boolean_TryParse = typeof(bool).GetMethod(nameof(bool.TryParse), new Type[] { typeof(string), typeof(bool).MakeByRefType() })!;
		public static MethodInfo Byte_TryParse = typeof(byte).GetMethod(nameof(byte.TryParse), new Type[] { typeof(string), typeof(byte).MakeByRefType() })!;
		public static MethodInfo SByte_TryParse = typeof(sbyte).GetMethod(nameof(sbyte.TryParse), new Type[] { typeof(string), typeof(sbyte).MakeByRefType() })!;
		public static MethodInfo Int16_TryParse = typeof(short).GetMethod(nameof(short.TryParse), new Type[] { typeof(string), typeof(short).MakeByRefType() })!;
		public static MethodInfo UInt16_TryParse = typeof(ushort).GetMethod(nameof(ushort.TryParse), new Type[] { typeof(string), typeof(ushort).MakeByRefType() })!;
		public static MethodInfo Int32_TryParse = typeof(int).GetMethod(nameof(int.TryParse), new Type[] { typeof(string), typeof(int).MakeByRefType() })!;
		public static MethodInfo UInt32_TryParse = typeof(uint).GetMethod(nameof(uint.TryParse), new Type[] { typeof(string), typeof(uint).MakeByRefType() })!;
		public static MethodInfo Int64_TryParse = typeof(long).GetMethod(nameof(long.TryParse), new Type[] { typeof(string), typeof(long).MakeByRefType() })!;
		public static MethodInfo UInt64_TryParse = typeof(ulong).GetMethod(nameof(ulong.TryParse), new Type[] { typeof(string), typeof(ulong).MakeByRefType() })!;
		public static MethodInfo Single_TryParse = typeof(float).GetMethod(nameof(float.TryParse), new Type[] { typeof(string), typeof(float).MakeByRefType() })!;
		public static MethodInfo Double_TryParse = typeof(double).GetMethod(nameof(double.TryParse), new Type[] { typeof(string), typeof(double).MakeByRefType() })!;
		public static MethodInfo Decimal_TryParse = typeof(decimal).GetMethod(nameof(decimal.TryParse), new Type[] { typeof(string), typeof(decimal).MakeByRefType() })!;
		public static MethodInfo DateTime_TryParse = typeof(DateTime).GetMethod(nameof(DateTime.TryParse), new Type[] { typeof(string), typeof(DateTime).MakeByRefType() })!;
		public static MethodInfo DateTime_TryParseExact = typeof(DateTime).GetMethod(nameof(DateTime.TryParseExact), new Type[] { typeof(string), typeof(string), typeof(IFormatProvider), typeof(DateTimeStyles), typeof(DateTime).MakeByRefType() })!;

		public static MethodInfo String_Trim = typeof(string).GetMethod(nameof(string.Trim), Type.EmptyTypes)!;
		public static MethodInfo String_get_Length = typeof(string).GetProperty(nameof(string.Length), BindingFlags.Public | BindingFlags.Instance)!.GetGetMethod()!;
		public static MethodInfo String_Substring = typeof(string).GetMethod(nameof(string.Substring), new Type[] { typeof(int), typeof(int) })!;
		public static MethodInfo String_Replace = typeof(string).GetMethod(nameof(string.Replace), new Type[] { typeof(string), typeof(string) })!;
		public static MethodInfo String_IsNullOrWhiteSpace = typeof(string).GetMethod(nameof(string.IsNullOrWhiteSpace), new Type[] { typeof(string) })!;
		public static MethodInfo String_StartsWith = typeof(string).GetMethod(nameof(string.StartsWith), new Type[] { typeof(char) })!;
		public static MethodInfo String_EndsWith = typeof(string).GetMethod(nameof(string.EndsWith), new Type[] { typeof(char) })!;

		public static ConstructorInfo CsvFormatException_ctor2 = typeof(CsvFormatException).GetConstructor(new Type[] { typeof(string), typeof(string) })!;
		public static ConstructorInfo CsvFormatException_ctor3 = typeof(CsvFormatException).GetConstructor(new Type[] { typeof(Type), typeof(string), typeof(string) })!;
		public static ConstructorInfo CsvFormatException_ctor4 = typeof(CsvFormatException).GetConstructor(new Type[] { typeof(Type), typeof(string), typeof(string), typeof(string) })!;

		public static MethodInfo StringBuilder_Append_Char = typeof(StringBuilder).GetMethod(nameof(StringBuilder.Append), new Type[] { typeof(char) })!;
		public static MethodInfo StringBuilder_Append_String = typeof(StringBuilder).GetMethod(nameof(StringBuilder.Append), new Type[] { typeof(string) })!;
		public static MethodInfo StringBuilder_Append_Boolean = typeof(StringBuilder).GetMethod(nameof(StringBuilder.Append), new Type[] { typeof(bool) })!;
		public static MethodInfo StringBuilder_Append_Byte = typeof(StringBuilder).GetMethod(nameof(StringBuilder.Append), new Type[] { typeof(byte) })!;
		public static MethodInfo StringBuilder_Append_SByte = typeof(StringBuilder).GetMethod(nameof(StringBuilder.Append), new Type[] { typeof(sbyte) })!;
		public static MethodInfo StringBuilder_Append_Int16 = typeof(StringBuilder).GetMethod(nameof(StringBuilder.Append), new Type[] { typeof(short) })!;
		public static MethodInfo StringBuilder_Append_UInt16 = typeof(StringBuilder).GetMethod(nameof(StringBuilder.Append), new Type[] { typeof(ushort) })!;
		public static MethodInfo StringBuilder_Append_Int32 = typeof(StringBuilder).GetMethod(nameof(StringBuilder.Append), new Type[] { typeof(int) })!;
		public static MethodInfo StringBuilder_Append_UInt32 = typeof(StringBuilder).GetMethod(nameof(StringBuilder.Append), new Type[] { typeof(uint) })!;
		public static MethodInfo StringBuilder_Append_Int64 = typeof(StringBuilder).GetMethod(nameof(StringBuilder.Append), new Type[] { typeof(long) })!;
		public static MethodInfo StringBuilder_Append_UInt64 = typeof(StringBuilder).GetMethod(nameof(StringBuilder.Append), new Type[] { typeof(ulong) })!;
		public static MethodInfo StringBuilder_Append_Single = typeof(StringBuilder).GetMethod(nameof(StringBuilder.Append), new Type[] { typeof(float) })!;
		public static MethodInfo StringBuilder_Append_Double = typeof(StringBuilder).GetMethod(nameof(StringBuilder.Append), new Type[] { typeof(double) })!;
		public static MethodInfo StringBuilder_Append_Decimal = typeof(StringBuilder).GetMethod(nameof(StringBuilder.Append), new Type[] { typeof(decimal) })!;
		public static MethodInfo StringBuilder_Append_Object = typeof(StringBuilder).GetMethod(nameof(StringBuilder.Append), new Type[] { typeof(object) })!;

		public static MethodInfo Boolean_ToString = typeof(bool).GetMethod(nameof(bool.ToString), Type.EmptyTypes)!;
		public static MethodInfo Byte_ToString = typeof(byte).GetMethod(nameof(byte.ToString), Type.EmptyTypes)!;
		public static MethodInfo SByte_ToString = typeof(sbyte).GetMethod(nameof(sbyte.ToString), Type.EmptyTypes)!;
		public static MethodInfo Int16_ToString = typeof(short).GetMethod(nameof(short.ToString), Type.EmptyTypes)!;
		public static MethodInfo UInt16_ToString = typeof(ushort).GetMethod(nameof(ushort.ToString), Type.EmptyTypes)!;
		public static MethodInfo Int32_ToString = typeof(int).GetMethod(nameof(int.ToString), Type.EmptyTypes)!;
		public static MethodInfo UInt32_ToString = typeof(uint).GetMethod(nameof(uint.ToString), Type.EmptyTypes)!;
		public static MethodInfo Int64_ToString = typeof(long).GetMethod(nameof(long.ToString), Type.EmptyTypes)!;
		public static MethodInfo UInt64_ToString = typeof(ulong).GetMethod(nameof(ulong.ToString), Type.EmptyTypes)!;
		public static MethodInfo Single_ToString = typeof(float).GetMethod(nameof(float.ToString), Type.EmptyTypes)!;
		public static MethodInfo Double_ToString = typeof(double).GetMethod(nameof(double.ToString), Type.EmptyTypes)!;
		public static MethodInfo Decimal_ToString = typeof(decimal).GetMethod(nameof(decimal.ToString), Type.EmptyTypes)!;
		public static MethodInfo DateTime_ToString = typeof(DateTime).GetMethod(nameof(DateTime.ToString), Type.EmptyTypes)!;
		public static MethodInfo DateTime_ToString_Format = typeof(DateTime).GetMethod(nameof(DateTime.ToString), new Type[] { typeof(string) })!;

		public static MethodInfo NullableDateTime_get_HasValue = typeof(DateTime?).GetProperty(nameof(Nullable<DateTime>.HasValue), BindingFlags.Public | BindingFlags.Instance)!.GetGetMethod()!;
		public static MethodInfo NullableDateTime_get_Value = typeof(DateTime?).GetProperty(nameof(Nullable<DateTime>.Value), BindingFlags.Public | BindingFlags.Instance)!.GetGetMethod()!;
	}
}
