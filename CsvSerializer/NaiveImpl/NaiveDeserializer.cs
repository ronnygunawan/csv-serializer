using Csv.Parser;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace Csv.NaiveImpl {
	internal class NaiveDeserializer<T> : IDeserializer where T : notnull {
		private enum DeserializeAs {
			Boolean,
			SByte,
			Byte,
			Int16,
			UInt16,
			Int32,
			UInt32,
			Int64,
			UInt64,
			Single,
			Double,
			Decimal,
			String,
			DateTime
		}

		private readonly PropertyInfo[] _properties;
		private readonly CsvColumnAttribute?[] _columnAttributes;
		private readonly DeserializeAs[] _deserializeAs;
		private readonly bool[] _isNullable;

		public NaiveDeserializer() {
			_properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
			_columnAttributes = new CsvColumnAttribute?[_properties.Length];
			_deserializeAs = new DeserializeAs[_properties.Length];
			_isNullable = new bool[_properties.Length];
			for (int i = 0; i < _properties.Length; i++) {
				_columnAttributes[i] = _properties[i].GetCustomAttribute<CsvColumnAttribute>();
				switch (_properties[i].PropertyType) {
					case Type tSByte when tSByte == typeof(sbyte):
						_deserializeAs[i] = DeserializeAs.SByte;
						_isNullable[i] = false;
						break;
					case Type tNullableSByte when Nullable.GetUnderlyingType(tNullableSByte) == typeof(sbyte):
						_deserializeAs[i] = DeserializeAs.SByte;
						_isNullable[i] = true;
						break;
					case Type tByte when tByte == typeof(byte):
						_deserializeAs[i] = DeserializeAs.Byte;
						_isNullable[i] = false;
						break;
					case Type tNullableByte when Nullable.GetUnderlyingType(tNullableByte) == typeof(byte):
						_deserializeAs[i] = DeserializeAs.Byte;
						_isNullable[i] = true;
						break;
					case Type tInt16 when tInt16 == typeof(short):
						_deserializeAs[i] = DeserializeAs.Int16;
						_isNullable[i] = false;
						break;
					case Type tNullableInt16 when Nullable.GetUnderlyingType(tNullableInt16) == typeof(short):
						_deserializeAs[i] = DeserializeAs.Int16;
						_isNullable[i] = true;
						break;
					case Type tUInt16 when tUInt16 == typeof(ushort):
						_deserializeAs[i] = DeserializeAs.UInt16;
						_isNullable[i] = false;
						break;
					case Type tNullableUInt16 when Nullable.GetUnderlyingType(tNullableUInt16) == typeof(ushort):
						_deserializeAs[i] = DeserializeAs.UInt16;
						_isNullable[i] = true;
						break;
					case Type tInt32 when tInt32 == typeof(int):
						_deserializeAs[i] = DeserializeAs.Int32;
						_isNullable[i] = false;
						break;
					case Type tNullableInt32 when Nullable.GetUnderlyingType(tNullableInt32) == typeof(int):
						_deserializeAs[i] = DeserializeAs.Int32;
						_isNullable[i] = true;
						break;
					case Type tUint32 when tUint32 == typeof(uint):
						_deserializeAs[i] = DeserializeAs.UInt32;
						_isNullable[i] = false;
						break;
					case Type tNullableUint32 when Nullable.GetUnderlyingType(tNullableUint32) == typeof(uint):
						_deserializeAs[i] = DeserializeAs.UInt32;
						_isNullable[i] = true;
						break;
					case Type tInt64 when tInt64 == typeof(long):
						_deserializeAs[i] = DeserializeAs.Int64;
						_isNullable[i] = false;
						break;
					case Type tNullableInt64 when Nullable.GetUnderlyingType(tNullableInt64) == typeof(long):
						_deserializeAs[i] = DeserializeAs.Int64;
						_isNullable[i] = true;
						break;
					case Type tUInt64 when tUInt64 == typeof(ulong):
						_deserializeAs[i] = DeserializeAs.UInt64;
						_isNullable[i] = false;
						break;
					case Type tNullableUInt64 when Nullable.GetUnderlyingType(tNullableUInt64) == typeof(ulong):
						_deserializeAs[i] = DeserializeAs.UInt64;
						_isNullable[i] = true;
						break;
					case Type tSingle when tSingle == typeof(float):
						_deserializeAs[i] = DeserializeAs.Single;
						_isNullable[i] = false;
						break;
					case Type tNullableSingle when Nullable.GetUnderlyingType(tNullableSingle) == typeof(float):
						_deserializeAs[i] = DeserializeAs.Single;
						_isNullable[i] = true;
						break;
					case Type tDouble when tDouble == typeof(double):
						_deserializeAs[i] = DeserializeAs.Double;
						_isNullable[i] = false;
						break;
					case Type tNullableDouble when Nullable.GetUnderlyingType(tNullableDouble) == typeof(double):
						_deserializeAs[i] = DeserializeAs.Double;
						_isNullable[i] = true;
						break;
					case Type tDecimal when tDecimal == typeof(decimal):
						_deserializeAs[i] = DeserializeAs.Decimal;
						_isNullable[i] = false;
						break;
					case Type tNullableDecimal when Nullable.GetUnderlyingType(tNullableDecimal) == typeof(decimal):
						_deserializeAs[i] = DeserializeAs.Decimal;
						_isNullable[i] = true;
						break;
					case Type tBoolean when tBoolean == typeof(bool):
						_deserializeAs[i] = DeserializeAs.Boolean;
						_isNullable[i] = false;
						break;
					case Type tNullableBoolean when Nullable.GetUnderlyingType(tNullableBoolean) == typeof(bool):
						_deserializeAs[i] = DeserializeAs.Boolean;
						_isNullable[i] = true;
						break;
					case Type tString when tString == typeof(string):
						_deserializeAs[i] = DeserializeAs.String;
						_isNullable[i] = true;
						break;
					case Type tDateTime when tDateTime == typeof(DateTime):
						_deserializeAs[i] = DeserializeAs.DateTime;
						_isNullable[i] = false;
						break;
					case Type tNullableDateTime when Nullable.GetUnderlyingType(tNullableDateTime) == typeof(DateTime):
						_deserializeAs[i] = DeserializeAs.DateTime;
						_isNullable[i] = true;
						break;
					default:
						throw new CsvTypeException(_properties[i].PropertyType);
				}
			}
		}

		public object DeserializeItem(string line, char separator) {
			List<string> columns = StringSplitter.SplitLine(line, separator);
			if (_properties.Length != columns.Count) throw new CsvFormatException(typeof(T), line, $"Row must consists of {_properties.Length} columns.");
			T item = Activator.CreateInstance<T>();
			for (int i = 0; i < _properties.Length; i++) {
				switch (_deserializeAs[i]) {
					case DeserializeAs.SByte:
						if (sbyte.TryParse(columns[i], out sbyte vSByte)) {
							_properties[i].SetValue(item, vSByte);
						} else if (!_isNullable[i] || !string.IsNullOrWhiteSpace(columns[i])) {
							throw new CsvFormatException(typeof(T), _properties[i].Name, columns[i], "Input string was not in correct sbyte format.");
						}
						break;
					case DeserializeAs.Byte:
						if (byte.TryParse(columns[i], out byte vByte)) {
							_properties[i].SetValue(item, vByte);
						} else if (!_isNullable[i] || !string.IsNullOrWhiteSpace(columns[i])) {
							throw new CsvFormatException(typeof(T), _properties[i].Name, columns[i], "Input string was not in correct byte format.");
						}
						break;
					case DeserializeAs.Int16:
						if (short.TryParse(columns[i], out short vInt16)) {
							_properties[i].SetValue(item, vInt16);
						} else if (!_isNullable[i] || !string.IsNullOrWhiteSpace(columns[i])) {
							throw new CsvFormatException(typeof(T), _properties[i].Name, columns[i], "Input string was not in correct Int16 format.");
						}
						break;
					case DeserializeAs.UInt16:
						if (ushort.TryParse(columns[i], out ushort vUInt16)) {
							_properties[i].SetValue(item, vUInt16);
						} else if (!_isNullable[i] || !string.IsNullOrWhiteSpace(columns[i])) {
							throw new CsvFormatException(typeof(T), _properties[i].Name, columns[i], "Input string was not in correct UInt16 format.");
						}
						break;
					case DeserializeAs.Int32:
						if (int.TryParse(columns[i], out int vInt32)) {
							_properties[i].SetValue(item, vInt32);
						} else if (!_isNullable[i] || !string.IsNullOrWhiteSpace(columns[i])) {
							throw new CsvFormatException(typeof(T), _properties[i].Name, columns[i], "Input string was not in correct Int32 format.");
						}
						break;
					case DeserializeAs.UInt32:
						if (uint.TryParse(columns[i], out uint vUInt32)) {
							_properties[i].SetValue(item, vUInt32);
						} else if (!_isNullable[i] || !string.IsNullOrWhiteSpace(columns[i])) {
							throw new CsvFormatException(typeof(T), _properties[i].Name, columns[i], "Input string was not in correct UInt32 format.");
						}
						break;
					case DeserializeAs.Int64:
						if (long.TryParse(columns[i], out long vInt64)) {
							_properties[i].SetValue(item, vInt64);
						} else if (!_isNullable[i] || !string.IsNullOrWhiteSpace(columns[i])) {
							throw new CsvFormatException(typeof(T), _properties[i].Name, columns[i], "Input string was not in correct Int64 format.");
						}
						break;
					case DeserializeAs.UInt64:
						if (ulong.TryParse(columns[i], out ulong vUInt64)) {
							_properties[i].SetValue(item, vUInt64);
						} else if (!_isNullable[i] || !string.IsNullOrWhiteSpace(columns[i])) {
							throw new CsvFormatException(typeof(T), _properties[i].Name, columns[i], "Input string was not in correct UInt64 format.");
						}
						break;
					case DeserializeAs.Single:
						if (float.TryParse(columns[i], out float vSingle)) {
							_properties[i].SetValue(item, vSingle);
						} else if (!_isNullable[i] || !string.IsNullOrWhiteSpace(columns[i])) {
							throw new CsvFormatException(typeof(T), _properties[i].Name, columns[i], "Input string was not in correct floating point format.");
						}
						break;
					case DeserializeAs.Double:
						if (double.TryParse(columns[i], out double vDouble)) {
							_properties[i].SetValue(item, vDouble);
						} else if (!_isNullable[i] || !string.IsNullOrWhiteSpace(columns[i])) {
							throw new CsvFormatException(typeof(T), _properties[i].Name, columns[i], "Input string was not in correct floating point format.");
						}
						break;
					case DeserializeAs.Decimal:
						if (decimal.TryParse(columns[i], out decimal vDecimal)) {
							_properties[i].SetValue(item, vDecimal);
						} else if (!_isNullable[i] || !string.IsNullOrWhiteSpace(columns[i])) {
							throw new CsvFormatException(typeof(T), _properties[i].Name, columns[i], "Input string was not in correct decimal format.");
						}
						break;
					case DeserializeAs.Boolean:
						if (bool.TryParse(columns[i], out bool vBoolean)) {
							_properties[i].SetValue(item, vBoolean);
						} else if (!_isNullable[i] || !string.IsNullOrWhiteSpace(columns[i])) {
							throw new CsvFormatException(typeof(T), _properties[i].Name, columns[i], "Input string was not in correct Boolean format.");
						}
						break;
					case DeserializeAs.String:
						string s = columns[i].Trim();
						if (s.StartsWith('"')
							&& s.EndsWith('"')) {
							s = s[1..^1];
							s = s.Replace("\"\"", "\"");
							_properties[i].SetValue(item, s);
						} else if (!string.IsNullOrWhiteSpace(columns[i])) {
							throw new CsvFormatException(typeof(T), _properties[i].Name, columns[i]);
						}
						break;
					case DeserializeAs.DateTime:
						s = columns[i].Trim();
						if (s.StartsWith('"')
							&& s.EndsWith('"')) {
							s = s[1..^1];
							DateTime vDateTime;
							if (_columnAttributes[i]?.DateFormat switch {
								string dateFormat => DateTime.TryParseExact(s, dateFormat, null, DateTimeStyles.AssumeLocal, out vDateTime),
								_ => DateTime.TryParse(s, null, DateTimeStyles.AssumeLocal, out vDateTime)
							}) {
								_properties[i].SetValue(item, vDateTime);
							} else if (!_isNullable[i] || !string.IsNullOrWhiteSpace(s)) {
								if (_columnAttributes[i]?.DateFormat is string dateFormat) {
									throw new CsvFormatException(typeof(T), _properties[i].Name, columns[i], $"Input string was not in correct DateTime format. Expected format was '{dateFormat}'.");
								} else {
									throw new CsvFormatException(typeof(T), _properties[i].Name, columns[i], "Input string was not in correct DateTime format.");
								}
							}
						} else if (!_isNullable[i] || !string.IsNullOrWhiteSpace(columns[i])) {
							throw new CsvFormatException(typeof(T), _properties[i].Name, columns[i], "Input string was not in correct DateTime format.");
						}
						break;
					default:
						throw new NotImplementedException();
				}
			}
			return item;
		}
	}
}
