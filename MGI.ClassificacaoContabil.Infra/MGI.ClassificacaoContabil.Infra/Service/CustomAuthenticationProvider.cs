using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;

namespace Infra.Service
{
    public class CustomAuthenticationProvider : IAuthenticationProvider
    {
        private readonly string _accessToken;

        public CustomAuthenticationProvider(string accessToken)
        {
            _accessToken = accessToken;
        }

        public Task AuthenticateRequestAsync(RequestInformation request, Dictionary<string, object>? additionalAuthenticationContext = null, CancellationToken cancellationToken = default)
        {
            request.Headers["Authorization"] = new List<string> { $"Bearer {_accessToken}" };
            return Task.CompletedTask;
        }
    }
}
