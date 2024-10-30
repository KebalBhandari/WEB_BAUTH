namespace WEB_BA.Services
{
    public class FirebaseAuthenticationException : Exception
    {
        public int Code { get; }
        public string? FirebaseMessage { get; }

        public FirebaseAuthenticationException(int code, string firebaseMessage)
            : base($"Firebase Authentication Error {code}: {firebaseMessage}")
        {
            Code = code;
            FirebaseMessage = firebaseMessage;
        }
    }

}
