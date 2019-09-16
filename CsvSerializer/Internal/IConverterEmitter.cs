using System;
using System.Reflection.Emit;
using System.Text;

namespace Csv.Internal {
	internal interface IConverterEmitter {
		/// <summary>
		/// Value is loaded to the top of stack. Pop it. Under the value is a <see cref="StringBuilder"/>. Keep it there.
		/// <para>arg0 is an <see cref="IFormatProvider"/>.</para>
		/// </summary>
		void EmitAppendToStringBuilder(ILGenerator gen, LocalBuilder? local, LocalBuilder? secondaryLocal, CsvColumnAttribute? attribute);

		/// <summary>
		/// Unquoted string literal is loaded as a <see cref="ReadOnlySpan{char}"/> to top of stack. Pop it then push its deserialized value to stack.
		/// <para>arg0 is an IFormatProvider.</para>
		/// </summary>
		void EmitDeserialize(ILGenerator gen, LocalBuilder? local, LocalBuilder? secondaryLocal, CsvColumnAttribute? attribute);
	}
}
