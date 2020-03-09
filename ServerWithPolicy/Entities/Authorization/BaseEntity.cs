using System;

namespace ServerWithPolicy.Entities.Authorization
{
    public class BaseEntity
    {
        public Guid TenantId { get; set; }
        public Guid Id { get; set; }
    }
}