[CmdletBinding()]
param(
	[Parameter(Mandatory=$True,Position=0)]
	[string]$RoleAssignmentExcelFile,

	[Parameter(Mandatory=$True,Position=1)]
	[string]$ClaimRulesOutputFile
)

.\Register-PPJCmdlets.ps1 -CmdletAssemblyPath (gi ./Automation.dll).FullName
Import-Module ./ClaimRules.psm1

Write-Host ("Converting role-to-permission assignments in {0} to AD FS claim rules syntax" -f (gi $RoleAssignmentExcelFile).FullName)
$crs = ConvertTo-ClaimRules -RoleAssignmentFile (gi $RoleAssignmentExcelFile).FullName

Write-Host ("Writing {0} claim rules to {1}" -f ($crs | measure).Count, (gi $ClaimRulesOutputFile).FullName)
($crs | New-ClaimRule) -join [System.String]::Format("{0}{0}", [System.Environment]::NewLine) | Set-Content -Path $ClaimRulesOutputFile