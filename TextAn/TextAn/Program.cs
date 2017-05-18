using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Globalization;

namespace TextAn
{
    /// <summary>
    /// This is a sample program that shows how to use the Azure ML Text Analytics app: key phrases, language and sentiment detection. 
    /// </summary>
    static class Program
    {
        /// <summary>
        /// Azure portal URL.
        /// </summary>
        private const string BaseUrl = "https://westus.api.cognitive.microsoft.com/";

        /// <summary>
        /// Your account key goes here.
        /// </summary>
        private const string AccountKey = "1240204edbcc4e579225a916cbe7477e";

        /// <summary>
        /// Maximum number of languages to return in language detection API.
        /// </summary>
        private const int NumLanguages = 3;

        static void Main()
        {
            MakeRequests();
            Console.WriteLine("Hit ENTER to exit...");
            Console.ReadLine();
        }

        static async void MakeRequests()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseUrl);

                // Request headers.
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", AccountKey);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Request body. Insert your text data here in JSON format.
                byte[] byteData = Encoding.UTF8.GetBytes("{\"documents\":[" +
                    "{\"id\":\"1\",\"text\":\"I live in Texas. The cars are in basement. Tim Cook is here\"}," +
                    "{\"id\":\"2\",\"text\":\"El hardware inalámbrico no autorizado se puede introducir fácilmente. Los puntos de acceso inalámbricos son relativamente poco costosos y se implementan fácilmente\"}," +
                    "{\"id\":\"three\",\"text\":\"hello my world\"},]}");

                // Detect key phrases:
                var uri = "text/analytics/v2.0/keyPhrases";
                var response =  CallEndpoint(client, uri, byteData);
                Console.WriteLine("\nDetect key phrases response:\n" );
                //bool f = true;
                //while( f==true)
                //    {
                //    if(!response.Equals(""))

                //}
                // Detect language:
                var queryString = HttpUtility.ParseQueryString(string.Empty);
                var q = HttpUtility.ParseQueryString("hello");
                q["hi"] = 2.ToString();
                q["hello"] = 4.ToString();
                queryString["numberOfLanguagesToDetect"] = NumLanguages.ToString(CultureInfo.InvariantCulture);
                Console.WriteLine("qs:" + q);
                uri = "text/analytics/v2.0/languages?" + queryString;
                var response1 = await CallEndpoint(client, uri, byteData);
                Console.WriteLine("\nDetect language response:\n" + response1);

                // Detect sentiment:
                uri = "text/analytics/v2.0/sentiment";
                var response2 = await CallEndpoint(client, uri, byteData);
                Console.WriteLine("\nDetect sentiment response:\n" + response2);
            }
        }

        static async Task<String> CallEndpoint(HttpClient client, string uri, byte[] byteData)
        {
            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                Console.WriteLine("in callendpoint");
                var response = await client.PostAsync(uri, content);
                return await response.Content.ReadAsStringAsync();
            }
        }
    }
}