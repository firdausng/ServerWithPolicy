using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ServerWithPolicy.Infra.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ServerWithPolicy.Authorizations.Requirements
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly AuthorizationDbContext authorizationDbContext;

        public PermissionHandler(AuthorizationDbContext authorizationDbContext)
        {
            this.authorizationDbContext = authorizationDbContext;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {

            var identity = context.User.Identity as ClaimsIdentity;
            var userClaims = identity.Claims;
            var tenantIdClaim = userClaims.FirstOrDefault(c => c.Type == "tenant_id");
            var incomingRequirement = requirement.Permission;


            if (tenantIdClaim == null)
            {
                return;
            }

            var tenantId = Guid.Parse(tenantIdClaim.Value);
            var subject = context.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            if (subject == null)
            {
                return;
            }
            // implement cache here - {tenantId}{subject}{incomingRequirement}

            var rolesFromDb = await authorizationDbContext.Roles
                .Where(r => r.TenantId.Equals(tenantId))
                .Include(r => r.Subjects)
                .Where(x => x.Subjects.Any(r => r.Value.Equals(Guid.Parse(subject))))
                //.ThenInclude(r => r.Id.Equals(Guid.Parse(sub)))
                .Select(x => x.Name)
                .ToArrayAsync()
                 ;

            var permissions = await authorizationDbContext.Permissions
                .Where(r => r.TenantId.Equals(tenantId))
                .Where(r => r.Name.Equals(incomingRequirement))
                .Where(r => r.Roles.Any(a => rolesFromDb.Contains(a.Role.Name)))
                .Select(x => x.Name)
                .ToListAsync();


            
            if (permissions.Count == 0)
            {
                return;
            }

            context.Succeed(requirement);
        }
    }
}
