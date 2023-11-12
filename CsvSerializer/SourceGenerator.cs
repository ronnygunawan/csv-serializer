using System.Diagnostics;
using Csv.Internals;
using Microsoft.CodeAnalysis;

namespace CsvSerializer {
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
