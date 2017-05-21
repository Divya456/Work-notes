




using System;
using System.Net.Http.Headers;
using System.Text;
using System.Net.Http;
using System.Web;
using System.Threading.Tasks;

namespace Luis
{
    static class Program
    {
        static void Main()
        {
            MakeRequest();
            Console.WriteLine("Hit ENTER to exit...");
            Console.ReadLine();
        }

        static async void MakeRequest()
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "c4c536d2f36b48ee994ac561dce69290");

            var uri = "https://westus.api.cognitive.microsoft.com/luis/api/v2.0/apps/?" + queryString;

           

            // Request body
            byte[] byteData = Encoding.UTF8.GetBytes("Book me a flight to Paris");
            var response = await CallEndpoint(client, uri, byteData);
            //using (var content = new ByteArrayContent(byteData))
            //{
            //    content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
            //    response = await client.PostAsync(uri, content);
            //    var r=await response.Content.ReadAsStringAsync();
                
            //}
            Console.WriteLine("Resp:" + response);
        }

        static async Task<String> CallEndpoint(HttpClient client, string uri, byte[] byteData)
        {
            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");

                var response = await client.PostAsync(uri, content);
                return await response.Content.ReadAsStringAsync();
            }
        }
    }
}
