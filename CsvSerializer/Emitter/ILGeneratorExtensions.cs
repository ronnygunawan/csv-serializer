using System;
using Missil;
using System.Reflection.Emit;

namespace Csv.Emitter {
	internal static class ILGeneratorExtensions {
		public static ILGenerator Do(this ILGenerator gen, Action<ILGenerator> action) { action.Invoke(gen); return gen; }
		public static ILGenerator DeclareLocalIf<T>(this ILGenerator gen, bool condition, out LocalBuilder? local) {
			if (condition) {
				return gen.DeclareLocal<T>(out local);
			} else {
				local = null;
				return gen;
			}
		}
	}
}
