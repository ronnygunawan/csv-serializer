using FluentAssertions;
using FluentAssertions.Primitives;

namespace Tests.Utilities {
	internal static class FluentAssertionsExtensions {
		public static AndConstraint<StringAssertions> BeSimilarTo(this StringAssertions assertions, string expected, string? because = null, params object[] becauseArgs) {
			return assertions.Subject.Replace("\r\n", "\n").Replace("\r", "\n")
				.Should().Be(expected.Replace("\r\n", "\n").Replace("\r", "\n"), because, becauseArgs);
		}
	}
}
