using Microsoft.AspNetCore.Http;
using Microsoft.IO;
using Serilog;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace AprendendoSerilog.API.Helpers
{
    public class LogHelper
    {

        private static readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();

        public static async void EnrichFromRequest(IDiagnosticContext diagnosticContext, HttpContext httpContext)
        {
            var request = httpContext.Request;

            // Set all the common properties available for every request
            diagnosticContext.Set("Host", request.Host);
            diagnosticContext.Set("Protocol", request.Protocol);
            diagnosticContext.Set("Scheme", request.Scheme);

            // Set the content-type of the Response at this point
            diagnosticContext.Set("ContentType", httpContext.Response.ContentType);

            // Only set it if available. You're not sending sensitive data in a querystring right?!
            if (request.QueryString.HasValue)
            {
                diagnosticContext.Set("QueryString", request.QueryString.Value);
            }



            // Retrieve the IEndpointFeature selected for the request
            var endpoint = httpContext.GetEndpoint();
            if (endpoint is object) // endpoint != null
            {
                diagnosticContext.Set("EndpointName", endpoint.DisplayName);
            }

            diagnosticContext.Set("Request Body", await SerealizarBody(httpContext.Request));


        }

        private static async Task<string> SerealizarBody(HttpRequest request)
        {
            request.EnableBuffering();

            using var requestStream = _recyclableMemoryStreamManager.GetStream();
            
            await request.Body.CopyToAsync(requestStream);


            return await ReadStreamInChunks(requestStream);
        }


        private async static Task<string> ReadStreamInChunks(Stream stream)
        {
            int readChunkBufferLength = Convert.ToInt32(stream.Length) ;//4096;

            stream.Seek(0, SeekOrigin.Begin);

            using var textWriter = new StringWriter();
            using var reader = new StreamReader(stream);

            var readChunk = new char[readChunkBufferLength];
            int readChunkLength;

            do
            {
                readChunkLength = await reader.ReadBlockAsync(readChunk, 0, readChunkBufferLength);
                textWriter.Write(readChunk, 0, readChunkLength);
            } while (readChunkLength > 0);

            return textWriter.ToString();
        }
    }



}
