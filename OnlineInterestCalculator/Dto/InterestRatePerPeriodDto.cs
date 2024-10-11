using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineInterestCalculator.Dto
{
    internal record InterestRatePerPeriodDto(
        DateTime ValidFrom,
        DateTime ValidTo,
        decimal LegalRate,
        decimal DefaultRate);
}
