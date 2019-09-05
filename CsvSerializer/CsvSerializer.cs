using System;
using System.Collections.Generic;
using System.Linq;
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
			IDeserializer deserializer = SerializerFactory.GetOrCreateDeserializer<T>();
			List<object> items = deserializer.Deserialize(csv.AsSpan(), separator, hasHeaders);
			return items.Cast<T>().ToArray();
		}
	}
}
