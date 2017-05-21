
//using System;
//using System.Net.Http.Headers;
//using System.Text;
//using System.Net.Http;
//using System.Web;



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Globalization;
namespace TextMod
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
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "f3f0b49125a544fb9e9cac00e28ca429");

            // Request parameters
            queryString["autocorrect"] = "true";
            queryString["PII"] = "true";
            queryString["listId"] = "1";
            var uri = "https://westus.api.cognitive.microsoft.com/contentmoderator/moderate/v1.0/ProcessText/Screen/?language=eng&" + queryString;


            byte[] byteData = Encoding.UTF8.GetBytes("The quick brwon fox jumps over the lzay dog.                        I am dnt maive.My phojn 42667786. thi is n@gmail.com. She is bitch, fuck off");
           var response1 = await CallEndpoint(client, uri, byteData);
            Console.WriteLine("Res:" + response1);


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