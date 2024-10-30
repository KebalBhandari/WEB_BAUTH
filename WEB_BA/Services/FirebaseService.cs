using FirebaseAdmin.Auth;
using Newtonsoft.Json;
using System.Text;
using WEB_BA.Models;

namespace WEB_BA.Services
{
    public class FirebaseService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public FirebaseService(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;
        }

        public async Task<string?> LoginWithEmailAndPasswordAsync(string email, string password)
        {
            var apiKey = _configuration["Firebase:ApiKey"];
            var requestUrl = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={apiKey}";

            var requestBody = new
            {
                email = email,
                password = password,
                returnSecureToken = true
            };

            var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(requestUrl, content);

            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                var firebaseError = JsonConvert.DeserializeObject<FirebaseErrorResponse>(responseBody);

                if (firebaseError?.Error != null)
                {
                    throw new FirebaseAuthenticationException(firebaseError.Error.Code, firebaseError.Error.Message);
                }
                else
                {
                    throw new Exception("Login failed due to an unknown error.");
                }
            }
            else
            {
                var authResponse = JsonConvert.DeserializeObject<AuthResponse>(responseBody);

                if (authResponse?.IdToken == null)
                {
                    throw new Exception("Failed to retrieve authentication token.");
                }

                return authResponse.IdToken;
            }            
        }

        public async Task<string?> RegisterWithEmailAndPasswordAsync(string email, string password)
        {
            var apiKey = _configuration["Firebase:ApiKey"];
            var requestUrl = $"https://identitytoolkit.googleapis.com/v1/accounts:signUp?key={apiKey}";

            var requestBody = new
            {
                email = email,
                password = password,
                returnSecureToken = true
            };

            var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(requestUrl, content);

            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                var firebaseError = JsonConvert.DeserializeObject<FirebaseErrorResponse>(responseBody);

                if (firebaseError?.Error != null)
                {
                    throw new FirebaseAuthenticationException(firebaseError.Error.Code, firebaseError.Error.Message);
                }
                else
                {
                    throw new Exception("Registration failed due to an unknown error.");
                }
            }
            else
            {
                var authResponse = JsonConvert.DeserializeObject<AuthResponse>(responseBody);

                if (authResponse?.IdToken == null)
                {
                    throw new Exception("Failed to retrieve authentication token.");
                }

                return authResponse.IdToken;
            }
        }

        private class AuthResponse
        {
            [JsonProperty("idToken")]
            public string? IdToken { get; set; }

            [JsonProperty("refreshToken")]
            public string? RefreshToken { get; set; }

            [JsonProperty("expiresIn")]
            public string? ExpiresIn { get; set; }
        }

    }
}
