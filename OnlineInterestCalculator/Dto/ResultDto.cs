using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineInterestCalculator.Dto
{
    internal record ResultDto(
       List<ResultLineDto> ResultLines,
       decimal InitialCapital,
       decimal LegalInterestSum,
       decimal DefaultInterestSum,
       decimal LegalInterestTotal,
       decimal DefaultInterestTotal
       );

    internal record ResultLineDto(
        DateTime ValidFrom,
        DateTime ValidTo,
        int Days,
        ResultRateDto LegalRate,
        ResultRateDto DefaultRate
        );

    internal record ResultRateDto(
       decimal Rate,
       decimal Interest
       );
}

