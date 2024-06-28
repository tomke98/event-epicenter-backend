using Firebase.Auth;

namespace WebAPI.Authentication
{
    public static class FirebaseHelper
    {
        public static string? HandleFirebaseError(FirebaseAuthException firebaseAuthException)
        {
            string? errorMessage = firebaseAuthException.Message;
            string? errorCode = string.Empty;

            if (!string.IsNullOrEmpty(errorMessage))
            {
                int errorCodeStartIndex = errorMessage.IndexOf("message");
                int errorCodeEndIndex = errorMessage.IndexOf("errors");

                if (errorCodeStartIndex != -1 && errorCodeEndIndex != -1 && errorCodeEndIndex > errorCodeStartIndex)
                {
                    errorCodeStartIndex += "message".Length + 4;
                    errorCode = errorMessage.Substring(errorCodeStartIndex, errorCodeEndIndex - errorCodeStartIndex - 8).Trim();
                }
            }

            return errorCode;
        }
    }
}
