using System;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace Csv.NaiveImpl {
	internal class NaiveSerializer<T> : ISerializer where T : notnull {
		private readonly PropertyInfo[] _properties;
		private readonly CsvColumnAttribute?[] _columnAttributes;

		public NaiveSerializer() {
			_properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
			_columnAttributes = new CsvColumnAttribute?[_properties.Length];
			for (int i = 0; i < _properties.Length; i++) {
				_columnAttributes[i] = _properties[i].GetCustomAttribute<CsvColumnAttribute>();
			}
		}

		public void SerializeHeader(StringBuilder stringBuilder, char separator) {
			bool firstProperty = true;
			for (int i = 0; i < _properties.Length; i++) {
				if (!firstProperty) {
					stringBuilder.Append(separator);
				}
				stringBuilder.Append('"');
				stringBuilder.Append((_columnAttributes[i]?.Name ?? _properties[i].Name).Replace(@"\", @"\\").Replace("\"", "\\\""));
				stringBuilder.Append('"');
				firstProperty = false;
			}
			stringBuilder.Append("\r\n");
		}

		public void SerializeItem(StringBuilder stringBuilder, object item, char separator) {
			bool firstProperty = true;
			for (int i = 0; i < _properties.Length; i++) {
				if (!firstProperty) {
					stringBuilder.Append(separator);
				}
				switch (_properties[i].PropertyType) {
					case Type tSByte when tSByte == typeof(sbyte):
					case Type tByte when tByte == typeof(byte):
					case Type tShort when tShort == typeof(short):
					case Type tUshort when tUshort == typeof(ushort):
					case Type tInt when tInt == typeof(int):
					case Type tUint when tUint == typeof(uint):
					case Type tLong when tLong == typeof(long):
					case Type tUlong when tUlong == typeof(ulong):
					case Type tFloat when tFloat == typeof(float):
					case Type tDouble when tDouble == typeof(double):
					case Type tDecimal when tDecimal == typeof(decimal):
						stringBuilder.AppendFormat(CultureInfo.InvariantCulture, "{0}", _properties[i].GetValue(item));
						break;
					case Type tBool when tBool == typeof(bool):
						stringBuilder.Append(_properties[i].GetValue(item) is true ? "True" : "False");
						break;
					case Type tString when tString == typeof(string):
						stringBuilder.Append('"');
						stringBuilder.Append(((string?)_properties[i].GetValue(item))?.Replace(@"\", @"\\").Replace("\"", "\\\""));
						stringBuilder.Append('"');
						break;
					case Type tDateTime when tDateTime == typeof(DateTime):
						stringBuilder.Append('"');
						if (_columnAttributes[i]?.DateFormat is string dateFormat) {
							stringBuilder.Append(((DateTime?)_properties[i].GetValue(item))?.ToString(dateFormat, CultureInfo.InvariantCulture));
						} else {
							stringBuilder.Append(((DateTime?)_properties[i].GetValue(item))?.ToString(CultureInfo.InvariantCulture));
						}
						stringBuilder.Append('"');
						break;
					default:
						throw new InvalidOperationException($"{_properties[i].PropertyType.FullName} is not supported.");
				}
				firstProperty = false;
			}
			stringBuilder.Append("\r\n");
		}
	}
}
