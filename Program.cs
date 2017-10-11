using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ApiSamples
{
    class Program
    {
        // Put as a configuration settings as they will change when against production.
        const string url = @"{API_URL}";
        const string key = @"{API_KEY}"; // Store in a secure place

        static void Main(string[] args) => MainAsync(args).Wait();

        static async Task MainAsync(string[] args)
        {
            Console.WriteLine("\n---- Starting ----\n");

            var json = JsonConvert.SerializeObject(_mockData, _jsonSettings);
            var response = await PutBulkProducts(json, gzip: true);
            await CheckResponse(response);

            Console.WriteLine("---- Done ----");
            Console.WriteLine("\n Press any key ...");
            Console.ReadKey();
        }

        private static async Task<HttpResponseMessage> PutBulkProducts(string json, bool gzip)
        {
            Console.WriteLine("\nJson to send: ");
            Console.WriteLine(json);
            Console.WriteLine(string.Empty);

            using (var client = new HttpClient())
            {
                // Set api key authentication for request
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("ApiKey", key);

                HttpContent requestBody;

                // Gzip is recommended for large requests
                if (gzip)
                {
                    byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
                    MemoryStream ms = new MemoryStream();
                    using (GZipStream gzipStream = new GZipStream(ms, CompressionMode.Compress, true))
                    {
                        gzipStream.Write(jsonBytes, 0, jsonBytes.Length);
                    }
                    ms.Position = 0;

                    requestBody = new StreamContent(ms);
                    requestBody.Headers.ContentEncoding.Add("gzip"); // Tell api request body is gzipped
                }
                else
                {
                    requestBody = new StringContent(json);
                }

                requestBody.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                Console.Write(" > Sending ... ");
                return await client.PutAsync($"{url}/products/bulk", requestBody);
            }
        }

        private static async Task CheckResponse(HttpResponseMessage response)
        {
            var requestId = response.Headers.FirstOrDefault(x => x.Key.Equals("x-correlation-id", StringComparison.OrdinalIgnoreCase)).Value.FirstOrDefault();

            if (response.IsSuccessStatusCode)
            {
                // Yay! Success
                Console.WriteLine($"Success. Status code: {response.StatusCode}. Request Id: {requestId}");
            }
            else
            {
                Console.WriteLine($"Failed. Status code: {response.StatusCode}. Request Id: {requestId}");

                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    // Oh no - something is not right with the request
                    // If the problem is a validation error the response body will give what went wrong
                    var responseString = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(responseString);
                }

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    // maybe a incorrect api key
                    Console.WriteLine(" > please check api key");
                }

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    // Arrg - should not get this, but never say never
                    Console.WriteLine(" > oops ... something has gone wrong on the server!");
                    Console.WriteLine($" > please contact support with requestid {requestId}");
                }
            }
        }

        private static JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
        {
            // Not required, but there is no point in sending the extra data because of properties set to null, so just exclude them
            NullValueHandling = NullValueHandling.Ignore
        };

        ///<summary>Just some fake product data</summary>
        private static Product[] _mockData = new[]
        {
            new Product("Mock1", "MockS01", "Mock Product 1", 5),
            new Product("Mock2", "MockS02", "Mock Product 2", 14),
            new Product("Mock3", "MockS03", "Mock Product 3", 1),
            new Product("Mock4", "MockS04", "Mock Product 4", 0.5m) {
                Description = "Super pen with awesome widgets..",
                Published = true
            }
        };
    }
}
