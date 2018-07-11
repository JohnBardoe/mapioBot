using System;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using System.Windows.Forms;

namespace jsonSender
{
    public class UserIDclass
    {
        [JsonProperty("user_id")]
        public String userID;
    }
    public class UserCoords
    {
        [JsonProperty("user_id")]
        public string userID;
        [JsonProperty("longitude")]
        public double longitude;
        [JsonProperty("latitude")]
        public double latitude;
    }
    public class Status
    {
        [JsonProperty("status")]
        public string status;
    }
    public static class Sender
    {
        private static void exit(string s)
        {
            MessageBox.Show(s);
            Environment.Exit(0);
        }
        public static async Task MakeAsyncRequest(object payload, string uri)
        {
            string stringPayload;
            StringContent httpContent;
            stringPayload = await Task.Run(() => JsonConvert.SerializeObject(payload));
            httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");
            using (var httpClient = new HttpClient())
            {
                httpClient.Timeout = new TimeSpan(10000000);
                try
                {
                    var httpResponse = await httpClient.PostAsync(uri, httpContent);

                    if (httpResponse.Content != null)
                    {
                        var responseContent = await httpResponse.Content.ReadAsStringAsync();
                        Status stat = JsonConvert.DeserializeObject<Status>(responseContent);
                        if(stat.status != "OK")
                            exit("Server returned bad response. " + Environment.NewLine + "In other words, smth went wrong.");
                        
                    }
                    else exit("Server returned null response. " + Environment.NewLine + "Reduce rps count a bit pls");
                }
                catch (System.Reflection.TargetInvocationException e)
                {
                    exit("Host refused connection" + Environment.NewLine + e.Message);

                }
                catch (System.Net.Http.HttpRequestException e)
                {
                    exit("Can't connect to server" + Environment.NewLine + e.Message);
                }

            }
        }
    }
}
