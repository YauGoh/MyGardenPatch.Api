using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace MyGardenPatch.CodeGen
{
    class EntityIdDetails
    {
        public EntityIdDetails(string @namespace, string name)
        {
            Namespace = @namespace;
            Name = name;
        }

        public string Namespace { get; }
        public string Name { get; }
    }

    internal class EntityIdReceiver : ISyntaxContextReceiver
    {
        private readonly List<EntityIdDetails> _entityIds = new List<EntityIdDetails>();

        public IReadOnlyList<EntityIdDetails> EntityIds => _entityIds;

        public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
        {
            var recordDeclarationSyntax = context.Node as RecordDeclarationSyntax;

            if (recordDeclarationSyntax is null) return;

            if (recordDeclarationSyntax.BaseList?.Types.Any(t => t.ToFullString().Trim() == "IEntityId") != true) return;

            string @namespace = "";

            if (recordDeclarationSyntax.Ancestors().OfType<NamespaceDeclarationSyntax>().Any())
            {
                // Using namespace with braces
                @namespace = recordDeclarationSyntax
                    .Ancestors()
                    .OfType<NamespaceDeclarationSyntax>()
                    .First()
                    .DescendantNodes()
                    .OfType<QualifiedNameSyntax>()
                    .First()
                    .ToFullString()
                    .Trim();
            }
            else
            {
                // using top level namespace declaration, no braces
                @namespace = recordDeclarationSyntax.Parent
                    .ChildNodes()
                    .OfType<QualifiedNameSyntax>()
                    .First()
                    .ToFullString()
                    .Trim();
            }

            var name = (string)recordDeclarationSyntax.Identifier.Value;

            _entityIds.Add(new EntityIdDetails(@namespace, name));
        }
    }
}
