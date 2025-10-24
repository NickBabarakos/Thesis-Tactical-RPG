using Godot;
using System;
using System.Text.RegularExpressions;

/// <summary>
/// Μια κλάση υπεύθυνη για όλους τους υπολογισμούς που σχετίζονται με τη ζημιά των επιθέσεων.
/// </summary>
public static class dmgCalculator
{

	/// <summary>
	/// Η κύρια μέθοδος της κλάσης. Καλεί τις τρεις μεθόδους/βήματα του υπολογισμού της ζημιάς και επιστρέφει το τελικό αποτέλεσμα.
	/// </summary>
	public static int GetFinalDmg(character Attacker, character Defender, attackMoveData data)
	{
		int spdMod = speedComparison(Attacker, Defender, data);
		float vulResMod = vulResCalc(Defender, data);
		int moveDmg = atkPow(Attacker, Defender, data, spdMod, vulResMod);

		return moveDmg;
	}

	/// <summary>
	/// Υπολογίζει τον τροποποιητή ταχύτητας βάσει της διαφοράς ταχύτητας μεταξύ του αμυνόμενου και του επιτιθέμενου.
	/// </summary>
	private static int speedComparison(character Attacker, character Defender, attackMoveData data)
	{
		if (Attacker.SPD > (Defender.SPD + data.moveDelay)) { return 1; }
		else if (Attacker.SPD == (Defender.SPD + data.moveDelay)) { return 0; }
		else if (Attacker.SPD < (Defender.SPD + data.moveDelay)) { return -1; }
		return 0;
	}

	/// <summary>
	/// Υπολογίζει τον τροποποιητή αντοχών/αδυναμιών του αμυνόμενου σε σχέση με το τύπο της επίθεσης.
	/// </summary>
	private static float vulResCalc(character Defender, attackMoveData data)
	{
		int x, y;
		// Μετατρέπουμε τους αριθμούς σε string για να ελέγξουμε κάθε ψηφίο ξεχωριστά. 
		string vul = Defender.VUL.ToString();
		string res = Defender.RES.ToString();
		string atk = data.moveDmgType.ToString();
		int vulLen = vul.Length;
		int resLen = res.Length;
		int atkLen = atk.Length;
		int atkRes = 0;  // Μετράει πόσες φορές ένας τύπος επίθεσης βρίσκεται στις αντιστάσεις.
		int atkVul = 0;  // Μετράει πόσες φορές ένας τύπος επίθεσης βρίσκεται στις αδυναμίες.

		// Βρόγοχς για τον έλεγχο των αδυναμιών.
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

		// Βρόγχος για τον έλεγχο των αντιστάσεων.
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
		// Το τελικό σκορ είναι η διαφορά. Ένα αρνητικό σκορ σημαίνει ότι βρήκαμε περισσότερες αδυναμίες.
		atkRes = atkRes - atkVul;

		// Μετατρέπουμε το τελικό σκορ στο αντίστοιχο τροποποιητή.
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

	/// <summary>
    /// O κεντρικός τύπος που υπολογίζει τη ζημιά, συνδυάζοντας τα στατιστικά των χαρακτήρων, τη δύναμη της κίνησης και τους
	/// τροποποιητές που υπολογίστικαν προηγουμένως.
    /// </summary>
	public static int atkPow(character Attacker, character Defender, attackMoveData data, int spdMod, float vulResMod)
	{
		int atk_pow = 0;
		int x;
		string atk = data.moveDmgType.ToString();
		bool special = false;
		bool physical = false;
		int moveDmgCategory = 0; // 1=Special, 2=Physical, 3=Both

		// Καθορίζουμε την κατηγορία ζημιάς (Physical, Special, Both). Τα ψηφία 1-6 είναι Special, 7-9 είναι Physical.
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

		// Υπολογίζουμε τη βασική ζημιά πριν τους τροποποιητές.
		if (moveDmgCategory == 2) // Καθαρά Physical
		{
			atk_pow = (int)Math.Ceiling((Attacker.ATK - 0.5 * Defender.DEF) + data.movePower);
		}
		else if (moveDmgCategory == 1) // Καθαρά Special
		{
			atk_pow = (int)Math.Ceiling((Attacker.SP_ATK - 0.5 * Defender.SP_DEF) + data.movePower);
		}
		else if (moveDmgCategory == 3) // Both
		{
			atk_pow = (int)Math.Ceiling(0.5 * (Attacker.ATK - 0.5 * Defender.DEF) + 0.5 * (Attacker.SP_ATK - 0.5 * Defender.SP_DEF) + data.movePower);
		}

		// Εφαρμόζουμε τους τροποιητές και έναν γενικό τροποποιητή (0.5).
		atk_pow = (int)Math.Ceiling((atk_pow * vulResMod + 0.25 * (atk_pow * spdMod)) * 0.5);
		if (atk_pow == 0) { atk_pow = 1; } //Διασφαλίζουμε ότι η ζημιά είναι τουλάχιστον 1.
		return atk_pow;
	}



}
