using Duende.IdentityServer.Models;

namespace Identity;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId() , new IdentityResources.Profile()
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
            new("analysis"      , "Analysis API")
          , new("form"          , "Form API")
          , new("group"         , "Group API")
          , new("media"         , "Media API")
          , new("technique"     , "Technique API")
          , new("localisation"  , "Localisation API")
        };

    public static IEnumerable<Client> Clients =>
        new Client[]
        {
            // interactive client using code flow + pkce
            new()
            {
                AccessTokenType        = AccessTokenType.Jwt
              , ClientName             = "Prometheus Client"
              , ClientId               = "prometheus_client"
              , ClientSecrets          = { new Secret( "49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256() ) }
              , AllowedGrantTypes      = GrantTypes.Code
              , RedirectUris           = { "https://localhost:44300/signin-oidc" }
              , FrontChannelLogoutUri  = "https://localhost:44300/signout-oidc"
              , PostLogoutRedirectUris = { "https://localhost:44300/signout-callback-oidc" }
              , AllowOfflineAccess     = true
              , RequireClientSecret    = false
              , AllowedScopes =
                {
                    "openid"
                  , "profile"
                  , "analysis"
                  , "form"
                  , "group"
                  , "media"
                  , "technique"
                  , "localisation"
                }
            }
        };
}