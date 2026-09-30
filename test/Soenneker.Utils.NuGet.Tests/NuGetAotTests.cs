using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Soenneker.NuGet.Client.Abstract;
using Soenneker.Utils.NuGet.Responses.Catalog.Partials;

namespace Soenneker.Utils.NuGet.Tests;

public sealed class NuGetAotTests
{
    [Test]
    public async ValueTask Service_index_uses_generated_response_metadata()
    {
        using var client = new FakeClient();
        var util = new NuGetUtil(NullLogger<NuGetUtil>.Instance, client);
        var index = await util.GetIndex("https://example.invalid/v3/index.json");
        if (index.Resources?.Count != 1 || index.Resources[0].Type != "SearchQueryService")
            throw new Exception("Generated response metadata did not preserve NuGet property names.");
    }

    [Test]
    public void Repository_converter_supports_string_object_and_null()
    {
        var converter = new NuGetRepositoryConverter();
        foreach (string json in new[] { "\"https://example.invalid/repo\"", "{\"url\":\"https://example.invalid/repo\",\"branch\":\"main\"}" })
        {
            var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(json));
            reader.Read();
            NuGetRepository? value = converter.Read(ref reader, typeof(NuGetRepository), NuGetAotContext.Default.Options);
            if (value?.Url != "https://example.invalid/repo") throw new Exception("Repository conversion failed.");
            using var stream = new System.IO.MemoryStream();
            using (var writer = new Utf8JsonWriter(stream)) converter.Write(writer, value, NuGetAotContext.Default.Options);
            if (!Encoding.UTF8.GetString(stream.ToArray()).Contains("example.invalid")) throw new Exception("Repository serialization failed.");
        }
        var nullReader = new Utf8JsonReader("null"u8);
        nullReader.Read();
        if (converter.Read(ref nullReader, typeof(NuGetRepository), NuGetAotContext.Default.Options) is not null)
            throw new Exception("Null repository changed.");
    }

    private sealed class FakeClient : INuGetClient
    {
        private readonly HttpClient _client = new(new FakeHandler());
        public ValueTask<HttpClient> Get(CancellationToken cancellationToken = default) => ValueTask.FromResult(_client);
        public void Dispose() => _client.Dispose();
        public ValueTask DisposeAsync() { Dispose(); return ValueTask.CompletedTask; }
    }

    private sealed class FakeHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
            Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"version\":\"3.0.0\",\"resources\":[{\"@id\":\"https://example.invalid/search\",\"@type\":\"SearchQueryService\"}]}")
            });
    }
}

[JsonSerializable(typeof(NuGetRepository))]
internal partial class NuGetAotContext : JsonSerializerContext;
