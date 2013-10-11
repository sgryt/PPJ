using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automation
{
    public class RoleCell
    {
        private readonly GridCoordinate coordinate;
        private readonly string role;

        public RoleCell(
            GridCoordinate coordinate,
            string role
        )
        {
            if (coordinate == null) throw new ArgumentNullException("coordinate");
            if (role == null) throw new ArgumentNullException("role");

            this.coordinate = coordinate;
            this.role = role;
        }

        public GridCoordinate Coordinate { get { return this.coordinate; } }

        public string Role { get { return this.role; } }
    }
}
