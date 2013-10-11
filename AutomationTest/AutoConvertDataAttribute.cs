using Ploeh.AutoFixture.Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationTest
{
    public class AutoConvertDataAttribute
        : AutoDataAttribute
    {
        public AutoConvertDataAttribute()
        {
            base.Fixture.Customize(new ConvertToClaimRulesCustomization());
        }
    }

    public class InlineAutoConverteData
        : InlineAutoDataAttribute
    {
        public InlineAutoConverteData(params object[] values)
            : base(new AutoConvertDataAttribute(), values)
        {
        }
	}
}
