using Excel;
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
        : Cmdlet
    {
        private IExcelDataReader excelDataReader;

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

        protected override void BeginProcessing()
        {
            WriteVerbose(string.Format(
                CultureInfo.CurrentCulture,
                "Opening file {0} for reading as an Excel open-xml formatted spreadsheet",
                this.roleAssignmentFile
                )
            );

            this.excelDataReader = ExcelReaderFactory.CreateOpenXmlReader(
                File.Open(this.roleAssignmentFile, FileMode.Open, FileAccess.Read)
            );
        }

        protected override void EndProcessing()
        {
            this.excelDataReader.Close();
        }
    }
}
