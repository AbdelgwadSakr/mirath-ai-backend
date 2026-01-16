using System.Text.RegularExpressions;
using MirathAI.Api.Models;

namespace MirathAI.Api.Services
{
    public class ArabicHeirsParser
    {
        public AssistantHeirsDto Parse(string text)
        {
            text = Normalize(text);
            var dto = new AssistantHeirsDto();

            // جنس المتوفى
            bool female = Regex.IsMatch(text, @"\bتوفيت\b|\bامرأه\b|\bامراه\b");
            dto.DeceasedGender = female ? "female" : "male";

            // الزوج / الزوجات
            if (!female)
            {
                // المتوفى رجل → زوجات
                dto.WivesCount = ExtractCount(text, "زوجه") ?? 0;
                if (Regex.IsMatch(text, @"\bزوجته\b|\bزوجات\b") && dto.WivesCount == 0)
                    dto.WivesCount = 1;
            }

            // الوالدان
            dto.HasMother = Regex.IsMatch(text, @"\bام\b|\bوالده\b");
            dto.HasFather = Regex.IsMatch(text, @"\bاب\b|\bوالد\b");

            // الأبناء
            dto.SonsCount = ExtractCount(text, "ابن") ?? 0;
            dto.DaughtersCount = ExtractCount(text, "بنت") ?? 0;

            // الإخوة
            dto.BrothersCount = ExtractCount(text, "اخ") ?? 0;
            dto.SistersCount = ExtractCount(text, "اخت") ?? 0;

            // ديون / وصية
            dto.MentionedDebts = Regex.IsMatch(text, @"دين|ديون|قرض");
            dto.MentionedWasiyyah = Regex.IsMatch(text, @"وصيه|اوصي|موصي");

            // التركة (آخر رقم كبير)
            dto.EstateAmount = ExtractMoney(text);

            return dto;
        }

        /* ================= Helpers ================= */

        private static string Normalize(string s)
        {
            s = s.Trim()
                .Replace("أ", "ا")
                .Replace("إ", "ا")
                .Replace("آ", "ا")
                .Replace("ة", "ه")
                .Replace("،", " ")
                .Replace(",", " ")
                .Replace("+", " ");

            s = Regex.Replace(s, @"\s+", " ");
            return s;
        }

        private static int? ExtractCount(string text, string word)
        {
            var m = Regex.Match(text, $@"(\d+)\s*{word}");
            if (m.Success) return int.Parse(m.Groups[1].Value);

            if (Regex.IsMatch(text, $@"\b{word}(ان|ين)\b")) return 2;
            if (Regex.IsMatch(text, $@"\b(ثلاث|تلات)\s+{word}")) return 3;
            if (Regex.IsMatch(text, $@"\b(اربع|اربعه)\s+{word}")) return 4;

            return null;
        }

        private static decimal? ExtractMoney(string text)
        {
            var matches = Regex.Matches(text, @"\b\d{4,}\b");
            if (matches.Count == 0) return null;

            return decimal.Parse(matches[^1].Value);
        }
    }
}
