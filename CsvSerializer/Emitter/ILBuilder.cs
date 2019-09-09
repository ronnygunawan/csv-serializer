using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Csv.Emitter {
	public class ILBuilder {
		private readonly ILGenerator _ilGenerator;

		public ILBuilder(ILGenerator ilGenerator) {
			_ilGenerator = ilGenerator;
		}

		public Label DefineLabel() { return _ilGenerator.DefineLabel(); }
		public ILBuilder DeclareLocal(Type type) { _ilGenerator.DeclareLocal(type); return this; }
		public ILBuilder DeclareLocal(Type type, out LocalBuilder localBuilder) { localBuilder = _ilGenerator.DeclareLocal(type); return this; }
		public ILBuilder DeclareLocalIf(bool condition, Type type, out LocalBuilder? localBuilder) {
			if (condition) {
				localBuilder = _ilGenerator.DeclareLocal(type);
			} else {
				localBuilder = null;
			}
			return this;
		}
		public ILBuilder DeclareLocalIfRequired(PropertyInfo[] properties, Type type, out LocalBuilder? localBuilder, out LocalBuilder? nullableLocalBuilder) {
			if (properties.Any(prop => prop.PropertyType == type || Nullable.GetUnderlyingType(prop.PropertyType) == type)) {
				localBuilder = _ilGenerator.DeclareLocal(type);
				if (properties.FirstOrDefault(prop => Nullable.GetUnderlyingType(prop.PropertyType) == type) is PropertyInfo nullableProperty) {
					nullableLocalBuilder = _ilGenerator.DeclareLocal(nullableProperty.PropertyType);
				} else {
					nullableLocalBuilder = null;
				}
			} else {
				localBuilder = null;
				nullableLocalBuilder = null;
			}
			return this;
		}
		public ILBuilder DeclareLocalIfRequired(PropertyInfo[] properties, Type type, out LocalBuilder? localBuilder) {
			if (properties.Any(prop => prop.PropertyType == type || Nullable.GetUnderlyingType(prop.PropertyType) == type)) {
				localBuilder = _ilGenerator.DeclareLocal(type);
			} else {
				localBuilder = null;
			}
			return this;
		}
		public ILBuilder Nop { get { _ilGenerator.Emit(OpCodes.Nop); return this; } }
		public ILBuilder Ldnull { get { _ilGenerator.Emit(OpCodes.Ldnull); return this; } }
		public ILBuilder Ldarg(LocalBuilder localBuilder) { _ilGenerator.Emit(OpCodes.Ldarg, localBuilder); return this; }
		public ILBuilder Ldarg(int i) {
			switch (i) {
				case 0: _ilGenerator.Emit(OpCodes.Ldarg_0); break;
				case 1: _ilGenerator.Emit(OpCodes.Ldarg_1); break;
				case 2: _ilGenerator.Emit(OpCodes.Ldarg_2); break;
				case 3: _ilGenerator.Emit(OpCodes.Ldarg_3); break;
				default: _ilGenerator.Emit(OpCodes.Ldarg, i); break;
			}
			return this;
		}
		public ILBuilder Ldarg_0 { get { _ilGenerator.Emit(OpCodes.Ldarg_0); return this; } }
		public ILBuilder Ldarg_1 { get { _ilGenerator.Emit(OpCodes.Ldarg_1); return this; } }
		public ILBuilder Ldarg_2 { get { _ilGenerator.Emit(OpCodes.Ldarg_2); return this; } }
		public ILBuilder Ldarg_3 { get { _ilGenerator.Emit(OpCodes.Ldarg_3); return this; } }
		public ILBuilder Ldstr(string s) { _ilGenerator.Emit(OpCodes.Ldstr, s); return this; }
		public ILBuilder Ldtoken(Type t) { _ilGenerator.Emit(OpCodes.Ldtoken, t); return this; }
		public ILBuilder Box(Type type) { _ilGenerator.Emit(OpCodes.Box, type); return this; }
		public ILBuilder Call(MethodInfo methodInfo) { _ilGenerator.Emit(OpCodes.Call, methodInfo); return this; }
		public ILBuilder Callvirt(MethodInfo methodInfo) { _ilGenerator.Emit(OpCodes.Callvirt, methodInfo); return this; }
		public ILBuilder Callx(MethodInfo methodInfo) {
			if (methodInfo.IsVirtual) {
				_ilGenerator.Emit(OpCodes.Callvirt, methodInfo);
			} else {
				_ilGenerator.Emit(OpCodes.Call, methodInfo);
			}
			return this;
		}
		public ILBuilder Newobj(ConstructorInfo constructorInfo) { _ilGenerator.Emit(OpCodes.Newobj, constructorInfo); return this; }
		public ILBuilder Initobj(Type t) { _ilGenerator.Emit(OpCodes.Initobj, t); return this; }
		public ILBuilder Stloc(LocalBuilder? local) {
			_ilGenerator.Emit(OpCodes.Stloc, local ?? throw new InvalidOperationException("local is not declared"));
			return this;
		}
		public ILBuilder Stloc(int i) {
			switch (i) {
				case 0: _ilGenerator.Emit(OpCodes.Stloc_0); break;
				case 1: _ilGenerator.Emit(OpCodes.Stloc_1); break;
				case 2: _ilGenerator.Emit(OpCodes.Stloc_2); break;
				case 3: _ilGenerator.Emit(OpCodes.Stloc_3); break;
				default: _ilGenerator.Emit(OpCodes.Stloc, i); break;
			}
			return this;
		}
		public ILBuilder Stloc_0 { get { _ilGenerator.Emit(OpCodes.Stloc_0); return this; } }
		public ILBuilder Stloc_1 { get { _ilGenerator.Emit(OpCodes.Stloc_1); return this; } }
		public ILBuilder Stloc_2 { get { _ilGenerator.Emit(OpCodes.Stloc_2); return this; } }
		public ILBuilder Stloc_3 { get { _ilGenerator.Emit(OpCodes.Stloc_3); return this; } }
		public ILBuilder Stloc_S(int i) {
			switch (i) {
				case 0: _ilGenerator.Emit(OpCodes.Stloc_0); break;
				case 1: _ilGenerator.Emit(OpCodes.Stloc_1); break;
				case 2: _ilGenerator.Emit(OpCodes.Stloc_2); break;
				case 3: _ilGenerator.Emit(OpCodes.Stloc_3); break;
				default: _ilGenerator.Emit(OpCodes.Stloc_S, i); break;
			}
			return this;
		}
		public ILBuilder Ldloc(int i) {
			switch (i) {
				case 0: _ilGenerator.Emit(OpCodes.Ldloc_0); break;
				case 1: _ilGenerator.Emit(OpCodes.Ldloc_1); break;
				case 2: _ilGenerator.Emit(OpCodes.Ldloc_2); break;
				case 3: _ilGenerator.Emit(OpCodes.Ldloc_3); break;
				default: _ilGenerator.Emit(OpCodes.Ldloc, i); break;
			}
			return this;
		}
		public ILBuilder Ldloc_0 { get { _ilGenerator.Emit(OpCodes.Ldloc_0); return this; } }
		public ILBuilder Ldloc_1 { get { _ilGenerator.Emit(OpCodes.Ldloc_1); return this; } }
		public ILBuilder Ldloc_2 { get { _ilGenerator.Emit(OpCodes.Ldloc_2); return this; } }
		public ILBuilder Ldloc_3 { get { _ilGenerator.Emit(OpCodes.Ldloc_3); return this; } }
		public ILBuilder Ldloc(LocalBuilder? local) {
			_ilGenerator.Emit(OpCodes.Ldloc, local ?? throw new InvalidOperationException("local is not declared"));
			return this;
		}
		public ILBuilder Ldc_I4(int i) {
			switch (i) {
				case -1: _ilGenerator.Emit(OpCodes.Ldc_I4_M1); break;
				case 0: _ilGenerator.Emit(OpCodes.Ldc_I4_0); break;
				case 1: _ilGenerator.Emit(OpCodes.Ldc_I4_1); break;
				case 2: _ilGenerator.Emit(OpCodes.Ldc_I4_2); break;
				case 3: _ilGenerator.Emit(OpCodes.Ldc_I4_3); break;
				case 4: _ilGenerator.Emit(OpCodes.Ldc_I4_4); break;
				case 5: _ilGenerator.Emit(OpCodes.Ldc_I4_5); break;
				case 6: _ilGenerator.Emit(OpCodes.Ldc_I4_6); break;
				case 7: _ilGenerator.Emit(OpCodes.Ldc_I4_7); break;
				case 8: _ilGenerator.Emit(OpCodes.Ldc_I4_8); break;
				default: _ilGenerator.Emit(OpCodes.Ldc_I4, i); break;
			}
			return this;
		}
		public ILBuilder Ldloca(int s) {
			_ilGenerator.Emit(OpCodes.Ldloca, s);
			return this;
		}
		public ILBuilder Ldloca_S(int s) {
			_ilGenerator.Emit(OpCodes.Ldloca_S, s);
			return this;
		}
		public ILBuilder Ldloc_S(int s) {
			_ilGenerator.Emit(OpCodes.Ldloc_S, s);
			return this;
		}
		public ILBuilder Ldloc_S(LocalBuilder? localBuilder) {
			_ilGenerator.Emit(OpCodes.Ldloc_S, localBuilder!);
			return this;
		}
		public ILBuilder Ldc_I4_S(char c) {
			_ilGenerator.Emit(OpCodes.Ldc_I4_S, c);
			return this;
		}
		public ILBuilder Ceq { get { _ilGenerator.Emit(OpCodes.Ceq); return this; } }
		public ILBuilder Sub { get { _ilGenerator.Emit(OpCodes.Sub); return this; } }
		public ILBuilder Br(out Label label) {
			label = _ilGenerator.DefineLabel();
			_ilGenerator.Emit(OpCodes.Br, label);
			return this;
		}
		public ILBuilder Br(Label label) {
			_ilGenerator.Emit(OpCodes.Br, label);
			return this;
		}
		public ILBuilder Br_S(out Label label) {
			label = _ilGenerator.DefineLabel();
			_ilGenerator.Emit(OpCodes.Br_S, label);
			return this;
		}
		public ILBuilder Br_S(Label label) {
			_ilGenerator.Emit(OpCodes.Br_S, label);
			return this;
		}
		public ILBuilder Brtrue(out Label label) {
			label = _ilGenerator.DefineLabel();
			_ilGenerator.Emit(OpCodes.Brtrue, label);
			return this;
		}
		public ILBuilder Brtrue(Label label) {
			_ilGenerator.Emit(OpCodes.Brtrue, label);
			return this;
		}
		public ILBuilder Brtrue_S(out Label label) {
			label = _ilGenerator.DefineLabel();
			_ilGenerator.Emit(OpCodes.Brtrue_S, label);
			return this;
		}
		public ILBuilder Brtrue_S(Label label) {
			_ilGenerator.Emit(OpCodes.Brtrue_S, label);
			return this;
		}
		public ILBuilder Brfalse(out Label label) {
			label = _ilGenerator.DefineLabel();
			_ilGenerator.Emit(OpCodes.Brfalse, label);
			return this;
		}
		public ILBuilder Brfalse(Label label) {
			_ilGenerator.Emit(OpCodes.Brfalse, label);
			return this;
		}
		public ILBuilder Brfalse_S(out Label label) {
			label = _ilGenerator.DefineLabel();
			_ilGenerator.Emit(OpCodes.Brfalse_S, label);
			return this;
		}
		public ILBuilder Brfalse_S(Label label) {
			_ilGenerator.Emit(OpCodes.Brfalse_S, label);
			return this;
		}
		public ILBuilder MarkLabel(out Label label) {
			label = _ilGenerator.DefineLabel();
			_ilGenerator.MarkLabel(label);
			return this;
		}
		public ILBuilder MarkLabel(Label label) {
			_ilGenerator.MarkLabel(label);
			return this;
		}
		public ILBuilder Throw { get { _ilGenerator.Emit(OpCodes.Throw); return this; } }
		public ILBuilder Dup { get { _ilGenerator.Emit(OpCodes.Dup); return this; } }
		public ILBuilder Pop { get { _ilGenerator.Emit(OpCodes.Pop); return this; } }
		public ILBuilder Ret { get { _ilGenerator.Emit(OpCodes.Ret); return this; } }
		public ILBuilder Do(Action<ILBuilder> action) { action.Invoke(this); return this; }
		public ILBuilder Do(Func<ILBuilder, ILBuilder> action) { action.Invoke(this); return this; }
	}
}
