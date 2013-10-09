using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace Automation
{
    [Cmdlet(VerbsData.ConvertTo, "ClaimRules")]
    public class ConvertToClaimRulesCommand
    {
        private string roleAssignmentFile;
        [Parameter(
            Mandatory = true,
            Position = 0,
            ValueFromPipeline = false,
            ValueFromPipelineByPropertyName = false)]
        public string RoleAssignmentFile
        { 
            get 
            {
                return this.roleAssignmentFile;
            }
            set
            {
                if (value == null) throw new ArgumentNullException("value");

                if (!File.Exists(value))
                {
                    throw new ArgumentException(
                        string.Format(
                            CultureInfo.CurrentCulture,
                            "The specified filename '{0}' could not be found on disk",
                            value)
                        );
                }

                this.roleAssignmentFile = value;
            }
        }
    }
}
