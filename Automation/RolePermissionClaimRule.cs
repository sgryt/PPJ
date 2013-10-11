using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automation
{
    public class RolePermissionClaimRule
    {
        private readonly string role;
        private readonly Uri permission;
        private readonly string value;
        private readonly string title;

        public RolePermissionClaimRule(
            string role,
            Uri permission,
            string value,
            string title
        )
        {
            if (role == null) throw new ArgumentNullException("role");
            if (permission == null) throw new ArgumentNullException("permission");
            if (value == null) throw new ArgumentNullException("value");
            if (title == null) throw new ArgumentNullException("title");

            this.role = role;
            this.permission = permission;
            this.value = value;
            this.title = title;
        }

        public string Role { get { return this.role; } }

        public Uri Permission { get { return this.permission; } }

        public string Value { get { return this.value; } }

        public string Title { get { return this.title; } }
    }
}
