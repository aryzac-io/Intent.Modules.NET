using System;
using System.Collections.Generic;
using Intent.RoslynWeaver.Attributes;

namespace Publish.AspNetCore.MassTransit.OutBoxEF.TestApplication.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }

        public string Email { get; set; }

        public string UserName { get; set; }

        public UserType Type { get; set; }

        public virtual ICollection<Preference> Preferences { get; set; } = new List<Preference>();
    }
}