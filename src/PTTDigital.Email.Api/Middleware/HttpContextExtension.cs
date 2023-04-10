using Microsoft.Extensions.Primitives;
using System.Net;
using System.Text;

namespace PTTDigital.Email.Api.Middleware;

public static class HttpContextExtension
{
    internal static async Task<StringContent> GetContent(this HttpContext context)
    {
        using (var stream = new StreamReader(context.Request.Body, Encoding.UTF8))
        {
            var bodyString = await stream.ReadToEndAsync();
            return new StringContent(bodyString, Encoding.UTF8, "application/json");
        }
    }

    internal static async Task<FormUrlEncodedContent> GetContentByFormUrlDncoded(this HttpContext context)
    {
        using (var stream = new StreamReader(context.Request.Body, Encoding.UTF8))
        {
            string encodeBody = await stream.ReadToEndAsync();
            var decodedBody = new Dictionary<string, string>();

            foreach (var encodedKeyValuePair in encodeBody.Split('&'))
            {
                string decodedKey = WebUtility.UrlDecode(encodedKeyValuePair.Split('=')[0]);
                string decodedValue = WebUtility.UrlDecode(encodedKeyValuePair.Split('=')[1]);
                decodedBody.Add(decodedKey, decodedValue);
            }
            return new FormUrlEncodedContent(decodedBody);
        }
    }

    internal static async Task<ByteArrayContent> GetContents(this HttpContext context)
    {
        using (var stream = new StreamReader(context.Request.Body, Encoding.UTF8))
        {
            string bodyString = await stream.ReadToEndAsync();

            if (string.IsNullOrWhiteSpace(bodyString))
                return new StringContent(string.Empty, Encoding.UTF8, "application/json");

            if ((bodyString.StartsWith("{") && bodyString.EndsWith("}")) || (bodyString.StartsWith("[") && bodyString.EndsWith("]")))
                return new StringContent(bodyString, Encoding.UTF8, "application/json");

            var decodedBody = new Dictionary<string, string>();

            foreach (var encodedKeyValuePair in bodyString.Split('&'))
            {
                string decodedKey = WebUtility.UrlDecode(encodedKeyValuePair.Split('=')[0]);
                string decodedValue = WebUtility.UrlDecode(encodedKeyValuePair.Split('=')[1]);
                decodedBody.Add(decodedKey, decodedValue);
            }
            return new FormUrlEncodedContent(decodedBody);
        }
    }

    internal static string? GetHeaderKey(this HttpContext context, string key)
    {
        context.Request.Headers.TryGetValue(key, out StringValues headerValue);
        string? header = headerValue.ToString();
        return header;
    }

    internal static string GetHeaderBearerAuthorization(this HttpContext context)
    {
        var authorization = context.Request.Headers.Authorization.ToString();
        authorization = authorization.Replace("Bearer ", "")
                                     .Replace("Bearer", "")
                                     .Replace("bearer ", "")
                                     .Replace("bearer", "");
        return authorization;
    }

    internal static async Task SetResponse(this HttpContext context, HttpResponseMessage resMessage)
    {
        context.Response.StatusCode = (int)resMessage.StatusCode;
        context.Response.ContentLength = resMessage.Content.Headers.ContentLength;
        context.Response.ContentType = resMessage.Content.Headers.ContentType?.ToString();
        await resMessage.Content.CopyToAsync(context.Response.Body);
    }

    internal static string GetApi(this string path)
    {
        path = path.ToLower();
        var str = path.Split("/");
        return string.Concat(str[1], str[3]);
    }

    internal static async Task<HttpRequestMessage> GetHttpRequestMessage(this HttpContext context, string apiKey)
    {
        var request = new HttpRequestMessage();
        request.AddHeader(context, apiKey);
        request.AddMethod(context);
        request.RequestUri = new Uri(context.Request.Path, UriKind.Relative);
        request.Content = await context.GetContents();
        return request;
    }

    internal static async Task<HttpRequestMessage> GetHttpRequestMessage(this HttpContext context, string relativePath, HttpMethod method, string apiKey)
    {
        var request = new HttpRequestMessage();
        request.AddHeader(context, apiKey);
        request.Method = method;
        request.RequestUri = new Uri(relativePath, UriKind.Relative);
        request.Content = await context.GetContents();
        return request;
    }

    private static void AddHeader(this HttpRequestMessage request, HttpContext context, string apiKey)
    {
        string? authorization = context.GetHeaderKey("Authorization");

        if (!string.IsNullOrWhiteSpace(authorization))
            request.Headers.Add("Authorization", authorization);

        if (!string.IsNullOrWhiteSpace(apiKey))
            request.Headers.Add("X-ApiKey", apiKey);
    }

    public static string[] GetSystemkey()
    {
        return new string[] { "Accept", "Connection", "Host", "User-Agent", "Accept-Encoding", "Cache-Control", "Content-Type", "Content-Length", "Postman-Token" }; //, "X-TraceId"
    }

    private static void AddMethod(this HttpRequestMessage request, HttpContext context)
    {
        var result = context.Request.Method.ToString() switch
        {
            "GET" => HttpMethod.Get,
            "POST" => HttpMethod.Post,
            "PUT" => HttpMethod.Put,
            "DELETE" => HttpMethod.Delete,
            _ => HttpMethod.Get
        };

        request.Method = result;
    }
}