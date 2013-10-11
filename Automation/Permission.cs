using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automation
{
    public class Permission
    {
        private readonly GridCoordinate uriCoordinate;
        private readonly Uri permission;
        private readonly string shortName;
        private readonly string title;

        public Permission(
            GridCoordinate uriCoordinate,
            Uri permission,
            string shortName,
            string title
        )
        {
            if (uriCoordinate == null) throw new ArgumentNullException("coordinate");
            if (permission == null) throw new ArgumentNullException("permission");
            if (shortName == null) throw new ArgumentNullException("shortName");
            if (title == null) throw new ArgumentNullException("title");

            this.uriCoordinate = uriCoordinate;
            this.permission = permission;
            this.shortName = shortName;
            this.title = title;
        }

        public GridCoordinate UriCoordinate { get { return this.uriCoordinate; } }

        public Uri Uri { get { return this.permission; } }

        public string ShortName { get { return this.shortName; } }

        public string Title { get { return this.title; } }
    }
}
