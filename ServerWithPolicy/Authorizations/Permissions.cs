using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ServerWithPolicy.Authorizations
{
    public enum Permissions
    {
        //Here is an example of very detailed control over something
        [Display(GroupName = "Feature1", Name = "Read", Description = "Can read Feature1s")]
        Feature1Read = 0x10,
        [Display(GroupName = "Feature1", Name = "Create", Description = "Can create a Feature1 entry")]
        Feature1Create = 0x11,
        [Display(GroupName = "Feature1", Name = "Update", Description = "Can update a Feature1 entry")]
        Feature1Update = 0x12,
        [Display(GroupName = "Feature1", Name = "Delete", Description = "Can delete a Feature1 entry")]
        Feature1Delete = 0x13,

        [Display(GroupName = "UserAdmin", Name = "Read users", Description = "Can list User")]
        UserRead = 0x20,
        //This is an example of grouping multiple actions under one permission
        [Display(GroupName = "UserAdmin", Name = "Alter user", Description = "Can do anything to the User")]
        UserChange = 0x21,

        [Display(GroupName = "UserAdmin", Name = "Read Roles", Description = "Can list Role")]
        RoleRead = 0x28,
        [Display(GroupName = "UserAdmin", Name = "Change Role", Description = "Can create, update or delete a Role")]
        RoleChange = 0x29,
    }
}
