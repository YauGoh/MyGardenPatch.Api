using Microsoft.AspNetCore.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace MyGardenPatch.WebApi.Tests.TestFixtureExtensions;

internal record FormDataFilePart(string Name, string Filename, string ContentType, string Content, Dictionary<string, string> Headers);

internal class FormDataRequestBuilder
{
    public readonly List<FormDataFilePart> parts = new List<FormDataFilePart>();
    private readonly Scenario _scenario;
    private readonly Dictionary<string, string> cookies;

    public FormDataRequestBuilder(Scenario scenario)
    {
        _scenario = scenario;
        cookies = new Dictionary<string, string>();
    }

    public FormDataRequestBuilder WithFile(string name, string filename, string contentType, string content, Dictionary<string, string> headers)
    {
        parts.Add(new FormDataFilePart(name, filename, contentType, content, headers));

        return this;
    }

    public FormDataRequestBuilder WithCommand<T>(T command)
    {
        parts.Add(new FormDataFilePart("command", "command", "application/json", JsonSerializer.Serialize<T>(command), new Dictionary<string, string>()));

        return this;
    }

    public void To(string relativeUrl) 
    {
        _scenario.ConfigureHttpContext(
            async httpContext =>
            {
                var formDataBoundary = $"----------{Guid.NewGuid():N}";
                var contentType = "multipart/form-data; boundary=" + formDataBoundary;

                httpContext.Request.Method = "POST";
                httpContext.Request.ContentType = contentType;

                using var multipartContent = new MultipartFormDataContent(formDataBoundary);

                FillMultiPartContent(multipartContent);

                await multipartContent.CopyToAsync(httpContext.Request.Body);
                httpContext.Request.ContentLength = httpContext.Request.Body.Length;
                httpContext.RelativeUrl(relativeUrl);
            });
    }

    private void FillMultiPartContent(MultipartFormDataContent multipartContent)
    {
        foreach(var part in parts)
        {
            var fileContent = new ByteArrayContent(Encoding.UTF8.GetBytes(part.Content));
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(part.ContentType);
            fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                FileName = part.Filename,
                Name = part.Name
            };
            foreach(var (name, value) in part.Headers)
            {
                fileContent.Headers.Add(name, value);
            }


            multipartContent.Add(fileContent, part.Filename, part.Filename);
        }
    }

}

internal static class FormDataExtensions
{
    internal static FormDataRequestBuilder PostFiles(this Scenario scenario)
    {
        return new FormDataRequestBuilder(scenario);

    }
}
