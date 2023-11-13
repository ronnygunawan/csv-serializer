using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Csv.Internals {
	internal static class StaticSourceFilesGenerator {
		public static void AddCsvSerializer(this GeneratorPostInitializationContext context) {
			context.AddSource(
				hintName: "CsvSerializer.g.cs",
				sourceText: SourceText.From(
					text: """
						#nullable enable
						using System;
						using System.Collections.Generic;
						using System.Globalization;
						using System.Linq;
						using System.Text;

						namespace Csv {
							public static class CsvSerializer {
								public static string Serialize<T>(
									IEnumerable<T> items,
									bool withHeaders = false,
									char delimiter = ',',
									IFormatProvider? provider = null
								) where T : notnull {
									ISerializer serializer = SerializerFactory.GetOrCreateSerializer<T>();
									StringBuilder stringBuilder = new();
									if (withHeaders) {
										serializer.SerializeHeader(delimiter, stringBuilder);
									}
									foreach (T item in items) {
										serializer.SerializeItem(provider, delimiter, stringBuilder, item);
									}
									return stringBuilder.ToString().TrimEnd();
								}

								public static T[] Deserialize<T>(string csv, bool hasHeaders = false, char delimiter = ',', IFormatProvider? provider = null) where T : notnull {
									IDeserializer deserializer = SerializerFactory.GetOrCreateDeserializer<T>();
									List<object> items = deserializer.Deserialize(provider, delimiter, hasHeaders, csv.AsMemory());
									return items.Cast<T>().ToArray();
								}
							}
						}

						""",
					encoding: Encoding.UTF8
				));
		}

		public static void AddAttributes(this GeneratorPostInitializationContext context) {
			context.AddSource(
				hintName: "Attributes.g.cs",
				sourceText: SourceText.From(
					text: """
						#nullable enable
						using System;

						namespace Csv {
							[AttributeUsage(AttributeTargets.Property)]
							public sealed class CsvColumnAttribute : Attribute {
								public string Name { get; }
								public string? DateFormat { get; set; }

								public CsvColumnAttribute(string name) {
									Name = name;
								}
							}
						}
						
						""",
					encoding: Encoding.UTF8
				)
			);
		}

		public static void AddExceptions(this GeneratorPostInitializationContext context) {
			context.AddSource(
				hintName: "Exceptions.g.cs",
				sourceText: SourceText.From(
					text: """
						#nullable enable
						using System;

						namespace Csv {
							public abstract class CsvException : Exception {
								public CsvException() {
								}

								public CsvException(string? message) : base(message) {
								}

								public CsvException(string? message, Exception? innerException) : base(message, innerException) {
								}
							}

							public sealed class CsvTypeException : CsvException {
								public Type Type { get; }
								public string? PropertyName { get; }

								public CsvTypeException(Type type) : base($"{type.Name} cannot be used in serialization or deserialization.") {
									Type = type;
								}

								public CsvTypeException(Type type, string? message) : base($"{type.Name} cannot be used in serialization or deserialization. {message}") {
									Type = type;
								}

								public CsvTypeException(Type type, string propertyName, string? message) : base($"{type.Name}.{propertyName} cannot be used in serialization or deserialization. {message}") {
									Type = type;
									PropertyName = propertyName;
								}

								public CsvTypeException(Type type, string propertyName, string? message, Exception? innerException) : base($"{type.Name}.{propertyName} cannot be used in serialization or deserialization. {message}", innerException) {
									Type = type;
									PropertyName = propertyName;
								}
							}

							public sealed class CsvPropertyTypeException : CsvException {
								public Type PropertyType { get; }

								public CsvPropertyTypeException(Type propertyType) : base($"Property of type {propertyType.Name} cannot be used in serialization or deserialization.") {
									PropertyType = propertyType;
								}

								public CsvPropertyTypeException(Type propertyType, string? message) : base($"Property of type {propertyType.Name} cannot be used in serialization or deserialization. {message}") {
									PropertyType = propertyType;
								}

								public CsvPropertyTypeException(Type propertyType, string? message, Exception? innerException) : base($"Property of type {propertyType.Name} cannot be used in serialization or deserialization. {message}", innerException) {
									PropertyType = propertyType;
								}
							}

							public sealed class CsvFormatException : CsvException {
								public Type? Type { get; }
								public string? PropertyName { get; }
								public string? Value { get; }

								public CsvFormatException(string value, string? message) : base($"Cannot deserialize '{value}'. {message}") {
									Value = value;
								}

								public CsvFormatException(Type type, string value, string? message) : base($"Cannot deserialize '{value}' into {type.Name}. {message}") {
									Type = type;
									Value = value;
								}

								public CsvFormatException(Type type, string propertyName, string value, string? message) : base($"Cannot deserialize '{value}' into {type.Name}.{propertyName}. {message}") {
									Type = type;
									PropertyName = propertyName;
									Value = value;
								}

								public CsvFormatException(Type type, string propertyName, string value, string? message, Exception? innerException) : base($"Cannot deserialize '{value}' into {type.Name}.{propertyName}. {message}", innerException) {
									Type = type;
									PropertyName = propertyName;
									Value = value;
								}
							}
						}
						
						""",
					encoding: Encoding.UTF8
				)
			);
		}

		public static void AddISerializer(this GeneratorPostInitializationContext context) {
			context.AddSource(
				hintName: "ISerializer.g.cs",
				sourceText: SourceText.From(
					text: """
						#nullable enable
						using System;
						using System.Text;

						namespace Csv {
							internal interface ISerializer {
								void SerializeHeader(char delimiter, StringBuilder stringBuilder);
								void SerializeItem(IFormatProvider? provider, char delimiter, StringBuilder stringBuilder, object item);
							}
						}
						
						""",
					encoding: Encoding.UTF8
				)
			);
		}

		public static void AddIDeserializer(this GeneratorPostInitializationContext context) {
			context.AddSource(
				hintName: "IDeserializer.g.cs",
				sourceText: SourceText.From(
					text: """
						#nullable enable
						using System;
						using System.Collections.Generic;

						namespace Csv {
							internal interface IDeserializer {
								List<object> Deserialize(IFormatProvider? provider, char delimiter, bool skipHeader, ReadOnlyMemory<char> csv);
							}
						}
						
						""",
					encoding: Encoding.UTF8
				)
			);
		}

		public static void AddIConverter(this GeneratorPostInitializationContext context) {
			context.AddSource(
				hintName: "IConverter.g.cs",
				sourceText: SourceText.From(
					text: """
						#nullable enable
						using System;
						using System.Text;

						namespace Csv.Internal {
							internal interface IConverter<T> {
								void AppendToStringBuilder(StringBuilder stringBuilder, IFormatProvider provider, T value, CsvColumnAttribute? attribute, char delimiter);
								T Deserialize(ReadOnlyMemory<char> text, IFormatProvider provider, CsvColumnAttribute? attribute);
							}
						}
						
						""",
					encoding: Encoding.UTF8
				)
			);
		}

		public static void AddStringSplitter(this GeneratorPostInitializationContext context) {
			context.AddSource(
				hintName: "StringSplitter.g.cs",
				sourceText: SourceText.From(
					text: """
						#nullable enable
						using System;
						using System.Collections.Generic;

						namespace Csv.Internal {
							internal static class StringSplitter {
								private enum ParserState {
									InStartingWhiteSpace,
									InUnquotedValue,
									InQuotedValue,
									InEscapeSequence,
									InTrailingWhiteSpace
								}

								public static List<ReadOnlyMemory<char>> ReadNextLine(ref ReadOnlyMemory<char> csv, char separator = ',') {
									ReadOnlySpan<char> span = csv.Span;
									List<ReadOnlyMemory<char>> columns = new();
									int startOfLiteral = 0;
									int endOfLiteral = 0;
									ParserState state = ParserState.InStartingWhiteSpace;
									for (int i = 0, length = csv.Length; i <= length; i++) {
										if (i == length) {
											switch (state) {
												case ParserState.InStartingWhiteSpace:
												case ParserState.InUnquotedValue:
												case ParserState.InEscapeSequence:
													columns.Add(csv[startOfLiteral..i]);
													csv = csv.Slice(csv.Length - 1, 0);
													return columns;
												case ParserState.InQuotedValue:
													throw new CsvFormatException(csv.ToString(), "End of file in quoted literal.");
												case ParserState.InTrailingWhiteSpace:
													columns.Add(csv.Slice(startOfLiteral, endOfLiteral - startOfLiteral + 1));
													csv = csv.Slice(csv.Length - 1, 0);
													return columns;
											}
										} else {
											switch (span[i]) {
												case '"':
													switch (state) {
														case ParserState.InStartingWhiteSpace:
															startOfLiteral = i;
															state = ParserState.InQuotedValue;
															break;
														case ParserState.InUnquotedValue:
															int endOfLine = span.IndexOf('\n');
															string line = endOfLine == -1 ? csv.ToString() : csv[..endOfLine].ToString();
															throw new CsvFormatException(line, $"Invalid character at position {i}: \"");
														case ParserState.InQuotedValue:
															state = ParserState.InEscapeSequence;
															break;
														case ParserState.InEscapeSequence:
															state = ParserState.InQuotedValue;
															break;
														case ParserState.InTrailingWhiteSpace:
															endOfLine = span.IndexOf('\n');
															line = endOfLine == -1 ? csv.ToString() : csv[..endOfLine].ToString();
															throw new CsvFormatException(line, $"Invalid character at position {i}: \"");
													}
													break;
												case char c when c == separator:
													switch (state) {
														case ParserState.InStartingWhiteSpace:
														case ParserState.InUnquotedValue:
														case ParserState.InEscapeSequence:
															columns.Add(csv[startOfLiteral..i]);
															startOfLiteral = i + 1;
															state = ParserState.InStartingWhiteSpace;
															break;
														case ParserState.InTrailingWhiteSpace:
															columns.Add(csv.Slice(startOfLiteral, endOfLiteral - startOfLiteral + 1));
															startOfLiteral = i + 1;
															state = ParserState.InStartingWhiteSpace;
															break;
													}
													break;
												case '\n':
													switch (state) {
														case ParserState.InStartingWhiteSpace:
														case ParserState.InUnquotedValue:
														case ParserState.InEscapeSequence:
															columns.Add(csv[startOfLiteral..i]);
															csv = csv[(i + 1)..];
															return columns;
														case ParserState.InTrailingWhiteSpace:
															columns.Add(csv.Slice(startOfLiteral, endOfLiteral - startOfLiteral + 1));
															csv = csv[(i + 1)..];
															return columns;
													}
													break;
												case char c:
													switch (state) {
														case ParserState.InStartingWhiteSpace:
															state = ParserState.InUnquotedValue;
															break;
														case ParserState.InEscapeSequence:
															endOfLiteral = i - 1;
															state = ParserState.InTrailingWhiteSpace;
															break;
														case ParserState.InTrailingWhiteSpace:
															if (!char.IsWhiteSpace(c)) {
																int endOfLine = span.IndexOf('\n');
																string line = endOfLine == -1 ? csv.ToString() : csv[..endOfLine].ToString();
																throw new CsvFormatException(line, $"Invalid character at position {i}: {c}");
															}
															break;
													}
													break;
											}
										}
									}
									throw new InvalidOperationException("Parser internal error.");
								}
							}
						}
						
						""",
					encoding: Encoding.UTF8
				)
			);
		}

		public static void AddNaiveSerializer(this GeneratorPostInitializationContext context) {
			context.AddSource(
				hintName: "NaiveSerializer.g.cs",
				sourceText: SourceText.From(
					text: """
						#nullable enable
						using System;
						using System.Reflection;
						using System.Text;

						namespace Csv.Internal.NaiveImpl {
							internal sealed class NaiveSerializer<T> : ISerializer where T : notnull {
								private enum SerializeAs {
									Number,
									String,
									DateTime,
									Uri,
									Enum
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
											case Type tEnum when tEnum.IsEnum:
												_serializeAs[i] = SerializeAs.Enum;
												break;
											case Type tNullableEnum when Nullable.GetUnderlyingType(tNullableEnum)?.IsEnum == true:
												_serializeAs[i] = SerializeAs.Enum;
												break;
											default:
												throw new CsvTypeException(_properties[i].PropertyType);
										}
									}
								}

								public void SerializeHeader(char delimiter, StringBuilder stringBuilder) {
									bool firstProperty = true;
									for (int i = 0; i < _properties.Length; i++) {
										if (!firstProperty) {
											stringBuilder.Append(delimiter);
										}
										stringBuilder.Append('"');
										stringBuilder.Append((_columnAttributes[i]?.Name ?? _properties[i].Name).Replace("\"", "\"\""));
										stringBuilder.Append('"');
										firstProperty = false;
									}
									stringBuilder.Append("\r\n");
								}

								public void SerializeItem(IFormatProvider? provider, char delimiter, StringBuilder stringBuilder, object item) {
									bool firstProperty = true;
									for (int i = 0; i < _properties.Length; i++) {
										if (!firstProperty) {
											stringBuilder.Append(delimiter);
										}
										switch (_serializeAs[i]) {
											case SerializeAs.Number:
												string? str = Convert.ToString(_properties[i].GetValue(item), provider);
												if (str is string && str.Contains(delimiter)) {
													stringBuilder.AppendFormat(provider, "\"{0}\"", str);
												} else {
													stringBuilder.AppendFormat(provider, "{0}", str);
												}
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
														stringBuilder.Append(dateTimeValue.ToString(dateFormat, provider));
													} else {
														stringBuilder.Append(dateTimeValue.ToString(provider));
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
											case SerializeAs.Enum:
												stringBuilder.AppendFormat(provider, "{0}", _properties[i].GetValue(item));
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
						
						""",
					encoding: Encoding.UTF8
				)
			);
		}

		public static void AddNaiveDeserializer(this GeneratorPostInitializationContext context) {
			context.AddSource(
				hintName: "NaiveDeserializer.g.cs",
				sourceText: SourceText.From(
					text: """
						#nullable enable
						using System;
						using System.Collections.Generic;
						using System.Globalization;
						using System.Reflection;

						namespace Csv.Internal.NaiveImpl {
							internal sealed class NaiveDeserializer<T> : IDeserializer where T : notnull {
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
									DateTime,
									Uri,
									Enum
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
											case Type tUri when tUri == typeof(Uri):
												_deserializeAs[i] = DeserializeAs.Uri;
												_isNullable[i] = true;
												break;
											case Type tEnum when tEnum.IsEnum:
												_deserializeAs[i] = DeserializeAs.Enum;
												_isNullable[i] = false;
												break;
											case Type tNullableEnum when Nullable.GetUnderlyingType(tNullableEnum)?.IsEnum == true:
												_deserializeAs[i] = DeserializeAs.Enum;
												_isNullable[i] = true;
												break;
											default:
												throw new CsvTypeException(_properties[i].PropertyType);
										}
									}
								}

								public List<object> Deserialize(IFormatProvider? provider, char delimiter, bool skipHeader, ReadOnlyMemory<char> csv) {
									bool firstRow = true;
									List<object> items = new();
									while (csv.Length > 0) {
										List<ReadOnlyMemory<char>> columns = StringSplitter.ReadNextLine(ref csv, delimiter);
										if (firstRow && skipHeader) {
											firstRow = false;
											continue;
										}
										if (_properties.Length != columns.Count) {
											int endOfLine = csv.Span.IndexOf('\n');
											string line = endOfLine == -1 ? csv.ToString() : csv.Slice(0, endOfLine).ToString();
											throw new CsvFormatException(typeof(T), line, $"Row must consists of {_properties.Length} columns.");
										}
										T item = Activator.CreateInstance<T>();
										for (int i = 0; i < _properties.Length; i++) {
											switch (_deserializeAs[i]) {
												case DeserializeAs.SByte:
													if (columns[i].Length >= 2 && columns[i].Span[0] == '"' && sbyte.TryParse(columns[i].Span[1..^1], NumberStyles.Integer, provider, out sbyte vSByte)) {
														_properties[i].SetValue(item, vSByte);
													} else if (sbyte.TryParse(columns[i].Span, NumberStyles.Integer, provider, out vSByte)) {
														_properties[i].SetValue(item, vSByte);
													} else if (!_isNullable[i] || columns[i].Length > 0) {
														throw new CsvFormatException(typeof(T), _properties[i].Name, columns[i].ToString(), "Input string was not in correct sbyte format.");
													}
													break;
												case DeserializeAs.Byte:
													if (columns[i].Length >= 2 && columns[i].Span[0] == '"' && byte.TryParse(columns[i].Span[1..^1], NumberStyles.Integer, provider, out byte vByte)) {
														_properties[i].SetValue(item, vByte);
													} else if (byte.TryParse(columns[i].Span, NumberStyles.Integer, provider, out vByte)) {
														_properties[i].SetValue(item, vByte);
													} else if (!_isNullable[i] || columns[i].Length > 0) {
														throw new CsvFormatException(typeof(T), _properties[i].Name, columns[i].ToString(), "Input string was not in correct byte format.");
													}
													break;
												case DeserializeAs.Int16:
													if (columns[i].Length >= 2 && columns[i].Span[0] == '"' && short.TryParse(columns[i].Span[1..^1], NumberStyles.Integer, provider, out short vInt16)) {
														_properties[i].SetValue(item, vInt16);
													} else if (short.TryParse(columns[i].Span, NumberStyles.Integer, provider, out vInt16)) {
														_properties[i].SetValue(item, vInt16);
													} else if (!_isNullable[i] || columns[i].Length > 0) {
														throw new CsvFormatException(typeof(T), _properties[i].Name, columns[i].ToString(), "Input string was not in correct Int16 format.");
													}
													break;
												case DeserializeAs.UInt16:
													if (columns[i].Length >= 2 && columns[i].Span[0] == '"' && ushort.TryParse(columns[i].Span[1..^1], NumberStyles.Integer, provider, out ushort vUInt16)) {
														_properties[i].SetValue(item, vUInt16);
													} else if (ushort.TryParse(columns[i].Span, NumberStyles.Integer, provider, out vUInt16)) {
														_properties[i].SetValue(item, vUInt16);
													} else if (!_isNullable[i] || columns[i].Length > 0) {
														throw new CsvFormatException(typeof(T), _properties[i].Name, columns[i].ToString(), "Input string was not in correct UInt16 format.");
													}
													break;
												case DeserializeAs.Int32:
													if (columns[i].Length >= 2 && columns[i].Span[0] == '"' && int.TryParse(columns[i].Span[1..^1], NumberStyles.Integer, provider, out int vInt32)) {
														_properties[i].SetValue(item, vInt32);
													} else if (int.TryParse(columns[i].Span, NumberStyles.Integer, provider, out vInt32)) {
														_properties[i].SetValue(item, vInt32);
													} else if (!_isNullable[i] || columns[i].Length > 0) {
														throw new CsvFormatException(typeof(T), _properties[i].Name, columns[i].ToString(), "Input string was not in correct Int32 format.");
													}
													break;
												case DeserializeAs.UInt32:
													if (columns[i].Length >= 2 && columns[i].Span[0] == '"' && uint.TryParse(columns[i].Span[1..^1], NumberStyles.Integer, provider, out uint vUInt32)) {
														_properties[i].SetValue(item, vUInt32);
													} else if (uint.TryParse(columns[i].Span, NumberStyles.Integer, provider, out vUInt32)) {
														_properties[i].SetValue(item, vUInt32);
													} else if (!_isNullable[i] || columns[i].Length > 0) {
														throw new CsvFormatException(typeof(T), _properties[i].Name, columns[i].ToString(), "Input string was not in correct UInt32 format.");
													}
													break;
												case DeserializeAs.Int64:
													if (columns[i].Length >= 2 && columns[i].Span[0] == '"' && long.TryParse(columns[i].Span[1..^1], NumberStyles.Integer, provider, out long vInt64)) {
														_properties[i].SetValue(item, vInt64);
													} else if (long.TryParse(columns[i].Span, NumberStyles.Integer, provider, out vInt64)) {
														_properties[i].SetValue(item, vInt64);
													} else if (!_isNullable[i] || columns[i].Length > 0) {
														throw new CsvFormatException(typeof(T), _properties[i].Name, columns[i].ToString(), "Input string was not in correct Int64 format.");
													}
													break;
												case DeserializeAs.UInt64:
													if (columns[i].Length >= 2 && columns[i].Span[0] == '"' && ulong.TryParse(columns[i].Span[1..^1], NumberStyles.Integer, provider, out ulong vUInt64)) {
														_properties[i].SetValue(item, vUInt64);
													} else if (ulong.TryParse(columns[i].Span, NumberStyles.Integer, provider, out vUInt64)) {
														_properties[i].SetValue(item, vUInt64);
													} else if (!_isNullable[i] || columns[i].Length > 0) {
														throw new CsvFormatException(typeof(T), _properties[i].Name, columns[i].ToString(), "Input string was not in correct UInt64 format.");
													}
													break;
												case DeserializeAs.Single:
													if (columns[i].Length >= 2 && columns[i].Span[0] == '"' && float.TryParse(columns[i].Span[1..^1], NumberStyles.Float, provider, out float vSingle)) {
														_properties[i].SetValue(item, vSingle);
													} else if (float.TryParse(columns[i].Span, NumberStyles.Float, provider, out vSingle)) {
														_properties[i].SetValue(item, vSingle);
													} else if (!_isNullable[i] || columns[i].Length > 0) {
														throw new CsvFormatException(typeof(T), _properties[i].Name, columns[i].ToString(), "Input string was not in correct floating point format.");
													}
													break;
												case DeserializeAs.Double:
													if (columns[i].Length >= 2 && columns[i].Span[0] == '"' && double.TryParse(columns[i].Span[1..^1], NumberStyles.Float, provider, out double vDouble)) {
														_properties[i].SetValue(item, vDouble);
													} else if (double.TryParse(columns[i].Span, NumberStyles.Float, provider, out vDouble)) {
														_properties[i].SetValue(item, vDouble);
													} else if (!_isNullable[i] || columns[i].Length > 0) {
														throw new CsvFormatException(typeof(T), _properties[i].Name, columns[i].ToString(), "Input string was not in correct floating point format.");
													}
													break;
												case DeserializeAs.Decimal:
													if (columns[i].Length >= 2 && columns[i].Span[0] == '"' && decimal.TryParse(columns[i].Span[1..^1], NumberStyles.Number, provider, out decimal vDecimal)) {
														_properties[i].SetValue(item, vDecimal);
													} else if (decimal.TryParse(columns[i].Span, NumberStyles.Number, provider, out vDecimal)) {
														_properties[i].SetValue(item, vDecimal);
													} else if (!_isNullable[i] || columns[i].Length > 0) {
														throw new CsvFormatException(typeof(T), _properties[i].Name, columns[i].ToString(), "Input string was not in correct decimal format.");
													}
													break;
												case DeserializeAs.Boolean:
													if (columns[i].Length >= 2 && columns[i].Span[0] == '"' && bool.TryParse(columns[i].Span[1..^1], out bool vBoolean)) {
														_properties[i].SetValue(item, vBoolean);
													} else if (bool.TryParse(columns[i].Span, out vBoolean)) {
														_properties[i].SetValue(item, vBoolean);
													} else if (!_isNullable[i] || columns[i].Length > 0) {
														throw new CsvFormatException(typeof(T), _properties[i].Name, columns[i].ToString(), "Input string was not in correct Boolean format.");
													}
													break;
												case DeserializeAs.String:
													string s = columns[i].ToString().Trim();
													if (s.StartsWith('"')
														&& s.EndsWith('"')) {
														s = s[1..^1];
													}
													s = s.Replace("\"\"", "\"").TrimEnd('\r');
													_properties[i].SetValue(item, s);
													break;
												case DeserializeAs.DateTime:
													s = columns[i].ToString().Trim();
													if (s.StartsWith('"')
														&& s.EndsWith('"')) {
														s = s[1..^1];
													}
													DateTime vDateTime;
													if (_columnAttributes[i]?.DateFormat switch {
														string dateFormat => DateTime.TryParseExact(s, dateFormat, null, DateTimeStyles.AssumeLocal, out vDateTime),
														_ => DateTime.TryParse(s, null, DateTimeStyles.AssumeLocal, out vDateTime)
													}) {
														_properties[i].SetValue(item, vDateTime);
													} else if (!_isNullable[i] || !string.IsNullOrWhiteSpace(s)) {
														if (_columnAttributes[i]?.DateFormat is string dateFormat) {
															throw new CsvFormatException(typeof(T), _properties[i].Name, columns[i].ToString(), $"Input string was not in correct DateTime format. Expected format was '{dateFormat}'.");
														} else {
															throw new CsvFormatException(typeof(T), _properties[i].Name, columns[i].ToString(), "Input string was not in correct DateTime format.");
														}
													} else {
														_properties[i].SetValue(item, null);
													}
													break;
												case DeserializeAs.Uri:
													s = columns[i].ToString().Trim();
													if (s.StartsWith('"')
														&& s.EndsWith('"')) {
														s = s[1..^1];
													}
													s = s.Replace("\"\"", "\"").TrimEnd('\r');
													if (string.IsNullOrWhiteSpace(s)) {
														_properties[i].SetValue(item, null);
													} else {
														_properties[i].SetValue(item, new Uri(s));
													}
													break;
												case DeserializeAs.Enum:
													Type enumType;
													if (_isNullable[i]) {
														enumType = Nullable.GetUnderlyingType(_properties[i].PropertyType)!;
													} else {
														enumType = _properties[i].PropertyType;
													}
													if (Enum.TryParse(enumType, columns[i].ToString(), out object? vEnum) && vEnum != null) {
														_properties[i].SetValue(item, vEnum);
													} else if (!_isNullable[i] || columns[i].Length > 0) {
														throw new CsvFormatException(typeof(T), _properties[i].Name, columns[i].ToString(), $"Input string was not a valid {_properties[i].PropertyType.Name} value.");
													}
													break;
												default:
													throw new NotImplementedException();
											}
										}
										items.Add(item);
									}
									return items;
								}
							}
						}
						
						""",
					encoding: Encoding.UTF8
				)
			);
		}
	}
}
