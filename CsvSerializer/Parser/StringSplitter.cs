using System.Collections.Generic;

namespace Csv.Parser {
	internal static class StringSplitter {
		public static List<string> SplitLine(string line, char separator = ',') {
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
