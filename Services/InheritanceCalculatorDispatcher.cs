using MirathAI.Api.DTOs;
using MirathAI.Api.Enums;

namespace MirathAI.Api.Services
{
    public class InheritanceCalculatorDispatcher
    {
        private readonly InheritanceCalculationService _calc;

        public InheritanceCalculatorDispatcher(InheritanceCalculationService calc)
        {
            _calc = calc;
        }

        public InheritanceResultDto Calculate(
            InheritanceRequestDto dto,
            FiqhMadhhab madhhab)
        {
            dto.Madhhab = madhhab.ToString().ToLower();

            return madhhab switch
            {
                // الحنفية: لا يعملون الرد
                FiqhMadhhab.Hanafi =>
                    _calc.Calculate(dto, applyRadd: false),

                // باقي المذاهب + الجمهور
                _ =>
                    _calc.Calculate(dto, applyRadd: true),
            };
        }
    }
}
