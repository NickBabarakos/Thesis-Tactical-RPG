using Godot;
using System;

/// <summary>
/// Ένα αντικείμενο που αντιπροσωπεύει την εκτέλεση μιας μεμονωμένης κίνησης. Είναι ο συντονιστής που λαμβάνει και καλέι τους
/// κατάλληλους calculators.
/// </summary>
public class move
{
	// --Αποτελέσματα--
	// Αυτές τις ιδιότητες διαβάζει το root μετά την εκτέλεση της κίνησης. 
	public string mdDialogue { get; private set; } // Ο διάλογος ανακοίνωσης της κίνησης.
	public string amDialogue { get; private set; } // Ο διάλογος του αποτελέσματος της κίνησης.
	public bool counterWasActive { get; private set; } // Ένα flag που δείχνει αν η κίνηση απέτυχε λόγω counter.

	// Τα δεδομένα που χρειάζεται η κλάση για να λειτουργήσει. 
	private baseMoveData moveData;
	private character _player;
	private character _helper;
	private character _enemy;
	private int _attack_pattern;
	private int[] _counter;

	public baseMoveData MoveData
	{
		get { return moveData; }
		set { moveData = value; }
	}

	public character Player
	{
		get { return _player; }
		set { _player = value; }
	}

	public character Helper
	{
		get { return _helper; }
		set { _helper = value; }
	}

	public character Enemy
	{
		get { return _enemy; }
		set { _enemy = value; }
	}

	public int attackPattern
	{
		get { return _attack_pattern; }
		set { _attack_pattern = value; }
	}

	public int[] Counter
	{
		get { return _counter; }
		set { _counter = value; }
	}

	private character Attacker, Defender;
	private Random rng = new Random();

	/// <summary>
	/// O κατασκευαστής. Αρχικοποιεί το αντικείμενο της κίνησης με όλα τα απαραίτητα δεδομένα.
	/// </summary>
	public move(baseMoveData MoveData, character player, character helper, character enemy, int attack_pattern, int[] counter)
	{
		moveData = MoveData;
		Player = player;
		Helper = helper;
		Enemy = enemy;
		attackPattern = attack_pattern;
		Counter = counter;
	}

	/// <summary>
	/// H κύρια μέθοδος εκτέλεσης.
	/// </summary>
	public void executeAction()
	{

		// Καθορίζουμε ποιος είναι ο επιτιθέμενος και ποιος ο αμυνόμενος για την κίνηση.
		switch (attackPattern)
		{
			case 1:
				Attacker = Player;
				Defender = Enemy;
				break;
			case 2:
				Attacker = Helper;
				Defender = Enemy;
				break;
			case 3:
				Attacker = Enemy;
				Defender = Player;
				break;
			default:
				Attacker = Enemy;
				Defender = Helper;
				break;
		}

		// Ελέγχουμε τον πραγματικό τύπο των δεδομένων για να αποφασίσουμε ποια λογική θα εκτελεστεί.
		if (moveData is attackMoveData attackData)
		{
			// Αν τα δεδομένα είναι τύπου attackData, καλούμε την μέθοδο για τις επιθέσεις.
			attackCalculations(attackData);
		}
		else if (moveData is statMoveData statData)
		{
			// Αν τα δεδομένα είναι τύπου statMoveData, κάνουμε ακόμα ένα έλεγχο.
			if (statData.movePower == 0) { hpCalculations(statData); } // Αν έχουμε movePower=0 σημαίνει ότι έχουμε κίνηση επαναφοράς πόντων ζωής.
			else{ statCalculations(statData); }
		}
	}

/// <summary>
/// Διαχειρίζεται τη λογική για μια επιθετική κίνηση.
/// </summary>
	public void attackCalculations(attackMoveData data)
	{
		// Αναθέτουμε τον υπολογίσουμο της βασικής ζημιάς σε εξειδικευμένη μέθοδο.
		int moveDmg = dmgCalculator.GetFinalDmg(Attacker, Defender, data);

		// Εφαρμόζουμε ειδικούς κανόνες που είναι μοναδικοί σε συγκεκριμένες κινήσεις.
		if (data.moveNumber == 111 || data.moveNumber == 113 || data.moveNumber == 114)
		{
			int rn = rng.Next(1, 3);
			if (rn == 1)
			{
				moveDmg = (int)Math.Ceiling(moveDmg + 0.25 * moveDmg);
				if (data.moveNumber == 113) { moveDmg = moveDmg + 1; }
			}
			else if (rn == 2) { moveDmg = (int)Math.Ceiling(moveDmg - 0.25 * moveDmg); }
		}

		// Δημιουργούμε τον διάλογο ανακοίνωσης.
		mdDialogue = $"{Attacker.Name} {data.dialogue1} {Defender.Name} {data.dialogue2}";

		// Ειδικός κανόνας για την κίνηση arrows που μπορεί να έχει πάνω από 1 χτυπήματα.
		if (data.moveNumber == 103)
		{
			int arrows = rng.Next(1, 4);
			moveDmg = moveDmg * arrows;
			mdDialogue = $"{Attacker.Name} {data.dialogue1} {Defender.Name} {arrows} {data.dialogue2}";
		}

		// Εφαρμόζουμε τη ζημιά και δημιουργούμε τον διάλογο αποτελέσματος.
		Defender.CHP = Defender.CHP - moveDmg;
		amDialogue = $"{Defender.Name} TOOK {moveDmg} POINTS OF DAMAGE!";
		counterWasActive = false; // Μια επίθεση δεν αποτυγχάνει ποτέ εξαιτίας των counters.
	}

