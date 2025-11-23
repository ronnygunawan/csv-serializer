using System;
using System.Collections.Generic;
using System.Linq;
using Csv;
using Shouldly;
using Xunit;

namespace Tests {
	public sealed class GeneratedSourceTests {
		[Fact]
		public void GeneratedClassesAreDeclaredInternal() {
			List<Type> generatedTypes = (
				from type in typeof(ISerializer).Assembly.GetTypes()
				where type.Namespace != null && type.Namespace.StartsWith("Csv")
				select type
			).ToList();

			foreach (Type generatedType in generatedTypes) {
				generatedType.IsVisible.ShouldBeFalse();
			}
		}
	}
}
