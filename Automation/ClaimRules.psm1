function New-ClaimRule
{
param (
	[Parameter(Mandatory=$True,Position=0)]
	[string]$ClaimRuleTitlePrefix,
	
	[Parameter(ValueFromPipeline=$true)]$RolePermission
)
    Process 
	{ 
@"
@RuleTemplate = "MapClaims"
@RuleName = "{0}{1}"
c:[Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role", Value =~ "^(?i){2}$"]
 => issue(Type = "{3}", Issuer = c.Issuer, OriginalIssuer = c.OriginalIssuer, Value = "{4}", ValueType = c.ValueType);
"@ -f $ClaimRuleTitlePrefix, $RolePermission.Title, $RolePermission.Role.Replace(" ", "\ "), $RolePermission.Permission.AbsoluteUri, $RolePermission.Value
	} 
}