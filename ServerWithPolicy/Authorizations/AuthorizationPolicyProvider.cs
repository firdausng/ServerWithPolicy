using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using ServerWithPolicy.Authorizations.Requirements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerWithPolicy.Authorizations
{
    public class AuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
    {
        public AuthorizationPolicyProvider(IOptions<AuthorizationOptions> options) : base(options)
        {
        }


        public async override Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            // check static policies first
            var policy = await base.GetPolicyAsync(policyName);

            if (policy == null)
            {
                policy = new AuthorizationPolicyBuilder()
                    .AddRequirements(new PermissionRequirement(policyName))
                    .Build();
            }

            return policy;
        }
    }
}
