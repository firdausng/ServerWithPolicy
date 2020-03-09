using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerWithPolicy.Entities.Authorization
{
    public class Role : BaseEntity
    {
        public string Name { get; set; }
        public List<Subject> Subjects { get; internal set; } = new List<Subject>();
        public List<IdentityRole> IdentityRoles { get; internal set; } = new List<IdentityRole>();
        public List<PermissionRole> Permissions { get; set; } = new List<PermissionRole>();
    }

    public class IdentityRole : BaseEntity
    {
        public Guid RoleId { get; set; }
        public string Value { get; set; }
    }

    public class Subject : BaseEntity
    {
        public Guid RoleId { get; set; }
        public Guid Value { get; set; }
    }

    public class PermissionRole
    {
        public Guid PermissionId { get; set; }
        public Permission Permission { get; set; }
        public Guid RoleId { get; set; }
        public Role Role { get; set; }
    }

    public class Permission : BaseEntity
    {
        public string Name { get; set; }
        public List<PermissionRole> Roles { get; set; } = new List<PermissionRole>();
    }
}
