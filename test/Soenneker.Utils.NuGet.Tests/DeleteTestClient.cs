using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Soenneker.NuGet.Client.Abstract;

namespace Soenneker.Utils.NuGet.Tests;

internal sealed class DeleteTestClient(HttpMessageHandler handler) : INuGetClient
{
    private readonly HttpClient _client = new(handler);
    public ValueTask<HttpClient> Get(CancellationToken cancellationToken = default) => ValueTask.FromResult(_client);
    public void Dispose() => _client.Dispose();
    public ValueTask DisposeAsync() { Dispose(); return ValueTask.CompletedTask; }
}
