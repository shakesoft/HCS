using System.Linq;
using System.Threading.Tasks;
using OpenIddict.Server;
using OpenIddict.Server.AspNetCore;
using OpenIddict.Abstractions;
using Volo.Abp.DependencyInjection;

namespace HC.OpenIddict;

/// <summary>
/// Handler to add client ID to token audiences when creating tokens.
/// This allows introspection to return sensitive claims for the client.
/// </summary>
public class AddClientIdToAudienceHandler : IOpenIddictServerHandler<OpenIddictServerEvents.ProcessSignInContext>, ITransientDependency
{
    public ValueTask HandleAsync(OpenIddictServerEvents.ProcessSignInContext context)
    {
        // Add client ID to audiences if it's not already included
        if (context.Principal != null && context.Request != null)
        {
            var clientId = context.Request.ClientId;
            if (!string.IsNullOrEmpty(clientId))
            {
                var audiences = context.Principal.GetAudiences().ToList();
                if (!audiences.Contains(clientId))
                {
                    audiences.Add(clientId);
                    context.Principal.SetAudiences(audiences);
                }
            }
        }

        return default;
    }
}

