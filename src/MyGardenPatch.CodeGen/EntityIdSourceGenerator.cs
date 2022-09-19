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
    [System.Text.Json.Serialization.JsonConverter(typeof(${Name}Converter))]
    public partial record struct ${Name}
    {
        public ${Name} (Guid value) { Value = value; }

        public ${Name} () : this(Guid.NewGuid()) { }

        public Guid Value { get; }

        public static implicit operator ${Name}(Guid value) => new ${Name}(value);

        public override string ToString() => Value.ToString();
    }

    public class ${Name}Converter : System.Text.Json.Serialization.JsonConverter<${Name}>
    {
        public override ${Name} Read(
            ref System.Text.Json.Utf8JsonReader reader, 
            Type typeToConvert, 
            System.Text.Json.JsonSerializerOptions options)
        {
            var str = reader.GetString() ?? throw new InvalidOperationException(""Guid string expected"");

            return new ${Name}(new Guid(str));
            }

            public override void Write(
                System.Text.Json.Utf8JsonWriter writer, 
                ${Name} value, 
                System.Text.Json.JsonSerializerOptions options)
            {
                writer.WriteStringValue(value.Value.ToString());
            }
        }
    }";

            context.RegisterForSyntaxNotifications(() => new EntityIdReceiver());
        }
    }
}
