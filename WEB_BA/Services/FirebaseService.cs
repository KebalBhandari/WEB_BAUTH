using Google.Cloud.Firestore;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Newtonsoft.Json;
using System.Text;

public class FirebaseService
{
    private readonly IConfiguration _configuration;
    private readonly FirestoreDb _firestoreDb;
    private readonly HttpClient _httpClient;

    public FirebaseService(IConfiguration configuration, HttpClient httpClient)
    {
        _configuration = configuration;
        _httpClient = httpClient;

        var pathToKey = _configuration["Firebase:AdminSdkJsonPath"];
        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", pathToKey);
        _firestoreDb = FirestoreDb.Create(_configuration["Firebase:ProjectId"]);

        if (FirebaseApp.DefaultInstance == null)
        {
            FirebaseApp.Create();
        }
    }

    // Login with email and password using Firebase Authentication REST API
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

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Login failed. Please check your credentials.");
        }

        var responseBody = await response.Content.ReadAsStringAsync();
        var authResponse = JsonConvert.DeserializeObject<AuthResponse>(responseBody);

        // Check if authResponse or IdToken is null and handle accordingly
        if (authResponse?.IdToken == null)
        {
            throw new Exception("Failed to retrieve authentication token.");
        }

        return authResponse.IdToken; // Return the ID token for the authenticated session
    }


    // Create a new user with email and password
    public async Task<UserRecord> CreateUserAsync(string email, string password)
    {
        var userArgs = new UserRecordArgs
        {
            Email = email,
            Password = password,
            EmailVerified = false
        };
        return await FirebaseAuth.DefaultInstance.CreateUserAsync(userArgs);
    }

    // Verify an ID token (acts as a login verification)
    public async Task<FirebaseToken> VerifyIdTokenAsync(string idToken)
    {
        return await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(idToken);
    }

    // Retrieve user details by their Firebase UID
    public async Task<UserRecord> GetUserByIdAsync(string userId)
    {
        return await FirebaseAuth.DefaultInstance.GetUserAsync(userId);
    }

    // Firestore: Retrieve user data by document ID
    public async Task<DocumentSnapshot> GetUserData(string userId)
    {
        var docRef = _firestoreDb.Collection("users").Document(userId);
        return await docRef.GetSnapshotAsync();
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
