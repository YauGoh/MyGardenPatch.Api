using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
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

            var assembly = Assembly.GetExecutingAssembly();

            var resourceName = assembly
                .GetManifestResourceNames()
                .Single(str => str.EndsWith("EntityId.cs.template"));

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            using (var reader = new StreamReader(stream))
            {
                template = reader.ReadToEnd();
            }

            context.RegisterForSyntaxNotifications(() => new EntityIdReceiver());
        }
    }
}
