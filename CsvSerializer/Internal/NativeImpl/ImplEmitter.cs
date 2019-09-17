using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Csv.Internal.NativeImpl {
	internal class ImplEmitter<TInterface> where TInterface : class {
		private readonly TypeBuilder _typeBuilder;
		private Type? _type;

		public ImplEmitter(string className) {
			AssemblyName assemblyName = new AssemblyName("CsvSerializer.Dynamic");
			AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndCollect);
			ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("CsvSerializer.DynamicModule");
			_typeBuilder = moduleBuilder.DefineType(
				name: className,
				attr: TypeAttributes.Public |
					TypeAttributes.Class |
					TypeAttributes.AutoClass |
					TypeAttributes.AnsiClass |
					TypeAttributes.BeforeFieldInit |
					TypeAttributes.AutoLayout,
				parent: null,
				interfaces: new Type[] { typeof(TInterface) });
		}

		public void ImplementAction(string methodName, Action<ILGenerator> ilGenerator) {
			if (_type != null) {
				throw new InvalidOperationException("Type already created.");
			}
			MethodBuilder methodBuilder = _typeBuilder.DefineMethod(methodName,
				MethodAttributes.Public |
				MethodAttributes.Virtual,
				typeof(void),
				new Type[] { });
			ILGenerator gen = methodBuilder.GetILGenerator();
			ilGenerator.Invoke(gen);
			_typeBuilder.DefineMethodOverride(methodBuilder, typeof(TInterface).GetMethod(methodName, new Type[] { })!);
		}

		public void ImplementAction<T1>(string methodName, Action<ILGenerator> ilGenerator) {
			if (_type != null) {
				throw new InvalidOperationException("Type already created.");
			}
			MethodBuilder methodBuilder = _typeBuilder.DefineMethod(methodName,
				MethodAttributes.Public |
				MethodAttributes.Virtual,
				typeof(void),
				new Type[] { typeof(T1) });
			ILGenerator gen = methodBuilder.GetILGenerator();
			ilGenerator.Invoke(gen);
			_typeBuilder.DefineMethodOverride(methodBuilder, typeof(TInterface).GetMethod(methodName, new Type[] { typeof(T1) })!);
		}

		public void ImplementAction<T1, T2>(string methodName, Action<ILGenerator> ilGenerator) {
			if (_type != null) {
				throw new InvalidOperationException("Type already created.");
			}
			MethodBuilder methodBuilder = _typeBuilder.DefineMethod(methodName,
				MethodAttributes.Public |
				MethodAttributes.Virtual,
				typeof(void),
				new Type[] { typeof(T1), typeof(T2) });
			ILGenerator gen = methodBuilder.GetILGenerator();
			ilGenerator.Invoke(gen);
			_typeBuilder.DefineMethodOverride(methodBuilder, typeof(TInterface).GetMethod(methodName, new Type[] { typeof(T1), typeof(T2) })!);
		}

		public void ImplementAction<T1, T2, T3>(string methodName, Action<ILGenerator> ilGenerator) {
			if (_type != null) {
				throw new InvalidOperationException("Type already created.");
			}
			MethodBuilder methodBuilder = _typeBuilder.DefineMethod(methodName,
				MethodAttributes.Public |
				MethodAttributes.Virtual,
				typeof(void),
				new Type[] { typeof(T1), typeof(T2), typeof(T3) });
			ILGenerator gen = methodBuilder.GetILGenerator();
			ilGenerator.Invoke(gen);
			_typeBuilder.DefineMethodOverride(methodBuilder, typeof(TInterface).GetMethod(methodName, new Type[] { typeof(T1), typeof(T2), typeof(T3) })!);
		}

		public void ImplementAction<T1, T2, T3, T4>(string methodName, Action<ILGenerator> ilGenerator) {
			if (_type != null) {
				throw new InvalidOperationException("Type already created.");
			}
			MethodBuilder methodBuilder = _typeBuilder.DefineMethod(methodName,
				MethodAttributes.Public |
				MethodAttributes.Virtual,
				typeof(void),
				new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) });
			ILGenerator gen = methodBuilder.GetILGenerator();
			ilGenerator.Invoke(gen);
			_typeBuilder.DefineMethodOverride(methodBuilder, typeof(TInterface).GetMethod(methodName, new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) })!);
		}

		public void ImplementFunc<TReturn>(string methodName, Action<ILGenerator> ilGenerator) {
			if (_type != null) {
				throw new InvalidOperationException("Type already created.");
			}
			MethodBuilder methodBuilder = _typeBuilder.DefineMethod(methodName,
				MethodAttributes.Public |
				MethodAttributes.Virtual,
				typeof(TReturn),
				new Type[] { });
			ILGenerator gen = methodBuilder.GetILGenerator();
			ilGenerator.Invoke(gen);
			_typeBuilder.DefineMethodOverride(methodBuilder, typeof(TInterface).GetMethod(methodName, new Type[] { })!);
		}

		public void ImplementFunc<TReturn, T1>(string methodName, Action<ILGenerator> ilGenerator) {
			if (_type != null) {
				throw new InvalidOperationException("Type already created.");
			}
			MethodBuilder methodBuilder = _typeBuilder.DefineMethod(methodName,
				MethodAttributes.Public |
				MethodAttributes.Virtual,
				typeof(TReturn),
				new Type[] { typeof(T1) });
			ILGenerator gen = methodBuilder.GetILGenerator();
			ilGenerator.Invoke(gen);
			_typeBuilder.DefineMethodOverride(methodBuilder, typeof(TInterface).GetMethod(methodName, new Type[] { typeof(T1) })!);
		}

		public void ImplementFunc<TReturn, T1, T2>(string methodName, Action<ILGenerator> ilGenerator) {
			if (_type != null) {
				throw new InvalidOperationException("Type already created.");
			}
			MethodBuilder methodBuilder = _typeBuilder.DefineMethod(methodName,
				MethodAttributes.Public |
				MethodAttributes.Virtual,
				typeof(TReturn),
				new Type[] { typeof(T1), typeof(T2) });
			ILGenerator gen = methodBuilder.GetILGenerator();
			ilGenerator.Invoke(gen);
			_typeBuilder.DefineMethodOverride(methodBuilder, typeof(TInterface).GetMethod(methodName, new Type[] { typeof(T1), typeof(T2) })!);
		}

		public void ImplementFunc<TReturn, T1, T2, T3>(string methodName, Action<ILGenerator> ilGenerator) {
			if (_type != null) {
				throw new InvalidOperationException("Type already created.");
			}
			MethodBuilder methodBuilder = _typeBuilder.DefineMethod(methodName,
				MethodAttributes.Public |
				MethodAttributes.Virtual,
				typeof(TReturn),
				new Type[] { typeof(T1), typeof(T2), typeof(T3) });
			ILGenerator gen = methodBuilder.GetILGenerator();
			ilGenerator.Invoke(gen);
			_typeBuilder.DefineMethodOverride(methodBuilder, typeof(TInterface).GetMethod(methodName, new Type[] { typeof(T1), typeof(T2), typeof(T3) })!);
		}

		public void ImplementFunc<TReturn, T1, T2, T3, T4>(string methodName, Action<ILGenerator> ilGenerator) {
			if (_type != null) {
				throw new InvalidOperationException("Type already created.");
			}
			MethodBuilder methodBuilder = _typeBuilder.DefineMethod(methodName,
				MethodAttributes.Public |
				MethodAttributes.Virtual,
				typeof(TReturn),
				new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) });
			ILGenerator gen = methodBuilder.GetILGenerator();
			ilGenerator.Invoke(gen);
			_typeBuilder.DefineMethodOverride(methodBuilder, typeof(TInterface).GetMethod(methodName, new Type[] { typeof(T1), typeof(T2), typeof(T3), typeof(T4) })!);
		}

		public TInterface CreateInstance() {
			if (_type == null) {
				_type = _typeBuilder.CreateType()!;
			}
			return (TInterface)Activator.CreateInstance(_type)!;
		}
	}
}
