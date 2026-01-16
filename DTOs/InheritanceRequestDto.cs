public class InheritanceRequestDto
{
    /* ================= التركة ================= */
    public bool HasCash { get; set; }
    public decimal CashAmount { get; set; }

    public bool HasGold { get; set; }
    public decimal GoldGrams { get; set; }

    public bool HasRealEstate { get; set; }
    public decimal RealEstateValue { get; set; }

    public bool HasLand { get; set; }
    public decimal LandValue { get; set; }

    /* ================= ديون ووصية ================= */
    public bool HasDebt { get; set; }
    public decimal DebtAmount { get; set; }

    public bool HasWill { get; set; }
    public decimal WillAmount { get; set; }

    /* ================= بيانات المتوفى ================= */
    public string DeceasedGender { get; set; } = "male";

    /* ================= الزوج / الزوجات ================= */
    public bool HasSpouse { get; set; }   // ✅ أضف السطر ده

    public int WivesCount { get; set; }

    /* ================= الأبناء ================= */
    public int Sons { get; set; }
    public int Daughters { get; set; }

    /* ================= الوالدان ================= */
    public bool HasFather { get; set; }
    public bool HasMother { get; set; }

    /* ================= الإخوة ================= */
    public int Brothers { get; set; }
    public int Sisters { get; set; }

    public int MaternalBrothers { get; set; }
    public int MaternalSisters { get; set; }

    public string Madhhab { get; set; } = "jumhur";
}
