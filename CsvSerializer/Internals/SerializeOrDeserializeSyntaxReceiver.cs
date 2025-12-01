using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Csv.Internals {
	internal sealed class SerializeOrDeserializeSyntaxReceiver : ISyntaxContextReceiver {
		public readonly Dictionary<ITypeSymbol, List<Location>> InvocationLocationsByTypeSymbol = new(SymbolEqualityComparer.Default);

		/// <summary>
		/// Tracks methods containing CsvSerializer calls with type parameters.
		/// Key: Original method symbol (without type substitution)
		/// Value: List of (type parameter index, invocation location)
		/// </summary>
		public readonly Dictionary<IMethodSymbol, List<(int TypeParameterIndex, Location InvocationLocation)>> MethodsWithTypeParameterUsage = new(SymbolEqualityComparer.Default);

		/// <summary>
		/// Tracks invocations of generic methods.
		/// Key: Original method definition
		/// Value: List of concrete type arguments passed at each call site
		/// </summary>
		public readonly Dictionary<IMethodSymbol, List<(ITypeSymbol[] TypeArguments, Location CallSiteLocation)>> GenericMethodInvocations = new(SymbolEqualityComparer.Default);

		public void OnVisitSyntaxNode(GeneratorSyntaxContext context) {
			// Track invocations of any generic method (to resolve type parameters later)
			if (context.Node is InvocationExpressionSyntax invocation) {
				if (context.SemanticModel.GetSymbolInfo(invocation).Symbol is IMethodSymbol calledMethod
					&& calledMethod.IsGenericMethod
					&& calledMethod.TypeArguments.Length > 0) {
					// Get the original method definition
					IMethodSymbol originalMethod = calledMethod.OriginalDefinition;

					// Check if all type arguments are concrete types
					bool allConcrete = true;
					foreach (ITypeSymbol typeArg in calledMethod.TypeArguments) {
						if (typeArg.TypeKind == TypeKind.TypeParameter) {
							allConcrete = false;
							break;
						}
					}

					if (allConcrete) {
						// Store the concrete type arguments for this method invocation
						if (!GenericMethodInvocations.TryGetValue(originalMethod, out var invocations)) {
							invocations = [];
							GenericMethodInvocations.Add(originalMethod, invocations);
						}
						ITypeSymbol[] typeArgs = new ITypeSymbol[calledMethod.TypeArguments.Length];
						calledMethod.TypeArguments.CopyTo(typeArgs, 0);
						invocations.Add((typeArgs, invocation.GetLocation()));
					}
				}
			}

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
				ProcessCsvSerializerCall(context, serializeInvocation, "Serialize");
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
				ProcessCsvSerializerCall(context, deserializeInvocation, "Deserialize");
			}
		}

		private void ProcessCsvSerializerCall(GeneratorSyntaxContext context, InvocationExpressionSyntax invocation, string methodName) {
			// Resolve into symbol
			IMethodSymbol? methodSymbol = context.SemanticModel.GetSymbolInfo(invocation).Symbol as IMethodSymbol;

			// Make sure it's Csv.CsvSerializer.Serialize/Deserialize()
			if (methodSymbol is not {
				ContainingType: {
					Name: "CsvSerializer",
					ContainingNamespace.Name: "Csv"
				}
			} || methodSymbol.Name != methodName) {
				return;
			}

			// Check if there's a type argument
			if (methodSymbol.TypeArguments is not { Length: 1 } typeArguments) {
				return;
			}

			ITypeSymbol typeArgument = typeArguments[0];

			// If the type argument is a concrete type with a declaration, register it directly
			if (typeArgument is {
				Name.Length: > 0,
				DeclaringSyntaxReferences.Length: > 0,
				TypeKind: TypeKind.Class or TypeKind.Struct
			}) {
				RegisterInvocation(typeArgument, invocation.GetLocation());
				return;
			}

			// If the type argument is a type parameter, track the containing method
			if (typeArgument is ITypeParameterSymbol typeParameter) {
				// Find the containing method
				IMethodSymbol? containingMethod = FindContainingMethod(invocation, context.SemanticModel);
				if (containingMethod is null) {
					return;
				}

				// Find which type parameter index this corresponds to
				int typeParamIndex = -1;
				for (int i = 0; i < containingMethod.TypeParameters.Length; i++) {
					if (SymbolEqualityComparer.Default.Equals(containingMethod.TypeParameters[i], typeParameter)) {
						typeParamIndex = i;
						break;
					}
				}

				if (typeParamIndex == -1) {
					// The type parameter might be from a containing type, not the method
					// For now, we skip these cases
					return;
				}

				// Get the original method definition for consistent tracking
				IMethodSymbol originalMethod = containingMethod.OriginalDefinition;

				if (!MethodsWithTypeParameterUsage.TryGetValue(originalMethod, out var usages)) {
					usages = [];
					MethodsWithTypeParameterUsage.Add(originalMethod, usages);
				}
				usages.Add((typeParamIndex, invocation.GetLocation()));
			}
		}

		private static IMethodSymbol? FindContainingMethod(SyntaxNode node, SemanticModel semanticModel) {
			SyntaxNode? current = node.Parent;
			while (current is not null) {
				if (current is MethodDeclarationSyntax or LocalFunctionStatementSyntax) {
					ISymbol? symbol = semanticModel.GetDeclaredSymbol(current);
					if (symbol is IMethodSymbol methodSymbol) {
						return methodSymbol;
					}
				}
				current = current.Parent;
			}
			return null;
		}

		private void RegisterInvocation(ITypeSymbol typeSymbol, Location invocationLocation) {
			if (!InvocationLocationsByTypeSymbol.TryGetValue(typeSymbol, out List<Location>? invocationLocations)) {
				invocationLocations = [];
				InvocationLocationsByTypeSymbol.Add(typeSymbol, invocationLocations);
			}
			invocationLocations.Add(invocationLocation);
		}
	}
}
