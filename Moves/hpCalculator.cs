using Godot;
using System;

public static class hpCalculator
{
    public static int ApplyHeal(character Target, statMoveData data)
    {
        Random rng = new Random();
        int initialHeal = (rng.Next(1, 4)) * 2;

        if (data.moveTarget == "Enemy") { initialHeal = initialHeal * 2; }

        int previousHP = Target.CHP;
        Target.CHP += initialHeal;

        if (Target.CHP > Target.HP) { Target.CHP = Target.HP; }

        return Target.CHP - previousHP;
    }
}
