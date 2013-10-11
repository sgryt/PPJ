using Excel;
using System;
using System.Collections.Generic;
using System.Data;
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
        : Cmdlet, IDisposable
    {
        private IExcelDataReader excelDataReader;
        private DataTableReader dataReader;
        private List<Permission> permissionCells = new List<Permission>(10);
        private List<RoleCell> roleCells = new List<RoleCell>(10);

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

        private string sheetName = "Rettigheder";
        [Parameter(
            Mandatory = false,
            Position = 0,
            ValueFromPipeline = false,
            ValueFromPipelineByPropertyName = false)]
        public string SheetName
        {
            get
            {
                return this.sheetName;
            }
            set
            {
                if (value == null) throw new ArgumentNullException("value");

                this.sheetName = value;
            }
        }

        private GridCoordinate permissionIdsStartCell = new GridCoordinate(4, 1);
        [Parameter(
            Mandatory = false,
            Position = 0,
            ValueFromPipeline = false,
            ValueFromPipelineByPropertyName = false)]
        public GridCoordinate PermissionIdsStartCell
        {
            get
            {
                return this.permissionIdsStartCell;
            }
            set
            {
                if (value == null) throw new ArgumentNullException("value");

                this.permissionIdsStartCell = value;
            }
        }

        private string permissionsUriPrefix = "urn:dsdn:ppj:";
        [Parameter(
            Mandatory = false,
            Position = 0,
            ValueFromPipeline = false,
            ValueFromPipelineByPropertyName = false)]
        public string PermissionsUriPrefix
        {
            get
            {
                return this.permissionsUriPrefix;
            }
            set
            {
                if (value == null) throw new ArgumentNullException("value");

                this.permissionsUriPrefix = value;
            }
        }

        private int permissionsTitleColumnIndex = 3;
        [Parameter(
            Mandatory = false,
            Position = 0,
            ValueFromPipeline = false,
            ValueFromPipelineByPropertyName = false)]
        public int PermissionsTitleColumnIndex
        {
            get
            {
                return this.permissionsTitleColumnIndex;
            }
            set
            {
                if (value < 0) throw new ArgumentException("value MUST BE non-negative");

                this.permissionsTitleColumnIndex = value;
            }
        }

        private int permissionsShortNameColumnIndex = 2;
        [Parameter(
            Mandatory = false,
            Position = 0,
            ValueFromPipeline = false,
            ValueFromPipelineByPropertyName = false)]
        public int PermissionsShortNameColumnIndex
        {
            get
            {
                return this.permissionsShortNameColumnIndex;
            }
            set
            {
                if (value < 0) throw new ArgumentException("value MUST BE non-negative");

                this.permissionsShortNameColumnIndex = value;
            }
        }

        private GridCoordinate roleValuesStartCell = new GridCoordinate(1, 5);
        [Parameter(
            Mandatory = false,
            Position = 0,
            ValueFromPipeline = false,
            ValueFromPipelineByPropertyName = false)]
        public GridCoordinate RoleValuesStartCell
        {
            get
            {
                return this.roleValuesStartCell;
            }
            set
            {
                if (value == null) throw new ArgumentNullException("value");

                this.roleValuesStartCell = value;
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

            if (!this.excelDataReader.IsValid)
            {
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        "The specified file {0} is not a valid open-xml formatted Excel file.{1}Error details:{1}{2}",
                        this.roleAssignmentFile,
                        Environment.NewLine,
                        this.excelDataReader.ExceptionMessage
                    )
                );
            }

            var dataSet = this.excelDataReader.AsDataSet();
            var sheet = dataSet.Tables[this.sheetName];
            if (sheet == null)
            {
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        "The Excel data file {0} does not contain the specified sheet '{1}'",
                        this.roleAssignmentFile,
                        this.sheetName
                    )
                );
            }

            this.dataReader = dataSet.CreateDataReader(sheet);
            if (this.dataReader == null)
            {
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        "The specified sheet name '{0}' in the Excel data file '{1}' cannot be opened for reading",
                        this.sheetName,
                        this.roleAssignmentFile
                    )
                );
            }

            using (var permissionCellsReader = dataSet.CreateDataReader(sheet))
            {
                int rowIndex = -1;
                while (permissionCellsReader.Read())
                {
                    ++rowIndex;

                    var permissionUri = permissionCellsReader.GetValue(this.permissionIdsStartCell.ColIndex) as string;
                    var permissionShortName = permissionCellsReader.GetValue(this.permissionsShortNameColumnIndex) as string;
                    var permissionTitle = permissionCellsReader.GetValue(this.permissionsTitleColumnIndex) as string;
                    if (string.IsNullOrWhiteSpace(permissionTitle))
                    {
                        permissionTitle = permissionUri;
                    } 

                    if (string.IsNullOrWhiteSpace(permissionUri)
                        || !permissionUri.StartsWith(this.permissionsUriPrefix))
                    {
                        continue;
                    }

                    this.permissionCells.Add(
                        new Permission(
                            new GridCoordinate(rowIndex, this.permissionIdsStartCell.ColIndex),
                            new Uri(permissionUri, UriKind.Absolute),
                            permissionShortName,
                            permissionTitle
                        )
                    );
                }
            }

            var disctinctPermissions = this.permissionCells.Select(rc => rc.ShortName).Distinct().ToList();
            if (disctinctPermissions.Count != this.permissionCells.Count)
            {
                var duplicates = this.permissionCells
                    .GroupBy(rc => rc.ShortName)
                    .Where(g => g.Count() > 1);

                var message = duplicates.Aggregate(
                    "",
                    (acc, dupe) => string.Format(CultureInfo.CurrentCulture,
                        "{0}'{1}' (row numbers {2}){3}",
                        acc,
                        dupe.Key,
                        string.Join(",", dupe.Select(d => (d.UriCoordinate.RowIndex + 1).ToString())),
                        Environment.NewLine
                        ));
                throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                    "Duplicate permissions found. Terminating:{0}{1}",
                    Environment.NewLine,
                    message
                ));
            }

            var colIndex = this.RoleValuesStartCell.ColIndex - 1;
            this.roleCells = sheet
                .Rows[this.RoleValuesStartCell.RowIndex]
                .ItemArray
                .Skip(this.RoleValuesStartCell.ColIndex)
                .Where(val =>
                {
                    ++colIndex;
                    var str = val as string;
                    return !string.IsNullOrWhiteSpace(str); 
                })
                .Select(val => new RoleCell(
                        new GridCoordinate(this.roleValuesStartCell.RowIndex, colIndex),
                        (string)val
                    )
                )
                .ToList();

            var disctinctRoleNames = this.roleCells.Select(rc => rc.Role).Distinct().ToList();
            if (disctinctRoleNames.Count != this.roleCells.Count)
            {
                var duplicates = this.roleCells
                    .GroupBy(rc => rc.Role)
                    .Where(g => g.Count() > 1);

                var message = duplicates.Aggregate(
                    "",
                    (acc, dupe) => string.Format(CultureInfo.CurrentCulture,
                        "{0}'{1}' (column numbers {2}){3}",
                        acc,
                        dupe.Key,
                        string.Join(",", dupe.Select(d => (d.Coordinate.ColIndex + 1).ToString())),
                        Environment.NewLine
                        ));
                throw new InvalidDataException(string.Format(CultureInfo.CurrentCulture,
                    "Duplicate role names found. Terminating:{0}{1}",
                    Environment.NewLine,
                    message
                ));
            }
        }

        protected override void ProcessRecord()
        {
            var roleColumnIndices = this.roleCells
                .Select(rc => rc.Coordinate.ColIndex)
                .ToList();

            int rowIndex = -1;
            while (this.dataReader.Read())
            {
                ++rowIndex;
                if (rowIndex <= this.RoleValuesStartCell.RowIndex)
                {
                    continue;
                }

                var columnValues = new object[dataReader.FieldCount];
                var readValues = dataReader.GetValues(columnValues);
                int colIndex = this.roleValuesStartCell.ColIndex;
                var nonEmptyRoleAssignmentColumnIndices = new List<int>();
                for (int i = this.RoleValuesStartCell.ColIndex; i < readValues; ++i)
                {
                    // This seems to be the default when nothing is filled into a cell
                    if (dataReader.IsDBNull(i))
                    {
                        continue;
                    }
                    // If the user specified blanks-only, skip the role assignment as it will be almost indistinguishable from the null-case above in the Excel UI
                    if (columnValues[i] != null &&
                        columnValues[i].GetType() == typeof(string)
                        && string.IsNullOrWhiteSpace((string)columnValues[i]))
                    {
                        continue;
                    }
                    nonEmptyRoleAssignmentColumnIndices.Add(i);
                }

                // Check for an actual permission value on the row with the role-markings.
                if (!this.permissionCells.Any(pc => pc.UriCoordinate.RowIndex == rowIndex))
                {
                    if (nonEmptyRoleAssignmentColumnIndices.Any())
                    {
                        WriteWarning(string.Format(CultureInfo.CurrentCulture,
                            "The following role values are marked as if linked to a permission, but on a row (number {2}) that contains no permissions URI value{1}{0}{1}The markings will not have any effect.",
                            nonEmptyRoleAssignmentColumnIndices.Aggregate("", (acc, value) => acc + this.roleCells.Single(rc => rc.Coordinate.ColIndex == value).Role + ","),
                            Environment.NewLine,
                            rowIndex + 1
                            )
                       );
                    }
                    continue;
                }
                // If there are no 'consumers' for the current permission, notify the user
                if (!nonEmptyRoleAssignmentColumnIndices.Any())
                {
                    WriteWarning(string.Format(CultureInfo.CurrentCulture,
                        "The permission {0} (row number {1}) is not marked for any roles. No user will ever be granted this permission.",
                        this.permissionCells.Single(pc => pc.UriCoordinate.RowIndex == rowIndex).Uri.AbsoluteUri,
                        rowIndex + 1
                    ));

                    continue;
                }

                var permissionCell = this.permissionCells.Single(pc => pc.UriCoordinate.RowIndex == rowIndex);
                // Emit a permission-for-role claim rule for each marked role value
                var claimRules = this.roleCells
                    .Where(rc => nonEmptyRoleAssignmentColumnIndices.Contains(rc.Coordinate.ColIndex))
                    .Select(rc => new RolePermissionClaimRule(
                        rc.Role, 
                        permissionCell.Uri, 
                        permissionCell.ShortName,
                        permissionCell.Title))
                    .ToList();

                WriteObject(claimRules, true);
            }
        }

        protected override void EndProcessing()
        {
            this.dataReader.Close();
            this.excelDataReader.Close();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.excelDataReader != null)
                {
                    this.excelDataReader.Dispose();
                }
                if (this.dataReader != null)
                {
                    this.dataReader.Dispose();
                }
            }
        }
    }
}
