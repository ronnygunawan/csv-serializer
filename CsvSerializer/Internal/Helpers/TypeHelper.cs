namespace Csv.Internal.Helpers {
	internal static class TypeHelper {
		public static bool IsPublicAndIsNotAnonymous<T>() =>
			typeof(T).Name is string className
			&& (className.Length < 15
			|| className[0] != '<'
			|| className[1] != '>')
			&& typeof(T).IsPublic;

		public static bool IsInternalOrAnonymous<T>() => !IsPublicAndIsNotAnonymous<T>();
	}
}
