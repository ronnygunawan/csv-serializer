using System;
using System.Reflection;
using System.Text;

namespace Csv.NaiveImpl {
	internal class NaiveSerializer<T> : ISerializer where T : notnull {
		private enum SerializeAs {
			Number,
			String,
			DateTime,
			Uri
		}

		private readonly PropertyInfo[] _properties;
		private readonly CsvColumnAttribute?[] _columnAttributes;
		private readonly SerializeAs[] _serializeAs;

		public NaiveSerializer() {
			_properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
			_columnAttributes = new CsvColumnAttribute?[_properties.Length];
			_serializeAs = new SerializeAs[_properties.Length];
			for (int i = 0; i < _properties.Length; i++) {
				_columnAttributes[i] = _properties[i].GetCustomAttribute<CsvColumnAttribute>();
				switch (_properties[i].PropertyType) {
					case Type tSByte when tSByte == typeof(sbyte):
					case Type tNullableSByte when Nullable.GetUnderlyingType(tNullableSByte) == typeof(sbyte):
					case Type tByte when tByte == typeof(byte):
					case Type tNullableByte when Nullable.GetUnderlyingType(tNullableByte) == typeof(byte):
					case Type tInt16 when tInt16 == typeof(short):
					case Type tNullableInt16 when Nullable.GetUnderlyingType(tNullableInt16) == typeof(short):
					case Type tUInt16 when tUInt16 == typeof(ushort):
					case Type tNullableUInt16 when Nullable.GetUnderlyingType(tNullableUInt16) == typeof(ushort):
					case Type tInt32 when tInt32 == typeof(int):
					case Type tNullableInt32 when Nullable.GetUnderlyingType(tNullableInt32) == typeof(int):
					case Type tUint32 when tUint32 == typeof(uint):
					case Type tNullableUint32 when Nullable.GetUnderlyingType(tNullableUint32) == typeof(uint):
					case Type tInt64 when tInt64 == typeof(long):
					case Type tNullableInt64 when Nullable.GetUnderlyingType(tNullableInt64) == typeof(long):
					case Type tUInt64 when tUInt64 == typeof(ulong):
					case Type tNullableUInt64 when Nullable.GetUnderlyingType(tNullableUInt64) == typeof(ulong):
					case Type tSingle when tSingle == typeof(float):
					case Type tNullableSingle when Nullable.GetUnderlyingType(tNullableSingle) == typeof(float):
					case Type tDouble when tDouble == typeof(double):
					case Type tNullableDouble when Nullable.GetUnderlyingType(tNullableDouble) == typeof(double):
					case Type tDecimal when tDecimal == typeof(decimal):
					case Type tNullableDecimal when Nullable.GetUnderlyingType(tNullableDecimal) == typeof(decimal):
					case Type tBoolean when tBoolean == typeof(bool):
					case Type tNullableBoolean when Nullable.GetUnderlyingType(tNullableBoolean) == typeof(bool):
						_serializeAs[i] = SerializeAs.Number;
						break;
					case Type tString when tString == typeof(string):
						_serializeAs[i] = SerializeAs.String;
						break;
					case Type tDateTime when tDateTime == typeof(DateTime):
					case Type tNullableDateTime when Nullable.GetUnderlyingType(tNullableDateTime) == typeof(DateTime):
						_serializeAs[i] = SerializeAs.DateTime;
						break;
					case Type tUri when tUri == typeof(Uri):
						_serializeAs[i] = SerializeAs.Uri;
						break;
					default:
						throw new CsvTypeException(_properties[i].PropertyType);
				}
			}
		}

		public void SerializeHeader(StringBuilder stringBuilder, char separator) {
			bool firstProperty = true;
			for (int i = 0; i < _properties.Length; i++) {
				if (!firstProperty) {
					stringBuilder.Append(separator);
				}
				stringBuilder.Append('"');
				stringBuilder.Append((_columnAttributes[i]?.Name ?? _properties[i].Name).Replace("\"", "\"\""));
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
				switch (_serializeAs[i]) {
					case SerializeAs.Number:
						stringBuilder.AppendFormat("{0}", _properties[i].GetValue(item));
						break;
					case SerializeAs.String:
						if (((string?)_properties[i].GetValue(item))?.Replace("\"", "\"\"") is string stringValue) {
							stringBuilder.Append('"');
							stringBuilder.Append(stringValue);
							stringBuilder.Append('"');
						}
						break;
					case SerializeAs.DateTime:
						if (((DateTime?)_properties[i].GetValue(item)) is DateTime dateTimeValue) {
							stringBuilder.Append('"');
							if (_columnAttributes[i]?.DateFormat is string dateFormat) {
								stringBuilder.Append(dateTimeValue.ToString(dateFormat));
							} else {
								stringBuilder.Append(dateTimeValue.ToString());
							}
							stringBuilder.Append('"');
						}
						break;
					case SerializeAs.Uri:
						if (((Uri?)_properties[i].GetValue(item)) is Uri uri && uri.ToString().Replace("\"", "\"\"") is string uriString) {
							stringBuilder.Append('"');
							stringBuilder.Append(uriString);
							stringBuilder.Append('"');
						}
						break;
					default:
						throw new NotImplementedException();
				}
				firstProperty = false;
			}
			stringBuilder.Append("\r\n");
		}
	}
}
