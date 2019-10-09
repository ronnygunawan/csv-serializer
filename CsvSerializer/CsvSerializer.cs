using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;

//#if DEBUG
[assembly: InternalsVisibleTo("Tests")]
//#endif
[assembly: InternalsVisibleTo("CsvSerializer.Dynamic")]
[assembly: AllowPartiallyTrustedCallers]
[assembly: SecurityTransparent]
[assembly: SecurityRules(SecurityRuleSet.Level2, SkipVerificationInFullTrust = true)]
namespace Csv {
	public static class CsvSerializer {
		public static string Serialize<T>(IEnumerable<T> items, bool withHeaders = false, char delimiter = ',', IFormatProvider? provider = null) where T : notnull {
			ISerializer serializer = Internal.NativeImpl.SerializerFactory.GetOrCreate<T>();
			StringBuilder stringBuilder = new StringBuilder();
			if (withHeaders) {
				serializer.SerializeHeader(delimiter, stringBuilder);
			}
			foreach (T item in items) {
				serializer.SerializeItem(provider ?? CultureInfo.CurrentCulture, delimiter, stringBuilder, item);
			}
			return stringBuilder.ToString().TrimEnd();
		}

		public static T[] Deserialize<T>(string csv, bool hasHeaders = false, char delimiter = ',', IFormatProvider? provider = null) where T : notnull {
			IDeserializer deserializer = Internal.NativeImpl.DeserializerFactory.GetOrCreate<T>();
			List<object> items = deserializer.Deserialize(provider ?? CultureInfo.CurrentCulture, delimiter, hasHeaders, csv.AsMemory());
			return items.Cast<T>().ToArray();
		}
	}
}
