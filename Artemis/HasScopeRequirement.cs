using Microsoft.AspNetCore.Authorization;
using System;

namespace Artemis
{
    // This code is part of Auth0 implementation - https://auth0.com/docs/quickstart/backend/aspnet-core-webapi

    public class HasScopeRequirement : IAuthorizationRequirement
    {
        public string Issuer { get; }
        public string Scope { get; }

        public HasScopeRequirement(string scope, string issuer)
        {
            Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            Issuer = issuer ?? throw new ArgumentNullException(nameof(issuer));
        }
    }
}
