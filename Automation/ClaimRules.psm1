function New-ClaimRule
{
param (
	[Parameter(ValueFromPipeline=$true)]$RolePermission
)
    Process 
	{ 
@"
@RuleName = "{0}"
c:[Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role", Value == "{1}"]
 => issue(Type = "{2}", Value = "{3}");
"@ -f $RolePermission.Title, $RolePermission.Role, $RolePermission.Permission.AbsoluteUri, $RolePermission.Value
	} 
}