namespace MyGardenPatch.LocalIdentity.Templates;

internal class ResourceTemplates
{
    internal static async Task<Template> Load(string resource)
    {
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"MyGardenPatch.LocalIdentity.Templates.{resource}")!;
        using var reader = new StreamReader(stream);

        var content = await reader.ReadToEndAsync();

        return Template.Parse(content);
    }

    internal static async Task<string> Render<TModel>(string resource, TModel model)
    {
        var template = await Load(resource);
        var hash = Hash.FromAnonymousObject(model);

        return template.Render(hash);
    }
}