	/// <summary>
	/// Διαχειρίζεται τη λογική μιας κίνησης επαναφοράς πόντων ζωής.
	/// </summary>
	public void hpCalculations(statMoveData data)
	{
		counterWasActive = false;

		character Target = null;

		// Βρίσκουμε τον στόχο της κίνησης.
		if (data.moveTarget == "Player") { Target = Player; }
		else if (data.moveTarget == "Helper") { Target = Helper; }
		else if (data.moveTarget == "Enemy") { Target = Enemy; }
		else { GD.PushWarning("Δεν βρέθηκε χαρακτήρας"); }

		// Καλούμε τον hpCalculator για να κάνει την εφαρμογή της επαναφοράς και παίρνουμε το αποτέλεσμα.
		int moveHP = hpCalculator.ApplyHeal(Target, data);

		// Δημιουργούμε τους κατάλληλους διαλόγους ανάλογα με τον στόχο.
		if (data.moveTarget == "Player")
		{
			mdDialogue = $"{Helper.Name} {data.dialogue1} {Player.Name}";
			amDialogue = $"{Player.Name} {data.dialogue2} {moveHP} HP!";
		}
		else if (data.moveTarget == "Helper")
		{
			mdDialogue = $"{Helper.Name} {data.dialogue1}";
			amDialogue = $"{Helper.Name} {data.dialogue2} {moveHP} HP!";
		}
		else if (data.moveTarget == "Enemy")
		{
			mdDialogue = $"{Enemy.Name} {data.dialogue1}";
			amDialogue = $"{Enemy.Name} {data.dialogue2} {moveHP} HP!";
		}
		else { GD.PushWarning("Δεν βρέθηκε χαρακτήρας"); }
	}

	/// <summary>
	/// Διαχειρίζεται τη λογική για μια κίνηση αλλαγής στατιστικών.
	/// </summary>
	public void statCalculations(statMoveData data)
	{
		character Target = null;

		if (data.moveTarget == "Player") { Target = Player; }
		else if (data.moveTarget == "Helper") { Target = Helper; }
		else if (data.moveTarget == "Enemy") { Target = Enemy; }
		else { GD.PushWarning("Δεν βρέθηκε χαρακτήρας"); }

		GD.Print($"DEF before {Player.DEF}");
		// Καλούμε τον statChangeCalculator. Αυτος θα ελέγξει τους counters και θα εφαρμόσει την αλλαγή.
		bool success = statChangeCalculator.ApplyStatChange(Target, data, Counter);
		GD.Print($"DEF after {Player.DEF}");

		// Ανάλογα με το αν η κίνηση πέτυχε, ορίζουμε το αποτέλεσμα.
		if (success)
		{
			counterWasActive = false;
			mdDialogue = $"{Attacker.Name} {data.dialogue1}";
			amDialogue = $"{Target.Name} {data.dialogue2}";
		}
		else
		{
			counterWasActive = true;
			mdDialogue = data.dialogue3;
		}
	}

}
