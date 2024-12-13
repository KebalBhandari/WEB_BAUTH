using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;
using WEB_BA.Controllers;

namespace WEB_BA.DataProvider
{
    public class ApiCall
    {
        // Initialize the HttpClient with default configurations
        public static HttpClient Initial()
        {
            var client = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(10)
            };
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            string baseUrl = Startup.baseApiUrl; // Replace with actual base URL
            client.BaseAddress = new Uri(baseUrl);
            return client;
        }

        // Perform API call without an object
        public static async Task<string> ApiCallWithoutObject(string url, string action)
        {
            try
            {
                HttpClient client = Initial();
                HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(""), Encoding.UTF8, "application/json");
                HttpResponseMessage response = action.ToLower() == "post"
                    ? await client.PostAsync(url, httpContent)
                    : await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                return "Null";
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        // Perform API call with a string payload
        public static async Task<string> ApiCallWithString(string url, string payload, string action)
        {
            try
            {
                HttpClient client = Initial();
                HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
                HttpResponseMessage response = action.ToLower() == "post"
                    ? await client.PostAsync(url, httpContent)
                    : await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                return "Null";
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        // Perform API call with an object payload
        public static async Task<string> ApiCallWithObject(string url, object payload, string action)
        {
            try
            {
                HttpClient client = Initial();
                HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
                HttpResponseMessage response = action.ToLower() == "post"
                    ? await client.PostAsync(url, httpContent)
                    : await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                return "Null";
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        public static async Task<string> JWTApiCallWithObject(string url, object payload, string action, string JWToken)
        {
            try
            {
                HttpClient client = Initial();
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", JWToken);
                HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
                HttpResponseMessage response = action.ToLower() == "post"
                    ? await client.PostAsync(url, httpContent)
                    : await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                return "Null";
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        // Helper to handle exceptions
        private static string HandleException(Exception ex)
        {
            string exceptionMessage = ex.ToString();
            return exceptionMessage.Length > 1500 ? exceptionMessage.Substring(0, 1500) : exceptionMessage;
        }

        // Remove JSON encoding artifacts from a string
        public static string RemoveEncoding(string encodedJson)
        {
            var sb = new StringBuilder(encodedJson);
            sb.Replace("\\", string.Empty);
            sb.Replace("\"[", string.Empty);
            sb.Replace("]\"", string.Empty);
            return sb.ToString();
        }

        // Remove JSON encoding artifacts for arrays
        public static string RemoveEncodingOnJsonArray(string encodedJson)
        {
            var sb = new StringBuilder(encodedJson);
            sb.Replace("\\", string.Empty);
            sb.Replace("\"[", "[");
            sb.Replace("]\"", "]");
            return sb.ToString();
        }

        // Remove array brackets from a JSON string
        public static string RemoveArray(string encodedJson)
        {
            var sb = new StringBuilder(encodedJson);
            sb.Replace("[", string.Empty);
            sb.Replace("]", string.Empty);
            return sb.ToString();
        }

        // Create HttpContent for an object
        public static HttpContent GetHttpContentForObject(object httpContent)
        {
            return new StringContent(JsonConvert.SerializeObject(httpContent), Encoding.UTF8, "application/json");
        }

        // Create HttpContent for a string
        public static HttpContent GetHttpContentForString(string httpContent)
        {
            return new StringContent(JsonConvert.SerializeObject(httpContent), Encoding.UTF8, "application/json");
        }
    }
}
