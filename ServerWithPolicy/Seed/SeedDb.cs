using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ServerWithPolicy.Authorizations;
using ServerWithPolicy.Entities.Authorization;
using ServerWithPolicy.Entities.Main;
using ServerWithPolicy.Infra.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ServerWithPolicy.Seed
{
    public class SeedDb
    {
        private readonly ApplicationDbContext applicationDbContext;
        private readonly ILogger<SeedDb> logger;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly AuthorizationDbContext authorizationDbContext;

        public SeedDb(ApplicationDbContext applicationDbContext,
            ILogger<SeedDb> logger,
            UserManager<ApplicationUser> userManager,
            AuthorizationDbContext authorizationDbContext
            )
        {
            this.applicationDbContext = applicationDbContext;
            this.logger = logger;
            this.userManager = userManager;
            this.authorizationDbContext = authorizationDbContext;

            applicationDbContext.Database.Migrate();
            authorizationDbContext.Database.Migrate();
        }

        private Tenant mainTenant;
        private Tenant secondTenant;


        private ApplicationUser alice;
        private ApplicationUser daus;
        private ApplicationUser gee;

        public void SeedTenants()
        {
            mainTenant = applicationDbContext.Tenants.Where(t => t.Name.Equals("Testnt")).FirstOrDefaultAsync().Result;
            if (mainTenant == null)
            {
                logger.LogInformation("cannot find Testnt tenant");
                mainTenant = new Tenant
                {
                    Name = "Testnt",

                };
                applicationDbContext.Tenants.Add(mainTenant);
                applicationDbContext.SaveChanges();
                logger.LogInformation("Testnt tenant created");
            }
            else
            {
                logger.LogInformation("Testnt tenant created");
            }


            secondTenant = applicationDbContext.Tenants.Where(t => t.Name.Equals("second")).FirstOrDefaultAsync().Result;
            if (secondTenant == null)
            {
                logger.LogInformation("cannot find second tenant");
                secondTenant = new Tenant
                {
                    Name = "second",

                };
                applicationDbContext.Tenants.Add(secondTenant);
                applicationDbContext.SaveChanges();
                logger.LogInformation("second tenant created");
            }
            else
            {
                logger.LogInformation("second tenant created");
            }
        }
        public void SeedUsers()
        {
            alice = userManager.FindByNameAsync("alice").Result;
            if (alice == null)
            {
                logger.LogInformation("Creating Alice");
                alice = new ApplicationUser
                {
                    UserName = "alice",
                    Email = "AliceSmith@email.com",
                    EmailConfirmed = true,
                    TenantId = mainTenant.Id
                };
                var result = userManager.CreateAsync(alice, "Password@01").Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                result = userManager.AddClaimsAsync(alice, new Claim[]{
                        new Claim("tenant_id", alice.TenantId.ToString()),
                    }).Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }
                logger.LogInformation("alice created");
            }

            daus = userManager.FindByNameAsync("daus").Result;
            if (daus == null)
            {
                logger.LogInformation("Creating daus");
                daus = new ApplicationUser
                {
                    UserName = "daus",
                    Email = "daus@email.com",
                    EmailConfirmed = true,
                    TenantId = secondTenant.Id
                };
                var result = userManager.CreateAsync(daus, "Password@01").Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }

                result = userManager.AddClaimsAsync(daus, new Claim[]{
                        new Claim("tenant_id", daus.TenantId.ToString()),
                    }).Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }
                logger.LogInformation("daus created");
            }

            gee = userManager.FindByNameAsync("gee").Result;
            if (gee == null)
            {
                logger.LogInformation("Creating gee");
                gee = new ApplicationUser
                {
                    UserName = "gee",
                    Email = "gee@email.com",
                    EmailConfirmed = true,
                    TenantId = mainTenant.Id
                };
                var result = userManager.CreateAsync(gee, "Password@01").Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }
                result = userManager.AddClaimsAsync(gee, new Claim[]{
                        new Claim("tenant_id", gee.TenantId.ToString()),
                    }).Result;
                if (!result.Succeeded)
                {
                    throw new Exception(result.Errors.First().Description);
                }
                logger.LogInformation("gee created");
            }
        }

        public void SeedPolicy()
        {
            var pp = authorizationDbContext.Permissions
                .Where(p => p.TenantId.Equals(mainTenant.Id))
                .Include(p => p.Roles)
                .ThenInclude(p => p.Role)
                .ToList();
            if (pp.Count == 0)
            {
                logger.LogInformation("Creating all permissions from enum");
                foreach (Permissions p in Enum.GetValues(typeof(Permissions)))
                {
                    pp.Add(new Permission
                    {
                        Name = p.ToString(),
                        TenantId = mainTenant.Id
                    });
                }

                authorizationDbContext.Permissions.AddRange(pp);
                
            }



            var admin = authorizationDbContext.Roles
                .Where(p => p.TenantId.Equals(mainTenant.Id))
                .Include(p => p.Permissions)
                .ThenInclude(p => p.Permission)
                .FirstOrDefault(p => p.Name.Equals("Admin"));
            if (admin == null)
            {
                logger.LogInformation("cannot find admin subject");
                admin = new Role
                {
                    //Value = Guid.Parse(alice.Id),
                    TenantId = mainTenant.Id,
                    Name = "Admin",
                    Subjects = new List<Subject>
                        {
                            new Subject{TenantId= mainTenant.Id, Value=Guid.Parse(alice.Id)},
                            //new Subject{TenantId= tenantId, Value=Guid.Parse("e3cc45c3-abb7-4a58-9267-15ea763706ee")},
                        }

                };
                var userChangePermission = pp.Where(p => p.Name.Equals(Permissions.UserChange.ToString())).Single();
                admin.Permissions.Add(new PermissionRole
                {
                    Permission = userChangePermission
                });
                logger.LogInformation("Creating admin role");
                authorizationDbContext.Roles.Add(admin);
            }
            authorizationDbContext.SaveChanges();
        }
    }
}