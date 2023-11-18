using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Csv.Internals {
	internal static class NativeImplGenerator {
		public static void AddNativeImplementations(this GeneratorExecutionContext context) {
			if (context.SyntaxContextReceiver is not SerializeOrDeserializeSyntaxReceiver {
				InvocationLocationsByTypeSymbol: { } invocationLocationsByTypeSymbol
			}) {
				return;
			}

			StringBuilder nativeImplRegistration = new();

			foreach (KeyValuePair<ITypeSymbol, List<Location>> kvp in invocationLocationsByTypeSymbol) {
				try {
					// Add native implementations for typeSymbol
					(string fullTypeName, string serializerName, string deserializerName) = context.AddNativeImplementations(
						typeSymbol: kvp.Key,
						invocationLocations: kvp.Value
					);

					// Add native implementations registrations
					nativeImplRegistration.Append($$"""
								SERIALIZER_BY_TYPE.Add(typeof({{fullTypeName}}), new Csv.Internal.NativeImpl.{{serializerName}}());
								DESERIALIZER_BY_TYPE.Add(typeof({{fullTypeName}}), new Csv.Internal.NativeImpl.{{deserializerName}}());

					"""
					);
				} catch (DiagnosticReportedException) {
					continue;
				}
			}

			context.AddSource(
				hintName: "SerializerFactory.g.cs",
				sourceText: SourceText.From(
					text: $$"""
					#nullable enable
					using System;
					using System.Collections.Generic;
					using Csv.Internal.NaiveImpl;
					
					namespace Csv {
						internal static class SerializerFactory {
							private static readonly Dictionary<Type, ISerializer> SERIALIZER_BY_TYPE = new();
							private static readonly Dictionary<Type, IDeserializer> DESERIALIZER_BY_TYPE = new();

							static SerializerFactory() {
								{{nativeImplRegistration.ToString().Trim()}}
							}
					
							public static ISerializer GetOrCreateSerializer<T>() where T : notnull {
								if (SERIALIZER_BY_TYPE.TryGetValue(typeof(T), out ISerializer? serializer)) return serializer;
								serializer = new NaiveSerializer<T>();
								SERIALIZER_BY_TYPE.Add(typeof(T), serializer);
								return serializer;
							}
					
							public static IDeserializer GetOrCreateDeserializer<T>() where T : notnull {
								if (DESERIALIZER_BY_TYPE.TryGetValue(typeof(T), out IDeserializer? deserializer)) return deserializer;
								deserializer = new NaiveDeserializer<T>();
								DESERIALIZER_BY_TYPE.Add(typeof(T), deserializer);
								return deserializer;
							}
						}
					}
					
					""",
					encoding: Encoding.UTF8
				)
			);

			context.AddSource(
				hintName: "NativeStringSplitter.g.cs",
				sourceText: SourceText.From(
					text: """
					#nullable enable
					using System;

					namespace Csv.Internal.NativeImpl {
						internal static class NativeStringSplitter {
							private enum ParserState {
								InStartingWhiteSpace,
								InUnquotedValue,
								InQuotedValue,
								InEscapeSequence,
								InTrailingWhiteSpace
							}

							public static int ReadNextLine(ref ReadOnlyMemory<char> csv, ref Span<ReadOnlyMemory<char>> columns, char separator = ',') {
								ReadOnlySpan<char> span = csv.Span;
								int startOfLiteral = 0;
								int endOfLiteral = 0;
								int col = 0;
								ParserState state = ParserState.InStartingWhiteSpace;
								for (int i = 0, length = csv.Length; i <= length; i++) {
									if (i == length) {
										switch (state) {
											case ParserState.InStartingWhiteSpace:
											case ParserState.InUnquotedValue:
											case ParserState.InEscapeSequence:
												columns[col] = csv[startOfLiteral..i];
												csv = csv.Slice(csv.Length - 1, 0);
												return col + 1;
											case ParserState.InQuotedValue:
												throw new CsvFormatException(csv.ToString(), "End of file in quoted literal.");
											case ParserState.InTrailingWhiteSpace:
												columns[col] = csv.Slice(startOfLiteral, endOfLiteral - startOfLiteral + 1);
												csv = csv.Slice(csv.Length - 1, 0);
												return col + 1;
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
														columns[col++] = csv[startOfLiteral..i];
														startOfLiteral = i + 1;
														state = ParserState.InStartingWhiteSpace;
														break;
													case ParserState.InTrailingWhiteSpace:
														columns[col++] = csv.Slice(startOfLiteral, endOfLiteral - startOfLiteral + 1);
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
														columns[col] = csv[startOfLiteral..i];
														csv = csv[(i + 1)..];
														return col + 1;
													case ParserState.InTrailingWhiteSpace:
														columns[col] = csv.Slice(startOfLiteral, endOfLiteral - startOfLiteral + 1);
														csv = csv[(i + 1)..];
														return col + 1;
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

		private static (
			string FullTypeName,
			string SerializerName,
			string DeserializerName
		) AddNativeImplementations(
			this GeneratorExecutionContext context,
			ITypeSymbol typeSymbol,
			List<Location> invocationLocations
		) {
			string fullTypeName = GetFullName(typeSymbol);
			string serializerName = $"{fullTypeName.Replace('.', '_')}Serializer";
			string deserializerName = $"{fullTypeName.Replace('.', '_')}Deserializer";

			List<IPropertySymbol> propertySymbols = typeSymbol.GetMembers()
				.OfType<IPropertySymbol>()
				.Where(prop => prop.DeclaredAccessibility == Accessibility.Public && !prop.IsStatic)
				.ToList();

			StringBuilder serializeHeaderBuilder = new();
			StringBuilder writeHeaderBuilder = new();
			StringBuilder serializeItemWithProviderBuilder = new();
			StringBuilder writeItemWithProviderBuilder = new();
			StringBuilder serializeItemWithoutProviderBuilder = new();
			StringBuilder writeItemWithoutProviderBuilder = new();
			StringBuilder deserializeWithProviderBuilder = new();
			StringBuilder readWithProviderBuilder = new();
			StringBuilder deserializeWithoutProviderBuilder = new();
			StringBuilder readWithoutProviderBuilder = new();

			int col = 0;
			bool strDeclared = false;
			foreach (IPropertySymbol propertySymbol in propertySymbols) {
				// Delimiters
				if (col > 0) {
					serializeHeaderBuilder.AppendLine("""
								stringBuilder.Append(delimiter);
					""");
					writeHeaderBuilder.AppendLine("""
								streamWriter.Write(delimiter);
					""");
					serializeItemWithProviderBuilder.AppendLine("""
									stringBuilder.Append(delimiter);
					""");
					writeItemWithProviderBuilder.AppendLine("""
									streamWriter.Write(delimiter);
					""");
					serializeItemWithoutProviderBuilder.AppendLine("""
									stringBuilder.Append(delimiter);
					""");
					writeItemWithoutProviderBuilder.AppendLine("""
									streamWriter.Write(delimiter);
					""");
				}

				// Header serializers
				string? columnName = propertySymbol.GetAttributes()
					.Where(attr => attr.AttributeClass?.Name == "CsvColumnAttribute" && attr.ConstructorArguments.Length >= 1)
					.Select(attr => attr.ConstructorArguments[0])
					.Where(arg => arg.Kind == TypedConstantKind.Primitive)
					.Select(arg => arg.Value)
					.OfType<string>()
					.FirstOrDefault();
				string? dateFormat = propertySymbol.GetAttributes()
					.Where(attr => attr.AttributeClass?.Name == "CsvColumnAttribute")
					.SelectMany(attr => attr.NamedArguments.Where(arg => arg.Key == "DateFormat").Select(arg => arg.Value))
					.Where(arg => arg.Kind == TypedConstantKind.Primitive)
					.Select(arg => arg.Value)
					.OfType<string>()
					.FirstOrDefault();

				columnName ??= propertySymbol.Name ?? "";

				serializeHeaderBuilder.AppendLine($$"""
							stringBuilder.Append('"').Append("{{columnName}}").Append('"');
				""");
				writeHeaderBuilder.AppendLine($$"""
							streamWriter.Write('"');
							streamWriter.Write("{{columnName}}");
							streamWriter.Write('"');
				""");

				// Item serializers
				ITypeSymbol propertyTypeSymbol = propertySymbol.Type;
				switch (propertyTypeSymbol) {
					case { SpecialType: SpecialType.System_SByte }:
					case { SpecialType: SpecialType.System_Byte }:
					case { SpecialType: SpecialType.System_Int16 }:
					case { SpecialType: SpecialType.System_UInt16 }:
					case { SpecialType: SpecialType.System_Int32 }:
					case { SpecialType: SpecialType.System_UInt32 }:
					case { SpecialType: SpecialType.System_Int64 }:
					case { SpecialType: SpecialType.System_UInt64 }:
						if (!strDeclared) {
							serializeItemWithProviderBuilder.AppendLine("""
											string? str;
							""");
							writeItemWithProviderBuilder.AppendLine("""
											string? str;
							""");
							serializeItemWithoutProviderBuilder.AppendLine("""
											string? str;
							""");
							writeItemWithoutProviderBuilder.AppendLine("""
											string? str;
							""");
							strDeclared = true;
						}
						serializeItemWithProviderBuilder.AppendLine($$"""
									str = i.{{propertySymbol.Name}}.ToString(provider);
									stringBuilder.Append(str);
						""");
						writeItemWithProviderBuilder.AppendLine($$"""
									str = i.{{propertySymbol.Name}}.ToString(provider);
									streamWriter.Write(str);
						""");
						serializeItemWithoutProviderBuilder.AppendLine($$"""
									str = i.{{propertySymbol.Name}}.ToString();
									stringBuilder.Append(str);
						""");
						writeItemWithoutProviderBuilder.AppendLine($$"""
									str = i.{{propertySymbol.Name}}.ToString();
									streamWriter.Write(str);
						""");
						break;
					case { SpecialType: SpecialType.System_Single }:
					case { SpecialType: SpecialType.System_Double }:
					case { SpecialType: SpecialType.System_Decimal }:
						if (!strDeclared) {
							serializeItemWithProviderBuilder.AppendLine("""
											string? str;
							""");
							writeItemWithProviderBuilder.AppendLine("""
											string? str;
							""");
							serializeItemWithoutProviderBuilder.AppendLine("""
											string? str;
							""");
							writeItemWithoutProviderBuilder.AppendLine("""
											string? str;
							""");
							strDeclared = true;
						}
						serializeItemWithProviderBuilder.AppendLine($$"""
										str = i.{{propertySymbol.Name}}.ToString(provider);
										if (str.Contains(delimiter)) {
											stringBuilder.Append('"').Append(str).Append('"');
										} else {
											stringBuilder.Append(str);
										}
						""");
						writeItemWithProviderBuilder.AppendLine($$"""
										str = i.{{propertySymbol.Name}}.ToString(provider);
										if (str.Contains(delimiter)) {
											streamWriter.Write('"');
											streamWriter.Write(str);
											streamWriter.Write('"');
										} else {
											streamWriter.Write(str);
										}
						""");
						serializeItemWithoutProviderBuilder.AppendLine($$"""
										str = i.{{propertySymbol.Name}}.ToString();
										if (str.Contains(delimiter)) {
											stringBuilder.Append('"').Append(str).Append('"');
										} else {
											stringBuilder.Append(str);
										}
						""");
						writeItemWithoutProviderBuilder.AppendLine($$"""
										str = i.{{propertySymbol.Name}}.ToString();
										if (str.Contains(delimiter)) {
											streamWriter.Write('"');
											streamWriter.Write(str);
											streamWriter.Write('"');
										} else {
											streamWriter.Write(str);
										}
						""");
						break;
					case { SpecialType: SpecialType.System_Boolean }:
						serializeItemWithProviderBuilder.AppendLine($$"""
										if (i.{{propertySymbol.Name}}) {
											stringBuilder.Append("True");
										} else {
											stringBuilder.Append("False");
										}
						""");
						serializeItemWithoutProviderBuilder.AppendLine($$"""
										if (i.{{propertySymbol.Name}}) {
											stringBuilder.Append("True");
										} else {
											stringBuilder.Append("False");
										}
						""");
						break;
					case { SpecialType: SpecialType.System_String }:
						serializeItemWithProviderBuilder.AppendLine($$"""
										if (i.{{propertySymbol.Name}} is not null) {
											stringBuilder.Append('"');
											stringBuilder.Append(i.{{propertySymbol.Name}}.Replace("\"", "\"\""));
											stringBuilder.Append('"');
										}
						""");
						serializeItemWithoutProviderBuilder.AppendLine($$"""
										if (i.{{propertySymbol.Name}} is not null) {
											stringBuilder.Append('"');
											stringBuilder.Append(i.{{propertySymbol.Name}}.Replace("\"", "\"\""));
											stringBuilder.Append('"');
										}
						""");
						break;
					case { TypeKind: TypeKind.Enum }:
						serializeItemWithProviderBuilder.AppendLine($$"""
										stringBuilder.Append(i.{{propertySymbol.Name}}.ToString());
						""");
						serializeItemWithoutProviderBuilder.AppendLine($$"""
										stringBuilder.Append(i.{{propertySymbol.Name}}.ToString());
						""");
						break;
					case { SpecialType: SpecialType.System_DateTime }:
						if (dateFormat is not null) {
							serializeItemWithProviderBuilder.AppendLine($$"""
											stringBuilder.Append('"');
											stringBuilder.Append(i.{{propertySymbol.Name}}.ToString("{{dateFormat.Replace("\\", "\\\\")}}", provider).Replace("\"", "\"\""));
											stringBuilder.Append('"');
							""");
							serializeItemWithoutProviderBuilder.AppendLine($$"""
											stringBuilder.Append('"');
											stringBuilder.Append(i.{{propertySymbol.Name}}.ToString("{{dateFormat.Replace("\\", "\\\\")}}").Replace("\"", "\"\""));
											stringBuilder.Append('"');
							""");
						} else {
							serializeItemWithProviderBuilder.AppendLine($$"""
											stringBuilder.Append('"');
											stringBuilder.Append(i.{{propertySymbol.Name}}.ToString(provider).Replace("\"", "\"\""));
											stringBuilder.Append('"');
							""");
							serializeItemWithoutProviderBuilder.AppendLine($$"""
											stringBuilder.Append('"');
											stringBuilder.Append(i.{{propertySymbol.Name}}.ToString().Replace("\"", "\"\""));
											stringBuilder.Append('"');
							""");
						}
						break;
					case { Name: "Uri", ContainingNamespace.Name: "System" }:
						serializeItemWithProviderBuilder.AppendLine($$"""
										if (i.{{propertySymbol.Name}} is not null) {
											stringBuilder.Append('"');
											stringBuilder.Append(i.{{propertySymbol.Name}}.ToString().Replace("\"", "\"\""));
											stringBuilder.Append('"');
										}
						""");
						serializeItemWithoutProviderBuilder.AppendLine($$"""
										if (i.{{propertySymbol.Name}} is not null) {
											stringBuilder.Append('"');
											stringBuilder.Append(i.{{propertySymbol.Name}}.ToString().Replace("\"", "\"\""));
											stringBuilder.Append('"');
										}
						""");
						break;
					case { IsValueType: true, NullableAnnotation: NullableAnnotation.Annotated }:
						if (propertyTypeSymbol is not INamedTypeSymbol { TypeArguments: { Length: 1 } typeArguments }) {
							break;
						}
						ITypeSymbol underlyingTypeSymbol = typeArguments[0];
						switch (underlyingTypeSymbol) {
							case { SpecialType: SpecialType.System_SByte }:
							case { SpecialType: SpecialType.System_Byte }:
							case { SpecialType: SpecialType.System_Int16 }:
							case { SpecialType: SpecialType.System_UInt16 }:
							case { SpecialType: SpecialType.System_Int32 }:
							case { SpecialType: SpecialType.System_UInt32 }:
							case { SpecialType: SpecialType.System_Int64 }:
							case { SpecialType: SpecialType.System_UInt64 }:
								if (!strDeclared) {
									serializeItemWithProviderBuilder.AppendLine("""
													string? str;
									""");
									serializeItemWithoutProviderBuilder.AppendLine("""
													string? str;
									""");
									strDeclared = true;
								}
								serializeItemWithProviderBuilder.AppendLine($$"""
												str = i.{{propertySymbol.Name}}?.ToString(provider);
												if (str is not null) {
													stringBuilder.Append(str);
												}
								""");
								serializeItemWithoutProviderBuilder.AppendLine($$"""
												str = i.{{propertySymbol.Name}}?.ToString();
												if (str is not null) {
													stringBuilder.Append(str);
												}
								""");
								break;
							case { SpecialType: SpecialType.System_Single }:
							case { SpecialType: SpecialType.System_Double }:
							case { SpecialType: SpecialType.System_Decimal }:
								if (!strDeclared) {
									serializeItemWithProviderBuilder.AppendLine("""
													string? str;
									""");
									serializeItemWithoutProviderBuilder.AppendLine("""
													string? str;
									""");
									strDeclared = true;
								}
								serializeItemWithProviderBuilder.AppendLine($$"""
												str = i.{{propertySymbol.Name}}?.ToString(provider);
												if (str is not null) {
													if (str.Contains(delimiter)) {
														stringBuilder.Append('"').Append(str).Append('"');
													} else {
														stringBuilder.Append(str);
													}
												}
								""");
								serializeItemWithoutProviderBuilder.AppendLine($$"""
												str = i.{{propertySymbol.Name}}?.ToString();
												if (str is not null) {
													if (str.Contains(delimiter)) {
														stringBuilder.Append('"').Append(str).Append('"');
													} else {
														stringBuilder.Append(str);
													}
												}
								""");
								break;
							case { SpecialType: SpecialType.System_Boolean }:
								serializeItemWithProviderBuilder.AppendLine($$"""
												if (i.{{propertySymbol.Name}} == true) {
													stringBuilder.Append("True");
												} else if (i.{{propertySymbol.Name}} == false) {
													stringBuilder.Append("False");
												}
								""");
								serializeItemWithoutProviderBuilder.AppendLine($$"""
												if (i.{{propertySymbol.Name}} == true) {
													stringBuilder.Append("True");
												} else if (i.{{propertySymbol.Name}} == false) {
													stringBuilder.Append("False");
												}
								""");
								break;
							case { SpecialType: SpecialType.System_String }:
								serializeItemWithProviderBuilder.AppendLine($$"""
												if (i.{{propertySymbol.Name}} is not null) {
													stringBuilder.Append('"');
													stringBuilder.Append(i.{{propertySymbol.Name}}.Replace("\"", "\"\""));
													stringBuilder.Append('"');
												}
								""");
								serializeItemWithoutProviderBuilder.AppendLine($$"""
												if (i.{{propertySymbol.Name}} is not null) {
													stringBuilder.Append('"');
													stringBuilder.Append(i.{{propertySymbol.Name}}.Replace("\"", "\"\""));
													stringBuilder.Append('"');
												}
								""");
								break;
							case { TypeKind: TypeKind.Enum }:
								serializeItemWithProviderBuilder.AppendLine($$"""
												if (i.{{propertySymbol.Name}}.HasValue) {
													stringBuilder.Append(i.{{propertySymbol.Name}}.Value.ToString());
												}
								""");
								serializeItemWithoutProviderBuilder.AppendLine($$"""
												if (i.{{propertySymbol.Name}}.HasValue) {
													stringBuilder.Append(i.{{propertySymbol.Name}}.Value.ToString());
												}
								""");
								break;
							case { SpecialType: SpecialType.System_DateTime }:
								if (dateFormat is not null) {
									serializeItemWithProviderBuilder.AppendLine($$"""
													if (i.{{propertySymbol.Name}}.HasValue) {
														stringBuilder.Append('"');
														stringBuilder.Append(i.{{propertySymbol.Name}}.Value.ToString("{{dateFormat.Replace("\\", "\\\\")}}", provider).Replace("\"", "\"\""));
														stringBuilder.Append('"');
													}
									""");
									serializeItemWithoutProviderBuilder.AppendLine($$"""
													if (i.{{propertySymbol.Name}}.HasValue) {
														stringBuilder.Append('"');
														stringBuilder.Append(i.{{propertySymbol.Name}}.Value.ToString("{{dateFormat.Replace("\\", "\\\\")}}").Replace("\"", "\"\""));
														stringBuilder.Append('"');
													}
									""");
								} else {
									serializeItemWithProviderBuilder.AppendLine($$"""
													if (i.{{propertySymbol.Name}}.HasValue) {
														stringBuilder.Append('"');
														stringBuilder.Append(i.{{propertySymbol.Name}}.Value.ToString(provider).Replace("\"", "\"\""));
														stringBuilder.Append('"');
													}
									""");
									serializeItemWithoutProviderBuilder.AppendLine($$"""
													if (i.{{propertySymbol.Name}}.HasValue) {
														stringBuilder.Append('"');
														stringBuilder.Append(i.{{propertySymbol.Name}}.Value.ToString().Replace("\"", "\"\""));
														stringBuilder.Append('"');
													}
									""");
								}
								break;
							default:
								foreach (Location invocationLocation in invocationLocations) {
									context.ReportDiagnostic(
										Diagnostic.Create(
											descriptor: new(
												id: "CSV0001",
												title: "Unsupported property type",
												messageFormat: "{0} is not supported by CsvSerializer.",
												category: "Usage",
												defaultSeverity: DiagnosticSeverity.Error,
												isEnabledByDefault: true
											),
											location: invocationLocation,
											messageArgs: underlyingTypeSymbol.Name
										)
									);
								}
								throw new DiagnosticReportedException();
						}
						break;
					default:
						foreach (Location invocationLocation in invocationLocations) {
							context.ReportDiagnostic(
								Diagnostic.Create(
									descriptor: new(
										id: "CSV0001",
										title: "Unsupported property type",
										messageFormat: "{0} is not supported by CsvSerializer.",
										category: "Usage",
										defaultSeverity: DiagnosticSeverity.Error,
										isEnabledByDefault: true
									),
									location: invocationLocation,
									messageArgs: propertyTypeSymbol.Name
								)
							);
						}
						throw new DiagnosticReportedException();
				}

				// Deserializers
				switch (propertyTypeSymbol) {
					case { SpecialType: SpecialType.System_SByte }:
						deserializeWithProviderBuilder.AppendLine($$"""
											if (columns[{{col}}].Length >= 2 && columns[{{col}}].Span[0] == '"' && sbyte.TryParse(columns[{{col}}].Span[1..^1], NumberStyles.Integer, provider, out sbyte v{{propertySymbol.Name}})) {
												item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
											} else if (sbyte.TryParse(columns[{{col}}].Span, NumberStyles.Integer, provider, out v{{propertySymbol.Name}})) {
												item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
											} else {
												throw new CsvFormatException(typeof({{fullTypeName}}), "{{propertySymbol.Name}}", columns[{{col}}].ToString(), "Input string was not in correct sbyte format.");
											}
						""");
						deserializeWithoutProviderBuilder.AppendLine($$"""
											if (columns[{{col}}].Length >= 2 && columns[{{col}}].Span[0] == '"' && sbyte.TryParse(columns[{{col}}].Span[1..^1], out sbyte v{{propertySymbol.Name}})) {
												item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
											} else if (sbyte.TryParse(columns[{{col}}].Span, out v{{propertySymbol.Name}})) {
												item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
											} else {
												throw new CsvFormatException(typeof({{fullTypeName}}), "{{propertySymbol.Name}}", columns[{{col}}].ToString(), "Input string was not in correct sbyte format.");
											}
						""");
						break;
					case { SpecialType: SpecialType.System_Byte }:
						deserializeWithProviderBuilder.AppendLine($$"""
											if (columns[{{col}}].Length >= 2 && columns[{{col}}].Span[0] == '"' && byte.TryParse(columns[{{col}}].Span[1..^1], NumberStyles.Integer, provider, out byte v{{propertySymbol.Name}})) {
												item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
											} else if (byte.TryParse(columns[{{col}}].Span, NumberStyles.Integer, provider, out v{{propertySymbol.Name}})) {
												item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
											} else {
												throw new CsvFormatException(typeof({{fullTypeName}}), "{{propertySymbol.Name}}", columns[{{col}}].ToString(), "Input string was not in correct byte format.");
											}
						""");
						deserializeWithoutProviderBuilder.AppendLine($$"""
											if (columns[{{col}}].Length >= 2 && columns[{{col}}].Span[0] == '"' && byte.TryParse(columns[{{col}}].Span[1..^1], out byte v{{propertySymbol.Name}})) {
												item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
											} else if (byte.TryParse(columns[{{col}}].Span, out v{{propertySymbol.Name}})) {
												item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
											} else {
												throw new CsvFormatException(typeof({{fullTypeName}}), "{{propertySymbol.Name}}", columns[{{col}}].ToString(), "Input string was not in correct byte format.");
											}
						""");
						break;
					case { SpecialType: SpecialType.System_Int16 }:
						deserializeWithProviderBuilder.AppendLine($$"""
											if (columns[{{col}}].Length >= 2 && columns[{{col}}].Span[0] == '"' && short.TryParse(columns[{{col}}].Span[1..^1], NumberStyles.Integer, provider, out short v{{propertySymbol.Name}})) {
												item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
											} else if (short.TryParse(columns[{{col}}].Span, NumberStyles.Integer, provider, out v{{propertySymbol.Name}})) {
												item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
											} else {
												throw new CsvFormatException(typeof({{fullTypeName}}), "{{propertySymbol.Name}}", columns[{{col}}].ToString(), "Input string was not in correct short format.");
											}
						""");
						deserializeWithoutProviderBuilder.AppendLine($$"""
											if (columns[{{col}}].Length >= 2 && columns[{{col}}].Span[0] == '"' && short.TryParse(columns[{{col}}].Span[1..^1], out short v{{propertySymbol.Name}})) {
												item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
											} else if (short.TryParse(columns[{{col}}].Span, out v{{propertySymbol.Name}})) {
												item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
											} else {
												throw new CsvFormatException(typeof({{fullTypeName}}), "{{propertySymbol.Name}}", columns[{{col}}].ToString(), "Input string was not in correct short format.");
											}
						""");
						break;
					case { SpecialType: SpecialType.System_UInt16 }:
						deserializeWithProviderBuilder.AppendLine($$"""
											if (columns[{{col}}].Length >= 2 && columns[{{col}}].Span[0] == '"' && ushort.TryParse(columns[{{col}}].Span[1..^1], NumberStyles.Integer, provider, out ushort v{{propertySymbol.Name}})) {
												item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
											} else if (ushort.TryParse(columns[{{col}}].Span, NumberStyles.Integer, provider, out v{{propertySymbol.Name}})) {
												item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
											} else {
												throw new CsvFormatException(typeof({{fullTypeName}}), "{{propertySymbol.Name}}", columns[{{col}}].ToString(), "Input string was not in correct ushort format.");
											}
						""");
						deserializeWithoutProviderBuilder.AppendLine($$"""
											if (columns[{{col}}].Length >= 2 && columns[{{col}}].Span[0] == '"' && ushort.TryParse(columns[{{col}}].Span[1..^1], out ushort v{{propertySymbol.Name}})) {
												item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
											} else if (ushort.TryParse(columns[{{col}}].Span, out v{{propertySymbol.Name}})) {
												item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
											} else {
												throw new CsvFormatException(typeof({{fullTypeName}}), "{{propertySymbol.Name}}", columns[{{col}}].ToString(), "Input string was not in correct ushort format.");
											}
						""");
						break;
					case { SpecialType: SpecialType.System_Int32 }:
						deserializeWithProviderBuilder.AppendLine($$"""
											if (columns[{{col}}].Length >= 2 && columns[{{col}}].Span[0] == '"' && int.TryParse(columns[{{col}}].Span[1..^1], NumberStyles.Integer, provider, out int v{{propertySymbol.Name}})) {
												item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
											} else if (int.TryParse(columns[{{col}}].Span, NumberStyles.Integer, provider, out v{{propertySymbol.Name}})) {
												item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
											} else {
												throw new CsvFormatException(typeof({{fullTypeName}}), "{{propertySymbol.Name}}", columns[{{col}}].ToString(), "Input string was not in correct int format.");
											}
						""");
						deserializeWithoutProviderBuilder.AppendLine($$"""
											if (columns[{{col}}].Length >= 2 && columns[{{col}}].Span[0] == '"' && int.TryParse(columns[{{col}}].Span[1..^1], out int v{{propertySymbol.Name}})) {
												item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
											} else if (int.TryParse(columns[{{col}}].Span, out v{{propertySymbol.Name}})) {
												item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
											} else {
												throw new CsvFormatException(typeof({{fullTypeName}}), "{{propertySymbol.Name}}", columns[{{col}}].ToString(), "Input string was not in correct int format.");
											}
						""");
						break;
					case { SpecialType: SpecialType.System_UInt32 }:
						deserializeWithProviderBuilder.AppendLine($$"""
											if (columns[{{col}}].Length >= 2 && columns[{{col}}].Span[0] == '"' && uint.TryParse(columns[{{col}}].Span[1..^1], NumberStyles.Integer, provider, out uint v{{propertySymbol.Name}})) {
												item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
											} else if (uint.TryParse(columns[{{col}}].Span, NumberStyles.Integer, provider, out v{{propertySymbol.Name}})) {
												item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
											} else {
												throw new CsvFormatException(typeof({{fullTypeName}}), "{{propertySymbol.Name}}", columns[{{col}}].ToString(), "Input string was not in correct uint format.");
											}
						""");
						deserializeWithoutProviderBuilder.AppendLine($$"""
											if (columns[{{col}}].Length >= 2 && columns[{{col}}].Span[0] == '"' && uint.TryParse(columns[{{col}}].Span[1..^1], out uint v{{propertySymbol.Name}})) {
												item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
											} else if (uint.TryParse(columns[{{col}}].Span, out v{{propertySymbol.Name}})) {
												item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
											} else {
												throw new CsvFormatException(typeof({{fullTypeName}}), "{{propertySymbol.Name}}", columns[{{col}}].ToString(), "Input string was not in correct uint format.");
											}
						""");
						break;
					case { SpecialType: SpecialType.System_Int64 }:
						deserializeWithProviderBuilder.AppendLine($$"""
											if (columns[{{col}}].Length >= 2 && columns[{{col}}].Span[0] == '"' && long.TryParse(columns[{{col}}].Span[1..^1], NumberStyles.Integer, provider, out long v{{propertySymbol.Name}})) {
												item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
											} else if (long.TryParse(columns[{{col}}].Span, NumberStyles.Integer, provider, out v{{propertySymbol.Name}})) {
												item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
											} else {
												throw new CsvFormatException(typeof({{fullTypeName}}), "{{propertySymbol.Name}}", columns[{{col}}].ToString(), "Input string was not in correct long format.");
											}
						""");
						deserializeWithoutProviderBuilder.AppendLine($$"""
											if (columns[{{col}}].Length >= 2 && columns[{{col}}].Span[0] == '"' && long.TryParse(columns[{{col}}].Span[1..^1], out long v{{propertySymbol.Name}})) {
												item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
											} else if (long.TryParse(columns[{{col}}].Span, out v{{propertySymbol.Name}})) {
												item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
											} else {
												throw new CsvFormatException(typeof({{fullTypeName}}), "{{propertySymbol.Name}}", columns[{{col}}].ToString(), "Input string was not in correct long format.");
											}
						""");
						break;
					case { SpecialType: SpecialType.System_UInt64 }:
						deserializeWithProviderBuilder.AppendLine($$"""
											if (columns[{{col}}].Length >= 2 && columns[{{col}}].Span[0] == '"' && ulong.TryParse(columns[{{col}}].Span[1..^1], NumberStyles.Integer, provider, out ulong v{{propertySymbol.Name}})) {
												item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
											} else if (ulong.TryParse(columns[{{col}}].Span, NumberStyles.Integer, provider, out v{{propertySymbol.Name}})) {
												item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
											} else {
												throw new CsvFormatException(typeof({{fullTypeName}}), "{{propertySymbol.Name}}", columns[{{col}}].ToString(), "Input string was not in correct ulong format.");
											}
						""");
						deserializeWithoutProviderBuilder.AppendLine($$"""
											if (columns[{{col}}].Length >= 2 && columns[{{col}}].Span[0] == '"' && ulong.TryParse(columns[{{col}}].Span[1..^1], out ulong v{{propertySymbol.Name}})) {
												item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
											} else if (ulong.TryParse(columns[{{col}}].Span, out v{{propertySymbol.Name}})) {
												item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
											} else {
												throw new CsvFormatException(typeof({{fullTypeName}}), "{{propertySymbol.Name}}", columns[{{col}}].ToString(), "Input string was not in correct ulong format.");
											}
						""");
						break;
					case { SpecialType: SpecialType.System_Single }:
						deserializeWithProviderBuilder.AppendLine($$"""
											if (columns[{{col}}].Length >= 2 && columns[{{col}}].Span[0] == '"' && float.TryParse(columns[{{col}}].Span[1..^1], NumberStyles.Float, provider, out float v{{propertySymbol.Name}})) {
												item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
											} else if (float.TryParse(columns[{{col}}].Span, NumberStyles.Float, provider, out v{{propertySymbol.Name}})) {
												item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
											} else {
												throw new CsvFormatException(typeof({{fullTypeName}}), "{{propertySymbol.Name}}", columns[{{col}}].ToString(), "Input string was not in correct float format.");
											}
						""");
						deserializeWithoutProviderBuilder.AppendLine($$"""
											if (columns[{{col}}].Length >= 2 && columns[{{col}}].Span[0] == '"' && float.TryParse(columns[{{col}}].Span[1..^1], out float v{{propertySymbol.Name}})) {
												item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
											} else if (float.TryParse(columns[{{col}}].Span, out v{{propertySymbol.Name}})) {
												item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
											} else {
												throw new CsvFormatException(typeof({{fullTypeName}}), "{{propertySymbol.Name}}", columns[{{col}}].ToString(), "Input string was not in correct float format.");
											}
						""");
						break;
					case { SpecialType: SpecialType.System_Double }:
						deserializeWithProviderBuilder.AppendLine($$"""
											if (columns[{{col}}].Length >= 2 && columns[{{col}}].Span[0] == '"' && double.TryParse(columns[{{col}}].Span[1..^1], NumberStyles.Float, provider, out double v{{propertySymbol.Name}})) {
												item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
											} else if (double.TryParse(columns[{{col}}].Span, NumberStyles.Float, provider, out v{{propertySymbol.Name}})) {
												item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
											} else {
												throw new CsvFormatException(typeof({{fullTypeName}}), "{{propertySymbol.Name}}", columns[{{col}}].ToString(), "Input string was not in correct double format.");
											}
						""");
						deserializeWithoutProviderBuilder.AppendLine($$"""
											if (columns[{{col}}].Length >= 2 && columns[{{col}}].Span[0] == '"' && double.TryParse(columns[{{col}}].Span[1..^1], out double v{{propertySymbol.Name}})) {
												item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
											} else if (double.TryParse(columns[{{col}}].Span, out v{{propertySymbol.Name}})) {
												item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
											} else {
												throw new CsvFormatException(typeof({{fullTypeName}}), "{{propertySymbol.Name}}", columns[{{col}}].ToString(), "Input string was not in correct double format.");
											}
						""");
						break;
					case { SpecialType: SpecialType.System_Decimal }:
						deserializeWithProviderBuilder.AppendLine($$"""
											if (columns[{{col}}].Length >= 2 && columns[{{col}}].Span[0] == '"' && decimal.TryParse(columns[{{col}}].Span[1..^1], NumberStyles.Number, provider, out decimal v{{propertySymbol.Name}})) {
												item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
											} else if (decimal.TryParse(columns[{{col}}].Span, NumberStyles.Number, provider, out v{{propertySymbol.Name}})) {
												item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
											} else {
												throw new CsvFormatException(typeof({{fullTypeName}}), "{{propertySymbol.Name}}", columns[{{col}}].ToString(), "Input string was not in correct decimal format.");
											}
						""");
						deserializeWithoutProviderBuilder.AppendLine($$"""
											if (columns[{{col}}].Length >= 2 && columns[{{col}}].Span[0] == '"' && decimal.TryParse(columns[{{col}}].Span[1..^1], out decimal v{{propertySymbol.Name}})) {
												item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
											} else if (decimal.TryParse(columns[{{col}}].Span, out v{{propertySymbol.Name}})) {
												item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
											} else {
												throw new CsvFormatException(typeof({{fullTypeName}}), "{{propertySymbol.Name}}", columns[{{col}}].ToString(), "Input string was not in correct decimal format.");
											}
						""");
						break;
					case { SpecialType: SpecialType.System_Boolean }:
						deserializeWithProviderBuilder.AppendLine($$"""
											if (columns[{{col}}].Length >= 2 && columns[{{col}}].Span[0] == '"' && bool.TryParse(columns[{{col}}].Span[1..^1], out bool v{{propertySymbol.Name}})) {
												item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
											} else if (bool.TryParse(columns[{{col}}].Span, out v{{propertySymbol.Name}})) {
												item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
											} else {
												throw new CsvFormatException(typeof({{fullTypeName}}), "{{propertySymbol.Name}}", columns[{{col}}].ToString(), "Input string was not in correct Boolean format.");
											}
						""");
						deserializeWithoutProviderBuilder.AppendLine($$"""
											if (columns[{{col}}].Length >= 2 && columns[{{col}}].Span[0] == '"' && bool.TryParse(columns[{{col}}].Span[1..^1], out bool v{{propertySymbol.Name}})) {
												item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
											} else if (bool.TryParse(columns[{{col}}].Span, out v{{propertySymbol.Name}})) {
												item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
											} else {
												throw new CsvFormatException(typeof({{fullTypeName}}), "{{propertySymbol.Name}}", columns[{{col}}].ToString(), "Input string was not in correct Boolean format.");
											}
						""");
						break;
					case { SpecialType: SpecialType.System_String }:
						deserializeWithProviderBuilder.AppendLine($$"""
											string v{{propertySymbol.Name}} = columns[{{col}}].ToString().Trim();
											if (v{{propertySymbol.Name}}.StartsWith('"')
												&& v{{propertySymbol.Name}}.EndsWith('"')) {
												v{{propertySymbol.Name}} = v{{propertySymbol.Name}}[1..^1];
											}
											v{{propertySymbol.Name}} = v{{propertySymbol.Name}}.Replace("\"\"", "\"").TrimEnd('\r');
											item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
						""");
						deserializeWithoutProviderBuilder.AppendLine($$"""
											string v{{propertySymbol.Name}} = columns[{{col}}].ToString().Trim();
											if (v{{propertySymbol.Name}}.StartsWith('"')
												&& v{{propertySymbol.Name}}.EndsWith('"')) {
												v{{propertySymbol.Name}} = v{{propertySymbol.Name}}[1..^1];
											}
											v{{propertySymbol.Name}} = v{{propertySymbol.Name}}.Replace("\"\"", "\"").TrimEnd('\r');
											item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
						""");
						break;
					case { TypeKind: TypeKind.Enum }: {
							string fullEnumName = GetFullName(propertyTypeSymbol);
							deserializeWithProviderBuilder.AppendLine($$"""
												if (Enum.TryParse(columns[{{col}}].ToString(), out {{fullEnumName}} v{{propertySymbol.Name}})) {
													item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
												} else {
													throw new CsvFormatException(typeof({{fullTypeName}}), "{{propertySymbol.Name}}", columns[{{col}}].ToString(), "Input string was not a valid {{fullEnumName}} value.");
												}
							""");
							deserializeWithoutProviderBuilder.AppendLine($$"""
												if (Enum.TryParse(columns[{{col}}].ToString(), out {{fullEnumName}} v{{propertySymbol.Name}})) {
													item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
												} else {
													throw new CsvFormatException(typeof({{fullTypeName}}), "{{propertySymbol.Name}}", columns[{{col}}].ToString(), "Input string was not a valid {{fullEnumName}} value.");
												}
							""");
							break;
						}
					case { SpecialType: SpecialType.System_DateTime }:
						if (dateFormat is not null) {
							deserializeWithProviderBuilder.AppendLine($$"""
												string s{{propertySymbol.Name}} = columns[{{col}}].ToString().Trim();
												if (s{{propertySymbol.Name}}.StartsWith('"')
													&& s{{propertySymbol.Name}}.EndsWith('"')) {
													s{{propertySymbol.Name}} = s{{propertySymbol.Name}}[1..^1];
												}
												if (DateTime.TryParseExact(s{{propertySymbol.Name}}, "{{dateFormat.Replace("\\", "\\\\")}}", provider, DateTimeStyles.AssumeLocal, out DateTime v{{propertySymbol.Name}})) {
													item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
												} else {
													throw new CsvFormatException(typeof({{fullTypeName}}), "{{propertySymbol.Name}}", columns[{{col}}].ToString(), "Input string was not in correct DateTime format. Expected format was '{{dateFormat}}'.");
												}
							""");
							deserializeWithoutProviderBuilder.AppendLine($$"""
												string s{{propertySymbol.Name}} = columns[{{col}}].ToString().Trim();
												if (s{{propertySymbol.Name}}.StartsWith('"')
													&& s{{propertySymbol.Name}}.EndsWith('"')) {
													s{{propertySymbol.Name}} = s{{propertySymbol.Name}}[1..^1];
												}
												if (DateTime.TryParseExact(s{{propertySymbol.Name}}, "{{dateFormat.Replace("\\", "\\\\")}}", null, DateTimeStyles.AssumeLocal, out DateTime v{{propertySymbol.Name}})) {
													item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
												} else {
													throw new CsvFormatException(typeof({{fullTypeName}}), "{{propertySymbol.Name}}", columns[{{col}}].ToString(), "Input string was not in correct DateTime format. Expected format was '{{dateFormat}}'.");
												}
							""");
						} else {
							deserializeWithProviderBuilder.AppendLine($$"""
												string s{{propertySymbol.Name}} = columns[{{col}}].ToString().Trim();
												if (s{{propertySymbol.Name}}.StartsWith('"')
													&& s{{propertySymbol.Name}}.EndsWith('"')) {
													s{{propertySymbol.Name}} = s{{propertySymbol.Name}}[1..^1];
												}
												if (DateTime.TryParse(s{{propertySymbol.Name}}, provider, DateTimeStyles.AssumeLocal, out DateTime v{{propertySymbol.Name}})) {
													item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
												} else {
													throw new CsvFormatException(typeof({{fullTypeName}}), "{{propertySymbol.Name}}", columns[{{col}}].ToString(), "Input string was not in correct DateTime format.");
												}
							""");
							deserializeWithoutProviderBuilder.AppendLine($$"""
												string s{{propertySymbol.Name}} = columns[{{col}}].ToString().Trim();
												if (s{{propertySymbol.Name}}.StartsWith('"')
													&& s{{propertySymbol.Name}}.EndsWith('"')) {
													s{{propertySymbol.Name}} = s{{propertySymbol.Name}}[1..^1];
												}
												if (DateTime.TryParse(s{{propertySymbol.Name}}, null, DateTimeStyles.AssumeLocal, out DateTime v{{propertySymbol.Name}})) {
													item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
												} else {
													throw new CsvFormatException(typeof({{fullTypeName}}), "{{propertySymbol.Name}}", columns[{{col}}].ToString(), "Input string was not in correct DateTime format.");
												}
							""");
						}
						break;
					case { Name: "Uri", ContainingNamespace.Name: "System" }:
						deserializeWithProviderBuilder.AppendLine($$"""
											string v{{propertySymbol.Name}} = columns[{{col}}].ToString().Trim();
											if (v{{propertySymbol.Name}}.StartsWith('"')
												&& v{{propertySymbol.Name}}.EndsWith('"')) {
												v{{propertySymbol.Name}} = v{{propertySymbol.Name}}[1..^1];
											}
											v{{propertySymbol.Name}} = v{{propertySymbol.Name}}.Replace("\"\"", "\"").TrimEnd('\r');
											if (string.IsNullOrWhiteSpace(v{{propertySymbol.Name}})) {
												item.{{propertySymbol.Name}} = null;
											} else {
												item.{{propertySymbol.Name}} = new Uri(v{{propertySymbol.Name}});
											}
						""");
						deserializeWithoutProviderBuilder.AppendLine($$"""
											string v{{propertySymbol.Name}} = columns[{{col}}].ToString().Trim();
											if (v{{propertySymbol.Name}}.StartsWith('"')
												&& v{{propertySymbol.Name}}.EndsWith('"')) {
												v{{propertySymbol.Name}} = v{{propertySymbol.Name}}[1..^1];
											}
											v{{propertySymbol.Name}} = v{{propertySymbol.Name}}.Replace("\"\"", "\"").TrimEnd('\r');
											if (string.IsNullOrWhiteSpace(v{{propertySymbol.Name}})) {
												item.{{propertySymbol.Name}} = null;
											} else {
												item.{{propertySymbol.Name}} = new Uri(v{{propertySymbol.Name}});
											}
						""");
						break;
					case { IsValueType: true, NullableAnnotation: NullableAnnotation.Annotated }:
						if (propertyTypeSymbol is not INamedTypeSymbol { TypeArguments: { Length: 1 } typeArguments }) {
							break;
						}
						ITypeSymbol underlyingTypeSymbol = typeArguments[0];
						switch (underlyingTypeSymbol) {
							case { SpecialType: SpecialType.System_SByte }:
								deserializeWithProviderBuilder.AppendLine($$"""
													if (columns[{{col}}].Length >= 2 && columns[{{col}}].Span[0] == '"' && sbyte.TryParse(columns[{{col}}].Span[1..^1], NumberStyles.Integer, provider, out sbyte v{{propertySymbol.Name}})) {
														item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
													} else if (sbyte.TryParse(columns[{{col}}].Span, NumberStyles.Integer, provider, out v{{propertySymbol.Name}})) {
														item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
													} else if (columns[{{col}}].Length == 0) {
														item.{{propertySymbol.Name}} = null;
													} else {
														throw new CsvFormatException(typeof({{fullTypeName}}), "{{propertySymbol.Name}}", columns[{{col}}].ToString(), "Input string was not in correct sbyte format.");
													}
								""");
								deserializeWithoutProviderBuilder.AppendLine($$"""
													if (columns[{{col}}].Length >= 2 && columns[{{col}}].Span[0] == '"' && sbyte.TryParse(columns[{{col}}].Span[1..^1], out sbyte v{{propertySymbol.Name}})) {
														item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
													} else if (sbyte.TryParse(columns[{{col}}].Span, out v{{propertySymbol.Name}})) {
														item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
													} else if (columns[{{col}}].Length == 0) {
														item.{{propertySymbol.Name}} = null;
													} else {
														throw new CsvFormatException(typeof({{fullTypeName}}), "{{propertySymbol.Name}}", columns[{{col}}].ToString(), "Input string was not in correct sbyte format.");
													}
								""");
								break;
							case { SpecialType: SpecialType.System_Byte }:
								deserializeWithProviderBuilder.AppendLine($$"""
													if (columns[{{col}}].Length >= 2 && columns[{{col}}].Span[0] == '"' && byte.TryParse(columns[{{col}}].Span[1..^1], NumberStyles.Integer, provider, out byte v{{propertySymbol.Name}})) {
														item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
													} else if (byte.TryParse(columns[{{col}}].Span, NumberStyles.Integer, provider, out v{{propertySymbol.Name}})) {
														item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
													} else if (columns[{{col}}].Length == 0) {
														item.{{propertySymbol.Name}} = null;
													} else {
														throw new CsvFormatException(typeof({{fullTypeName}}), "{{propertySymbol.Name}}", columns[{{col}}].ToString(), "Input string was not in correct byte format.");
													}
								""");
								deserializeWithoutProviderBuilder.AppendLine($$"""
													if (columns[{{col}}].Length >= 2 && columns[{{col}}].Span[0] == '"' && byte.TryParse(columns[{{col}}].Span[1..^1], out byte v{{propertySymbol.Name}})) {
														item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
													} else if (byte.TryParse(columns[{{col}}].Span, out v{{propertySymbol.Name}})) {
														item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
													} else if (columns[{{col}}].Length == 0) {
														item.{{propertySymbol.Name}} = null;
													} else {
														throw new CsvFormatException(typeof({{fullTypeName}}), "{{propertySymbol.Name}}", columns[{{col}}].ToString(), "Input string was not in correct byte format.");
													}
								""");
								break;
							case { SpecialType: SpecialType.System_Int16 }:
								deserializeWithProviderBuilder.AppendLine($$"""
													if (columns[{{col}}].Length >= 2 && columns[{{col}}].Span[0] == '"' && short.TryParse(columns[{{col}}].Span[1..^1], NumberStyles.Integer, provider, out short v{{propertySymbol.Name}})) {
														item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
													} else if (short.TryParse(columns[{{col}}].Span, NumberStyles.Integer, provider, out v{{propertySymbol.Name}})) {
														item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
													} else if (columns[{{col}}].Length == 0) {
														item.{{propertySymbol.Name}} = null;
													} else {
														throw new CsvFormatException(typeof({{fullTypeName}}), "{{propertySymbol.Name}}", columns[{{col}}].ToString(), "Input string was not in correct short format.");
													}
								""");
								deserializeWithoutProviderBuilder.AppendLine($$"""
													if (columns[{{col}}].Length >= 2 && columns[{{col}}].Span[0] == '"' && short.TryParse(columns[{{col}}].Span[1..^1], out short v{{propertySymbol.Name}})) {
														item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
													} else if (short.TryParse(columns[{{col}}].Span, out v{{propertySymbol.Name}})) {
														item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
													} else if (columns[{{col}}].Length == 0) {
														item.{{propertySymbol.Name}} = null;
													} else {
														throw new CsvFormatException(typeof({{fullTypeName}}), "{{propertySymbol.Name}}", columns[{{col}}].ToString(), "Input string was not in correct short format.");
													}
								""");
								break;
							case { SpecialType: SpecialType.System_UInt16 }:
								deserializeWithProviderBuilder.AppendLine($$"""
													if (columns[{{col}}].Length >= 2 && columns[{{col}}].Span[0] == '"' && ushort.TryParse(columns[{{col}}].Span[1..^1], NumberStyles.Integer, provider, out ushort v{{propertySymbol.Name}})) {
														item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
													} else if (ushort.TryParse(columns[{{col}}].Span, NumberStyles.Integer, provider, out v{{propertySymbol.Name}})) {
														item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
													} else if (columns[{{col}}].Length == 0) {
														item.{{propertySymbol.Name}} = null;
													} else {
														throw new CsvFormatException(typeof({{fullTypeName}}), "{{propertySymbol.Name}}", columns[{{col}}].ToString(), "Input string was not in correct ushort format.");
													}
								""");
								deserializeWithoutProviderBuilder.AppendLine($$"""
													if (columns[{{col}}].Length >= 2 && columns[{{col}}].Span[0] == '"' && ushort.TryParse(columns[{{col}}].Span[1..^1], out ushort v{{propertySymbol.Name}})) {
														item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
													} else if (ushort.TryParse(columns[{{col}}].Span, out v{{propertySymbol.Name}})) {
														item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
													} else if (columns[{{col}}].Length == 0) {
														item.{{propertySymbol.Name}} = null;
													} else {
														throw new CsvFormatException(typeof({{fullTypeName}}), "{{propertySymbol.Name}}", columns[{{col}}].ToString(), "Input string was not in correct ushort format.");
													}
								""");
								break;
							case { SpecialType: SpecialType.System_Int32 }:
								deserializeWithProviderBuilder.AppendLine($$"""
													if (columns[{{col}}].Length >= 2 && columns[{{col}}].Span[0] == '"' && int.TryParse(columns[{{col}}].Span[1..^1], NumberStyles.Integer, provider, out int v{{propertySymbol.Name}})) {
														item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
													} else if (int.TryParse(columns[{{col}}].Span, NumberStyles.Integer, provider, out v{{propertySymbol.Name}})) {
														item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
													} else if (columns[{{col}}].Length == 0) {
														item.{{propertySymbol.Name}} = null;
													} else {
														throw new CsvFormatException(typeof({{fullTypeName}}), "{{propertySymbol.Name}}", columns[{{col}}].ToString(), "Input string was not in correct int format.");
													}
								""");
								deserializeWithoutProviderBuilder.AppendLine($$"""
													if (columns[{{col}}].Length >= 2 && columns[{{col}}].Span[0] == '"' && int.TryParse(columns[{{col}}].Span[1..^1], out int v{{propertySymbol.Name}})) {
														item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
													} else if (int.TryParse(columns[{{col}}].Span, out v{{propertySymbol.Name}})) {
														item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
													} else if (columns[{{col}}].Length == 0) {
														item.{{propertySymbol.Name}} = null;
													} else {
														throw new CsvFormatException(typeof({{fullTypeName}}), "{{propertySymbol.Name}}", columns[{{col}}].ToString(), "Input string was not in correct int format.");
													}
								""");
								break;
							case { SpecialType: SpecialType.System_UInt32 }:
								deserializeWithProviderBuilder.AppendLine($$"""
													if (columns[{{col}}].Length >= 2 && columns[{{col}}].Span[0] == '"' && uint.TryParse(columns[{{col}}].Span[1..^1], NumberStyles.Integer, provider, out uint v{{propertySymbol.Name}})) {
														item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
													} else if (uint.TryParse(columns[{{col}}].Span, NumberStyles.Integer, provider, out v{{propertySymbol.Name}})) {
														item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
													} else if (columns[{{col}}].Length == 0) {
														item.{{propertySymbol.Name}} = null;
													} else {
														throw new CsvFormatException(typeof({{fullTypeName}}), "{{propertySymbol.Name}}", columns[{{col}}].ToString(), "Input string was not in correct uint format.");
													}
								""");
								deserializeWithoutProviderBuilder.AppendLine($$"""
													if (columns[{{col}}].Length >= 2 && columns[{{col}}].Span[0] == '"' && uint.TryParse(columns[{{col}}].Span[1..^1], out uint v{{propertySymbol.Name}})) {
														item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
													} else if (uint.TryParse(columns[{{col}}].Span, out v{{propertySymbol.Name}})) {
														item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
													} else if (columns[{{col}}].Length == 0) {
														item.{{propertySymbol.Name}} = null;
													} else {
														throw new CsvFormatException(typeof({{fullTypeName}}), "{{propertySymbol.Name}}", columns[{{col}}].ToString(), "Input string was not in correct uint format.");
													}
								""");
								break;
							case { SpecialType: SpecialType.System_Int64 }:
								deserializeWithProviderBuilder.AppendLine($$"""
													if (columns[{{col}}].Length >= 2 && columns[{{col}}].Span[0] == '"' && long.TryParse(columns[{{col}}].Span[1..^1], NumberStyles.Integer, provider, out long v{{propertySymbol.Name}})) {
														item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
													} else if (long.TryParse(columns[{{col}}].Span, NumberStyles.Integer, provider, out v{{propertySymbol.Name}})) {
														item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
													} else if (columns[{{col}}].Length == 0) {
														item.{{propertySymbol.Name}} = null;
													} else {
														throw new CsvFormatException(typeof({{fullTypeName}}), "{{propertySymbol.Name}}", columns[{{col}}].ToString(), "Input string was not in correct long format.");
													}
								""");
								deserializeWithoutProviderBuilder.AppendLine($$"""
													if (columns[{{col}}].Length >= 2 && columns[{{col}}].Span[0] == '"' && long.TryParse(columns[{{col}}].Span[1..^1], out long v{{propertySymbol.Name}})) {
														item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
													} else if (long.TryParse(columns[{{col}}].Span, out v{{propertySymbol.Name}})) {
														item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
													} else if (columns[{{col}}].Length == 0) {
														item.{{propertySymbol.Name}} = null;
													} else {
														throw new CsvFormatException(typeof({{fullTypeName}}), "{{propertySymbol.Name}}", columns[{{col}}].ToString(), "Input string was not in correct long format.");
													}
								""");
								break;
							case { SpecialType: SpecialType.System_UInt64 }:
								deserializeWithProviderBuilder.AppendLine($$"""
													if (columns[{{col}}].Length >= 2 && columns[{{col}}].Span[0] == '"' && ulong.TryParse(columns[{{col}}].Span[1..^1], NumberStyles.Integer, provider, out ulong v{{propertySymbol.Name}})) {
														item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
													} else if (ulong.TryParse(columns[{{col}}].Span, NumberStyles.Integer, provider, out v{{propertySymbol.Name}})) {
														item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
													} else if (columns[{{col}}].Length == 0) {
														item.{{propertySymbol.Name}} = null;
													} else {
														throw new CsvFormatException(typeof({{fullTypeName}}), "{{propertySymbol.Name}}", columns[{{col}}].ToString(), "Input string was not in correct ulong format.");
													}
								""");
								deserializeWithoutProviderBuilder.AppendLine($$"""
													if (columns[{{col}}].Length >= 2 && columns[{{col}}].Span[0] == '"' && ulong.TryParse(columns[{{col}}].Span[1..^1], out ulong v{{propertySymbol.Name}})) {
														item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
													} else if (ulong.TryParse(columns[{{col}}].Span, out v{{propertySymbol.Name}})) {
														item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
													} else if (columns[{{col}}].Length == 0) {
														item.{{propertySymbol.Name}} = null;
													} else {
														throw new CsvFormatException(typeof({{fullTypeName}}), "{{propertySymbol.Name}}", columns[{{col}}].ToString(), "Input string was not in correct ulong format.");
													}
								""");
								break;
							case { SpecialType: SpecialType.System_Single }:
								deserializeWithProviderBuilder.AppendLine($$"""
													if (columns[{{col}}].Length >= 2 && columns[{{col}}].Span[0] == '"' && float.TryParse(columns[{{col}}].Span[1..^1], NumberStyles.Float, provider, out float v{{propertySymbol.Name}})) {
														item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
													} else if (float.TryParse(columns[{{col}}].Span, NumberStyles.Float, provider, out v{{propertySymbol.Name}})) {
														item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
													} else if (columns[{{col}}].Length == 0) {
														item.{{propertySymbol.Name}} = null;
													} else {
														throw new CsvFormatException(typeof({{fullTypeName}}), "{{propertySymbol.Name}}", columns[{{col}}].ToString(), "Input string was not in correct float format.");
													}
								""");
								deserializeWithoutProviderBuilder.AppendLine($$"""
													if (columns[{{col}}].Length >= 2 && columns[{{col}}].Span[0] == '"' && float.TryParse(columns[{{col}}].Span[1..^1], out float v{{propertySymbol.Name}})) {
														item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
													} else if (float.TryParse(columns[{{col}}].Span, out v{{propertySymbol.Name}})) {
														item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
													} else if (columns[{{col}}].Length == 0) {
														item.{{propertySymbol.Name}} = null;
													} else {
														throw new CsvFormatException(typeof({{fullTypeName}}), "{{propertySymbol.Name}}", columns[{{col}}].ToString(), "Input string was not in correct float format.");
													}
								""");
								break;
							case { SpecialType: SpecialType.System_Double }:
								deserializeWithProviderBuilder.AppendLine($$"""
													if (columns[{{col}}].Length >= 2 && columns[{{col}}].Span[0] == '"' && double.TryParse(columns[{{col}}].Span[1..^1], NumberStyles.Float, provider, out double v{{propertySymbol.Name}})) {
														item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
													} else if (double.TryParse(columns[{{col}}].Span, NumberStyles.Float, provider, out v{{propertySymbol.Name}})) {
														item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
													} else if (columns[{{col}}].Length == 0) {
														item.{{propertySymbol.Name}} = null;
													} else {
														throw new CsvFormatException(typeof({{fullTypeName}}), "{{propertySymbol.Name}}", columns[{{col}}].ToString(), "Input string was not in correct double format.");
													}
								""");
								deserializeWithoutProviderBuilder.AppendLine($$"""
													if (columns[{{col}}].Length >= 2 && columns[{{col}}].Span[0] == '"' && double.TryParse(columns[{{col}}].Span[1..^1], out double v{{propertySymbol.Name}})) {
														item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
													} else if (double.TryParse(columns[{{col}}].Span, out v{{propertySymbol.Name}})) {
														item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
													} else if (columns[{{col}}].Length == 0) {
														item.{{propertySymbol.Name}} = null;
													} else {
														throw new CsvFormatException(typeof({{fullTypeName}}), "{{propertySymbol.Name}}", columns[{{col}}].ToString(), "Input string was not in correct double format.");
													}
								""");
								break;
							case { SpecialType: SpecialType.System_Decimal }:
								deserializeWithProviderBuilder.AppendLine($$"""
													if (columns[{{col}}].Length >= 2 && columns[{{col}}].Span[0] == '"' && decimal.TryParse(columns[{{col}}].Span[1..^1], NumberStyles.Number, provider, out decimal v{{propertySymbol.Name}})) {
														item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
													} else if (decimal.TryParse(columns[{{col}}].Span, NumberStyles.Number, provider, out v{{propertySymbol.Name}})) {
														item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
													} else if (columns[{{col}}].Length == 0) {
														item.{{propertySymbol.Name}} = null;
													} else {
														throw new CsvFormatException(typeof({{fullTypeName}}), "{{propertySymbol.Name}}", columns[{{col}}].ToString(), "Input string was not in correct decimal format.");
													}
								""");
								deserializeWithoutProviderBuilder.AppendLine($$"""
													if (columns[{{col}}].Length >= 2 && columns[{{col}}].Span[0] == '"' && decimal.TryParse(columns[{{col}}].Span[1..^1], out decimal v{{propertySymbol.Name}})) {
														item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
													} else if (decimal.TryParse(columns[{{col}}].Span, out v{{propertySymbol.Name}})) {
														item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
													} else if (columns[{{col}}].Length == 0) {
														item.{{propertySymbol.Name}} = null;
													} else {
														throw new CsvFormatException(typeof({{fullTypeName}}), "{{propertySymbol.Name}}", columns[{{col}}].ToString(), "Input string was not in correct decimal format.");
													}
								""");
								break;
							case { SpecialType: SpecialType.System_Boolean }:
								deserializeWithProviderBuilder.AppendLine($$"""
													if (columns[{{col}}].Length >= 2 && columns[{{col}}].Span[0] == '"' && bool.TryParse(columns[{{col}}].Span[1..^1], out bool v{{propertySymbol.Name}})) {
														item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
													} else if (bool.TryParse(columns[{{col}}].Span, out v{{propertySymbol.Name}})) {
														item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
													} else if (columns[{{col}}].Length == 0) {
														item.{{propertySymbol.Name}} = null;
													} else {
														throw new CsvFormatException(typeof({{fullTypeName}}), "{{propertySymbol.Name}}", columns[{{col}}].ToString(), "Input string was not in correct Boolean format.");
													}
								""");
								deserializeWithoutProviderBuilder.AppendLine($$"""
													if (columns[{{col}}].Length >= 2 && columns[{{col}}].Span[0] == '"' && bool.TryParse(columns[{{col}}].Span[1..^1], out bool v{{propertySymbol.Name}})) {
														item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
													} else if (bool.TryParse(columns[{{col}}].Span, out v{{propertySymbol.Name}})) {
														item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
													} else if (columns[{{col}}].Length == 0) {
														item.{{propertySymbol.Name}} = null;
													} else {
														throw new CsvFormatException(typeof({{fullTypeName}}), "{{propertySymbol.Name}}", columns[{{col}}].ToString(), "Input string was not in correct Boolean format.");
													}
								""");
								break;
							case { TypeKind: TypeKind.Enum }: {
									string fullEnumName = GetFullName(underlyingTypeSymbol);
									deserializeWithProviderBuilder.AppendLine($$"""
														if (columns[{{col}}].ToString() is not { Length: > 0 } s{{propertySymbol.Name}}) {
															item.{{propertySymbol.Name}} = null;
														} else if (Enum.TryParse(s{{propertySymbol.Name}}, out {{fullEnumName}} v{{propertySymbol.Name}})) {
															item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
														} else {
															throw new CsvFormatException(typeof({{fullTypeName}}), "{{propertySymbol.Name}}", s{{propertySymbol.Name}}, "Input string was not a valid {{fullEnumName}} value.");
														}
									""");
									deserializeWithoutProviderBuilder.AppendLine($$"""
														if (columns[{{col}}].ToString() is not { Length: > 0 } s{{propertySymbol.Name}}) {
															item.{{propertySymbol.Name}} = null;
														} else if (Enum.TryParse(s{{propertySymbol.Name}}, out {{fullEnumName}} v{{propertySymbol.Name}})) {
															item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
														} else {
															throw new CsvFormatException(typeof({{fullTypeName}}), "{{propertySymbol.Name}}", s{{propertySymbol.Name}}, "Input string was not a valid {{fullEnumName}} value.");
														}
									""");
									break;
								}
							case { SpecialType: SpecialType.System_DateTime }:
								if (dateFormat is not null) {
									deserializeWithProviderBuilder.AppendLine($$"""
														string s{{propertySymbol.Name}} = columns[{{col}}].ToString().Trim();
														if (s{{propertySymbol.Name}}.StartsWith('"')
															&& s{{propertySymbol.Name}}.EndsWith('"')) {
															s{{propertySymbol.Name}} = s{{propertySymbol.Name}}[1..^1];
														}
														if (s{{propertySymbol.Name}}.Length == 0) {
															item.{{propertySymbol.Name}} = null;
														} else if (DateTime.TryParseExact(s{{propertySymbol.Name}}, "{{dateFormat.Replace("\\", "\\\\")}}", provider, DateTimeStyles.AssumeLocal, out DateTime v{{propertySymbol.Name}})) {
															item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
														} else {
															throw new CsvFormatException(typeof({{fullTypeName}}), "{{propertySymbol.Name}}", columns[{{col}}].ToString(), "Input string was not in correct DateTime format. Expected format was '{{dateFormat}}'.");
														}
									""");
									deserializeWithoutProviderBuilder.AppendLine($$"""
														string s{{propertySymbol.Name}} = columns[{{col}}].ToString().Trim();
														if (s{{propertySymbol.Name}}.StartsWith('"')
															&& s{{propertySymbol.Name}}.EndsWith('"')) {
															s{{propertySymbol.Name}} = s{{propertySymbol.Name}}[1..^1];
														}
														if (s{{propertySymbol.Name}}.Length == 0) {
															item.{{propertySymbol.Name}} = null;
														} else if (DateTime.TryParseExact(s{{propertySymbol.Name}}, "{{dateFormat.Replace("\\", "\\\\")}}", null, DateTimeStyles.AssumeLocal, out DateTime v{{propertySymbol.Name}})) {
															item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
														} else {
															throw new CsvFormatException(typeof({{fullTypeName}}), "{{propertySymbol.Name}}", columns[{{col}}].ToString(), "Input string was not in correct DateTime format. Expected format was '{{dateFormat}}'.");
														}
									""");
								} else {
									deserializeWithProviderBuilder.AppendLine($$"""
														string s{{propertySymbol.Name}} = columns[{{col}}].ToString().Trim();
														if (s{{propertySymbol.Name}}.StartsWith('"')
															&& s{{propertySymbol.Name}}.EndsWith('"')) {
															s{{propertySymbol.Name}} = s{{propertySymbol.Name}}[1..^1];
														}
														if (s{{propertySymbol.Name}}.Length == 0) {
															item.{{propertySymbol.Name}} = null;
														} else if (DateTime.TryParse(s{{propertySymbol.Name}}, provider, DateTimeStyles.AssumeLocal, out DateTime v{{propertySymbol.Name}})) {
															item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
														} else {
															throw new CsvFormatException(typeof({{fullTypeName}}), "{{propertySymbol.Name}}", columns[{{col}}].ToString(), "Input string was not in correct DateTime format.");
														}
									""");
									deserializeWithoutProviderBuilder.AppendLine($$"""
														string s{{propertySymbol.Name}} = columns[{{col}}].ToString().Trim();
														if (s{{propertySymbol.Name}}.StartsWith('"')
															&& s{{propertySymbol.Name}}.EndsWith('"')) {
															s{{propertySymbol.Name}} = s{{propertySymbol.Name}}[1..^1];
														}
														if (s{{propertySymbol.Name}}.Length == 0) {
															item.{{propertySymbol.Name}} = null;
														} else if (DateTime.TryParse(s{{propertySymbol.Name}}, null, DateTimeStyles.AssumeLocal, out DateTime v{{propertySymbol.Name}})) {
															item.{{propertySymbol.Name}} = v{{propertySymbol.Name}};
														} else {
															throw new CsvFormatException(typeof({{fullTypeName}}), "{{propertySymbol.Name}}", columns[{{col}}].ToString(), "Input string was not in correct DateTime format.");
														}
									""");
								}
								break;
#if DEBUG
							default:
								foreach (Location invocationLocation in invocationLocations) {
									context.ReportDiagnostic(
										Diagnostic.Create(
											descriptor: new(
												id: "CSV0001",
												title: "Unsupported property type",
												messageFormat: "{0} is not supported by CsvSerializer.",
												category: "Usage",
												defaultSeverity: DiagnosticSeverity.Error,
												isEnabledByDefault: true
											),
											location: invocationLocation,
											messageArgs: underlyingTypeSymbol.Name
										)
									);
								}
								throw new DiagnosticReportedException();
#endif
						}
						break;
#if DEBUG
					default:
						foreach (Location invocationLocation in invocationLocations) {
							context.ReportDiagnostic(
								Diagnostic.Create(
									descriptor: new(
										id: "CSV0001",
										title: "Unsupported property type",
										messageFormat: "{0} is not supported by CsvSerializer.",
										category: "Usage",
										defaultSeverity: DiagnosticSeverity.Error,
										isEnabledByDefault: true
									),
									location: invocationLocation,
									messageArgs: propertyTypeSymbol.Name
								)
							);
						}
						throw new DiagnosticReportedException();
#endif
				}
				col++;
			}

			context.AddSource(
				hintName: $"{serializerName}.g.cs",
				sourceText: SourceText.From(
					text: $$"""
					#nullable enable
					using System;
					using System.IO;
					using System.Text;

					namespace Csv.Internal.NativeImpl {
						internal sealed class {{serializerName}} : ISerializer {
							public void SerializeHeader(char delimiter, StringBuilder stringBuilder) {
								{{serializeHeaderBuilder.ToString().Trim()}}
								stringBuilder.Append("\r\n");
							}

							public void SerializeHeader(char delimiter, StreamWriter streamWriter) {
								{{writeHeaderBuilder.ToString().Trim()}}
								streamWriter.Write("\r\n");
							}

							public void SerializeItem(IFormatProvider? provider, char delimiter, StringBuilder stringBuilder, object item) {
								{{fullTypeName}} i = ({{fullTypeName}})item;
								if (provider is null) {
									{{serializeItemWithoutProviderBuilder.ToString().Trim()}}
								} else {
									{{serializeItemWithProviderBuilder.ToString().Trim()}}
								}
								stringBuilder.Append("\r\n");
							}

							public void SerializeItem(IFormatProvider? provider, char delimiter, StreamWriter streamWriter, object item) {
								{{fullTypeName}} i = ({{fullTypeName}})item;
								if (provider is null) {
									{{writeItemWithoutProviderBuilder.ToString().Trim()}}
								} else {
									{{writeItemWithProviderBuilder.ToString().Trim()}}
								}
								streamWriter.Write("\r\n");
							}
						}
					}

					""",
					encoding: Encoding.UTF8
				)
			);

			context.AddSource(
				hintName: $"{deserializerName}.g.cs",
				sourceText: SourceText.From(
					text: $$"""
					#nullable enable
					using System;
					using System.Collections.Generic;
					using System.Globalization;
					using System.IO;

					namespace Csv.Internal.NativeImpl {
						internal sealed class {{deserializerName}} : IDeserializer {
							public List<object> Deserialize(IFormatProvider? provider, char delimiter, bool skipHeader, ReadOnlyMemory<char> csv) {
								List<object> items = new();
								Span<ReadOnlyMemory<char>> columns = new ReadOnlyMemory<char>[{{propertySymbols.Count}}];
								bool firstRow = true;
								while (csv.Length > 0) {
									try {
										int columnsRead = NativeStringSplitter.ReadNextLine(ref csv, ref columns, delimiter);
										if (columnsRead != {{propertySymbols.Count}}) {
											int endOfLine = csv.Span.IndexOf('\n');
											string line = endOfLine == -1 ? csv.ToString() : csv.Slice(0, endOfLine).ToString();
											throw new CsvFormatException(typeof({{fullTypeName}}), line, "Row must consists of {{propertySymbols.Count}} columns.");
										}
									} catch (IndexOutOfRangeException) {
										int endOfLine = csv.Span.IndexOf('\n');
										string line = endOfLine == -1 ? csv.ToString() : csv.Slice(0, endOfLine).ToString();
										throw new CsvFormatException(typeof({{fullTypeName}}), line, "Row must consists of {{propertySymbols.Count}} columns.");
									}
									if (firstRow) {
										firstRow = false;
										if (skipHeader) {
											continue;
										}
									}
									{{fullTypeName}} item = Activator.CreateInstance<{{fullTypeName}}>();
									if (provider is null) {
										{{deserializeWithoutProviderBuilder.ToString().Trim()}}
									} else {
										{{deserializeWithProviderBuilder.ToString().Trim()}}
									}
									items.Add(item);
								}
								return items;
							}

							public IEnumerable<object> Deserialize(IFormatProvider? provider, char delimiter, bool skipHeader, StreamReader csvReader) {
								bool firstRow = true;
								while (!csvReader.EndOfStream) {
									string line = csvReader.ReadLine()!;
									if (_(line, ref firstRow, out {{fullTypeName}}? item)) {
										yield return line;
									}

									bool _(string line, ref bool firstRow, out {{fullTypeName}}? item) {
										Span<ReadOnlyMemory<char>> columns = new ReadOnlyMemory<char>[{{propertySymbols.Count}}];
										ReadOnlyMemory<char> lineMemory = line.AsMemory();
										try {
											int columnsRead = NativeStringSplitter.ReadNextLine(ref lineMemory, ref columns, delimiter);
											if (columnsRead != {{propertySymbols.Count}}) {
												throw new CsvFormatException(typeof({{fullTypeName}}), line, "Row must consists of {{propertySymbols.Count}} columns.");
											}
										} catch (IndexOutOfRangeException) {
											throw new CsvFormatException(typeof({{fullTypeName}}), line, "Row must consists of {{propertySymbols.Count}} columns.");
										}
										if (firstRow) {
											firstRow = false;
											if (skipHeader) {
												item = null;
												return false;
											}
										}
										item = Activator.CreateInstance<{{fullTypeName}}>();
										if (provider is null) {
											{{readWithoutProviderBuilder.ToString().Trim()}}
										} else {
											{{readWithProviderBuilder.ToString().Trim()}}
										}
										return true;
									}
								}
							}
						}
					}

					""",
					encoding: Encoding.UTF8
				)
			);

			return (
				FullTypeName: fullTypeName,
				SerializerName: serializerName,
				DeserializerName: deserializerName
			);
		}

		private static string GetFullName(ITypeSymbol typeSymbol) {
			INamespaceSymbol? @namespace;
			string fullName;
			if (typeSymbol.ContainingType is { } containingType) {
				fullName = $"{containingType.Name}.{typeSymbol.Name}";
				@namespace = containingType.ContainingNamespace;
			} else {
				fullName = typeSymbol.Name;
				@namespace = typeSymbol.ContainingNamespace;
			}

			while (@namespace is { Name.Length: > 0 }) {
				fullName = $"{@namespace.Name}.{fullName}";
				@namespace = @namespace.ContainingNamespace;
			}

			return fullName;
		}
	}
}
