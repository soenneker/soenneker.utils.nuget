using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Soenneker.Utils.NuGet.Tests;

internal sealed class DeleteTestHandler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> delete) : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.Method == HttpMethod.Delete)
            return delete(request, cancellationToken);

        string body = request.RequestUri!.AbsolutePath.Contains("index.json")
            ? """{"resources":[{"@id":"https://delete.invalid/search","@type":"SearchQueryService"},{"@id":"https://delete.invalid/api/v2/package/","@type":"PackagePublish/2.0.0"}]}"""
            : """{"data":[{"id":"Test.Package","versions":[{"version":"1.0.0"},{"version":"2.0.0"},{"version":"3.0.0"},{"version":"4.0.0"},{"version":"5.0.0"},{"version":"6.0.0"}]}]}""";
        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(body) });
    }
}
