using Csv.Parser;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace Csv.NaiveImpl {
	internal class NaiveDeserializer<T> : IDeserializer where T : notnull {
		private readonly PropertyInfo[] _properties;
		private readonly CsvColumnAttribute?[] _columnAttributes;

		public NaiveDeserializer() {
			_properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
			_columnAttributes = new CsvColumnAttribute?[_properties.Length];
			for (int i = 0; i < _properties.Length; i++) {
				_columnAttributes[i] = _properties[i].GetCustomAttribute<CsvColumnAttribute>();
			}
		}

		public object DeserializeItem(string line, char separator) {
			List<string> columns = StringSplitter.SplitLine(line, separator);
			if (_properties.Length != columns.Count) throw new FormatException($"Line must contain exactly {_properties.Length} columns.");
			T item = Activator.CreateInstance<T>();
			for (int i = 0; i < _properties.Length; i++) {
				switch (_properties[i].PropertyType) {
					case Type tSByte when tSByte == typeof(sbyte) && sbyte.Parse(columns[i], CultureInfo.InvariantCulture) is sbyte vSByte:
						_properties[i].SetValue(item, vSByte);
						break;
					case Type tByte when tByte == typeof(byte) && byte.Parse(columns[i], CultureInfo.InvariantCulture) is byte vByte:
						_properties[i].SetValue(item, vByte);
						break;
					case Type tShort when tShort == typeof(short) && short.Parse(columns[i], CultureInfo.InvariantCulture) is short vShort:
						_properties[i].SetValue(item, vShort);
						break;
					case Type tUshort when tUshort == typeof(ushort) && ushort.Parse(columns[i], CultureInfo.InvariantCulture) is ushort vUshort:
						_properties[i].SetValue(item, vUshort);
						break;
					case Type tInt when tInt == typeof(int) && int.Parse(columns[i], CultureInfo.InvariantCulture) is int vInt:
						_properties[i].SetValue(item, vInt);
						break;
					case Type tUint when tUint == typeof(uint) && uint.Parse(columns[i], CultureInfo.InvariantCulture) is uint vUint:
						_properties[i].SetValue(item, vUint);
						break;
					case Type tLong when tLong == typeof(long) && long.Parse(columns[i], CultureInfo.InvariantCulture) is long vLong:
						_properties[i].SetValue(item, vLong);
						break;
					case Type tUlong when tUlong == typeof(ulong) && ulong.Parse(columns[i], CultureInfo.InvariantCulture) is ulong vUlong:
						_properties[i].SetValue(item, vUlong);
						break;
					case Type tFloat when tFloat == typeof(float) && float.Parse(columns[i], CultureInfo.InvariantCulture) is float vFloat:
						_properties[i].SetValue(item, vFloat);
						break;
					case Type tDouble when tDouble == typeof(double) && double.Parse(columns[i], CultureInfo.InvariantCulture) is double vDouble:
						_properties[i].SetValue(item, vDouble);
						break;
					case Type tDecimal when tDecimal == typeof(decimal) && decimal.Parse(columns[i], CultureInfo.InvariantCulture) is decimal vDecimal:
						_properties[i].SetValue(item, vDecimal);
						break;
					case Type tBool when tBool == typeof(bool):
						_properties[i].SetValue(item, columns[i].ToLowerInvariant().Trim() == "true");
						break;
					case Type tString when tString == typeof(string):
						string s = columns[i].Trim();
						s = s[1..^1];
						s = s.Replace("\\\"", "\"").Replace(@"\\", @"\");
						_properties[i].SetValue(item, s);
						break;
					case Type tDateTime when tDateTime == typeof(DateTime):
						s = columns[i].Trim();
						s = s[1..^1];
						if (_columnAttributes[i]?.DateFormat is string dateFormat) {
							_properties[i].SetValue(item, DateTime.ParseExact(s, dateFormat, CultureInfo.InvariantCulture));
						} else {
							_properties[i].SetValue(item, DateTime.Parse(s, CultureInfo.InvariantCulture));
						}
						break;
					default:
						throw new InvalidOperationException($"{_properties[0].PropertyType.FullName} is not supported.");
				}
			}
			return item;
		}
	}
}
