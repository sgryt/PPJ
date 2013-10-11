[CmdletBinding()]
param(
	[Parameter(Mandatory=$True,Position=0)]
	[string]$CmdletAssemblyPath
)

Import-Module -Name $CmdletAssemblyPath 
Import-Module ./ClaimRules.psm1
