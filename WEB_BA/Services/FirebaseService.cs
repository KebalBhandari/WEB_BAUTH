using Google.Cloud.Firestore;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using WEB_BA.Models;
using Google.Cloud.Firestore.V1;

namespace WEB_BA.Services
{
    public class FirebaseService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly FirestoreDb _firestoreDb;

        public FirebaseService(IConfiguration configuration, HttpClient httpClient)
        {
            _configuration = configuration;
            _httpClient = httpClient;

            string? projectId = _configuration["Firebase:ProjectId"];
            string? credentialsPath = _configuration["Firebase:AdminSdkJsonPath"];

            if (string.IsNullOrEmpty(projectId))
            {
                throw new InvalidOperationException("Firebase:ProjectId is not configured.");
            }

            if (string.IsNullOrEmpty(credentialsPath))
            {
                throw new InvalidOperationException("Firebase:AdminSdkJsonPath is not configured.");
            }

            // Build the FirestoreClient with credentials
            FirestoreClientBuilder builder = new FirestoreClientBuilder
            {
                CredentialsPath = credentialsPath
            };
            FirestoreClient client = builder.Build();

            // Create the FirestoreDb with the client
            _firestoreDb = FirestoreDb.Create(projectId, client);
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
                    throw new FirebaseAuthenticationException(firebaseError.Error.Code, firebaseError.Error.Message ?? "Unknown error occurred!");
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

                var userProfile = DecodeIdToken(authResponse.IdToken);
                Console.WriteLine($"Welcome, {userProfile.DisplayName}!");

                return authResponse.IdToken;
            }
        }

        public async Task<string?> RegisterWithEmailAndPasswordAsync(string email, string password, string username)
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
                    throw new FirebaseAuthenticationException(firebaseError.Error.Code, firebaseError.Error.Message ?? "Unknown error occurred!");
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

                var updateProfileUrl = $"https://identitytoolkit.googleapis.com/v1/accounts:update?key={apiKey}";

                var updateProfileRequestBody = new
                {
                    idToken = authResponse.IdToken,
                    displayName = username,
                    returnSecureToken = true
                };

                var updateProfileContent = new StringContent(JsonConvert.SerializeObject(updateProfileRequestBody), Encoding.UTF8, "application/json");

                var updateProfileResponse = await _httpClient.PostAsync(updateProfileUrl, updateProfileContent);

                var updateProfileResponseBody = await updateProfileResponse.Content.ReadAsStringAsync();

                if (!updateProfileResponse.IsSuccessStatusCode)
                {
                    var firebaseError = JsonConvert.DeserializeObject<FirebaseErrorResponse>(updateProfileResponseBody);

                    if (firebaseError?.Error != null)
                    {
                        throw new FirebaseAuthenticationException(firebaseError.Error.Code, firebaseError.Error.Message ?? "Unknown error occurred during profile update.");
                    }
                    else
                    {
                        throw new Exception("Failed to update user profile due to an unknown error.");
                    }
                }

                return authResponse.IdToken;
            }
        }

        public UserProfile DecodeIdToken(string idToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(idToken);

            var claims = jwtToken.Claims;

            var localId = claims.FirstOrDefault(c => c.Type == "user_id")?.Value;
            var email = claims.FirstOrDefault(c => c.Type == "email")?.Value;
            var displayName = claims.FirstOrDefault(c => c.Type == "name")?.Value;

            var userProfile = new UserProfile
            {
                LocalId = localId,
                Email = email,
                DisplayName = displayName
            };

            return userProfile;
        }

        public async Task<UserProfile?> GetUserProfileAsync(string idToken)
        {
            var apiKey = _configuration["Firebase:ApiKey"];
            var requestUrl = $"https://identitytoolkit.googleapis.com/v1/accounts:lookup?key={apiKey}";

            var requestBody = new
            {
                idToken = idToken
            };

            var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(requestUrl, content);

            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                var firebaseError = JsonConvert.DeserializeObject<FirebaseErrorResponse>(responseBody);

                if (firebaseError?.Error != null)
                {
                    throw new FirebaseAuthenticationException(firebaseError.Error.Code, firebaseError.Error.Message ?? "Unknown error occurred during profile retrieval.");
                }
                else
                {
                    throw new Exception("Failed to retrieve user profile due to an unknown error.");
                }
            }
            else
            {
                var userInfoResponse = JsonConvert.DeserializeObject<UserInfoResponse>(responseBody);

                if (userInfoResponse?.Users != null && userInfoResponse.Users.Any())
                {
                    var user = userInfoResponse.Users.First();

                    var userProfile = new UserProfile
                    {
                        LocalId = user.LocalId,
                        Email = user.Email,
                        DisplayName = user.DisplayName,
                    };

                    return userProfile;
                }
                else
                {
                    throw new Exception("User not found in the response.");
                }
            }
        }

        public async Task SaveUserDataAsync(string userId, UserDataModel userData)
        {
            DocumentReference docRef = _firestoreDb.Collection("tests").Document();

            var testData = new Dictionary<string, object>
            {
                { "userId", userId },
                { "timestamp", Timestamp.GetCurrentTimestamp() }
            };

            // Save main document
            await docRef.SetAsync(testData);

            if (userData.Timings != null && userData.Timings.Count > 0)
            {
                CollectionReference timingsCollection = docRef.Collection("timings");
                for (int i = 0; i < userData.Timings.Count; i++)
                {
                    var attemptTimings = new Dictionary<string, object>
            {
                { "attempt", i + 1 },
                { "timings", userData.Timings[i] }
            };
                    await timingsCollection.AddAsync(attemptTimings);
                }
            }

            // Save 'dotTimings' as subcollection
            if (userData.DotTimings != null && userData.DotTimings.Count > 0)
            {
                CollectionReference dotTimingsCollection = docRef.Collection("dotTimings");
                for (int i = 0; i < userData.DotTimings.Count; i++)
                {
                    var attemptDotTimings = new Dictionary<string, object>
            {
                { "attempt", i + 1 },
                { "timings", userData.DotTimings[i] }
            };
                    await dotTimingsCollection.AddAsync(attemptDotTimings);
                }
            }

            // Save 'shapeTimings' as subcollection
            if (userData.ShapeTimings != null && userData.ShapeTimings.Count > 0)
            {
                CollectionReference shapeTimingsCollection = docRef.Collection("shapeTimings");
                for (int i = 0; i < userData.ShapeTimings.Count; i++)
                {
                    var attemptShapeTimings = new Dictionary<string, object>
            {
                { "attempt", i + 1 },
                { "timings", userData.ShapeTimings[i] }
            };
                    await shapeTimingsCollection.AddAsync(attemptShapeTimings);
                }
            }

            // Save 'keyHoldTimes' as subcollection
            if (userData.KeyHoldTimes != null && userData.KeyHoldTimes.Count > 0)
            {
                CollectionReference keyHoldTimesCollection = docRef.Collection("keyHoldTimes");
                for (int i = 0; i < userData.KeyHoldTimes.Count; i++)
                {
                    var attemptKeyHoldTimes = new Dictionary<string, object>
            {
                { "attempt", i + 1 },
                { "keyHoldTimes", userData.KeyHoldTimes[i] }
            };
                    await keyHoldTimesCollection.AddAsync(attemptKeyHoldTimes);
                }
            }

            // Save 'shapeMouseMovements' as subcollection
            if (userData.ShapeMouseMovements != null && userData.ShapeMouseMovements.Count > 0)
            {
                CollectionReference shapeMouseMovementsCollection = docRef.Collection("shapeMouseMovements");
                for (int i = 0; i < userData.ShapeMouseMovements.Count; i++)
                {
                    var attemptMouseMovements = new Dictionary<string, object>
            {
                { "attempt", i + 1 },
                { "mouseMovements", userData.ShapeMouseMovements[i] }
            };
                    await shapeMouseMovementsCollection.AddAsync(attemptMouseMovements);
                }
            }
        }


        // Method to get user data from Firestore
        public async Task<List<UserDataModel>> GetUserDataAsync(string userId)
        {
            Console.WriteLine($"Fetching data for userId: {userId}");
            try
            {
                // Query the 'tests' collection for all documents with the given userId
                CollectionReference testsCollection = _firestoreDb.Collection("tests");
                Query query = testsCollection
                    .WhereEqualTo("userId", userId)
                    .OrderByDescending("timestamp"); // Remove .Limit(1) to get all documents

                QuerySnapshot querySnapshot = await query.GetSnapshotAsync();

                if (querySnapshot.Documents.Count > 0)
                {
                    Console.WriteLine($"{querySnapshot.Documents.Count} documents found for the user.");

                    List<UserDataModel> userDataList = new List<UserDataModel>();

                    foreach (DocumentSnapshot mainDoc in querySnapshot.Documents)
                    {
                        UserDataModel userData = new UserDataModel();

                        // Initialize the lists
                        userData.Timings = new List<List<double>>();
                        userData.DotTimings = new List<List<double>>();
                        userData.ShapeTimings = new List<List<ShapeTiming>>();
                        userData.KeyHoldTimes = new List<List<KeyHoldTime>>();
                        userData.ShapeMouseMovements = new List<List<MouseMovement>>();

                        // Retrieve subcollections for the current document
                        await RetrieveSubcollectionsAsync(mainDoc, userData);

                        // Add the userData to the list
                        userDataList.Add(userData);
                    }

                    return userDataList;
                }
                else
                {
                    Console.WriteLine("No documents found for the user.");
                    return new List<UserDataModel>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in GetUserDataAsync: {ex.Message}");
                throw;
            }
        }

        // Helper method to retrieve subcollections for a document
        private async Task RetrieveSubcollectionsAsync(DocumentSnapshot mainDoc, UserDataModel userData)
        {
            // Retrieve 'timings' subcollection
            CollectionReference timingsCollection = mainDoc.Reference.Collection("timings");
            QuerySnapshot timingsSnapshot = await timingsCollection.GetSnapshotAsync();
            foreach (DocumentSnapshot doc in timingsSnapshot.Documents)
            {
                if (doc.ContainsField("timings"))
                {
                    List<object> timingsObjList = doc.GetValue<List<object>>("timings");
                    List<double> attemptTimings = timingsObjList.Select(t => Convert.ToDouble(t)).ToList();
                    userData.Timings.Add(attemptTimings);
                }
            }

            // Retrieve 'dotTimings' subcollection
            CollectionReference dotTimingsCollection = mainDoc.Reference.Collection("dotTimings");
            QuerySnapshot dotTimingsSnapshot = await dotTimingsCollection.GetSnapshotAsync();
            foreach (DocumentSnapshot doc in dotTimingsSnapshot.Documents)
            {
                if (doc.ContainsField("timings"))
                {
                    List<object> dotTimingsObjList = doc.GetValue<List<object>>("timings");
                    List<double> attemptDotTimings = dotTimingsObjList.Select(t => Convert.ToDouble(t)).ToList();
                    userData.DotTimings.Add(attemptDotTimings);
                }
            }

            // Retrieve 'shapeTimings' subcollection
            CollectionReference shapeTimingsCollection = mainDoc.Reference.Collection("shapeTimings");
            QuerySnapshot shapeTimingsSnapshot = await shapeTimingsCollection.GetSnapshotAsync();
            foreach (DocumentSnapshot doc in shapeTimingsSnapshot.Documents)
            {
                if (doc.ContainsField("timings"))
                {
                    List<Dictionary<string, object>> shapeTimingsList = doc.GetValue<List<Dictionary<string, object>>>("timings");
                    List<ShapeTiming> attemptShapeTimings = new List<ShapeTiming>();
                    foreach (var item in shapeTimingsList)
                    {
                        ShapeTiming shapeTiming = new ShapeTiming
                        {
                            ReactionTime = Convert.ToDouble(item["reactionTime"]),
                            IsCorrect = Convert.ToInt32(item["isCorrect"])
                        };
                        attemptShapeTimings.Add(shapeTiming);
                    }
                    userData.ShapeTimings.Add(attemptShapeTimings);
                }
            }

            // Retrieve 'keyHoldTimes' subcollection
            CollectionReference keyHoldTimesCollection = mainDoc.Reference.Collection("keyHoldTimes");
            QuerySnapshot keyHoldTimesSnapshot = await keyHoldTimesCollection.GetSnapshotAsync();
            foreach (DocumentSnapshot doc in keyHoldTimesSnapshot.Documents)
            {
                if (doc.ContainsField("keyHoldTimes"))
                {
                    List<Dictionary<string, object>> keyHoldTimesList = doc.GetValue<List<Dictionary<string, object>>>("keyHoldTimes");
                    List<KeyHoldTime> attemptKeyHoldTimes = new List<KeyHoldTime>();
                    foreach (var item in keyHoldTimesList)
                    {
                        KeyHoldTime keyHoldTime = new KeyHoldTime
                        {
                            Duration = Convert.ToDouble(item["duration"])
                        };
                        attemptKeyHoldTimes.Add(keyHoldTime);
                    }
                    userData.KeyHoldTimes.Add(attemptKeyHoldTimes);
                }
            }

            // Retrieve 'shapeMouseMovements' subcollection
            CollectionReference shapeMouseMovementsCollection = mainDoc.Reference.Collection("shapeMouseMovements");
            QuerySnapshot shapeMouseMovementsSnapshot = await shapeMouseMovementsCollection.GetSnapshotAsync();
            foreach (DocumentSnapshot doc in shapeMouseMovementsSnapshot.Documents)
            {
                if (doc.ContainsField("mouseMovements"))
                {
                    List<Dictionary<string, object>> mouseMovementsList = doc.GetValue<List<Dictionary<string, object>>>("mouseMovements");
                    List<MouseMovement> attemptMouseMovements = new List<MouseMovement>();
                    foreach (var item in mouseMovementsList)
                    {
                        MouseMovement mouseMovement = new MouseMovement
                        {
                            Time = Convert.ToDouble(item["time"]),
                            X = Convert.ToDouble(item["x"]),
                            Y = Convert.ToDouble(item["y"])
                        };
                        attemptMouseMovements.Add(mouseMovement);
                    }
                    userData.ShapeMouseMovements.Add(attemptMouseMovements);
                }
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

        public class UserProfile
        {
            public string? LocalId { get; set; }
            public string? Email { get; set; }
            public string? DisplayName { get; set; }
        }

        private class UserInfoResponse
        {
            [JsonProperty("users")]
            public List<UserInfo>? Users { get; set; }
        }

        private class UserInfo
        {
            [JsonProperty("localId")]
            public string? LocalId { get; set; }

            [JsonProperty("email")]
            public string? Email { get; set; }

            [JsonProperty("displayName")]
            public string? DisplayName { get; set; }
        }

        private class FirebaseErrorResponse
        {
            [JsonProperty("error")]
            public FirebaseError? Error { get; set; }
        }

        private class FirebaseError
        {
            [JsonProperty("code")]
            public int Code { get; set; }

            [JsonProperty("message")]
            public string? Message { get; set; }
        }
    }
}
