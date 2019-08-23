using System;
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
					if (char.IsLetterOrDigit(c) || c == '.' || c == '-' || c == '+') {
						goto IN_VALUE_LITERAL;
					} else {
						throw new FormatException($"Invalid token: {c}");
					}
			}

		IN_STRING_LITERAL:
			{
				startOfLiteral = i++;
				bool inEscapeCharacter = false;
			IN_STRING_LITERAL_LOOP:
				if (i == line.Length) {
					throw new FormatException("Newline in string literal.");
				}
				if (inEscapeCharacter) {
					inEscapeCharacter = false;
				} else {
					switch (line[i]) {
						case '\\':
							inEscapeCharacter = true;
							break;
						case '"':
							endOfLiteral = ++i;
							goto HAS_LITERAL_LOOP;
					}
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
							throw new FormatException("Invalid value literal.");
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
						throw new FormatException("A column can't have multiple literals.");
					}
			}
		}
	}
}
