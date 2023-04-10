using PTTDigital.Email.Application.Utility;
using PTTDigital.Email.Common;
using System.IO.Compression;

namespace PTTDigital.Email.Api.Middleware;

internal class DecompressionMiddleware
{
    private const string encodingGzip = "gzip";
    private readonly static string[] supportCompressionTypes = new [] { encodingGzip };
    private readonly RequestDelegate _next = null;

    public DecompressionMiddleware(RequestDelegate next)
    {
        this._next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var contentEncoding = context.Request.Headers.ContentEncoding.ToString();
        if (string.IsNullOrEmpty(contentEncoding))
        {
            await _next(context);
            return;
        }

        if (!supportCompressionTypes.Contains(contentEncoding))
        {
            context.Response.StatusCode = StatusCodes.Status415UnsupportedMediaType;
            await context.Response.WriteAsJsonAsync(ResultMessage.Error("Invalid ContentEncoding", $"Not supported {contentEncoding}"));
            return;
        }

        context.Request.EnableBuffering();

        using var compressedStream = new MemoryStream();
        await context.Request.Body.CopyToAsync(compressedStream);
        compressedStream.Seek(0, SeekOrigin.Begin);

        var requestBodyBuffer = contentEncoding switch
        {
            encodingGzip => DecompressGzip(compressedStream),
            _ => throw new NotSupportedException(),
        };

        using var requestBodyStream = new MemoryStream(requestBodyBuffer);
        requestBodyStream.Seek(0, SeekOrigin.Begin);
        context.Request.Body = requestBodyStream;

        await _next(context);
    }

    private static byte[] DecompressGzip(MemoryStream compressedStream)
    {
        using var zipStream = new GZipStream(compressedStream, CompressionMode.Decompress);
        using var resultStream = new MemoryStream();

        zipStream.CopyTo(resultStream);
        resultStream.Seek(0, SeekOrigin.Begin);
        return resultStream.ToArray();
    }
}