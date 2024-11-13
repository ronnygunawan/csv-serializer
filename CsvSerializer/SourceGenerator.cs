#if DEBUG
using System.Diagnostics;
#endif
using Csv.Internals;
using Microsoft.CodeAnalysis;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace CsvSerializer {
#pragma warning restore IDE0130 // Namespace does not match folder structure
	[Generator]
	public class SourceGenerator : ISourceGenerator {
		public void Initialize(GeneratorInitializationContext context) {
#if DEBUG
			if (!Debugger.IsAttached) {
				// Uncomment following line to debug the generator
				// Debugger.Launch();
			}
#endif
			context.RegisterForPostInitialization(context => {
				context.AddCsvSerializer();
				context.AddAttributes();
				context.AddExceptions();
				context.AddISerializer();
				context.AddIDeserializer();
				context.AddIConverter();
				context.AddStringSplitter();
				context.AddNaiveSerializer();
				context.AddNaiveDeserializer();
			});

			context.RegisterForSyntaxNotifications(() => new SerializeOrDeserializeSyntaxReceiver());
		}

		public void Execute(GeneratorExecutionContext context) {
			context.AddNativeImplementations();
		}
	}
}
