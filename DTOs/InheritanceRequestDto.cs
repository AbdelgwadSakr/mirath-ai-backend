namespace MirathAI.Api.DTOs
{
    public class InheritanceRequestDto
    {
        public bool HasCash { get; set; }
        public decimal CashAmount { get; set; }

        public bool HasGold { get; set; }
        public decimal GoldGrams { get; set; }

        public bool HasRealEstate { get; set; }
        public decimal RealEstateValue { get; set; }

        public bool HasLand { get; set; }
        public decimal LandValue { get; set; }

        public bool HasDebt { get; set; }
        public decimal DebtAmount { get; set; }

        public bool HasWill { get; set; }
        public decimal WillAmount { get; set; }

        public string DeceasedGender { get; set; } = "male";
        public bool HasSpouse { get; set; }
        public int Sons { get; set; }
        public int Daughters { get; set; }
        public bool HasFather { get; set; }
        public bool HasMother { get; set; }

        public string Madhhab { get; set; } = "jumhur";
    }
}
