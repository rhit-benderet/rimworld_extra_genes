using RimWorld;
using Verse;

namespace OOPhoenixLords
{
    public class FullyCapitalizedXenotypeDef : XenotypeDef
    {
        public override TaggedString LabelCap
        {
            get
            {
                if (label.NullOrEmpty())
                {
                    return null;
                }

                if (cachedLabelCap.NullOrEmpty())
                {
                    cachedLabelCap = GenText.ToTitleCaseSmart(label);
                }

                return cachedLabelCap;
            }
        }
    }
}