using Csv.Internal.Helpers;
using Csv.Internal.NaiveImpl;
using Missil;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Csv.Internal.NativeImpl {
	internal static class SerializerFactory {
		private static readonly ConcurrentDictionary<Type, ISerializer> _cache = new ConcurrentDictionary<Type, ISerializer>();

		public static ISerializer GetOrCreate<T>() where T : notnull {
			if (_cache.TryGetValue(typeof(T), out ISerializer? serializer)) return serializer;
			//if (TypeHelper.IsInternalOrAnonymous<T>()) {
				serializer = new NaiveSerializer<T>();
			//} else {
			//	ImplEmitter<ISerializer> implEmitter = new ImplEmitter<ISerializer>($"S{typeof(T).GUID.ToString("N")}");
			//	implEmitter.ImplementAction<char, StringBuilder>("SerializeHeader", gen => DefineSerializeHeader<T>(gen));
			//	implEmitter.ImplementAction<IFormatProvider, char, StringBuilder, object>("SerializeItem", gen => DefineSerializeItem<T>(gen));
			//	serializer = implEmitter.CreateInstance();
			//}
			_cache.TryAdd(typeof(T), serializer);
			return serializer;
		}

		private static void DefineSerializeHeader<T>(ILGenerator gen) {
			gen
				.Ldarg_2()
				.Emit(gen => {
					bool firstProperty = true;
					foreach (PropertyInfo property in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)) {
						CsvColumnAttribute? columnAttribute = property.GetCustomAttribute<CsvColumnAttribute>();
						if (!firstProperty) {
							gen
								.Ldarg_1()
								.Callvirt<StringBuilder>("Append", typeof(char));
						}
						gen
							.Ldstr("\"" + (columnAttribute?.Name ?? property.Name).Replace("\"", "\"\"") + "\"")
							.Callvirt<StringBuilder>("Append", typeof(string));
						firstProperty = false;
					}
				})
				.Ldstr("\r\n")
				.Callvirt<StringBuilder>("Append", typeof(string))
				.Pop()
				.Ret();
		}

		private static void DefineSerializeItem<T>(ILGenerator gen) {
			PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
			Dictionary<Type, LocalBuilder> localByType = new Dictionary<Type, LocalBuilder>();

			gen
				.Ldarg_3()
				.Emit(gen => {
					bool firstProperty = true;
					foreach (PropertyInfo property in properties) {
						if (!property.CanRead) throw new CsvTypeException(typeof(T), property.Name, "Property doesn't have a public getter.");
						CsvColumnAttribute? columnAttribute = property.GetCustomAttribute<CsvColumnAttribute>();
						gen
							.Emit(gen => firstProperty switch {
								false => gen
									.Ldarg_2()
									.Callvirt<StringBuilder>("Append", typeof(char)),
								_ => gen
							})
							.Emit(gen => gen
								.Ldarg(4)
								.Callvirt(property.GetGetMethod()!)
								.Emit(gen => {
									IConverterEmitter converterEmitter = ConverterFactory.GetOrCreateEmitter(property.PropertyType);
									ConverterEmitterAttribute converterEmitterAttribute = converterEmitter.GetType().GetMethod("EmitAppendToStringBuilder")!.GetCustomAttribute<ConverterEmitterAttribute>()!;
									LocalBuilder? local = null;
									LocalBuilder? secondaryLocal = null;
									if (converterEmitterAttribute.NullableOfGenericParameterIsPrimaryLocalType && converterEmitter.GetType().IsGenericType) {
										Type genericParameter = converterEmitter.GetType().GetGenericArguments()[0];
										Type localType = typeof(Nullable<>).MakeGenericType(genericParameter);
										if (!localByType.TryGetValue(localType, out local)) {
											local = gen.DeclareLocal(localType);
											localByType.Add(localType, local);
										}
									} else if (converterEmitterAttribute.GenericParameterIsPrimaryLocalType && converterEmitter.GetType().IsGenericType) {
										Type localType = converterEmitter.GetType().GetGenericArguments()[0];
										if (!localByType.TryGetValue(localType, out local)) {
											local = gen.DeclareLocal(localType);
											localByType.Add(localType, local);
										}
									} else if (converterEmitterAttribute.PrimaryLocalType is Type localType) {
										if (!localByType.TryGetValue(localType, out local)) {
											local = gen.DeclareLocal(localType);
											localByType.Add(localType, local);
										}
									}
									if (converterEmitterAttribute.SecondaryLocalType is Type secondaryLocalType) {
										if (!localByType.TryGetValue(secondaryLocalType, out secondaryLocal)) {
											secondaryLocal = gen.DeclareLocal(secondaryLocalType);
											localByType.Add(secondaryLocalType, secondaryLocal);
										}
									}
									converterEmitter.EmitAppendToStringBuilder(gen, local, secondaryLocal, columnAttribute);
								})
							);
						firstProperty = false;
					}
				})
				.Ldstr("\r\n")
				.Callvirt<StringBuilder>("Append", typeof(string))
				.Pop()
				.Ret();
		}
	}
}
