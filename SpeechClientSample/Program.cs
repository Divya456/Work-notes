// ---------------------------------------------------------------------------------------------------------------------
//  <copyright file="Program.cs" company="Microsoft">
//    Copyright (c) Microsoft Corporation.  All rights reserved.
// MIT LicensePermission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//  </copyright>
//  ---------------------------------------------------------------------------------------------------------------------

namespace SpeechClientSample
{
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using CognitiveServicesAuthorization;
    using Microsoft.Bing.Speech;
    using System.Media;

    /// <summary>
    /// This sample program shows how to use <see cref="SpeechClient"/> APIs to perform speech recognition.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Short phrase mode URL
        /// </summary>
        private static readonly Uri ShortPhraseUrl = new Uri(@"wss://speech.platform.bing.com/api/service/recognition");

        /// <summary>
        /// The long dictation URL
        /// </summary>
        private static readonly Uri LongDictationUrl = new Uri(@"wss://speech.platform.bing.com/api/service/recognition/continuous");

        /// <summary>
        /// A completed task
        /// </summary>
        private static readonly Task CompletedTask = Task.FromResult(true);

        /// <summary>
        /// Cancellation token used to stop sending the audio.
        /// </summary>
        private readonly CancellationTokenSource cts = new CancellationTokenSource();

        /// <summary>
        /// The entry point to this sample program. It validates the input arguments
        /// and sends a speech recognition request using the Microsoft.Bing.Speech APIs.
        /// </summary>
        /// <param name="args">The input arguments.</param>
        public static void Main(string[] args)
        {
            //Console.ReadKey();
            // Validate the input arguments count.
            /*if (args.Length < 4)
            {
                DisplayHelp("Invalid number of arguments.");
                //Console.ReadKey();
                return;
            }

            // Ensure the audio file exists.
            if (!File.Exists(args[0]))
            {
                DisplayHelp("Audio file not found.");
                //Console.ReadKey();
                return;
            }

            if (!"long".Equals(args[2], StringComparison.OrdinalIgnoreCase) && !"short".Equals(args[2], StringComparison.OrdinalIgnoreCase))
            {
                DisplayHelp("Invalid RecognitionMode.");
                //Console.ReadKey();
                return;
            }*/
            // Send a speech recognition request for the audio.
            var p = new Program();
            //p.Run("C:\Users\t - visp\Downloads\female.wav", "en - US". long, "e6650295b3b544a9a22142fc7a4b8a94");
            // p.Run(args[0], args[1], char.ToLower(args[2][0]) == 'l' ? LongDictationUrl : ShortPhraseUrl, args[3]).Wait();
            p.Run("C:/Users/t-dinar/Downloads/maleSpeech.wav", "en-US", LongDictationUrl, "e6650295b3b544a9a22142fc7a4b8a94").Wait();
            Console.ReadKey();
           
        }

        /// <summary>
        /// Invoked when the speech client receives a partial recognition hypothesis from the server.
        /// </summary>
        /// <param name="args">The partial response recognition result.</param>
        /// <returns>
        /// A task
        /// </returns>
        public Task OnPartialResult(RecognitionPartialResult args)
        {
            //Console.WriteLine("--- Partial result received by OnPartialResult ---");

            // Print the partial response recognition hypothesis.
            Console.WriteLine(args.DisplayText);

            Console.WriteLine();

            return CompletedTask;
        }

        private static void PlayAudio(object sender, GenericEventArgs<Stream> args)
        {
            Console.WriteLine(args.EventData);
            Console.WriteLine(sender.ToString());
            // For SoundPlayer to be able to play the wav file, it has to be encoded in PCM.
            // Use output audio format AudioOutputFormat.Riff16Khz16BitMonoPcm to do that.
            SoundPlayer player = new SoundPlayer(args.EventData);
            player.PlaySync();
            args.EventData.Dispose();
        }

        /// <summary>
        /// Handler an error when a TTS request failed.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="GenericEventArgs{Exception}"/> instance containing the event data.</param>
        private static void ErrorHandler(object sender, GenericEventArgs<Exception> e)
        {
            Console.WriteLine("Unable to complete the TTS request: [{0}]", e.ToString());
        }

