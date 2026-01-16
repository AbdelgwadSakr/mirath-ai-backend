namespace MirathAI.Api.DTOs
{
    public class InheritanceResultDto
    {
        public decimal TotalEstate { get; set; }
        public List<InheritanceShareDto> Shares { get; set; } = new();
        public string Madhhab { get; set; } = "jumhur";
        public string Note { get; set; } = "";
    }

    public class InheritanceShareDto
    {
        public string Heir { get; set; } = "";
        public decimal ShareRatio { get; set; }   // 0.125
        public decimal Amount { get; set; }       // قيمة فعلية
        public string Reason { get; set; } = "";
        public string Evidence { get; set; } = "";
        public string? Note { get; set; }
    }
}
