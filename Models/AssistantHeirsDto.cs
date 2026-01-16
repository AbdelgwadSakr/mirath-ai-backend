namespace MirathAI.Api.Models
{
    public class AssistantHeirsDto
    {
        // جنس المتوفى
        public string DeceasedGender { get; set; } = "male";

        // الزوجات (للذكر فقط)
        public int WivesCount { get; set; }

        // الوالدان
        public bool HasFather { get; set; }
        public bool HasMother { get; set; }

        // الأبناء
        public int SonsCount { get; set; }
        public int DaughtersCount { get; set; }

        // الإخوة الأشقاء / لأب
        public int BrothersCount { get; set; }
        public int SistersCount { get; set; }

        // ديون ووصية
        public bool MentionedDebts { get; set; }
        public bool MentionedWasiyyah { get; set; }

        // التركة
        public decimal? EstateAmount { get; set; }
    }
}
