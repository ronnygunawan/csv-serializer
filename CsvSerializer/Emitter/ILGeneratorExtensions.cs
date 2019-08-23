using System.Reflection.Emit;

namespace Csv.Emitter {
	internal static class ILGeneratorExtensions {
		public static ILBuilder EmitFollowingLines(this ILGenerator gen) => new ILBuilder(gen);
	}
}