        /// <summary>
        /// Invoked when the speech client receives a phrase recognition result(s) from the server.
        /// </summary>
        /// <param name="args">The recognition result.</param>
        /// <returns>
        /// A task
        /// </returns>
        public Task OnRecognitionResult(RecognitionResult args)
        {
            var response = args;

            Console.WriteLine();
            Console.WriteLine("Starting Authtentication");
            string accessToken;
            Authentication auth = new Authentication("e6650295b3b544a9a22142fc7a4b8a94");
            accessToken = auth.GetAccessToken();
            Console.WriteLine("Token: {0}\n", accessToken);
            Console.Write(accessToken);

            Console.WriteLine("--- Phrase result received by OnRecognitionResult ---");

            Console.WriteLine("***** Phrase Recognition Status = [{0}] ***", response.RecognitionStatus);
            if (response.Phrases != null)
            {
                foreach (var result in response.Phrases)
                {
                    // Print the recognition phrase display text.
                    Console.WriteLine("{0} (Confidence:{1})", result.DisplayText, result.Confidence);

                    Console.WriteLine("Starting TTSSample request code execution.");

                    string requestUri = "https://speech.platform.bing.com/synthesize";

                    var cortana = new Synthesize();
            
                    cortana.OnAudioAvailable += PlayAudio;
                    cortana.OnError += ErrorHandler;

                    // Reuse Synthesize object to minimize latency
                    cortana.Speak(CancellationToken.None, new Synthesize.InputOptions()
                    {
                        RequestUri = new Uri(requestUri),
                        // Text to be spoken.
                        Text = result.DisplayText,
                        VoiceType = Gender.Female,
                        // Refer to the documentation for complete list of supported locales.
                        Locale = "en-GB",
                        // You can also customize the output voice. Refer to the documentation to view the different
                        // voices that the TTS service can output.
                        VoiceName = "Microsoft Server Speech Text to Speech Voice (en-GB, HazelRUS)",
                        // Service can return audio in different output format.
                        OutputFormat = AudioOutputFormat.Riff16Khz16BitMonoPcm,
                        AuthorizationToken = "Bearer " + accessToken,
                    }).Wait();
                    Console.ReadKey();
                    break;
                 }
            }

            Console.WriteLine();
            return CompletedTask;
        }

        /// <summary>
        /// Sends a speech recognition request to the speech service
        /// </summary>
        /// <param name="audioFile">The audio file.</param>
        /// <param name="locale">The locale.</param>
        /// <param name="serviceUrl">The service URL.</param>
        /// <param name="subscriptionKey">The subscription key.</param>
        /// <returns>
        /// A task
        /// </returns>
        public async Task Run(string audioFile, string locale, Uri serviceUrl, string subscriptionKey)
        {
            // create the preferences object
            var preferences = new Preferences(locale, serviceUrl, new CognitiveServicesAuthorizationProvider(subscriptionKey));

            // Create a a speech client
            using (var speechClient = new SpeechClient(preferences))
            {
                speechClient.SubscribeToPartialResult(this.OnPartialResult);
                speechClient.SubscribeToRecognitionResult(this.OnRecognitionResult);

                // create an audio content and pass it a stream.
                using (var audio = new FileStream(audioFile, FileMode.Open, FileAccess.Read))
                {
                    var deviceMetadata = new DeviceMetadata(DeviceType.Near, DeviceFamily.Desktop, NetworkType.Ethernet, OsName.Windows, "1607", "Dell", "T3600");
                    var applicationMetadata = new ApplicationMetadata("SampleApp", "1.0.0");
                    var requestMetadata = new RequestMetadata(Guid.NewGuid(), deviceMetadata, applicationMetadata, "SampleAppService");

                    await speechClient.RecognizeAsync(new SpeechInput(audio, requestMetadata), this.cts.Token).ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Display the list input arguments required by the program.
        /// </summary>
        /// <param name="message">The message.</param>
        private static void DisplayHelp(string message = null)
        {
            if (string.IsNullOrEmpty(message))
            {
                message = "SpeechClientSample Help";
            }

            Console.WriteLine(message);
            Console.WriteLine();
            Console.WriteLine("Arg[0]: Specify an input audio wav file.");
            Console.WriteLine("Arg[1]: Specify the audio locale.");
            Console.WriteLine("Arg[2]: Recognition mode [Short|Long].");
            Console.WriteLine("Arg[3]: Specify the subscription key to access the Speech Recognition Service.");
            Console.WriteLine();
            Console.WriteLine("Sign up at https://www.microsoft.com/cognitive-services/ with a client/subscription id to get a client secret key.");
        }
    }
}
