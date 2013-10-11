using Automation;
using Ploeh.AutoFixture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutomationTest
{
    public class ConvertToClaimRulesCustomization
        : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<ConvertToClaimRulesCommand>(c => 
                c
                    .OmitAutoProperties()
                    .With(cmd => cmd.RoleAssignmentFile, ".\\PPJ rettigheder 2013.10.08.xlsx")
            );
        }
    }
}
