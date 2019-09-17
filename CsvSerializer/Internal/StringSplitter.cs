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
			List<ReadOnlyMemory<char>> columns = new List<ReadOnlyMemory<char>>();
			int startOfLiteral = 0;
			int endOfLiteral = 0;
			ParserState state = ParserState.InStartingWhiteSpace;
			for (int i = 0, length = csv.Length; i <= length; i++) {
				if (i == length) {
					switch (state) {
						case ParserState.InStartingWhiteSpace:
						case ParserState.InUnquotedValue:
						case ParserState.InEscapeSequence:
							columns.Add(csv.Slice(startOfLiteral, i - startOfLiteral));
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
									string line = endOfLine == -1 ? csv.ToString() : csv.Slice(0, endOfLine).ToString();
									throw new CsvFormatException(line, $"Invalid character at position {i}: \"");
								case ParserState.InQuotedValue:
									state = ParserState.InEscapeSequence;
									break;
								case ParserState.InEscapeSequence:
									state = ParserState.InQuotedValue;
									break;
								case ParserState.InTrailingWhiteSpace:
									endOfLine = span.IndexOf('\n');
									line = endOfLine == -1 ? csv.ToString() : csv.Slice(0, endOfLine).ToString();
									throw new CsvFormatException(line, $"Invalid character at position {i}: \"");
							}
							break;
						case char c when c == separator:
							switch (state) {
								case ParserState.InStartingWhiteSpace:
								case ParserState.InUnquotedValue:
								case ParserState.InEscapeSequence:
									columns.Add(csv.Slice(startOfLiteral, i - startOfLiteral));
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
									columns.Add(csv.Slice(startOfLiteral, i - startOfLiteral));
									csv = csv.Slice(i + 1);
									return columns;
								case ParserState.InTrailingWhiteSpace:
									columns.Add(csv.Slice(startOfLiteral, endOfLiteral - startOfLiteral + 1));
									csv = csv.Slice(i + 1);
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
										string line = endOfLine == -1 ? csv.ToString() : csv.Slice(0, endOfLine).ToString();
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

		public static List<string> OldSplitLine(string line, char separator = ',') {
			List<string> columns = new List<string>();
			int i = 0;
			int startOfLiteral;
			int endOfLiteral;

		EXPECTING_TOKEN:
			if (i == line.Length) {
				columns.Add(string.Empty);
				return columns;
			}
			switch (line[i]) {
				case ' ':
					i++;
					goto EXPECTING_TOKEN;
				case '"':
					goto IN_STRING_LITERAL;
				case char c:
					if (c == separator) {
						columns.Add(string.Empty);
						i++;
						goto EXPECTING_TOKEN;
					} else if (char.IsLetterOrDigit(c) || c == '.' || c == '-' || c == '+') {
						goto IN_VALUE_LITERAL;
					} else {
						throw new CsvFormatException(line, $"Invalid character at position {i}: {c}");
					}
			}

		IN_STRING_LITERAL:
			{
				startOfLiteral = i++;
				bool maybeInEscapeCharacter = false;
			IN_STRING_LITERAL_LOOP:
				if (i == line.Length) {
					if (maybeInEscapeCharacter) {
						columns.Add(line[startOfLiteral..i]);
						return columns;
					} else {
						throw new CsvFormatException(line, "Newline in string literal.");
					}
				}
				if (maybeInEscapeCharacter) {
					maybeInEscapeCharacter = false;
					if (line[i] != '"') {
						endOfLiteral = i;
						goto HAS_LITERAL_LOOP;
					}
				} else if (line[i] == '"') {
					maybeInEscapeCharacter = true;
				}
				i++;
				goto IN_STRING_LITERAL_LOOP;
			}

		IN_VALUE_LITERAL:
			{
				startOfLiteral = i++;
			IN_VALUE_LITERAL_LOOP:
				if (i == line.Length) {
					columns.Add(string.Empty);
					return columns;
				}
				switch (line[i]) {
					case ' ':
						endOfLiteral = i++;
						goto HAS_LITERAL_LOOP;
					case char c:
						if (c == separator) {
							columns.Add(line[startOfLiteral..i]);
							i++;
							goto EXPECTING_TOKEN;
						} else if (char.IsLetterOrDigit(c) || c == '.' || c == '-' || c == '+') {
							i++;
							goto IN_VALUE_LITERAL_LOOP;
						} else {
							throw new CsvFormatException(line, $"Invalid character at position {i}: {c}");
						}
				}
			}

		HAS_LITERAL_LOOP:
			if (i == line.Length) {
				columns.Add(line[startOfLiteral..endOfLiteral]);
				return columns;
			}
			switch (line[i]) {
				case ' ':
					i++;
					goto HAS_LITERAL_LOOP;
				case char c:
					if (c == separator) {
						columns.Add(line[startOfLiteral..endOfLiteral]);
						i++;
						goto EXPECTING_TOKEN;
					} else {
						throw new CsvFormatException(line, $"Invalid character at position {i}: {c}");
					}
			}
		}
	}
}
