using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using Csv.Parser;

namespace Csv.Emitter {
	internal static class Methods {
		public static MethodInfo StringSplitter_SplitLine = typeof(StringSplitter).GetMethod(nameof(StringSplitter.SplitLine), new Type[] { typeof(string), typeof(char) })!;

		public static MethodInfo List_String_get_Count = typeof(List<string>).GetProperty(nameof(List<string>.Count), BindingFlags.Public | BindingFlags.Instance)!.GetGetMethod()!;
		public static MethodInfo List_String_get_Item = typeof(List<string>).GetProperties(BindingFlags.Public | BindingFlags.Instance).Single(prop => prop.GetIndexParameters().Length > 0).GetGetMethod()!;

		public static MethodInfo CultureInfo_get_InvariantCulture = typeof(CultureInfo).GetProperty(nameof(CultureInfo.InvariantCulture), BindingFlags.Public | BindingFlags.Static)!.GetGetMethod()!;

		public static MethodInfo Boolean_Parse = typeof(bool).GetMethod(nameof(bool.Parse), new Type[] { typeof(string) })!;
		public static MethodInfo Byte_Parse = typeof(byte).GetMethod(nameof(byte.Parse), new Type[] { typeof(string), typeof(IFormatProvider) })!;
		public static MethodInfo SByte_Parse = typeof(sbyte).GetMethod(nameof(sbyte.Parse), new Type[] { typeof(string), typeof(IFormatProvider) })!;
		public static MethodInfo Int16_Parse = typeof(short).GetMethod(nameof(short.Parse), new Type[] { typeof(string), typeof(IFormatProvider) })!;
		public static MethodInfo UInt16_Parse = typeof(ushort).GetMethod(nameof(ushort.Parse), new Type[] { typeof(string), typeof(IFormatProvider) })!;
		public static MethodInfo Int32_Parse = typeof(int).GetMethod(nameof(int.Parse), new Type[] { typeof(string), typeof(IFormatProvider) })!;
		public static MethodInfo UInt32_Parse = typeof(uint).GetMethod(nameof(uint.Parse), new Type[] { typeof(string), typeof(IFormatProvider) })!;
		public static MethodInfo Int64_Parse = typeof(long).GetMethod(nameof(long.Parse), new Type[] { typeof(string), typeof(IFormatProvider) })!;
		public static MethodInfo UInt64_Parse = typeof(ulong).GetMethod(nameof(ulong.Parse), new Type[] { typeof(string), typeof(IFormatProvider) })!;
		public static MethodInfo Single_Parse = typeof(float).GetMethod(nameof(float.Parse), new Type[] { typeof(string), typeof(IFormatProvider) })!;
		public static MethodInfo Double_Parse = typeof(double).GetMethod(nameof(double.Parse), new Type[] { typeof(string), typeof(IFormatProvider) })!;
		public static MethodInfo Decimal_Parse = typeof(decimal).GetMethod(nameof(decimal.Parse), new Type[] { typeof(string), typeof(IFormatProvider) })!;
		public static MethodInfo DateTime_Parse = typeof(DateTime).GetMethod(nameof(DateTime.Parse), new Type[] { typeof(string), typeof(IFormatProvider) })!;
		public static MethodInfo DateTime_ParseExact = typeof(DateTime).GetMethod(nameof(DateTime.ParseExact), new Type[] { typeof(string), typeof(string), typeof(IFormatProvider) })!;

		public static MethodInfo String_ToLowerInvariant = typeof(string).GetMethod(nameof(string.ToLowerInvariant), Type.EmptyTypes)!;
		public static MethodInfo String_Trim = typeof(string).GetMethod(nameof(string.Trim), Type.EmptyTypes)!;
		public static MethodInfo String_get_Length = typeof(string).GetProperty(nameof(string.Length), BindingFlags.Public | BindingFlags.Instance)!.GetGetMethod()!;
		public static MethodInfo String_Substring = typeof(string).GetMethod(nameof(string.Substring), new Type[] { typeof(int), typeof(int) })!;
		public static MethodInfo String_Replace = typeof(string).GetMethod(nameof(string.Replace), new Type[] { typeof(string), typeof(string) })!;

		public static ConstructorInfo FormatException_ctor = typeof(FormatException).GetConstructor(new Type[] { typeof(string) })!;

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

		public static MethodInfo Boolean_ToString = typeof(bool).GetMethod(nameof(bool.ToString), new Type[] { typeof(IFormatProvider) })!;
		public static MethodInfo Byte_ToString = typeof(byte).GetMethod(nameof(byte.ToString), new Type[] { typeof(IFormatProvider) })!;
		public static MethodInfo SByte_ToString = typeof(sbyte).GetMethod(nameof(sbyte.ToString), new Type[] { typeof(IFormatProvider) })!;
		public static MethodInfo Int16_ToString = typeof(short).GetMethod(nameof(short.ToString), new Type[] { typeof(IFormatProvider) })!;
		public static MethodInfo UInt16_ToString = typeof(ushort).GetMethod(nameof(ushort.ToString), new Type[] { typeof(IFormatProvider) })!;
		public static MethodInfo Int32_ToString = typeof(int).GetMethod(nameof(int.ToString), new Type[] { typeof(IFormatProvider) })!;
		public static MethodInfo UInt32_ToString = typeof(uint).GetMethod(nameof(uint.ToString), new Type[] { typeof(IFormatProvider) })!;
		public static MethodInfo Int64_ToString = typeof(long).GetMethod(nameof(long.ToString), new Type[] { typeof(IFormatProvider) })!;
		public static MethodInfo UInt64_ToString = typeof(ulong).GetMethod(nameof(ulong.ToString), new Type[] { typeof(IFormatProvider) })!;
		public static MethodInfo Single_ToString = typeof(float).GetMethod(nameof(float.ToString), new Type[] { typeof(IFormatProvider) })!;
		public static MethodInfo Double_ToString = typeof(double).GetMethod(nameof(double.ToString), new Type[] { typeof(IFormatProvider) })!;
		public static MethodInfo Decimal_ToString = typeof(decimal).GetMethod(nameof(decimal.ToString), new Type[] { typeof(IFormatProvider) })!;
		public static MethodInfo DateTime_ToString = typeof(DateTime).GetMethod(nameof(DateTime.ToString), new Type[] { typeof(IFormatProvider) })!;
		public static MethodInfo DateTime_ToString_Format = typeof(DateTime).GetMethod(nameof(DateTime.ToString), new Type[] { typeof(string), typeof(IFormatProvider) })!;
	}
}
