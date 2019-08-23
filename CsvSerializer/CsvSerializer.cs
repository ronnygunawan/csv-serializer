using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;

#if DEBUG
[assembly: InternalsVisibleTo("Tests")]
#endif
[assembly: InternalsVisibleTo("CsvSerializer.Dynamic")]
[assembly: AllowPartiallyTrustedCallers]
[assembly: SecurityTransparent]
[assembly: SecurityRules(SecurityRuleSet.Level2, SkipVerificationInFullTrust = true)]
namespace Csv {
	public static class CsvSerializer {
		private static readonly string[] NEWLINES = { "\r\n", "\n" };

		public static string Serialize<T>(IEnumerable<T> items, bool withHeaders = false, char separator = ',') where T : notnull {
			ISerializer serializer = SerializerFactory.GetOrCreateSerializer<T>();
			StringBuilder stringBuilder = new StringBuilder();
			if (withHeaders) {
				serializer.SerializeHeader(stringBuilder, separator);
			}
			foreach (T item in items) {
				serializer.SerializeItem(stringBuilder, item, separator);
			}
			return stringBuilder.ToString().TrimEnd();
		}

		public static T[] Deserialize<T>(string csv, bool hasHeaders = false, char separator = ',') where T : notnull {
			string[] lines = csv.Trim().Split(NEWLINES, StringSplitOptions.None);
			IDeserializer deserializer = SerializerFactory.GetOrCreateDeserializer<T>();
			if (hasHeaders) {
				T[] items = new T[lines.Length - 1];
				for (int i = 1; i < lines.Length; i++) {
					items[i - 1] = (T)deserializer.DeserializeItem(lines[i], separator);
				}
				return items;
			} else {
				T[] items = new T[lines.Length];
				for (int i = 0; i < lines.Length; i++) {
					items[i] = (T)deserializer.DeserializeItem(lines[i], separator);
				}
				return items;
			}
		}
	}
}
