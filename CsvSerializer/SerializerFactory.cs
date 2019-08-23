using Csv.Emitter;
using Csv.NaiveImpl;
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
							.Ldc_I4_S('"')
							.Callvirt(Methods.StringBuilder_Append_Char)
							.Ldstr((columnAttribute?.Name ?? property.Name).Replace(@"\", @"\\").Replace("\"", "\\\""))
							.Callvirt(Methods.StringBuilder_Append_String)
							.Ldc_I4_S('"')
							.Callvirt(Methods.StringBuilder_Append_Char);

						firstProperty = false;
					}
				})
				.Ldstr("\r\n")
				.Callvirt(Methods.StringBuilder_Append_String)
				.Pop
				.Ret;
		}

		private static void DefineSerializeItem<T>(ILGenerator gen) {
			_ = gen.EmitFollowingLines()

				.DeclareLocal(typeof(IFormatProvider)) // loc_0
				.DeclareLocal(typeof(int)) // loc_1
				.DeclareLocal(typeof(double)) // loc_2
				.DeclareLocal(typeof(bool)) // loc_3
				.DeclareLocal(typeof(byte)) // loc_4
				.DeclareLocal(typeof(sbyte)) // loc_5
				.DeclareLocal(typeof(short)) // loc_6
				.DeclareLocal(typeof(ushort)) // loc_7
				.DeclareLocal(typeof(uint)) // loc_8
				.DeclareLocal(typeof(long)) // loc_9
				.DeclareLocal(typeof(ulong)) // loc_10
				.DeclareLocal(typeof(float)) // loc_11
				.DeclareLocal(typeof(decimal)) // loc_12
				.DeclareLocal(typeof(DateTime)) // loc_13

				.Call(Methods.CultureInfo_get_InvariantCulture)
				.Stloc_0
				.Ldarg_1
				.Do(emitter => {
					bool firstProperty = true;
					foreach (PropertyInfo property in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)) {
						if (!property.CanRead) throw new MissingMethodException($"{typeof(T).Name}.{property.Name} doesn't have a getter.");
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
									.Stloc_3
									.Ldloca_S(3)
									.Ldloc_0
									.Callx(Methods.Boolean_ToString)
									.Callvirt(Methods.StringBuilder_Append_String),
								_ when t == typeof(byte) => emitter
									.Ldarg_2
									.Callx(property.GetGetMethod()!)
									.Stloc_S(4)
									.Ldloca_S(4)
									.Ldloc_0
									.Callx(Methods.Byte_ToString)
									.Callvirt(Methods.StringBuilder_Append_String),
								_ when t == typeof(sbyte) => emitter
									.Ldarg_2
									.Callx(property.GetGetMethod()!)
									.Stloc_S(5)
									.Ldloca_S(5)
									.Ldloc_0
									.Callx(Methods.SByte_ToString)
									.Callvirt(Methods.StringBuilder_Append_String),
								_ when t == typeof(short) => emitter
									.Ldarg_2
									.Callx(property.GetGetMethod()!)
									.Stloc_S(6)
									.Ldloca_S(6)
									.Ldloc_0
									.Callx(Methods.Int16_ToString)
									.Callvirt(Methods.StringBuilder_Append_String),
								_ when t == typeof(ushort) => emitter
									.Ldarg_2
									.Callx(property.GetGetMethod()!)
									.Stloc_S(7)
									.Ldloca_S(7)
									.Ldloc_0
									.Callx(Methods.UInt16_ToString)
									.Callvirt(Methods.StringBuilder_Append_String),
								_ when t == typeof(int) => emitter
									.Ldarg_2
									.Callx(property.GetGetMethod()!)
									.Stloc_1
									.Ldloca_S(1)
									.Ldloc_0
									.Callx(Methods.Int32_ToString)
									.Callvirt(Methods.StringBuilder_Append_String),
								_ when t == typeof(uint) => emitter
									.Ldarg_2
									.Callx(property.GetGetMethod()!)
									.Stloc_S(8)
									.Ldloca_S(8)
									.Ldloc_0
									.Callx(Methods.UInt32_ToString)
									.Callvirt(Methods.StringBuilder_Append_String),
								_ when t == typeof(long) => emitter
									.Ldarg_2
									.Callx(property.GetGetMethod()!)
									.Stloc_S(9)
									.Ldloca_S(9)
									.Ldloc_0
									.Callx(Methods.Int64_ToString)
									.Callvirt(Methods.StringBuilder_Append_String),
								_ when t == typeof(ulong) => emitter
									.Ldarg_2
									.Callx(property.GetGetMethod()!)
									.Stloc_S(10)
									.Ldloca_S(10)
									.Ldloc_0
									.Callx(Methods.UInt64_ToString)
									.Callvirt(Methods.StringBuilder_Append_String),
								_ when t == typeof(float) => emitter
									.Ldarg_2
									.Callx(property.GetGetMethod()!)
									.Stloc_S(11)
									.Ldloca_S(11)
									.Ldloc_0
									.Callx(Methods.Single_ToString)
									.Callvirt(Methods.StringBuilder_Append_String),
								_ when t == typeof(double) => emitter
									.Ldarg_2
									.Callx(property.GetGetMethod()!)
									.Stloc_2
									.Ldloca_S(2)
									.Ldloc_0
									.Callx(Methods.Double_ToString)
									.Callvirt(Methods.StringBuilder_Append_String),
								_ when t == typeof(decimal) => emitter
									.Ldarg_2
									.Callx(property.GetGetMethod()!)
									.Stloc_S(12)
									.Ldloca_S(12)
									.Ldloc_0
									.Callx(Methods.Decimal_ToString)
									.Callvirt(Methods.StringBuilder_Append_String),
								_ when t == typeof(string) => emitter
									.Ldc_I4_S('"')
									.Callvirt(Methods.StringBuilder_Append_Char)
									.Ldarg_2
									.Callx(property.GetGetMethod()!)
									.Ldstr(@"\")
									.Ldstr(@"\\")
									.Callvirt(Methods.String_Replace)
									.Ldstr("\"")
									.Ldstr("\\\"")
									.Callvirt(Methods.String_Replace)
									.Call(Methods.StringBuilder_Append_String)
									.Ldc_I4_S('"')
									.Callvirt(Methods.StringBuilder_Append_Char),
								_ when t == typeof(DateTime) => emitter
									.Ldc_I4_S('"')
									.Callvirt(Methods.StringBuilder_Append_Char)
									.Ldarg_2
									.Callx(property.GetGetMethod()!)
									.Stloc_S(13)
									.Ldloca_S(13)
									.Do(emitter => property.GetCustomAttribute<CsvColumnAttribute>()?.DateFormat switch {
										string dateFormat => emitter
											.Ldstr(dateFormat)
											.Ldloc_0
											.Callvirt(Methods.DateTime_ToString_Format),
										_ => emitter
											.Ldloc_0
											.Callvirt(Methods.DateTime_ToString)
									})
									.Callvirt(Methods.StringBuilder_Append_String)
									.Ldc_I4_S('"')
									.Callvirt(Methods.StringBuilder_Append_Char),
								_ => throw new InvalidOperationException($"Unsupported column type: {t.Name}.")
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
				constructorInfo = typeof(T).GetConstructor(expectedConstructorSignature) ?? throw new MissingMethodException("No suitable constructor found.");
				isDefaultConstructor = false;
			}

			_ = gen.EmitFollowingLines()

				.DeclareLocal(typeof(List<string>))
				.DeclareLocal(typeof(bool))
				.DeclareLocal(typeof(object))
				.DeclareLocal(typeof(IFormatProvider))
				.DeclareLocal(typeof(T))

				.Call(Methods.CultureInfo_get_InvariantCulture)
				.Stloc_3
				.Ldarg_1
				.Ldarg_2
				.Call(Methods.StringSplitter_SplitLine)
				.Stloc_0
				.Ldloc_0
				.Callvirt(Methods.List_String_get_Count)
				.Ldc_I4(properties.Length)
				.Ceq
				.Stloc_1
				.Ldloc_1
				.Brfalse(out Label @else)

				.Do(emitter => isDefaultConstructor switch {
					true => emitter
						.Newobj(constructorInfo)
						.Stloc(4),
					_ => emitter
				})

				.Do(emitter => {
					for (int i = 0; i < properties.Length; i++) {
						if (isDefaultConstructor) {
							_ = emitter
								.Ldloc(4);
						}
						_ = emitter
							.Ldloc_0
							.Ldc_I4(i)
							.Callvirt(Methods.List_String_get_Item);
						_ = properties[i].PropertyType switch
						{
							Type t => t switch
							{
								_ when t == typeof(bool) => emitter
									.Call(Methods.Boolean_Parse),
								_ when t == typeof(byte) => emitter
									.Ldloc_3
									.Call(Methods.Byte_Parse),
								_ when t == typeof(sbyte) => emitter
									.Ldloc_3
									.Call(Methods.SByte_Parse),
								_ when t == typeof(short) => emitter
									.Ldloc_3
									.Call(Methods.Int16_Parse),
								_ when t == typeof(ushort) => emitter
									.Ldloc_3
									.Call(Methods.UInt16_Parse),
								_ when t == typeof(int) => emitter
									.Ldloc_3
									.Call(Methods.Int32_Parse),
								_ when t == typeof(uint) => emitter
									.Ldloc_3
									.Call(Methods.UInt32_Parse),
								_ when t == typeof(long) => emitter
									.Ldloc_3
									.Call(Methods.Int64_Parse),
								_ when t == typeof(ulong) => emitter
									.Ldloc_3
									.Call(Methods.UInt64_Parse),
								_ when t == typeof(float) => emitter
									.Ldloc_3
									.Call(Methods.Single_Parse),
								_ when t == typeof(double) => emitter
									.Ldloc_3
									.Call(Methods.Double_Parse),
								_ when t == typeof(decimal) => emitter
									.Ldloc_3
									.Call(Methods.Decimal_Parse),
								_ when t == typeof(string) => emitter
									.Ldc_I4(1)
									.Ldloc_0
									.Ldc_I4(i)
									.Callvirt(Methods.List_String_get_Item)
									.Callvirt(Methods.String_get_Length)
									.Ldc_I4(2)
									.Sub
									.Callvirt(Methods.String_Substring)
									.Ldstr("\\\"")
									.Ldstr("\"")
									.Callvirt(Methods.String_Replace)
									.Ldstr("\\\\")
									.Ldstr("\\")
									.Callvirt(Methods.String_Replace),
								_ when t == typeof(DateTime) => emitter
									.Ldc_I4(1)
									.Ldloc_0
									.Ldc_I4(i)
									.Callvirt(Methods.List_String_get_Item)
									.Callvirt(Methods.String_get_Length)
									.Ldc_I4(2)
									.Sub
									.Callvirt(Methods.String_Substring)
									.Do(emitter => properties[i].GetCustomAttribute<CsvColumnAttribute>()?.DateFormat switch {
										string dateFormat => emitter
											.Ldstr(dateFormat)
											.Ldloc_3
											.Call(Methods.DateTime_ParseExact),
										_ => emitter
											.Ldloc_3
											.Call(Methods.DateTime_Parse)
									}),
								_ => throw new InvalidOperationException($"Unsupported column type: {t.Name}.")
							}
						};
						if (isDefaultConstructor) {
							if (!properties[i].CanWrite) throw new MissingMethodException($"{typeof(T).Name}.{properties[i].Name} doesn't have a setter.");
							_ = emitter
								.Callvirt(properties[i].GetSetMethod()!);
						}
					}
					if (!isDefaultConstructor) {
						_ = emitter
							.Newobj(constructorInfo)
							.Stloc(4);
					}
				})
				.Br(out Label @endif)

				.MarkLabel(@else)
				.Ldstr($"Row must consists of {properties.Length} columns.")
				.Newobj(Methods.FormatException_ctor)
				.Throw

				.MarkLabel(@endif)
				.Ldloc(4)
				.Ret;
		}
	}
}
