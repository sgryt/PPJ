[CmdletBinding()]
param(
	[Parameter(Mandatory=$True,Position=0)]
	[string]$RoleAssignmentExcelFile,

	[Parameter(Mandatory=$True,Position=1)]
	[string]$ClaimRulesOutputFile, 

	[Parameter(Mandatory=$True,Position=2)]
	[string]$ClaimRuleTitlePrefix,

	[Parameter(Mandatory=$False,Position=3)]
	[string]$PermissionsUriPrefix = 'http://schemas.danskeregioner.dk/2013/10/identity/claims/rettighed/',

	[Parameter(Mandatory=$False,Position=4)]
	[Automation.GridCoordinate]$PermissionIdsStartCell,

	[Parameter(Mandatory=$False,Position=5)]
	[int]$PermissionsShortNameColumnIndex = 1,

	[Parameter(Mandatory=$False,Position=6)]
	[int]$PermissionsTitleColumnIndex = 1,

	[Parameter(Mandatory=$False,Position=7)]
	[Automation.GridCoordinate]$RoleValuesStartCell
)

if ($PermissionIdsStartCell -eq $null)
{
	 $PermissionIdsStartCell = new-object Automation.GridCoordinate(5,0)
}
if ($RoleValuesStartCell -eq $null)
{
	 $RoleValuesStartCell = new-object Automation.GridCoordinate(2,10)
}

.\Register-PPJCmdlets.ps1 -CmdletAssemblyPath (gi ./Automation.dll).FullName

Write-Host ("Converting role-to-permission assignments in {0} to AD FS claim rules syntax" -f (gi $RoleAssignmentExcelFile).FullName) -ForegroundColor DarkGreen
$crs = ConvertTo-ClaimRules `
	-RoleAssignmentFile (gi $RoleAssignmentExcelFile).FullName `
	-PermissionsUriPrefix $PermissionsUriPrefix `
	-PermissionIdsStartCell $PermissionIdsStartCell `
	-PermissionsShortNameColumnIndex $PermissionsShortNameColumnIndex `
	-PermissionsTitleColumnIndex $PermissionsTitleColumnIndex `
	-RoleValuesStartCell $RoleValuesStartCell

Write-Host ("Writing {0} claim rules to {1}" -f ($crs | measure).Count, (gi $ClaimRulesOutputFile).FullName) -ForegroundColor DarkGreen
($crs | New-ClaimRule -ClaimRuleTitlePrefix $ClaimRuleTitlePrefix) -join [System.String]::Format("{0}{0}", [System.Environment]::NewLine) | Set-Content -Path $ClaimRulesOutputFile -Encoding UTF8