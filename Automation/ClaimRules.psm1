function New-ClaimRule
{
param (
	[Parameter(ValueFromPipeline=$true)]$RolePermission
)
    Process 
	{ 
@"
@RuleTemplate = "MapClaims"
@RuleName = "{0}"
c:[Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role", Value =~ "^(?i){1}$"]
 => issue(Type = "{2}", Issuer = c.Issuer, OriginalIssuer = c.OriginalIssuer, Value = "{3}", ValueType = c.ValueType);
"@ -f $RolePermission.Title, $RolePermission.Role.Replace(" ", "\ "), $RolePermission.Permission.AbsoluteUri, $RolePermission.Value
	} 
}