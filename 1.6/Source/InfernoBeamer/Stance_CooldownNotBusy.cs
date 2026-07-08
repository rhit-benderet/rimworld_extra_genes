using Verse;

namespace OOPhoenixLords;

public class Stance_CooldownNotBusy(int ticks, LocalTargetInfo focusTarg, Verb verb)
	: Stance_Cooldown(ticks, focusTarg, verb)
{
	public override bool StanceBusy => false;
}