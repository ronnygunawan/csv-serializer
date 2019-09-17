using Csv.Internal.Helpers;
using Csv.Internal.NaiveImpl;
using Missil;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Csv.Internal.NativeImpl {
	internal static class DeserializerFactory {
		private static readonly ConcurrentDictionary<Type, IDeserializer> _cache = new ConcurrentDictionary<Type, IDeserializer>();

		public static IDeserializer GetOrCreate<T>() where T : notnull {
			if (_cache.TryGetValue(typeof(T), out IDeserializer? deserializer)) return deserializer;
			//if (TypeHelper.IsInternalOrAnonymous<T>()) {
				deserializer = new NaiveDeserializer<T>();
			//} else {
			//	ImplEmitter<IDeserializer> implEmitter = new ImplEmitter<IDeserializer>($"D{typeof(T).GUID.ToString("N")}");
			//	implEmitter.ImplementFunc<List<object>, IFormatProvider, char, bool, ReadOnlyMemory<char>>("Deserialize", gen => DefineDeserialize<T>(gen));
			//	deserializer = implEmitter.CreateInstance();
			//}
			_cache.TryAdd(typeof(T), deserializer);
			return deserializer;
		}

		private static void DefineDeserialize<T>(ILGenerator gen) {
			PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
			Dictionary<Type, LocalBuilder> localByType = new Dictionary<Type, LocalBuilder>();
			ConstructorInfo? constructorInfo = typeof(T).GetConstructor(Type.EmptyTypes);
			bool isDefaultConstructor;
			if (constructorInfo != null) {
				isDefaultConstructor = true;
			} else {
				Type[] expectedConstructorSignature = properties.Select(prop => prop.PropertyType).ToArray();
				constructorInfo = typeof(T).GetConstructor(expectedConstructorSignature) ?? throw new CsvTypeException(typeof(T), "No suitable constructor found.");
				isDefaultConstructor = false;
			}
			gen
				.DeclareLocal<bool>(out LocalBuilder firstRow)
				.DeclareLocal<List<object>>(out LocalBuilder items)
				.DeclareLocal<List<ReadOnlyMemory<char>>>(out LocalBuilder columns)
				.DeclareLocal<int>(out LocalBuilder endOfLine)
				.DeclareLocal<string>(out LocalBuilder excMessage)
				.DeclareLocal<T>(out LocalBuilder obj)
				.DeclareLocal<ReadOnlyMemory<char>>(out LocalBuilder col)
				.Ldc_I4_1()
				.Stloc(firstRow)
				.Newobj(typeof(List<Object>).GetConstructor(Type.EmptyTypes)!)
				.Stloc(items)
				.Label(out Label beginLoop)
					.Ldarg(4); // csv
					gen.CallvirtPropertyGet<ReadOnlyMemory<char>>("Length")
					.Brfalse(out Label endLoop)
					.Ldarga_S(4) // csv
					.Ldarg_2(); // delimiter
					gen.Call(typeof(StringSplitter).GetMethod("ReadNextLine", new Type[] { typeof(ReadOnlyMemory<char>).MakeByRefType(), typeof(char) })!)
					.Stloc(columns)
					.Ldarg_3() // skipHeader
					.Brfalse_S(out Label dontSkipHeader)
						.Ldloc(firstRow)
						.Brfalse_S(dontSkipHeader)
						.Ldnull()
						.Stloc(firstRow)
						.Br_S(beginLoop)
					.Label(dontSkipHeader)
					.Ldc_I4_X(properties.Length)
					.Ldloc(columns);
					gen.CallvirtPropertyGet<List<ReadOnlyMemory<char>>>("Count")
					.Ceq()
					.Brtrue_S(out Label validated)
						.Ldarg(4)
						.Ldc_I4_S((byte)'\n');
						gen.Call(typeof(ReadOnlyMemoryHelper).GetMethod("IndexOf", new Type[] { typeof(ReadOnlyMemory<char>), typeof(char) })!)
						.Stloc(endOfLine)
						.Ldloc(endOfLine)
						.Ldc_I4_M1()
						.Ceq()
						.Brfalse_S(out Label splitMemory)
							.Ldarg(4);
							gen.Callvirt<ReadOnlyMemory<char>>("ToString")
							.Stloc(excMessage)
							.Br_S(out Label throwExc)
						.Label(splitMemory)
							.Ldarg(4)
							.Ldc_I4_0()
							.Ldloc(endOfLine);
							gen.Call<ReadOnlyMemory<char>>("Slice", typeof(int), typeof(int));
							gen.Callvirt<ReadOnlyMemory<char>>("ToString")
							.Stloc(excMessage)
						.Label(throwExc)
						.Ldtoken<T>()
						.Ldloc(excMessage)
						.Ldstr($"Row must consists of {properties.Length} columns.")
						.Initobj<CsvFormatException>()
						.Throw()
					.Label(validated)
					.Emit(gen => {
						if (isDefaultConstructor) {
							gen
								.Newobj(constructorInfo)
								.Stloc(obj)
								.Ldloc(obj);
						}
					})
					.Emit(gen => {
						for (int i = 0; i < properties.Length; i++) {
							Type propertyType = properties[i].PropertyType;
							CsvColumnAttribute? columnAttribute = properties[i].GetCustomAttribute<CsvColumnAttribute>();
							IConverterEmitter converterEmitter = ConverterFactory.GetOrCreateEmitter(propertyType);
							ConverterEmitterAttribute converterEmitterAttribute = converterEmitter.GetType().GetMethod("EmitDeserialize")!.GetCustomAttribute<ConverterEmitterAttribute>()!;
							LocalBuilder? local = null;
							LocalBuilder? secondaryLocal = null;
							if (converterEmitterAttribute.PrimaryLocalType is Type localType) {
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
							gen
								.Emit(gen => {
									if (isDefaultConstructor) {
										gen.Ldloc(obj);
									}
								})
								.Ldloc(columns)
								.Ldc_I4_X(i);
								gen.Callvirt(typeof(List<ReadOnlyMemory<char>>).GetProperties(BindingFlags.Public | BindingFlags.Instance).Single(prop => prop.GetIndexParameters().Length > 0).GetGetMethod()!)
								.Stloc(col)
								.Ldloc(col)
								.Emit(gen => converterEmitter.EmitDeserialize(gen, local, secondaryLocal, columnAttribute))
								.Emit(gen => {
									if (isDefaultConstructor) {
										gen
											.Callvirt(properties[i].GetSetMethod()!);
									}
								});
						}
					})
					.Emit(gen => {
						if (!isDefaultConstructor) {
							gen
								.Newobj(constructorInfo);
						}
					})
					.Stloc(obj)
					.Ldloc(items)
					.Ldloc(obj);
					gen.Callvirt<List<object>>("Add", typeof(object))
					.Br(beginLoop)
				.Label(endLoop)
				.Ldloc(items)
				.Ret();
		}
	}
}
