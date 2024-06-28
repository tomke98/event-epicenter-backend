using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace WebAPI.Authentication
{
    public class FirebaseAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly FirebaseApp _firebaseApp;

        public FirebaseAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, FirebaseApp firebaseApp) : base(options, logger, encoder, clock)
        {
            _firebaseApp = firebaseApp;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Context.Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.Fail("Missing authorization header.");
            }

            string bearerToken = Context.Request.Headers["Authorization"].ToString();

            if (bearerToken == null || !bearerToken.StartsWith("Bearer "))
            {
                return AuthenticateResult.Fail("Invalid token scheme (missing Bearer prefix).");
            }
            try
            {
                var firebaseToken = await FirebaseAuth.GetAuth(_firebaseApp).VerifyIdTokenAsync(bearerToken["Bearer ".Length..]);

                return AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(new List<ClaimsIdentity>() { new ClaimsIdentity( ToClaims(firebaseToken.Claims), "Bearer") }), JwtBearerDefaults.AuthenticationScheme));
            }
            catch (Exception exception)
            {
                return AuthenticateResult.Fail(exception);
            }

        }

        private IEnumerable<Claim> ToClaims(IReadOnlyDictionary<string, object> claims)
        {
            return new List<Claim>()
                {
                    new Claim("id", claims["user_id"].ToString()),
                    new Claim("email", claims["email"].ToString()),
                    new Claim(ClaimTypes.Role, claims["role"].ToString())
                };
        }
    }
}
