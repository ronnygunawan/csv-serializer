using Csv.Emitter;
using Csv.NaiveImpl;
using Missil;
using System;
using System.Collections.Generic;
using System.Globalization;
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
								_ when t == typeof(Uri) => emitter
									.Ldarg_2
									.Callx(property.GetGetMethod()!)
									.Dup
									.Brfalse_S(out Label nullUri)
									.Callvirt(Methods.Uri_ToString)
									.MarkLabel(nullUri)
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
			//if (typeof(T).Name is string className
			//	&& (className.Length < 15
			//	|| className[0] != '<'
			//	|| className[1] != '>')
			//	&& typeof(T).IsPublic) {
			//	ImplEmitter<IDeserializer> implEmitter = new ImplEmitter<IDeserializer>($"Deserializer{typeof(T).GUID.ToString("N")}");
			//	implEmitter.ImplementFunc<List<object>, ReadOnlyMemory<char>, char, bool>("Deserialize", gen => DefineDeserialize<T>(gen));
			//	deserializer = implEmitter.CreateInstance();
			//	DESERIALIZER_BY_TYPE.Add(typeof(T), deserializer);
			//} else {
				deserializer = new NaiveDeserializer<T>();
			//}
			return deserializer;
		}

		private static void DefineDeserialize<T>(ILGenerator gen) {
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
					.Brfalse(out Label endLoop)
					.Ldarga_S(1)
					.Ldarg_2() // separator
					.Call(Methods.StringSplitter_ReadNextLine)
					.Stloc(columns)
					.Ldarg_3() // skipHeader
					.Brfalse_S(out Label noSkipHeader)
						.Ldloc(firstRow)
						.Brfalse_S(noSkipHeader)
						.Ldnull()
						.Stloc(firstRow)
						.Br_S(beginLoop)
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
					.Ldloc(obj)
					.Do(gen => {
						for (int i = 0; i < properties.Length; i++) {
							Type propertyType = properties[i].PropertyType;
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
							if (propertyType == typeof(bool)) {
								(local, tryParse, formatExcMessage) = (@bool, Methods.Boolean_TryParse, "Input string was not in correct Boolean format.");
								goto EMIT_VALUE_PARSER;
							} else if (Nullable.GetUnderlyingType(propertyType) == typeof(bool)) {
								(local, nullableLocal, tryParse, formatExcMessage) = (@bool, nBool, Methods.Boolean_TryParse, "Input string was not in correct Boolean format.");
								goto EMIT_NULLABLE_VALUE_PARSER;
							} else if (propertyType == typeof(byte)) {
								(local, tryParse, formatExcMessage) = (@byte, Methods.Byte_TryParse, "Input string was not in correct byte format.");
								goto EMIT_VALUE_PARSER;
							} else if (Nullable.GetUnderlyingType(propertyType) == typeof(byte)) {
								(local, nullableLocal, tryParse, formatExcMessage) = (@byte, nByte, Methods.Byte_TryParse, "Input string was not in correct byte format.");
								goto EMIT_NULLABLE_VALUE_PARSER;
							} else if (propertyType == typeof(sbyte)) {
								(local, tryParse, formatExcMessage) = (@sbyte, Methods.SByte_TryParse, "Input string was not in correct sbyte format.");
								goto EMIT_VALUE_PARSER;
							} else if (Nullable.GetUnderlyingType(propertyType) == typeof(sbyte)) {
								(local, nullableLocal, tryParse, formatExcMessage) = (@sbyte, nSbyte, Methods.SByte_TryParse, "Input string was not in correct sbyte format.");
								goto EMIT_NULLABLE_VALUE_PARSER;
							} else if (propertyType == typeof(short)) {
								(local, tryParse, formatExcMessage) = (@short, Methods.Int16_TryParse, "Input string was not in correct Int16 format.");
								goto EMIT_VALUE_PARSER;
							} else if (Nullable.GetUnderlyingType(propertyType) == typeof(short)) {
								(local, nullableLocal, tryParse, formatExcMessage) = (@short, nShort, Methods.Int16_TryParse, "Input string was not in correct Int16 format.");
								goto EMIT_NULLABLE_VALUE_PARSER;
							} else if (propertyType == typeof(ushort)) {
								(local, tryParse, formatExcMessage) = (@ushort, Methods.UInt16_TryParse, "Input string was not in correct UInt16 format.");
								goto EMIT_VALUE_PARSER;
							} else if (Nullable.GetUnderlyingType(propertyType) == typeof(ushort)) {
								(local, nullableLocal, tryParse, formatExcMessage) = (@ushort, nUshort, Methods.UInt16_TryParse, "Input string was not in correct UInt16 format.");
								goto EMIT_NULLABLE_VALUE_PARSER;
							} else if (propertyType == typeof(int)) {
								(local, tryParse, formatExcMessage) = (@int, Methods.Int32_TryParse, "Input string was not in correct Int32 format.");
								goto EMIT_VALUE_PARSER;
							} else if (Nullable.GetUnderlyingType(propertyType) == typeof(int)) {
								(local, nullableLocal, tryParse, formatExcMessage) = (@int, nInt, Methods.Int32_TryParse, "Input string was not in correct Int32 format.");
								goto EMIT_NULLABLE_VALUE_PARSER;
							} else if (propertyType == typeof(uint)) {
								(local, tryParse, formatExcMessage) = (@uint, Methods.UInt32_TryParse, "Input string was not in correct UInt32 format.");
								goto EMIT_VALUE_PARSER;
							} else if (Nullable.GetUnderlyingType(propertyType) == typeof(uint)) {
								(local, nullableLocal, tryParse, formatExcMessage) = (@uint, nUint, Methods.UInt32_TryParse, "Input string was not in correct UInt32 format.");
								goto EMIT_NULLABLE_VALUE_PARSER;
							} else if (propertyType == typeof(long)) {
								(local, tryParse, formatExcMessage) = (@long, Methods.Int64_TryParse, "Input string was not in correct Int64 format.");
								goto EMIT_VALUE_PARSER;
							} else if (Nullable.GetUnderlyingType(propertyType) == typeof(long)) {
								(local, nullableLocal, tryParse, formatExcMessage) = (@long, nLong, Methods.Int64_TryParse, "Input string was not in correct Int64 format.");
								goto EMIT_NULLABLE_VALUE_PARSER;
							} else if (propertyType == typeof(ulong)) {
								(local, tryParse, formatExcMessage) = (@ulong, Methods.UInt64_TryParse, "Input string was not in correct UInt64 format.");
								goto EMIT_VALUE_PARSER;
							} else if (Nullable.GetUnderlyingType(propertyType) == typeof(ulong)) {
								(local, nullableLocal, tryParse, formatExcMessage) = (@ulong, nUlong, Methods.UInt64_TryParse, "Input string was not in correct UInt64 format.");
								goto EMIT_NULLABLE_VALUE_PARSER;
							} else if (propertyType == typeof(float)) {
								(local, tryParse, formatExcMessage) = (@float, Methods.Single_TryParse, "Input string was not in correct floating point format.");
								goto EMIT_VALUE_PARSER;
							} else if (Nullable.GetUnderlyingType(propertyType) == typeof(float)) {
								(local, nullableLocal, tryParse, formatExcMessage) = (@float, nFloat, Methods.Single_TryParse, "Input string was not in correct floating point format.");
								goto EMIT_NULLABLE_VALUE_PARSER;
							} else if (propertyType == typeof(double)) {
								(local, tryParse, formatExcMessage) = (@double, Methods.Double_TryParse, "Input string was not in correct floating point format.");
								goto EMIT_VALUE_PARSER;
							} else if (Nullable.GetUnderlyingType(propertyType) == typeof(double)) {
								(local, nullableLocal, tryParse, formatExcMessage) = (@double, nDouble, Methods.Double_TryParse, "Input string was not in correct floating point format.");
								goto EMIT_NULLABLE_VALUE_PARSER;
							} else if (propertyType == typeof(decimal)) {
								(local, tryParse, formatExcMessage) = (@decimal, Methods.UInt16_TryParse, "Input string was not in correct decimal format.");
								goto EMIT_VALUE_PARSER;
							} else if (Nullable.GetUnderlyingType(propertyType) == typeof(decimal)) {
								(local, nullableLocal, tryParse, formatExcMessage) = (@decimal, nDecimal, Methods.UInt16_TryParse, "Input string was not in correct decimal format.");
								goto EMIT_NULLABLE_VALUE_PARSER;
							} else if (propertyType == typeof(string)) {
								gen
								.Callvirt(Methods.String_Trim)
								.Stloc(@string!)
								.Ldloc(@string!)
								.Ldc_I4_S((int)'"')
								.Callvirt(Methods.String_StartsWith)
								.Brfalse_S(out Label notString)
									.Ldloc(@string!)
									.Ldc_I4_S((int)'"')
									.Callvirt(Methods.String_EndsWith)
								.Brfalse_S(notString)
									.Ldloc(@string!)
									.Ldc_I4_1()
									.Ldloc(@string!)
									.Callvirt(Methods.String_get_Length)
									.Ldc_I4_2()
									.Sub()
									.Callvirt(Methods.String_Substring)
									.Ldstr("\"\"")
									.Ldstr("\"")
									.Callvirt(Methods.String_Replace)
									.Br_S(out Label end)
								.Label(notString)
									.Ldloc(@string!)
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
								.Label(end)
								.Callvirt(properties[i].GetSetMethod()!);
								continue;
							} else if (propertyType == typeof(DateTime)) {
								Label end = default;
								gen
								.Callvirt(Methods.String_Trim)
								.Stloc(@string!)
								.Ldloc(@string!)
								.Ldc_I4_S((byte)'"')
								.Callvirt(Methods.String_StartsWith)
								.Brfalse_S(out Label invalidString)
								.Ldloc(@string!)
								.Ldc_I4_S((byte)'"')
								.Callvirt(Methods.String_EndsWith)
								.Brfalse_S(invalidString)
								.Ldloc(@string!)
								.Ldc_I4_1()
								.Ldloc(@string!)
								.Callvirt(Methods.String_get_Length)
								.Ldc_I4_2()
								.Sub()
								.Callvirt(Methods.String_Substring)
								.Ldstr("\"\"")
								.Ldstr("\"")
								.Callvirt(Methods.String_Replace)
								.Stloc(@string!)
								.Ldloc(@string!)
								.Do(gen => {
									CsvColumnAttribute? columnAttribute = properties[i].GetCustomAttribute<CsvColumnAttribute>();
									if (columnAttribute?.DateFormat is string dateFormat) {
										gen
										.Ldstr(dateFormat)
										.Ldnull()
										.Ldc_I4_X((int)DateTimeStyles.AssumeLocal)
										.Ldloca_S(DateTime!)
										.Call(Methods.DateTime_TryParseExact)
										.Brfalse_S(out Label badFormat)
											.Ldloc(DateTime!)
											.Br_S(out end)
										.Label(badFormat)
										.Ldtoken<T>()
										.Ldstr(properties[i].Name)
										.Ldloc_S(1)
										.Ldstr($"Input string was not in correct DateTime format. Expected format was '{dateFormat}'.")
										.Newobj(Methods.CsvFormatException_ctor4)
										.Throw();
									} else {
										gen
										.Ldloca_S(DateTime!)
										.Call(Methods.DateTime_TryParse)
										.Brfalse_S(out Label badFormat)
											.Ldloc(DateTime!)
											.Br_S(out end)
										.Label(badFormat)
										.Ldtoken<T>()
										.Ldstr(properties[i].Name)
										.Ldloc_S(1)
										.Ldstr($"Input string was not in correct DateTime format.")
										.Newobj(Methods.CsvFormatException_ctor4)
										.Throw();
									}
								})
								.Label(invalidString)
								.Ldtoken<T>()
								.Ldstr(properties[i].Name)
								.Ldloc_S(1)
								.Newobj(Methods.CsvFormatException_ctor3)
								.Throw()
								.Label(end)
								.Callvirt(properties[i].GetSetMethod()!);
								continue;
							} else if (Nullable.GetUnderlyingType(propertyType) == typeof(DateTime)) {
								Label end = default;
								gen
								.Callvirt(Methods.String_Trim)
								.Stloc(@string!)
								.Ldloc(@string!)
								.Ldc_I4_S((byte)'"')
								.Callvirt(Methods.String_StartsWith)
								.Brfalse_S(out Label invalidString)
								.Ldloc(@string!)
								.Ldc_I4_S((byte)'"')
								.Callvirt(Methods.String_EndsWith)
								.Brfalse_S(invalidString)
								.Ldloc(@string!)
								.Ldc_I4_S((byte)'"')
								.Callvirt(Methods.String_EndsWith)
								.Brfalse_S(invalidString)
								.Ldloc(@string!)
								.Ldc_I4_1()
								.Ldloc(@string!)
								.Callvirt(Methods.String_get_Length)
								.Ldc_I4_2()
								.Sub()
								.Callvirt(Methods.String_Substring)
								.Ldstr("\"\"")
								.Ldstr("\"")
								.Callvirt(Methods.String_Replace)
								.Stloc(@string!)
								.Ldloc(@string!)
								.Do(gen => {
									CsvColumnAttribute? columnAttribute = properties[i].GetCustomAttribute<CsvColumnAttribute>();
									if (columnAttribute?.DateFormat is string dateFormat) {
										gen
										.Ldstr(dateFormat)
										.Ldnull()
										.Ldc_I4_X((int)DateTimeStyles.AssumeLocal)
										.Ldloca_S(DateTime!)
										.Call(Methods.DateTime_TryParseExact)
										.Brfalse_S(out Label badFormat)
											.Ldloc(DateTime!)
											.Br_S(out end)
										.Label(badFormat)
										.Ldtoken<T>()
										.Ldstr(properties[i].Name)
										.Ldloc_S(col)
										.Ldstr($"Input string was not in correct DateTime format. Expected format was '{dateFormat}'.")
										.Newobj(Methods.CsvFormatException_ctor4)
										.Throw();
									} else {
										gen
										.Ldloca_S(DateTime!)
										.Call(Methods.DateTime_TryParse)
										.Brfalse_S(out Label badFormat)
											.Ldloc(DateTime!)
											.Br_S(out end)
										.Label(badFormat)
										.Ldtoken<T>()
										.Ldstr(properties[i].Name)
										.Ldloc_S(col)
										.Ldstr($"Input string was not in correct DateTime format.")
										.Newobj(Methods.CsvFormatException_ctor4)
										.Throw();
									}
								})
								.Label(invalidString)
								.Ldloc(@string!)
								.Call(Methods.String_IsNullOrWhiteSpace)
								.Brfalse_S(out Label invalidToken)
									.Ldloca_S(nDateTime!)
									.Initobj<DateTime?>()
									.Ldloc_S(nDateTime!)
									.Br_S(end)
								.Label(invalidToken)
									.Ldtoken<T>()
									.Ldstr(properties[i].Name)
									.Ldloc_S(col)
									.Newobj(Methods.CsvFormatException_ctor3)
									.Throw()
								.Label(end)
								.Callvirt(properties[i].GetSetMethod()!);
								continue;
							} else {
								throw new CsvPropertyTypeException(properties[i].PropertyType);
							}
						EMIT_VALUE_PARSER:
							{
								gen
								.Ldloca_S(local!)
								.Call(tryParse!)
								.Stloc_S(parseSuccess)
								.Ldloc_S(parseSuccess)
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
								.Label(@endif)
								.Callvirt(properties[i].GetSetMethod()!);
								continue;
							}
						EMIT_NULLABLE_VALUE_PARSER:
							{
								gen
								.Ldloca_S(local!)
								.Call(tryParse!)
								.Stloc_S(parseSuccess)
								.Ldloc_S(parseSuccess)
								.Brfalse_S(out Label @elseif)
									.Ldloc_S(local!)
									.Newobj(properties[i].PropertyType.GetConstructor(new Type[] { local!.LocalType })!)
									.Br_S(out Label @endif)
								.Label(@elseif)
								.Ldloc_S(col)
								.Call(Methods.String_IsNullOrWhiteSpace)
								.Stloc_S(isEmptyString)
								.Ldloc_S(isEmptyString)
								.Brfalse_S(out Label @else)
									.Ldloca_S(nullableLocal!)
									.Initobj(properties[i].PropertyType)
									.Ldloc_S(nullableLocal!)
									.Br_S(@endif)
								.Label(@else)
									.Ldtoken<T>()
									.Ldstr(properties[i].Name)
									.Ldloc_S(col)
									.Ldstr(formatExcMessage)
									.Newobj(Methods.CsvFormatException_ctor4)
									.Throw()
								.Label(@endif)
								.Callvirt(properties[i].GetSetMethod()!);
								continue;
							}
						}
					})
					.Callvirt(Methods.List_object_Add)
					.Br(beginLoop)
				.Label(endLoop)
				.Ret();
		}
	}
}
