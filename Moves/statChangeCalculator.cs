using Godot;
using System;
using System.Reflection;

public static class statChangeCalculator
{
    public static bool ApplyStatChange (character Target, statMoveData data, int[] counters)
    {
        if (counters[data.moveCounter] != 0) { return false; }

        string[] stats = data.moveStat.Split(' ');

        foreach (string statName in stats)
        {
            PropertyInfo propertyInfo = typeof(character).GetProperty(statName);
            if (propertyInfo != null)
            {
                int currentValue = (int)propertyInfo.GetValue(Target);
                propertyInfo.SetValue(Target, currentValue + data.movePower);
            }
        }

        counters[data.moveCounter] = 1;
        return true;
    }
}
