using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Csv.Internals {
	internal sealed class SerializeOrDeserializeSyntaxReceiver : ISyntaxContextReceiver {
		public readonly Dictionary<ITypeSymbol, List<Location>> InvocationLocationsByTypeSymbol = new(SymbolEqualityComparer.Default);

		public void OnVisitSyntaxNode(GeneratorSyntaxContext context) {
			// CsvSerializer.Serialize() or
			// Csv.CsvSerializer.Serialize()
			if (context.Node is InvocationExpressionSyntax {
				Expression: MemberAccessExpressionSyntax {
					Expression: IdentifierNameSyntax {
						Identifier.Text: "CsvSerializer"
					} or MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.Text: "Csv"
						},
						Name.Identifier.Text: "CsvSerializer"
					},
					Name.Identifier.Text: "Serialize"
				},
				ArgumentList.Arguments.Count: > 0
			} serializeInvocation) {
				// Resolve into symbol
				IMethodSymbol? methodSymbol = context.SemanticModel.GetSymbolInfo(serializeInvocation).Symbol as IMethodSymbol;

				// Make sure it's Csv.CsvSerializer.Serialize()
				if (methodSymbol is not {
					ContainingType: {
						Name: "CsvSerializer",
						ContainingNamespace.Name: "Csv"
					},
					Name: "Serialize"
				}) {
					return;
				}

				// Make sure type argument is resolved and the type is explicitly declared
				if (methodSymbol.TypeArguments is not {
					Length: 1
				} typeArguments
				|| typeArguments[0] is not {
					Name.Length: > 0,
					DeclaringSyntaxReferences.Length: > 0,
					TypeKind: TypeKind.Class or TypeKind.Struct
				}) {
					return;
				}

				RegisterInvocation(typeArguments[0], serializeInvocation.GetLocation());
			}

			// CsvSerializer.Deserialize() or
			// Csv.CsvSerializer.Deserialize()
			if (context.Node is InvocationExpressionSyntax {
				Expression: MemberAccessExpressionSyntax {
					Expression: IdentifierNameSyntax {
						Identifier.Text: "CsvSerializer"
					} or MemberAccessExpressionSyntax {
						Expression: IdentifierNameSyntax {
							Identifier.Text: "Csv"
						},
						Name.Identifier.Text: "CsvSerializer"
					},
					Name.Identifier.Text: "Deserialize"
				},
				ArgumentList.Arguments.Count: > 0
			} deserializeInvocation) {
				// Resolve into symbol
				IMethodSymbol? methodSymbol = context.SemanticModel.GetSymbolInfo(deserializeInvocation).Symbol as IMethodSymbol;

				// Make sure it's Csv.CsvSerializer.Deserialize()
				if (methodSymbol is not {
					ContainingType: {
						Name: "CsvSerializer",
						ContainingNamespace.Name: "Csv"
					},
					Name: "Deserialize"
				}) {
					return;
				}

				// Make sure type argument is resolved and the type is explicitly declared
				if (methodSymbol.TypeArguments is not {
					Length: 1
				} typeArguments
				|| typeArguments[0] is not {
					Name.Length: > 0,
					DeclaringSyntaxReferences.Length: > 0,
					TypeKind: TypeKind.Class or TypeKind.Struct
				}) {
					return;
				}

				RegisterInvocation(typeArguments[0], deserializeInvocation.GetLocation());
			}
		}

		private void RegisterInvocation(ITypeSymbol typeSymbol, Location invocationLocation) {
			if (!InvocationLocationsByTypeSymbol.TryGetValue(typeSymbol, out List<Location>? invocationLocations)) {
				invocationLocations = new List<Location>();
				InvocationLocationsByTypeSymbol.Add(typeSymbol, invocationLocations);
			}
			invocationLocations.Add(invocationLocation);
		}
	}
}
