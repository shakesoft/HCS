using System.Linq;
using System.Threading.Tasks;
using OpenIddict.Server;
using OpenIddict.Server.AspNetCore;
using OpenIddict.Abstractions;
using Volo.Abp.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace HC.OpenIddict;

/// <summary>
/// Handler to add client ID to token audiences when creating tokens.
/// This allows introspection to return sensitive claims for the client.
/// Also adds all related Blazor Server client IDs to support introspection from any Blazor client.
/// </summary>
public class AddClientIdToAudienceHandler : IOpenIddictServerHandler<OpenIddictServerEvents.ProcessSignInContext>, ITransientDependency
{
    private readonly IConfiguration _configuration;

    public AddClientIdToAudienceHandler(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public ValueTask HandleAsync(OpenIddictServerEvents.ProcessSignInContext context)
    {
        // Add client ID to audiences if it's not already included
        if (context.Principal != null && context.Request != null)
        {
            var clientId = context.Request.ClientId;
            if (!string.IsNullOrEmpty(clientId))
            {
                var audiences = context.Principal.GetAudiences().ToList();
                
                // Add the requesting client ID
                if (!audiences.Contains(clientId))
                {
                    audiences.Add(clientId);
                }
                
                // Add all related Blazor Server client IDs to support introspection from any Blazor client
                var openIddictSection = _configuration.GetSection("OpenIddict:Applications");
                if (openIddictSection != null)
                {
                    // Add HC_BlazorServer if configured
                    var blazorServerClientId = openIddictSection["HC_BlazorServer:ClientId"];
                    if (!string.IsNullOrWhiteSpace(blazorServerClientId) && !audiences.Contains(blazorServerClientId))
                    {
                        audiences.Add(blazorServerClientId);
                    }
                    
                    // Add HC_BlazorServer_Prod (always add as it's commonly used)
                    var blazorServerProdClientId = openIddictSection["HC_BlazorServer_Prod:ClientId"];
                    if (string.IsNullOrWhiteSpace(blazorServerProdClientId))
                    {
                        blazorServerProdClientId = "HC_BlazorServer_Prod"; // Default value
                    }
                    if (!audiences.Contains(blazorServerProdClientId))
                    {
                        audiences.Add(blazorServerProdClientId);
                    }
                }
                
                context.Principal.SetAudiences(audiences);
            }
        }

        return default;
    }
}

