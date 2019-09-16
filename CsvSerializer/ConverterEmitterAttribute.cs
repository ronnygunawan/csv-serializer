using System;

namespace Csv {
	[AttributeUsage(AttributeTargets.Method)]
	internal class ConverterEmitterAttribute : Attribute {
		public Type? PrimaryLocalType { get; }
		public Type? SecondaryLocalType { get; }
		public bool GenericParameterIsPrimaryLocalType { get; }
		public bool NullableOfGenericParameterIsPrimaryLocalType { get; }

		public ConverterEmitterAttribute(
			Type? primaryLocalType = null,
			Type? secondaryLocalType = null,
			bool genericParameterIsPrimaryLocalType = false,
			bool nullableOfGenericParameterIsPrimaryLocalType = false
		) {
			PrimaryLocalType = primaryLocalType;
			SecondaryLocalType = secondaryLocalType;
			GenericParameterIsPrimaryLocalType = genericParameterIsPrimaryLocalType;
			NullableOfGenericParameterIsPrimaryLocalType = nullableOfGenericParameterIsPrimaryLocalType;
		}
	}
}
