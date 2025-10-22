using Godot;
using System;
using System.Text.RegularExpressions;

public static class dmgCalculator
{
    
    public static int speedComparison(character Attacker, character Defender, attackMoveData data)
	{
		if (Attacker.SPD > (Defender.SPD + data.moveDelay)) { return 1;}
		else if (Attacker.SPD == (Defender.SPD + data.moveDelay)) { return 0;}
		else if (Attacker.SPD < (Defender.SPD + data.moveDelay)) { return -1; }
		return 0;
	}

	public static float vulResCalc(character Defender, attackMoveData data)
	{
		int x, y;
		string vul = Defender.VUL.ToString();
		string res = Defender.RES.ToString();
		string atk = data.moveDmgType.ToString();
		int vulLen = vul.Length;
		int resLen = res.Length;
		int atkLen = atk.Length;
		int atkRes = 0;
		int atkVul = 0;

		for (x = 0; x < atkLen; x++)
		{
			for (y = 0; y < vulLen; y++)
			{
				if (vul[y] == atk[x])
				{
					atkVul = atkVul + 1;
				}
			}
		}

		for (x = 0; x < atkLen; x++)
		{
			for (y = 0; y < resLen; y++)
			{
				if (res[y] == atk[x])
				{
					atkRes = atkRes + 1;
				}
			}
		}
		atkRes = atkRes - atkVul;

		switch (atkRes)
		{
			case 2:
				return 0.25f;
			case 1:
				return 0.5f;
			case 0:
				return 1.0f;
			case -1:
				return 2.0f;
			case -2:
				return 4.0f;

		}

		GD.Print("Error in Vul-Res calculations");
		return 0;

	}

    public static int atkPow(character Attacker, character Defender, attackMoveData data, int spdMod, float vulResMod)
    {
        int atk_pow = 0;
        int x;
        string atk = data.moveDmgType.ToString();
        bool special = false;
        bool physical = false;
        int moveDmgCategory = 0;

        for (x = 0; x < atk.Length; x++)
        {
            if (Regex.IsMatch(atk[x].ToString(), "[1-6]") && special == false)
            {
                moveDmgCategory = moveDmgCategory + 1;
                special = true;
            }

            if (Regex.IsMatch(atk[x].ToString(), "[7-9]") && physical == false)
            {
                moveDmgCategory = moveDmgCategory + 2;
                physical = true;
            }
        }


        if (moveDmgCategory == 2)
        {
            atk_pow = (int)Math.Ceiling((Attacker.ATK - 0.5 * Defender.DEF) + data.movePower);
        }
        else if (moveDmgCategory == 1)
        {
            atk_pow = (int)Math.Ceiling((Attacker.SP_ATK - 0.5 * Defender.SP_DEF) + data.movePower);
        }
        else if (moveDmgCategory == 3)
        {
            atk_pow = (int)Math.Ceiling(0.5 * (Attacker.ATK - 0.5 * Defender.DEF) + 0.5 * (Attacker.SP_ATK - 0.5 * Defender.SP_DEF) + data.movePower);
        }

        atk_pow = (int)Math.Ceiling((atk_pow * vulResMod + 0.25 * (atk_pow * spdMod)) * 0.5);

        if (atk_pow == 0)
        {
            atk_pow = 1;
        }

        return atk_pow;
    }
    
    public static int GetFinalDmg(character Attacker, character Defender, attackMoveData data)
    {
        int spdMod = speedComparison(Attacker, Defender, data);
		float vulResMod = vulResCalc(Defender, data);
        int moveDmg = atkPow(Attacker, Defender, data, spdMod, vulResMod);

        return moveDmg;
    }
    
}
