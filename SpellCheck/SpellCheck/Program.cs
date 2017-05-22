/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SpellCheck
{
    // https://api.cognitive.microsoft.com/bing/v5.0/SpellCheck
    class Program
    {
        static void Main(string[] args)
        {
        }



        public async Task<SpellCheckResult> SpellCheckTextAsync(string text)
        {
            string requestUri = GenerateRequestUri(Constants.BingSpellCheckEndpoint, text, SpellCheckMode.Spell);
            var response = await SendRequestAsync(requestUri, Constants.BingSpellCheckApiKey);
            var spellCheckResults = JsonConvert.DeserializeObject<SpellCheckResult>(response);
            return spellCheckResults;
        }


        string GenerateRequestUri(string spellCheckEndpoint, string text, SpellCheckMode mode)
        {
            string requestUri = spellCheckEndpoint;
            requestUri += string.Format("?text={0}", text);                         // text to spell check
            requestUri += string.Format("&mode={0}", mode.ToString().ToLower());    // spellcheck mode - proof or spell
            return requestUri;
        }

        async Task<string> SendRequestAsync(string url, string apiKey)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", apiKey);
                var response = await httpClient.GetAsync(url);
                return await response.Content.ReadAsStringAsync();
            }
        }
    }
}
*/
 

    using System;
using System.Net.Http.Headers;
using System.Text;
using System.Net.Http;
using System.Web;
using System.Threading.Tasks;

namespace CSHttpClientSample
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
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "9ed07d8ac8a34f1d862f50bd09b2b90c");

            // Request parameters
            queryString["mode"] = "proof";
            // queryString["setLang"] = "en-US";
            queryString["mkt"] = "en-US";
            queryString["text"] = "johnny went down he krooked pasth";
            var uri = "https://api.cognitive.microsoft.com/bing/v5.0/spellcheck/?" + queryString;

           // HttpResponseMessage response;

            // Request body
            byte[] byteData = Encoding.UTF8.GetBytes("");
            var res = await CallEndpoint(client, uri, byteData);
            Console.WriteLine("Resp:" + res);


        }

        static async Task<String> CallEndpoint(HttpClient client, string uri, byte[] byteData)
        {
            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
                Console.WriteLine("in callendpoint");
                var response = await client.PostAsync(uri, content);
                return await response.Content.ReadAsStringAsync();
            }
        }
    }
}