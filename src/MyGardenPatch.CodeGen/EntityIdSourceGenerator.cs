using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace MyGardenPatch.CodeGen
{
    [Generator]
    public class EntityIdSourceGenerator : ISourceGenerator
    {
        private string template;

        public void Execute(GeneratorExecutionContext context)
        {
            if (!(context.SyntaxContextReceiver is EntityIdReceiver receiver))
                return;

            foreach (var entityId in receiver.EntityIds)
            {
                var code = template
                .Replace("${Namespace}", entityId.Namespace)
                .Replace("${Name}", entityId.Name);

                context.AddSource($"{entityId.Name}.g.cs", SourceText.From(code, Encoding.UTF8));
            }
        }

        public void Initialize(GeneratorInitializationContext context)
        {
//#if DEBUG
//            if (!System.Diagnostics.Debugger.IsAttached)
//            {
//                System.Diagnostics.Debugger.Launch();
//            }
//#endif

            template = @"using System;

namespace ${Namespace}
{
    public partial record struct ${Name}
    {
        public ${Name} (Guid value) { Value = value; }

        public ${Name} () : this(Guid.NewGuid()) { }

        public Guid Value { get; }

        public static implicit operator ${Name}(Guid value) => new ${Name} (value);
    }
}
";

            context.RegisterForSyntaxNotifications(() => new EntityIdReceiver());
        }
    }
}
