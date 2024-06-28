using Firebase.Auth;
using Firebase.Auth.Providers;
using Google.Cloud.Firestore;

namespace WebAPI.Authentication
{
    public static class FirebaseConfig
    {
        public static readonly FirebaseAuthConfig AuthConfig = new()
        {
            ApiKey = "AIzaSyA6EZLpJhjfeWbU-TrUMT0m2ad9qnIZTCs",
            AuthDomain = "event-epicenter.firebaseapp.com",
            Providers = new FirebaseAuthProvider[]
            {
                new EmailProvider()
            },
        };

        public static readonly FirestoreDb database = FirestoreDb.Create("event-epicenter");
    }
}
