using Duende.IdentityServer.Models;

namespace IdentityService;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
            new ApiScope("auctionApp", "Auction app full access"),
        };

    public static IEnumerable<Client> Clients =>
        new Client[]
        {
            new Client 
            {
            ClientId = "postmanClient",
            ClientName = "Postman Client",
            AllowedGrantTypes = GrantTypes.ResourceOwnerPassword, // Client credentials flow machin to machine communication, not safe just for testing/development, 
            ClientSecrets = { new Secret("postmanSecret".Sha256()) }, // Secret for the client, hashed secret using sha256. should be stored securely in production
            AllowedScopes = { "auctionApp", "openid", "profile" } // Scopes that the client has access to, in this case the auctionApp scope which is defined above and gives full access to the API, in production you should limit the scope to the minimum required for the client
            }
        };
}
