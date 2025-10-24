using Godot;
using System;

/// <summary>
/// Μια κλάση που περιέχει τη συνάρτηση αξιολογόησης. Παίρνει μια κατάστσση του παιχνιδιού (gameState) και τις δίνει 
/// ένα βαθμό (grade) που αντιστοιχεί με το πόσο καλή είναι για τον παίκτη.
/// </summary>
public static class Evaluator
{
	/// <summary>
    /// Αξιολογεί μια δεδομένη κατάσταση της μάχης και επιστρέφει τη βαθμολογία της.
    /// </summary>
	public static int evaluation(gameState state2, character enemy)
	{   //grade=η τελική βαθμολογία, sum= βοηθητική μεταβλητή για τον υπολογισμό των ενεργών counters
		int grade = 0, sum = 0, i;

		// #1 Αξιολόγηση των πόντων ζωής του παίκτη (Υψηλότερη Προτεραιότητα)
		if (state2.playerHP > 20) { grade = grade + 100; }
		else if (state2.playerHP > 10 && state2.playerHP < 21) { grade = grade + 50; }
		else if (state2.playerHP > 0 && state2.playerHP < 11) { grade = grade + 10; }
		else { grade = grade - 1000; }

		// #2 Αξιολόγηση των πόντων ζωής του βοηθού (Μεσαία Προτεραιότητα)
		if (state2.helperHP > 20) { grade = grade + 25; }
		else if (state2.helperHP > 10 && state2.helperHP < 21) { grade = grade + 15; }
		else if (state2.helperHP > 0 && state2.helperHP < 11) { grade = grade + 5; }
		else { grade = grade - 50; }

		// #3 Αξιολόγηση των πόντων ζωής του αντιπάλου (Υψηλή Προτεραιότητα για νίκη)
		if (state2.enemyHP > (enemy.HP * 0.33) && state2.enemyHP <= (enemy.HP * 0.66)) { grade = grade + 20; }
		else if (state2.enemyHP > 0 && state2.enemyHP < (enemy.HP * 0.33)) { grade = grade + 50; }
		else if (state2.enemyHP == 0) { grade = grade + 1000; }

		// #4 Αξιολόγηση των ενεργών κινήσεων αλλαγής στατιστικών.
		// Για κάθε θετική κίνηση αλλαγής στατιστικών δίνεται ένα μικρό bonus.
		for (i = 0; i < 4; i++) { if (state2.counterState[i] > 0) { sum = sum + 1; } }

		// Ειδικά bonus για συνδυασμούς συγκεκριμένων ενεργών κινήσεων.
		if (state2.counterState[0] == 1 && state2.counterState[1] == 1) { grade = grade + 10; } // Player ATK + DEF Boost
		if (state2.counterState[2] == 1 && state2.counterState[3] == 1) { grade = grade + 10; } // Enemy ATK + DEF debugg
		if (state2.counterState[0] == 0 && sum == 1 || state2.counterState[0] == 1 && sum == 2) { grade = grade + 17; }
		if (sum > 0 && sum < 4) { grade = grade + sum * 5; } else { grade = grade + 15; } //Γενικό bonus ανάλογα με τον αριθμό των ενεργών θετικών κινήσεων αλλαγής στατιστικών.

		// #5 Αξιολόγηση της ζημιάς που έκανε ο βοηθίς.
		// Η πρόκληση ζημιάς είναι σημαντική, αλλά είναι ακόμα πιο σημαντική όταν ο εχθρός έχει λιγους πόντους ζωής.
		if (state2.dmg > 0)
		{
			float dmgMul = 0.4f; //Βασικός τροποποιητής.

			// Αν ο παίκτης δεν έχει πολλούς πόντους ζημιάς, η ζημιά δεν είναι σημαντική.
			if (state2.playerHP > 0 && state2.playerHP < 11) { dmgMul = dmgMul * 0.01f; }
			if (state2.helperHP > 0 && state2.helperHP < 11) { dmgMul = dmgMul * 0.5f; }
			// Αν ο εχθρός έχει λίγους πόντους ζωής, η ζημιά αξίζει περισσότερο.
			if (state2.enemyHP < (enemy.HP * 0.33)) { dmgMul = dmgMul * 3f; }
			else if (state2.enemyHP <= (enemy.HP * 0.66f)) { dmgMul = dmgMul * 1.5f; }

			grade = grade + (int)Math.Floor(state2.dmg * dmgMul);
		}
		// #6 Αξιολόγηση της κίνησης επαναφοράς πόντων ζωής που έγινε.
		// Μια κίνηση επαναφοράς πόντων ζωής είναι καλύτερη όταν ο στόχος της έχει λίγους πόντους ζωής.
		if (state2.heal == 1)
		{
			if (state2.playerHP > 0 && state2.playerHP < 11) { grade = grade + 20; }
			else if (state2.playerHP > 10 && state2.playerHP < 21) { grade = grade + 5; }
		}
		else if (state2.heal == 2)
		{
			if (state2.helperHP > 0 && state2.helperHP < 11) { grade = grade + 10; }
		}


		return grade; // Επιστρέφουμε την τελική, συνολική βαθμολογία.

	}
}
