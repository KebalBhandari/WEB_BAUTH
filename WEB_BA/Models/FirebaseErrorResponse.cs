using Newtonsoft.Json;

namespace WEB_BA.Models
{
    public class FirebaseErrorResponse
    {
        [JsonProperty("error")]
        public FirebaseError? Error { get; set; }

        public class FirebaseError
        {
            [JsonProperty("code")]
            public int Code { get; set; }

            [JsonProperty("message")]
            public string? Message { get; set; }

            [JsonProperty("errors")]
            public List<FirebaseErrorDetail>? Errors { get; set; }
        }

        public class FirebaseErrorDetail
        {
            [JsonProperty("message")]
            public string? Message { get; set; }

            [JsonProperty("domain")]
            public string? Domain { get; set; }

            [JsonProperty("reason")]
            public string? Reason { get; set; }
        }

        public class AuthResponse
        {
            [JsonProperty("idToken")]
            public string? IdToken { get; set; }

            [JsonProperty("email")]
            public string? Email { get; set; }

            [JsonProperty("refreshToken")]
            public string? RefreshToken { get; set; }

            [JsonProperty("expiresIn")]
            public string? ExpiresIn { get; set; }

            [JsonProperty("localId")]
            public string? LocalId { get; set; }
        }
    }

}
