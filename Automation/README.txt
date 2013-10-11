# Open a Powershell prompt, navigate to the directory where the New-ClaimRulesFile.ps1 script is located.

# Set the value of following variable to match the location of the Excel spreadsheet with the role-to-permission assignments (both relative and absolute paths will work)
$roleAssignmentFile = '../../../AutomationTest/bin/Debug/PPJ rettigheder 2013.10.08.xlsx'

# Set the value of the following variable to the file that will receive the output claim rules. Please note that the file will be overwritten if it exists.
$claimRulesOutputFile = ./PPJClaimRules.txt

# Execute the following command
./New-ClaimRulesFile.ps1 -RoleAssignmentExcelFile $roleAssignmentFile -ClaimRulesOutputFile $claimRulesOutputFile 

# The $claimRulesOutputFile file can now be used to set the claim rules on either a claims provider trust, or a relying party trust, in AD FS.
