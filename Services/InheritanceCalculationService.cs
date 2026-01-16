using MirathAI.Api.DTOs;
using System.Linq;

namespace MirathAI.Api.Services
{
    public class InheritanceCalculationService
    {
        // ✅ OVERLOAD علشان أي استدعاء قديم يشتغل
        public InheritanceResultDto Calculate(InheritanceRequestDto p)
        {
            // الافتراضي: نفعّل الرد
            return Calculate(p, applyRadd: true);
        }

        // ===== الدالة الأساسية =====
        public InheritanceResultDto Calculate(
            InheritanceRequestDto p,
            bool applyRadd)
        {
            decimal totalEstate =
                (p.HasCash ? p.CashAmount : 0) +
                (p.HasGold ? p.GoldGrams : 0) +
                (p.HasRealEstate ? p.RealEstateValue : 0) +
                (p.HasLand ? p.LandValue : 0);

            var result = new InheritanceResultDto
            {
                TotalEstate = totalEstate,
                Madhhab = p.Madhhab
            };

            if (totalEstate <= 0)
            {
                result.Note = "لا توجد تركة";
                return result;
            }

            decimal remaining = 1m;

            bool hasDescendants = p.Sons + p.Daughters > 0;
            bool hasFather = p.HasFather;

            /* ================= الزوج / الزوجة ================= */

            if (p.DeceasedGender == "female" && p.HasSpouse)
            {
                decimal share = hasDescendants ? 1m / 4 : 1m / 2;
                remaining -= share;

                result.Shares.Add(new InheritanceShareDto
                {
                    Heir = "الزوج",
                    ShareRatio = share,
                    Amount = share * totalEstate
                });
            }

            if (p.DeceasedGender == "male" && p.WivesCount > 0)
            {
                decimal share = hasDescendants ? 1m / 8 : 1m / 4;
                remaining -= share;

                result.Shares.Add(new InheritanceShareDto
                {
                    Heir = p.WivesCount > 1 ? $"الزوجات ({p.WivesCount})" : "الزوجة",
                    ShareRatio = share,
                    Amount = share * totalEstate
                });
            }

            /* ================= الأم ================= */
            if (p.HasMother)
            {
                decimal share = hasDescendants ? 1m / 6 : 1m / 3;
                remaining -= share;

                result.Shares.Add(new InheritanceShareDto
                {
                    Heir = "الأم",
                    ShareRatio = share,
                    Amount = share * totalEstate
                });
            }

            /* ================= الأب ================= */
            if (hasFather)
            {
                if (hasDescendants)
                {
                    decimal share = 1m / 6;
                    remaining -= share;

                    result.Shares.Add(new InheritanceShareDto
                    {
                        Heir = "الأب",
                        ShareRatio = share,
                        Amount = share * totalEstate
                    });
                }
                else
                {
                    result.Shares.Add(new InheritanceShareDto
                    {
                        Heir = "الأب",
                        ShareRatio = remaining,
                        Amount = remaining * totalEstate,
                        Reason = "تعصيب"
                    });

                    remaining = 0;
                }
            }

            /* ================= الأبناء ================= */
            if (p.Sons > 0 && remaining > 0)
            {
                int units = p.Sons * 2 + p.Daughters;

                result.Shares.Add(new InheritanceShareDto
                {
                    Heir = p.Sons == 1 ? "الابن" : $"الأبناء ({p.Sons})",
                    ShareRatio = remaining * ((p.Sons * 2m) / units),
                    Amount = remaining * ((p.Sons * 2m) / units) * totalEstate,
                    Reason = "تعصيب"
                });

                if (p.Daughters > 0)
                {
                    result.Shares.Add(new InheritanceShareDto
                    {
                        Heir = p.Daughters == 1 ? "البنت" : $"البنات ({p.Daughters})",
                        ShareRatio = remaining * (p.Daughters / (decimal)units),
                        Amount = remaining * (p.Daughters / (decimal)units) * totalEstate,
                        Reason = "تعصيب مع الابن"
                    });
                }

                remaining = 0;
            }

            /* ================= البنات فقط ================= */
            if (p.Sons == 0 && p.Daughters > 0 && remaining > 0)
            {
                decimal share = p.Daughters == 1 ? 1m / 2 : 2m / 3;
                remaining -= share;

                result.Shares.Add(new InheritanceShareDto
                {
                    Heir = p.Daughters == 1 ? "البنت" : $"البنات ({p.Daughters})",
                    ShareRatio = share,
                    Amount = share * totalEstate
                });
            }

            /* ================= الإخوة ================= */
            bool siblingsBlocked =
                p.HasFather ||
                p.Sons > 0 ||
                p.Daughters > 0;

            if (!siblingsBlocked && p.Brothers + p.Sisters > 0 && remaining > 0)
            {
                int units = p.Brothers * 2 + p.Sisters;

                if (p.Brothers > 0)
                {
                    result.Shares.Add(new InheritanceShareDto
                    {
                        Heir = p.Brothers == 1 ? "الأخ" : $"الإخوة ({p.Brothers})",
                        ShareRatio = remaining * ((p.Brothers * 2m) / units),
                        Amount = remaining * ((p.Brothers * 2m) / units) * totalEstate,
                        Reason = "تعصيب"
                    });
                }

                if (p.Sisters > 0)
                {
                    result.Shares.Add(new InheritanceShareDto
                    {
                        Heir = p.Sisters == 1 ? "الأخت" : $"الأخوات ({p.Sisters})",
                        ShareRatio = remaining * (p.Sisters / (decimal)units),
                        Amount = remaining * (p.Sisters / (decimal)units) * totalEstate,
                        Reason = "تعصيب مع الأخ"
                    });
                }

                remaining = 0;
            }

            /* ================= الرد ================= */
            if (remaining > 0 && applyRadd)
            {
                var raddHeirs = result.Shares
                    .Where(s =>
                        !s.Heir.Contains("زوج") &&
                        s.Heir != "الأب" &&
                        s.Heir != "الأم")
                    .ToList();

                if (raddHeirs.Any())
                {
                    decimal totalShares = raddHeirs.Sum(s => s.ShareRatio);

                    foreach (var h in raddHeirs)
                    {
                        h.ShareRatio += (h.ShareRatio / totalShares) * remaining;
                        h.Amount = h.ShareRatio * totalEstate;
                        h.Note = "رَد";
                    }
                }
            }

            result.Note = "تم الحساب";
            return result;
        }
    }
}
