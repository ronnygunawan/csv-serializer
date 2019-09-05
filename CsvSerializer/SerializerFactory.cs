﻿using Csv.Emitter;
using Csv.NaiveImpl;
using Missil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Csv {
	internal static class SerializerFactory {
		private static readonly Dictionary<Type, ISerializer> SERIALIZER_BY_TYPE = new Dictionary<Type, ISerializer>();
		private static readonly Dictionary<Type, IDeserializer> DESERIALIZER_BY_TYPE = new Dictionary<Type, IDeserializer>();

		public static ISerializer GetOrCreateSerializer<T>() where T : notnull {
			if (SERIALIZER_BY_TYPE.TryGetValue(typeof(T), out ISerializer? serializer)) return serializer;
			if (typeof(T).Name is string className
				&& (className.Length < 15
				|| className[0] != '<'
				|| className[1] != '>')
				&& typeof(T).IsPublic) {
				ImplEmitter<ISerializer> implEmitter = new ImplEmitter<ISerializer>($"Serializer{typeof(T).GUID.ToString("N")}");
				implEmitter.ImplementAction<StringBuilder, char>("SerializeHeader", gen => DefineSerializeHeader<T>(gen));
				implEmitter.ImplementAction<StringBuilder, object, char>("SerializeItem", gen => DefineSerializeItem<T>(gen));
				serializer = implEmitter.CreateInstance();
				SERIALIZER_BY_TYPE.Add(typeof(T), serializer);
			} else {
				serializer = new NaiveSerializer<T>();
			}
			return serializer;
		}

		private static void DefineSerializeHeader<T>(ILGenerator gen) {
			_ = gen.EmitFollowingLines()

				.Ldarg_1
				.Do(emitter => {
					bool firstProperty = true;
					foreach (PropertyInfo property in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)) {
						CsvColumnAttribute? columnAttribute = property.GetCustomAttribute<CsvColumnAttribute>();

						if (!firstProperty) {
							_ = emitter
								.Ldarg_2
								.Callvirt(Methods.StringBuilder_Append_Char);
						}

						_ = emitter
							.Ldstr("\"" + (columnAttribute?.Name ?? property.Name).Replace("\"", "\"\"") + "\"")
							.Callvirt(Methods.StringBuilder_Append_String);

						firstProperty = false;
					}
				})
				.Ldstr("\r\n")
				.Callvirt(Methods.StringBuilder_Append_String)
				.Pop
				.Ret;
		}

		private static void DefineSerializeItem<T>(ILGenerator gen) {
			PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
			_ = gen.EmitFollowingLines()

				.DeclareLocal(typeof(string), out LocalBuilder? @string)
				.DeclareLocal(typeof(DateTime), out LocalBuilder? @DateTime)
				.DeclareLocal(typeof(DateTime?), out LocalBuilder? @NullableDateTime)

				.Ldarg_1
				.Do(emitter => {
					bool firstProperty = true;
					foreach (PropertyInfo property in properties) {
						if (!property.CanRead) throw new CsvTypeException(typeof(T), property.Name, "Property doesn't have a public getter.");
						CsvColumnAttribute? columnAttribute = property.GetCustomAttribute<CsvColumnAttribute>();

						if (!firstProperty) {
							_ = emitter
								.Ldarg_3
								.Callvirt(Methods.StringBuilder_Append_Char);
						}
						_ = property.PropertyType switch
						{
							Type t => t switch
							{
								_ when t == typeof(bool) => emitter
									.Ldarg_2
									.Callx(property.GetGetMethod()!)
									.Callvirt(Methods.StringBuilder_Append_Boolean),
								_ when Nullable.GetUnderlyingType(t) == typeof(bool) => emitter
									.Ldarg_2
									.Callx(property.GetGetMethod()!)
									.Box(typeof(bool?))
									.Callvirt(Methods.StringBuilder_Append_Object),
								_ when t == typeof(byte) => emitter
									.Ldarg_2
									.Callx(property.GetGetMethod()!)
									.Callvirt(Methods.StringBuilder_Append_Byte),
								_ when Nullable.GetUnderlyingType(t) == typeof(byte) => emitter
									.Ldarg_2
									.Callx(property.GetGetMethod()!)
									.Box(typeof(byte?))
									.Callvirt(Methods.StringBuilder_Append_Object),
								_ when t == typeof(sbyte) => emitter
									.Ldarg_2
									.Callx(property.GetGetMethod()!)
									.Callvirt(Methods.StringBuilder_Append_SByte),
								_ when Nullable.GetUnderlyingType(t) == typeof(sbyte) => emitter
									.Ldarg_2
									.Callx(property.GetGetMethod()!)
									.Box(typeof(sbyte?))
									.Callvirt(Methods.StringBuilder_Append_Object),
								_ when t == typeof(short) => emitter
									.Ldarg_2
									.Callx(property.GetGetMethod()!)
									.Callvirt(Methods.StringBuilder_Append_Int16),
								_ when Nullable.GetUnderlyingType(t) == typeof(short) => emitter
									.Ldarg_2
									.Callx(property.GetGetMethod()!)
									.Box(typeof(short?))
									.Callvirt(Methods.StringBuilder_Append_Object),
								_ when t == typeof(ushort) => emitter
									.Ldarg_2
									.Callx(property.GetGetMethod()!)
									.Callvirt(Methods.StringBuilder_Append_UInt16),
								_ when Nullable.GetUnderlyingType(t) == typeof(ushort) => emitter
									.Ldarg_2
									.Callx(property.GetGetMethod()!)
									.Box(typeof(ushort?))
									.Callvirt(Methods.StringBuilder_Append_Object),
								_ when t == typeof(int) => emitter
									.Ldarg_2
									.Callx(property.GetGetMethod()!)
									.Callvirt(Methods.StringBuilder_Append_Int32),
								_ when Nullable.GetUnderlyingType(t) == typeof(int) => emitter
									.Ldarg_2
									.Callx(property.GetGetMethod()!)
									.Box(typeof(int?))
									.Callvirt(Methods.StringBuilder_Append_Object),
								_ when t == typeof(uint) => emitter
									.Ldarg_2
									.Callx(property.GetGetMethod()!)
									.Callvirt(Methods.StringBuilder_Append_UInt32),
								_ when Nullable.GetUnderlyingType(t) == typeof(uint) => emitter
									.Ldarg_2
									.Callx(property.GetGetMethod()!)
									.Box(typeof(uint?))
									.Callvirt(Methods.StringBuilder_Append_Object),
								_ when t == typeof(long) => emitter
									.Ldarg_2
									.Callx(property.GetGetMethod()!)
									.Callvirt(Methods.StringBuilder_Append_Int64),
								_ when Nullable.GetUnderlyingType(t) == typeof(long) => emitter
									.Ldarg_2
									.Callx(property.GetGetMethod()!)
									.Box(typeof(long?))
									.Callvirt(Methods.StringBuilder_Append_Object),
								_ when t == typeof(ulong) => emitter
									.Ldarg_2
									.Callx(property.GetGetMethod()!)
									.Callvirt(Methods.StringBuilder_Append_UInt64),
								_ when Nullable.GetUnderlyingType(t) == typeof(ulong) => emitter
									.Ldarg_2
									.Callx(property.GetGetMethod()!)
									.Box(typeof(ulong?))
									.Callvirt(Methods.StringBuilder_Append_Object),
								_ when t == typeof(float) => emitter
									.Ldarg_2
									.Callx(property.GetGetMethod()!)
									.Callvirt(Methods.StringBuilder_Append_Single),
								_ when Nullable.GetUnderlyingType(t) == typeof(float) => emitter
									.Ldarg_2
									.Callx(property.GetGetMethod()!)
									.Box(typeof(float?))
									.Callvirt(Methods.StringBuilder_Append_Object),
								_ when t == typeof(double) => emitter
									.Ldarg_2
									.Callx(property.GetGetMethod()!)
									.Callvirt(Methods.StringBuilder_Append_Double),
								_ when Nullable.GetUnderlyingType(t) == typeof(double) => emitter
									.Ldarg_2
									.Callx(property.GetGetMethod()!)
									.Box(typeof(double?))
									.Callvirt(Methods.StringBuilder_Append_Object),
								_ when t == typeof(decimal) => emitter
									.Ldarg_2
									.Callx(property.GetGetMethod()!)
									.Callvirt(Methods.StringBuilder_Append_Decimal),
								_ when Nullable.GetUnderlyingType(t) == typeof(decimal) => emitter
									.Ldarg_2
									.Callx(property.GetGetMethod()!)
									.Box(typeof(decimal?))
									.Callvirt(Methods.StringBuilder_Append_Object),
								_ when t == typeof(string) => emitter
									.Ldarg_2
									.Callx(property.GetGetMethod()!)
									.Stloc(@string!.LocalIndex)
									.Ldloc(@string!.LocalIndex)
									.Brfalse(out Label @endif)
									.Ldc_I4_S('"')
									.Callvirt(Methods.StringBuilder_Append_Char)
									.Ldloc(@string!.LocalIndex)
									.Ldstr("\"")
									.Ldstr("\"\"")
									.Callvirt(Methods.String_Replace)
									.Call(Methods.StringBuilder_Append_String)
									.Ldc_I4_S('"')
									.Callvirt(Methods.StringBuilder_Append_Char)
									.MarkLabel(@endif),
								_ when t == typeof(DateTime) => emitter
									.Ldc_I4_S('"')
									.Callvirt(Methods.StringBuilder_Append_Char)
									.Ldarg_2
									.Callx(property.GetGetMethod()!)
									.Stloc(@DateTime!.LocalIndex)
									.Ldloca_S(@DateTime!.LocalIndex)
									.Do(emitter => property.GetCustomAttribute<CsvColumnAttribute>()?.DateFormat switch {
										string dateFormat => emitter
											.Ldstr(dateFormat)
											.Callvirt(Methods.DateTime_ToString_Format),
										_ => emitter
											.Callvirt(Methods.DateTime_ToString)
									})
									.Callvirt(Methods.StringBuilder_Append_String)
									.Ldc_I4_S('"')
									.Callvirt(Methods.StringBuilder_Append_Char),
								_ when Nullable.GetUnderlyingType(t) == typeof(DateTime) => emitter
									.Ldarg_2
									.Callx(property.GetGetMethod()!)
									.Stloc(@NullableDateTime!.LocalIndex)
									.Ldloca_S(@NullableDateTime!.LocalIndex)
									.Callvirt(Methods.NullableDateTime_get_HasValue)
									.Brfalse(out Label @endif)
									.Ldc_I4_S('"')
									.Callvirt(Methods.StringBuilder_Append_Char)
									.Ldarg_2
									.Callx(property.GetGetMethod()!)
									.Stloc(@NullableDateTime!.LocalIndex)
									.Ldloca_S(@NullableDateTime!.LocalIndex)
									.Callvirt(Methods.NullableDateTime_get_Value)
									.Stloc(@DateTime!.LocalIndex)
									.Ldloca_S(@DateTime!.LocalIndex)
									.Do(emitter => property.GetCustomAttribute<CsvColumnAttribute>()?.DateFormat switch {
										string dateFormat => emitter
											.Ldstr(dateFormat)
											.Callvirt(Methods.DateTime_ToString_Format),
										_ => emitter
											.Callvirt(Methods.DateTime_ToString)
									})
									.Callvirt(Methods.StringBuilder_Append_String)
									.Ldc_I4_S('"')
									.Callvirt(Methods.StringBuilder_Append_Char)
									.MarkLabel(endif),
								_ => throw new CsvTypeException(t)
							}
						};
						firstProperty = false;
					}
				})
				.Ldstr("\r\n")
				.Callvirt(Methods.StringBuilder_Append_String)
				.Pop
				.Ret;
		}

		public static IDeserializer GetOrCreateDeserializer<T>() where T : notnull {
			if (DESERIALIZER_BY_TYPE.TryGetValue(typeof(T), out IDeserializer? deserializer)) return deserializer;
			if (typeof(T).Name is string className
				&& (className.Length < 15
				|| className[0] != '<'
				|| className[1] != '>')
				&& typeof(T).IsPublic) {
				ImplEmitter<IDeserializer> implEmitter = new ImplEmitter<IDeserializer>($"Deserializer{typeof(T).GUID.ToString("N")}");
				implEmitter.ImplementFunc<object, string, char>("DeserializeItem", gen => DefineDeserializeItem<T>(gen));
				deserializer = implEmitter.CreateInstance();
				DESERIALIZER_BY_TYPE.Add(typeof(T), deserializer);
			} else {
				deserializer = new NaiveDeserializer<T>();
			}
			return deserializer;
		}

		private static void DefineDeserializeItem<T>(ILGenerator gen) {
			PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
			ConstructorInfo? constructorInfo = typeof(T).GetConstructor(Type.EmptyTypes);
			bool isDefaultConstructor;
			if (constructorInfo != null) {
				isDefaultConstructor = true;
			} else {
				Type[] expectedConstructorSignature = properties.Select(prop => prop.PropertyType).ToArray();
				constructorInfo = typeof(T).GetConstructor(expectedConstructorSignature) ?? throw new CsvTypeException(typeof(T), "No suitable constructor found.");
				isDefaultConstructor = false;
			}

			// arg1: ReadOnlySpan<char> csv
			// arg2: char separator
			// arg3: bool skipHeader
			gen
				.DeclareLocal<bool>(out LocalBuilder firstRow)
				.DeclareLocal<List<object>>(out LocalBuilder items)
				.DeclareLocal<List<string>>(out LocalBuilder columns)
				.DeclareLocal<int>(out LocalBuilder endOfLine)
				.DeclareLocal<string>(out LocalBuilder excMessage)
				.DeclareLocal<T>(out LocalBuilder obj)
				.DeclareLocal<string>(out LocalBuilder col)
				.DeclareLocal<bool>(out LocalBuilder parseSuccess)
				.DeclareLocal<bool>(out LocalBuilder isEmptyString)
				.DeclareLocalIf<bool>(properties.Any(prop => prop.PropertyType == typeof(bool) || Nullable.GetUnderlyingType(prop.PropertyType) == typeof(bool)), out LocalBuilder? @bool)
				.DeclareLocalIf<bool?>(properties.Any(prop => Nullable.GetUnderlyingType(prop.PropertyType) == typeof(bool)), out LocalBuilder? nBool)
				.DeclareLocalIf<byte>(properties.Any(prop => prop.PropertyType == typeof(byte) || Nullable.GetUnderlyingType(prop.PropertyType) == typeof(byte)), out LocalBuilder? @byte)
				.DeclareLocalIf<byte?>(properties.Any(prop => Nullable.GetUnderlyingType(prop.PropertyType) == typeof(byte)), out LocalBuilder? nByte)
				.DeclareLocalIf<sbyte>(properties.Any(prop => prop.PropertyType == typeof(sbyte) || Nullable.GetUnderlyingType(prop.PropertyType) == typeof(sbyte)), out LocalBuilder? @sbyte)
				.DeclareLocalIf<sbyte?>(properties.Any(prop => Nullable.GetUnderlyingType(prop.PropertyType) == typeof(sbyte)), out LocalBuilder? nSbyte)
				.DeclareLocalIf<short>(properties.Any(prop => prop.PropertyType == typeof(short) || Nullable.GetUnderlyingType(prop.PropertyType) == typeof(short)), out LocalBuilder? @short)
				.DeclareLocalIf<short?>(properties.Any(prop => Nullable.GetUnderlyingType(prop.PropertyType) == typeof(short)), out LocalBuilder? nShort)
				.DeclareLocalIf<ushort>(properties.Any(prop => prop.PropertyType == typeof(ushort) || Nullable.GetUnderlyingType(prop.PropertyType) == typeof(ushort)), out LocalBuilder? @ushort)
				.DeclareLocalIf<ushort?>(properties.Any(prop => Nullable.GetUnderlyingType(prop.PropertyType) == typeof(ushort)), out LocalBuilder? nUshort)
				.DeclareLocalIf<int>(properties.Any(prop => prop.PropertyType == typeof(int) || Nullable.GetUnderlyingType(prop.PropertyType) == typeof(int)), out LocalBuilder? @int)
				.DeclareLocalIf<int?>(properties.Any(prop => Nullable.GetUnderlyingType(prop.PropertyType) == typeof(int)), out LocalBuilder? nInt)
				.DeclareLocalIf<uint>(properties.Any(prop => prop.PropertyType == typeof(uint) || Nullable.GetUnderlyingType(prop.PropertyType) == typeof(uint)), out LocalBuilder? @uint)
				.DeclareLocalIf<uint?>(properties.Any(prop => Nullable.GetUnderlyingType(prop.PropertyType) == typeof(uint)), out LocalBuilder? nUint)
				.DeclareLocalIf<long>(properties.Any(prop => prop.PropertyType == typeof(long) || Nullable.GetUnderlyingType(prop.PropertyType) == typeof(long)), out LocalBuilder? @long)
				.DeclareLocalIf<long?>(properties.Any(prop => Nullable.GetUnderlyingType(prop.PropertyType) == typeof(long)), out LocalBuilder? nLong)
				.DeclareLocalIf<ulong>(properties.Any(prop => prop.PropertyType == typeof(ulong) || Nullable.GetUnderlyingType(prop.PropertyType) == typeof(ulong)), out LocalBuilder? @ulong)
				.DeclareLocalIf<ulong?>(properties.Any(prop => Nullable.GetUnderlyingType(prop.PropertyType) == typeof(ulong)), out LocalBuilder? nUlong)
				.DeclareLocalIf<float>(properties.Any(prop => prop.PropertyType == typeof(float) || Nullable.GetUnderlyingType(prop.PropertyType) == typeof(float)), out LocalBuilder? @float)
				.DeclareLocalIf<float?>(properties.Any(prop => Nullable.GetUnderlyingType(prop.PropertyType) == typeof(float)), out LocalBuilder? nFloat)
				.DeclareLocalIf<double>(properties.Any(prop => prop.PropertyType == typeof(double) || Nullable.GetUnderlyingType(prop.PropertyType) == typeof(double)), out LocalBuilder? @double)
				.DeclareLocalIf<double?>(properties.Any(prop => Nullable.GetUnderlyingType(prop.PropertyType) == typeof(double)), out LocalBuilder? nDouble)
				.DeclareLocalIf<decimal>(properties.Any(prop => prop.PropertyType == typeof(decimal) || Nullable.GetUnderlyingType(prop.PropertyType) == typeof(decimal)), out LocalBuilder? @decimal)
				.DeclareLocalIf<decimal?>(properties.Any(prop => Nullable.GetUnderlyingType(prop.PropertyType) == typeof(decimal)), out LocalBuilder? nDecimal)
				.DeclareLocalIf<string>(properties.Any(prop => prop.PropertyType == typeof(string)), out LocalBuilder? @string)
				.DeclareLocalIf<DateTime>(properties.Any(prop => prop.PropertyType == typeof(DateTime)), out LocalBuilder? @DateTime)
				.DeclareLocalIf<DateTime?>(properties.Any(prop => Nullable.GetUnderlyingType(prop.PropertyType) == typeof(DateTime)), out LocalBuilder? nDateTime)
				.Newobj(typeof(List<object>).GetConstructor(Type.EmptyTypes)!)
				.Stloc(items)
				.Label(out Label beginLoop)
					.Ldarg_1() // csv
					.Callvirt(Methods.ReadOnlySpan_Char_get_Length)
					.Brfalse_S(out Label endLoop)
					.Ldarga_S(1)
					.Ldarg_2() // separator
					.Call(Methods.StringSplitter_ReadNextLine)
					.Stloc(columns)
					.Ldarg_3() // skipHeader
					.Brfalse_S(out Label noSkipHeader)
						.Ldloc(firstRow)
						.Brtrue_S(beginLoop)
						.Ldnull()
						.Stloc(firstRow)
					.Label(noSkipHeader)
					.Ldc_I4_X(properties.Length)
					.Ldloc(columns)
					.Callvirt(Methods.List_String_get_Count)
					.Ceq()
					.Brtrue_S(out Label validated)
						.Ldarg_1()
						.Ldc_I4_S((byte)'\n')
						.Call(Methods.ReadOnlySpan_Char_IndexOf)
						.Stloc(endOfLine)
						.Ldloc(endOfLine)
						.Ldc_I4_M1()
						.Ceq()
						.Brfalse_S(out Label splitSpan)
							.Ldarg_1() // csv
							.Callvirt(Methods.ReadOnlySpan_Char_ToString)
							.Stloc(excMessage)
							.Br_S(out Label throwExc)
						.Label(splitSpan)
							.Ldarg_1() // csv
							.Ldc_I4_0()
							.Ldloc(endOfLine)
							.Call(Methods.ReadOnlySpan_Char_Slice)
							.Callvirt(Methods.ReadOnlySpan_Char_ToString)
							.Stloc(excMessage)
						.Label(throwExc)
						.Ldtoken<T>()
						.Ldloc(excMessage)
						.Ldstr($"Row must consists of {properties.Length} columns.")
						.Initobj<CsvFormatException>()
						.Throw()
					.Label(validated)
					.Newobj(constructorInfo)
					.Stloc(obj)
					.Do(gen => {
						for (int i = 0; i < properties.Length; i++) {
							LocalBuilder? local = null;
							LocalBuilder? nullableLocal = null;
							MethodInfo? tryParse = null;
							string? formatExcMessage = null;
							gen
							.Ldloc(obj)
							.Ldc_I4_X(i)
							.Callvirt(Methods.List_String_get_Item)
							.Stloc_S(col)
							.Ldloc_S(col);
							if (typeof(T) == typeof(bool)) {
								(local, tryParse, formatExcMessage) = (@bool, Methods.Boolean_TryParse, "Input string was not in correct Boolean format.");
								goto EMIT_VALUE_PARSER;
							} else if (Nullable.GetUnderlyingType(typeof(T)) == typeof(bool)) {
								(local, nullableLocal, tryParse, formatExcMessage) = (@bool, nBool, Methods.Boolean_TryParse, "Input string was not in correct Boolean format.");
								goto EMIT_NULLABLE_VALUE_PARSER;
							} else if (typeof(T) == typeof(byte)) {
								(local, tryParse, formatExcMessage) = (@byte, Methods.Byte_TryParse, "Input string was not in correct byte format.");
								goto EMIT_VALUE_PARSER;
							} else if (Nullable.GetUnderlyingType(typeof(T)) == typeof(byte)) {
								(local, nullableLocal, tryParse, formatExcMessage) = (@byte, nByte, Methods.Byte_TryParse, "Input string was not in correct byte format.");
								goto EMIT_NULLABLE_VALUE_PARSER;
							} else if (typeof(T) == typeof(sbyte)) {
								(local, tryParse, formatExcMessage) = (@sbyte, Methods.SByte_TryParse, "Input string was not in correct sbyte format.");
								goto EMIT_VALUE_PARSER;
							} else if (Nullable.GetUnderlyingType(typeof(T)) == typeof(sbyte)) {
								(local, nullableLocal, tryParse, formatExcMessage) = (@sbyte, nSbyte, Methods.SByte_TryParse, "Input string was not in correct sbyte format.");
								goto EMIT_NULLABLE_VALUE_PARSER;
							} else if (typeof(T) == typeof(short)) {
								(local, tryParse, formatExcMessage) = (@short, Methods.Int16_TryParse, "Input string was not in correct Int16 format.");
								goto EMIT_VALUE_PARSER;
							} else if (Nullable.GetUnderlyingType(typeof(T)) == typeof(short)) {
								(local, nullableLocal, tryParse, formatExcMessage) = (@short, nShort, Methods.Int16_TryParse, "Input string was not in correct Int16 format.");
								goto EMIT_NULLABLE_VALUE_PARSER;
							} else if (typeof(T) == typeof(ushort)) {
								(local, tryParse, formatExcMessage) = (@ushort, Methods.UInt16_TryParse, "Input string was not in correct UInt16 format.");
								goto EMIT_VALUE_PARSER;
							} else if (Nullable.GetUnderlyingType(typeof(T)) == typeof(ushort)) {
								(local, nullableLocal, tryParse, formatExcMessage) = (@ushort, nUshort, Methods.UInt16_TryParse, "Input string was not in correct UInt16 format.");
								goto EMIT_NULLABLE_VALUE_PARSER;
							} else if (typeof(T) == typeof(int)) {
								(local, tryParse, formatExcMessage) = (@int, Methods.Int32_TryParse, "Input string was not in correct Int32 format.");
								goto EMIT_VALUE_PARSER;
							} else if (Nullable.GetUnderlyingType(typeof(T)) == typeof(int)) {
								(local, nullableLocal, tryParse, formatExcMessage) = (@int, nInt, Methods.Int32_TryParse, "Input string was not in correct Int32 format.");
								goto EMIT_NULLABLE_VALUE_PARSER;
							} else if (typeof(T) == typeof(uint)) {
								(local, tryParse, formatExcMessage) = (@uint, Methods.UInt32_TryParse, "Input string was not in correct UInt32 format.");
								goto EMIT_VALUE_PARSER;
							} else if (Nullable.GetUnderlyingType(typeof(T)) == typeof(uint)) {
								(local, nullableLocal, tryParse, formatExcMessage) = (@uint, nUint, Methods.UInt32_TryParse, "Input string was not in correct UInt32 format.");
								goto EMIT_NULLABLE_VALUE_PARSER;
							} else if (typeof(T) == typeof(long)) {
								(local, tryParse, formatExcMessage) = (@long, Methods.Int64_TryParse, "Input string was not in correct Int64 format.");
								goto EMIT_VALUE_PARSER;
							} else if (Nullable.GetUnderlyingType(typeof(T)) == typeof(long)) {
								(local, nullableLocal, tryParse, formatExcMessage) = (@long, nLong, Methods.Int64_TryParse, "Input string was not in correct Int64 format.");
								goto EMIT_NULLABLE_VALUE_PARSER;
							} else if (typeof(T) == typeof(ulong)) {
								(local, tryParse, formatExcMessage) = (@ulong, Methods.UInt64_TryParse, "Input string was not in correct UInt64 format.");
								goto EMIT_VALUE_PARSER;
							} else if (Nullable.GetUnderlyingType(typeof(T)) == typeof(ulong)) {
								(local, nullableLocal, tryParse, formatExcMessage) = (@ulong, nUlong, Methods.UInt64_TryParse, "Input string was not in correct UInt64 format.");
								goto EMIT_NULLABLE_VALUE_PARSER;
							} else if (typeof(T) == typeof(float)) {
								(local, tryParse, formatExcMessage) = (@float, Methods.Single_TryParse, "Input string was not in correct floating point format.");
								goto EMIT_VALUE_PARSER;
							} else if (Nullable.GetUnderlyingType(typeof(T)) == typeof(float)) {
								(local, nullableLocal, tryParse, formatExcMessage) = (@float, nFloat, Methods.Single_TryParse, "Input string was not in correct floating point format.");
								goto EMIT_NULLABLE_VALUE_PARSER;
							} else if (typeof(T) == typeof(double)) {
								(local, tryParse, formatExcMessage) = (@double, Methods.Double_TryParse, "Input string was not in correct floating point format.");
								goto EMIT_VALUE_PARSER;
							} else if (Nullable.GetUnderlyingType(typeof(T)) == typeof(double)) {
								(local, nullableLocal, tryParse, formatExcMessage) = (@double, nDouble, Methods.Double_TryParse, "Input string was not in correct floating point format.");
								goto EMIT_NULLABLE_VALUE_PARSER;
							} else if (typeof(T) == typeof(decimal)) {
								(local, tryParse, formatExcMessage) = (@decimal, Methods.UInt16_TryParse, "Input string was not in correct decimal format.");
								goto EMIT_VALUE_PARSER;
							} else if (Nullable.GetUnderlyingType(typeof(T)) == typeof(decimal)) {
								(local, nullableLocal, tryParse, formatExcMessage) = (@decimal, nDecimal, Methods.UInt16_TryParse, "Input string was not in correct decimal format.");
								goto EMIT_NULLABLE_VALUE_PARSER;
							} else if (typeof(T) == typeof(string)) {
								gen
								.Callvirt(Methods.String_Trim)
								.Stloc((byte)@string!.LocalIndex)
								.Ldloc((byte)@string!.LocalIndex)
								.Ldc_I4_S((int)'"')
								.Callvirt(Methods.String_StartsWith)
								.Brfalse_S(out Label notString)
									.Ldloc((byte)@string!.LocalIndex)
									.Ldc_I4_S((int)'"')
									.Callvirt(Methods.String_EndsWith)
								.Brfalse_S(notString)
									.Ldloc((byte)@string!.LocalIndex)
									.Ldc_I4_1()
									.Ldloc((byte)@string!.LocalIndex)
									.Callvirt(Methods.String_get_Length)
									.Ldc_I4_2()
									.Sub()
									.Callvirt(Methods.String_Substring)
									.Ldstr("\"\"")
									.Ldstr("\"")
									.Callvirt(Methods.String_Replace)
									.Br_S(out Label end)
								.Label(notString)
									.Ldloc((byte)@string!.LocalIndex)
									.Call(Methods.String_IsNullOrWhiteSpace)
									.Brfalse_S(out Label invalidString)
									.Ldnull()
									.Br_S(end)
								.Label(invalidString)
									.Ldtoken<T>()
									.Ldstr(properties[i].Name)
									.Ldloc_S(col)
									.Newobj(Methods.CsvFormatException_ctor3)
									.Throw()
								.Label(end);
							} else if (typeof(T) == typeof(DateTime)) {

							} else if (Nullable.GetUnderlyingType(typeof(T)) == typeof(DateTime)) {

							} else {
								throw new CsvPropertyTypeException(typeof(T));
							}
						EMIT_VALUE_PARSER: {
								gen
								.Ldloca_S((byte)local!.LocalIndex)
								.Call(tryParse!)
								.Stloc_S((byte)parseSuccess.LocalIndex)
								.Ldloc_S((byte)parseSuccess.LocalIndex)
								.Brfalse_S(out Label @else)
									.Ldloc_S((byte)local!.LocalIndex)
									.Br_S(out Label @endif)
								.Label(@else)
									.Ldtoken<T>()
									.Ldstr(properties[i].Name)
									.Ldloc_S(col)
									.Ldstr(formatExcMessage!)
									.Newobj(Methods.CsvFormatException_ctor4)
									.Throw()
								.Label(@endif);
								continue;
							}
						EMIT_NULLABLE_VALUE_PARSER: {
								gen
								.Ldloca_S((byte)local!.LocalIndex)
								.Call(tryParse!)
								.Stloc_S((byte)parseSuccess.LocalIndex)
								.Ldloc_S((byte)parseSuccess.LocalIndex)
								.Brfalse_S(out Label @elseif)
									.Ldloc_S((byte)local!.LocalIndex)
									.Newobj(properties[i].PropertyType.GetConstructor(new Type[] { local!.LocalType })!)
									.Br_S(out Label @endif)
								.Label(@elseif)
								.Ldloc_S(col)
								.Call(Methods.String_IsNullOrWhiteSpace)
								.Stloc_S(isEmptyString)
								.Ldloc_S(isEmptyString)
								.Brfalse_S(out Label @else)
									.Ldloca_S((byte)nullableLocal!.LocalIndex)
									.Initobj(properties[i].PropertyType)
									.Ldloc_S((byte)nullableLocal!.LocalIndex)
									.Br_S(@endif)
								.Label(@else)
									.Ldtoken<T>()
									.Ldstr(properties[i].Name)
									.Ldloc_S(col)
									.Ldstr(formatExcMessage)
									.Newobj(Methods.CsvFormatException_ctor4)
									.Throw()
								.Label(@endif);
								continue;
							}
						}
					});


			//_ = gen.EmitFollowingLines()

			//	.DeclareLocal(typeof(List<string>))
			//	.DeclareLocal(typeof(string))
			//	.DeclareLocal(typeof(object))
			//	.DeclareLocal(typeof(T))
			//	.DeclareLocalIfRequired(properties, typeof(bool), out LocalBuilder? @bool, out LocalBuilder? @nBool)
			//	.DeclareLocalIfRequired(properties, typeof(byte), out LocalBuilder? @byte, out LocalBuilder? @nByte)
			//	.DeclareLocalIfRequired(properties, typeof(sbyte), out LocalBuilder? @sbyte, out LocalBuilder? @nSbyte)
			//	.DeclareLocalIfRequired(properties, typeof(short), out LocalBuilder? @short, out LocalBuilder? @nShort)
			//	.DeclareLocalIfRequired(properties, typeof(ushort), out LocalBuilder? @ushort, out LocalBuilder? @nUshort)
			//	.DeclareLocalIfRequired(properties, typeof(int), out LocalBuilder? @int, out LocalBuilder? @nInt)
			//	.DeclareLocalIfRequired(properties, typeof(uint), out LocalBuilder? @uint, out LocalBuilder? @nUint)
			//	.DeclareLocalIfRequired(properties, typeof(long), out LocalBuilder? @long, out LocalBuilder? @nLong)
			//	.DeclareLocalIfRequired(properties, typeof(ulong), out LocalBuilder? @ulong, out LocalBuilder? @nUlong)
			//	.DeclareLocalIfRequired(properties, typeof(float), out LocalBuilder? @float, out LocalBuilder? @nFloat)
			//	.DeclareLocalIfRequired(properties, typeof(double), out LocalBuilder? @double, out LocalBuilder? @nDouble)
			//	.DeclareLocalIfRequired(properties, typeof(decimal), out LocalBuilder? @decimal, out LocalBuilder? @nDecimal)
			//	.DeclareLocalIfRequired(properties, typeof(string), out LocalBuilder? @string)
			//	.DeclareLocalIfRequired(properties, typeof(DateTime), out LocalBuilder? @DateTime, out LocalBuilder? @nDateTime)
			//	.DeclareLocal(typeof(bool), out LocalBuilder? @parseSuccess)
			//	.DeclareLocal(typeof(bool), out LocalBuilder? @emptyString)

			//	.Ldarg_1
			//	.Ldarg_2
			//	.Call(Methods.StringSplitter_SplitLine)
			//	.Stloc_0
			//	.Ldloc_0
			//	.Callvirt(Methods.List_String_get_Count)
			//	.Ldc_I4(properties.Length)
			//	.Ceq
			//	.Brfalse(out Label @else)

			//	.Do(emitter => isDefaultConstructor switch {
			//		true => emitter
			//			.Newobj(constructorInfo)
			//			.Stloc_S(3),
			//		_ => emitter
			//	})

			//	.Do(emitter => {
			//		for (int i = 0; i < properties.Length; i++) {
			//			if (isDefaultConstructor) {
			//				_ = emitter
			//					.Ldloc_S(3);
			//			}
			//			_ = emitter
			//				.Ldloc_0
			//				.Ldc_I4(i)
			//				.Callvirt(Methods.List_String_get_Item)
			//				.Stloc_S(1)
			//				.Ldloc_S(1);
			//			ILBuilder emitParser(LocalBuilder? localBuilder, LocalBuilder? nullableLocalBuilder, MethodInfo tryParse, string formatExcMessage) {
			//				if (properties[i].PropertyType == localBuilder!.LocalType) {
			//					Label @else = emitter.DefineLabel();
			//					Label @endif = emitter.DefineLabel();
			//					return emitter
			//						.Ldloca_S(localBuilder!.LocalIndex)
			//						.Call(tryParse)
			//						.Stloc_S(parseSuccess!.LocalIndex)
			//						.Ldloc_S(parseSuccess!.LocalIndex)
			//						.Brfalse_S(@else)

			//						.Ldloc_S(localBuilder!.LocalIndex)
			//						.Br_S(@endif)

			//						.MarkLabel(@else)
			//						.Ldtoken(typeof(T))
			//						.Ldstr(properties[i].Name)
			//						.Ldloc_S(1)
			//						.Ldstr(formatExcMessage)
			//						.Newobj(Methods.CsvFormatException_ctor4)
			//						.Throw

			//						.MarkLabel(@endif);
			//				} else if (Nullable.GetUnderlyingType(properties[i].PropertyType) == localBuilder.LocalType) {
			//					Label @elseif = emitter.DefineLabel();
			//					Label @else = emitter.DefineLabel();
			//					Label @endif = emitter.DefineLabel();
			//					return emitter
			//						.Ldloca_S(localBuilder!.LocalIndex)
			//						.Call(tryParse)
			//						.Stloc_S(parseSuccess!.LocalIndex)
			//						.Ldloc_S(parseSuccess!.LocalIndex)
			//						.Brfalse_S(@elseif)

			//						.Ldloc_S(localBuilder!.LocalIndex)
			//						.Newobj(properties[i].PropertyType.GetConstructor(new Type[] { localBuilder!.LocalType })!)
			//						.Br_S(@endif)

			//						.MarkLabel(@elseif)
			//						.Ldloc_S(1)
			//						.Call(Methods.String_IsNullOrWhiteSpace)
			//						.Stloc_S(emptyString!.LocalIndex)
			//						.Ldloc_S(emptyString!.LocalIndex)
			//						.Brfalse_S(@else)

			//						.Ldloca_S(nullableLocalBuilder!.LocalIndex)
			//						.Initobj(properties[i].PropertyType)
			//						.Ldloc_S(nullableLocalBuilder!.LocalIndex)
			//						.Br_S(@endif)

			//						.MarkLabel(@else)
			//						.Ldtoken(typeof(T))
			//						.Ldstr(properties[i].Name)
			//						.Ldloc_S(1)
			//						.Ldstr(formatExcMessage)
			//						.Newobj(Methods.CsvFormatException_ctor4)
			//						.Throw

			//						.MarkLabel(@endif);
			//				} else throw new NotImplementedException();
			//			}
			//			_ = properties[i].PropertyType switch
			//			{
			//				Type t => t switch
			//				{
			//					_ when t == typeof(bool) =>
			//						emitParser(@bool, @nBool, Methods.Boolean_TryParse, "Input string was not in correct Boolean format."),
			//					_ when Nullable.GetUnderlyingType(t) == typeof(bool) =>
			//						emitParser(@bool, @nBool, Methods.Boolean_TryParse, "Input string was not in correct Boolean format."),
			//					_ when t == typeof(byte) =>
			//						emitParser(@byte, @nByte, Methods.Byte_TryParse, "Input string was not in correct byte format."),
			//					_ when Nullable.GetUnderlyingType(t) == typeof(byte) =>
			//						emitParser(@byte, @nByte, Methods.Byte_TryParse, "Input string was not in correct byte format."),
			//					_ when t == typeof(sbyte) =>
			//						emitParser(@sbyte, @nSbyte, Methods.SByte_TryParse, "Input string was not in correct sbyte format."),
			//					_ when Nullable.GetUnderlyingType(t) == typeof(sbyte) =>
			//						emitParser(@sbyte, @nSbyte, Methods.SByte_TryParse, "Input string was not in correct sbyte format."),
			//					_ when t == typeof(short) =>
			//						emitParser(@short, @nShort, Methods.Int16_TryParse, "Input string was not in correct Int16 format."),
			//					_ when Nullable.GetUnderlyingType(t) == typeof(short) =>
			//						emitParser(@short, @nShort, Methods.Int16_TryParse, "Input string was not in correct Int16 format."),
			//					_ when t == typeof(ushort) =>
			//						emitParser(@ushort, @nUshort, Methods.UInt16_TryParse, "Input string was not in correct UInt16 format."),
			//					_ when Nullable.GetUnderlyingType(t) == typeof(ushort) =>
			//						emitParser(@ushort, @nUshort, Methods.UInt16_TryParse, "Input string was not in correct UInt16 format."),
			//					_ when t == typeof(int) =>
			//						emitParser(@int, @nInt, Methods.Int32_TryParse, "Input string was not in correct Int32 format."),
			//					_ when Nullable.GetUnderlyingType(t) == typeof(int) =>
			//						emitParser(@int, @nInt, Methods.Int32_TryParse, "Input string was not in correct Int32 format."),
			//					_ when t == typeof(uint) =>
			//						emitParser(@uint, @nUint, Methods.UInt32_TryParse, "Input string was not in correct UInt32 format."),
			//					_ when Nullable.GetUnderlyingType(t) == typeof(uint) =>
			//						emitParser(@uint, @nUint, Methods.UInt32_TryParse, "Input string was not in correct UInt32 format."),
			//					_ when t == typeof(long) =>
			//						emitParser(@long, @nLong, Methods.Int64_TryParse, "Input string was not in correct Int64 format."),
			//					_ when Nullable.GetUnderlyingType(t) == typeof(long) =>
			//						emitParser(@long, @nLong, Methods.Int64_TryParse, "Input string was not in correct Int64 format."),
			//					_ when t == typeof(ulong) =>
			//						emitParser(@ulong, @nUlong, Methods.UInt64_TryParse, "Input string was not in correct UInt64 format."),
			//					_ when Nullable.GetUnderlyingType(t) == typeof(ulong) =>
			//						emitParser(@ulong, @nUlong, Methods.UInt64_TryParse, "Input string was not in correct UInt64 format."),
			//					_ when t == typeof(float) =>
			//						emitParser(@float, @nFloat, Methods.Single_TryParse, "Input string was not in correct floating point format."),
			//					_ when Nullable.GetUnderlyingType(t) == typeof(float) =>
			//						emitParser(@float, @nFloat, Methods.Single_TryParse, "Input string was not in correct floating point format."),
			//					_ when t == typeof(double) =>
			//						emitParser(@double, @nDouble, Methods.Double_TryParse, "Input string was not in correct floating point format."),
			//					_ when Nullable.GetUnderlyingType(t) == typeof(double) =>
			//						emitParser(@double, @nDouble, Methods.Double_TryParse, "Input string was not in correct floating point format."),
			//					_ when t == typeof(decimal) =>
			//						emitParser(@decimal, @nDecimal, Methods.Decimal_TryParse, "Input string was not in correct decimal format."),
			//					_ when Nullable.GetUnderlyingType(t) == typeof(decimal) =>
			//						emitParser(@decimal, @nDecimal, Methods.Decimal_TryParse, "Input string was not in correct decimal format."),
			//					_ when t == typeof(string) => emitter.Do(builder => {
			//						_ = builder
			//							.Callvirt(Methods.String_Trim)
			//							.Stloc(@string!.LocalIndex)
			//							.Ldloc(@string!.LocalIndex)
			//							.Ldc_I4_S('"')
			//							.Callvirt(Methods.String_StartsWith)
			//							.Brfalse_S(out Label @notString);
			//						_ = builder
			//							.Ldloc(@string!.LocalIndex)
			//							.Ldc_I4_S('"')
			//							.Callvirt(Methods.String_EndsWith)
			//							.Brfalse_S(notString)

			//							.Ldloc(@string!.LocalIndex)
			//							.Ldc_I4(1)
			//							.Ldloc(@string!.LocalIndex)
			//							.Callvirt(Methods.String_get_Length)
			//							.Ldc_I4(2)
			//							.Sub
			//							.Callvirt(Methods.String_Substring)
			//							.Ldstr("\"\"")
			//							.Ldstr("\"")
			//							.Callvirt(Methods.String_Replace)
			//							.Br_S(out Label @end);
			//						_ = builder
			//							.MarkLabel(@notString)
			//							.Ldloc(@string!.LocalIndex)
			//							.Call(Methods.String_IsNullOrWhiteSpace)
			//							.Brfalse_S(out Label @invalidString);
			//						_ = builder
			//							.Ldnull
			//							.Br_S(@end);
			//						_ = builder
			//							.MarkLabel(@invalidString)
			//							.Ldtoken(typeof(T))
			//							.Ldstr(properties[i].Name)
			//							.Ldloc_S(1)
			//							.Newobj(Methods.CsvFormatException_ctor3)
			//							.Throw

			//							.MarkLabel(@end);
			//					}),
			//					_ when t == typeof(DateTime) => emitter.Do(builder => {
			//						_ = builder
			//							.Callvirt(Methods.String_Trim)
			//							.Stloc(@string!.LocalIndex)
			//							.Ldloc(@string!.LocalIndex)
			//							.Ldc_I4_S('"')
			//							.Callvirt(Methods.String_StartsWith)
			//							.Brfalse_S(out Label @invalidString);
			//						_ = builder
			//							.Ldloc(@string!.LocalIndex)
			//							.Ldc_I4_S('"')
			//							.Callvirt(Methods.String_EndsWith)
			//							.Brfalse_S(@invalidString)

			//							.Ldloc(@string!.LocalIndex)
			//							.Ldc_I4(1)
			//							.Ldloc(@string!.LocalIndex)
			//							.Callvirt(Methods.String_get_Length)
			//							.Ldc_I4(2)
			//							.Sub
			//							.Callvirt(Methods.String_Substring)
			//							.Ldstr("\"\"")
			//							.Ldstr("\"")
			//							.Callvirt(Methods.String_Replace)
			//							.Stloc(@string!.LocalIndex)
			//							.Ldloc(@string!.LocalIndex);
			//						CsvColumnAttribute? columnAttribute = properties[i].GetCustomAttribute<CsvColumnAttribute>();
			//						Label @end = builder.DefineLabel();
			//						if (columnAttribute?.DateFormat is string dateFormat) {
			//							_ = builder
			//								.Ldstr(dateFormat)
			//								.Ldnull
			//								.Ldc_I4((int)DateTimeStyles.AssumeLocal)
			//								.Ldloca_S(@DateTime!.LocalIndex)
			//								.Call(Methods.DateTime_TryParseExact)
			//								.Brfalse_S(out Label @badFormat);
			//							_ = builder
			//								.Ldloc(@DateTime!.LocalIndex)
			//								.Br_S(@end);
			//							_ = builder
			//								.MarkLabel(badFormat)
			//								.Ldtoken(typeof(T))
			//								.Ldstr(properties[i].Name)
			//								.Ldloc_S(1)
			//								.Ldstr($"Input string was not in correct DateTime format. Expected format was '{dateFormat}'.")
			//								.Newobj(Methods.CsvFormatException_ctor4)
			//								.Throw;
			//						} else {
			//							_ = builder
			//								.Ldloca_S(@DateTime!.LocalIndex)
			//								.Call(Methods.DateTime_TryParse)
			//								.Brfalse_S(out Label @badFormat);
			//							_ = builder
			//								.Ldloc(@DateTime!.LocalIndex)
			//								.Br_S(@end);
			//							_ = builder
			//								.MarkLabel(badFormat)
			//								.Ldtoken(typeof(T))
			//								.Ldstr(properties[i].Name)
			//								.Ldloc_S(1)
			//								.Ldstr("Input string was not in correct DateTime format.")
			//								.Newobj(Methods.CsvFormatException_ctor4)
			//								.Throw;
			//						}
			//						_ = builder
			//							.MarkLabel(@invalidString)
			//							.Ldtoken(typeof(T))
			//							.Ldstr(properties[i].Name)
			//							.Ldloc_S(1)
			//							.Newobj(Methods.CsvFormatException_ctor3)
			//							.Throw

			//							.MarkLabel(@end);
			//					}),
			//					_ when Nullable.GetUnderlyingType(t) == typeof(DateTime) => emitter.Do(builder => {
			//						_ = builder
			//							.Callvirt(Methods.String_Trim)
			//							.Stloc(@string!.LocalIndex)
			//							.Ldloc(@string!.LocalIndex)
			//							.Ldc_I4_S('"')
			//							.Callvirt(Methods.String_StartsWith)
			//							.Brfalse_S(out Label @invalidString);
			//						_ = builder
			//							.Ldloc(@string!.LocalIndex)
			//							.Ldc_I4_S('"')
			//							.Callvirt(Methods.String_EndsWith)
			//							.Brfalse_S(@invalidString)

			//							.Ldloc(@string!.LocalIndex)
			//							.Ldc_I4(1)
			//							.Ldloc(@string!.LocalIndex)
			//							.Callvirt(Methods.String_get_Length)
			//							.Ldc_I4(2)
			//							.Sub
			//							.Callvirt(Methods.String_Substring)
			//							.Ldstr("\"\"")
			//							.Ldstr("\"")
			//							.Callvirt(Methods.String_Replace)
			//							.Stloc(@string!.LocalIndex)
			//							.Ldloc(@string!.LocalIndex);
			//						CsvColumnAttribute? columnAttribute = properties[i].GetCustomAttribute<CsvColumnAttribute>();
			//						Label @end = builder.DefineLabel();
			//						if (columnAttribute?.DateFormat is string dateFormat) {
			//							_ = builder
			//								.Ldstr(dateFormat)
			//								.Ldnull
			//								.Ldc_I4((int)DateTimeStyles.AssumeLocal)
			//								.Ldloca_S(@DateTime!.LocalIndex)
			//								.Call(Methods.DateTime_TryParseExact)
			//								.Brfalse_S(out Label @badFormat);
			//							_ = builder
			//								.Ldloc(@DateTime!.LocalIndex)
			//								.Newobj(typeof(DateTime?).GetConstructor(new Type[] { typeof(DateTime) })!)
			//								.Br_S(@end);
			//							_ = builder
			//								.MarkLabel(badFormat)
			//								.Ldtoken(typeof(T))
			//								.Ldstr(properties[i].Name)
			//								.Ldloc_S(1)
			//								.Ldstr($"Input string was not in correct DateTime format. Expected format was '{dateFormat}'.")
			//								.Newobj(Methods.CsvFormatException_ctor4)
			//								.Throw;
			//						} else {
			//							_ = builder
			//								.Ldloca_S(@DateTime!.LocalIndex)
			//								.Call(Methods.DateTime_TryParse)
			//								.Brfalse_S(out Label @badFormat);
			//							_ = builder
			//								.Ldloc(@DateTime!.LocalIndex)
			//								.Newobj(typeof(DateTime?).GetConstructor(new Type[] { typeof(DateTime) })!)
			//								.Br_S(@end);
			//							_ = builder
			//								.MarkLabel(badFormat)
			//								.Ldtoken(typeof(T))
			//								.Ldstr(properties[i].Name)
			//								.Ldloc_S(1)
			//								.Ldstr("Input string was not in correct DateTime format.")
			//								.Newobj(Methods.CsvFormatException_ctor4)
			//								.Throw;
			//						}
			//						_ = builder
			//							.MarkLabel(@invalidString)
			//							.Ldloc(@string!.LocalIndex)
			//							.Call(Methods.String_IsNullOrWhiteSpace)
			//							.Brfalse_S(out Label @invalidToken)
										
			//							.Ldloca_S(@nDateTime!.LocalIndex)
			//							.Initobj(typeof(DateTime?))
			//							.Ldloc_S(@nDateTime!.LocalIndex)
			//							.Br_S(@end);

			//						_ = builder
			//							.MarkLabel(invalidToken)
			//							.Ldtoken(typeof(T))
			//							.Ldstr(properties[i].Name)
			//							.Ldloc_S(1)
			//							.Newobj(Methods.CsvFormatException_ctor3)
			//							.Throw

			//							.MarkLabel(@end);
			//					}),
			//					_ => throw new CsvPropertyTypeException(t)
			//				}
			//			};
			//			if (isDefaultConstructor) {
			//				if (!properties[i].CanWrite) throw new CsvTypeException(typeof(T), properties[i].Name, $"Property doesn't have a setter.");
			//				_ = emitter
			//					.Callvirt(properties[i].GetSetMethod()!);
			//			}
			//		}
			//		if (!isDefaultConstructor) {
			//			_ = emitter
			//				.Newobj(constructorInfo)
			//				.Stloc_3;
			//		}
			//	})
			//	.Br(out Label @endif)

			//	.MarkLabel(@else)
			//	.Ldtoken(typeof(T))
			//	.Ldarg_1
			//	.Ldstr($"Row must consists of {properties.Length} columns.")
			//	.Newobj(Methods.CsvFormatException_ctor3)
			//	.Throw

			//	.MarkLabel(@endif)
			//	.Ldloc_3
			//	.Ret;
		}
	}
}
