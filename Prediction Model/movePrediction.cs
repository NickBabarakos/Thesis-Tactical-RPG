using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Υλοποιεί την λογική του συστήματος επιλογής κινήσεων για τον βοηθό. 
/// </summary>
public partial class movePrediction : Node
{
	// Τα δεδομένα που αντιπροσωπεύουν την κατάσταση της μάχης την στιγμή που καλείται το σύστημα.
	private character _player;
	private character _helper;
	private character _enemy;
	private int _chosenEnemy;
	private int[] _counter;
	private int[] _helperActions;
	private int[] _enemyActions;

	public character player
	{
		get { return _player; }
		set { _player = value; }
	}

	public character helper
	{
		get { return _helper; }
		set { _helper = value; }
	}

	public character enemy
	{
		get { return _enemy; }
		set { _enemy = value; }
	}

	public int chosenEnemy
	{
		get { return _chosenEnemy; }
		set { _chosenEnemy = value; }

	}

	public int[] counter
	{
		get { return _counter; }
		set { _counter = value; }
	}

	public int[] helperActions
	{
		get { return _helperActions; }
		set { _helperActions = value; }
	}

	public int[] enemyActions
	{
		get { return _enemyActions; }
		set { _enemyActions = value; }
	}

	private Random rng = new Random();

	// Ένα struct που συνδέει το index μιας κίνησης με την τελική βαθμολογία της.
	public struct moveScore
	{
		public int index { get; set; }
		public int finalGrade { get; set; }
	}


	public movePrediction(character Player, character Helper, character Enemy, int ChosenEnemy, int[] Counter, int[] HelperActions, int[] EnemyActions)
	{
		this.player = Player;
		this.helper = Helper;
		this.enemy = Enemy;
		this.chosenEnemy = ChosenEnemy;
		this.counter = Counter;
		this.helperActions = HelperActions;
		this.enemyActions = EnemyActions;
	}

	/// <summary>
    /// Η κύρια μέθοδος του συστήματος. Εκτελεί τον αλγόριθμο αναζήτησης και επιστρέφει το ID της καλύτερης κίνησης.
    /// </summary>
	public int chooseBestAction()
	{
		int i, k, j, number, sum, expectedValue, maxIndex, grade;
		var moveScores = new List<moveScore>();

		// Δημιουργούμε την αρχική κατάσταση του παιχνιδιού πριν από οποιαδήποτε προσομοίωση.
		gameState initialState = new gameState(player.CHP, helper.CHP, enemy.CHP, counter, 0, 0, false);

		// Εξετάζουμε κάθε τις κινήσεις που μπορεί να κάνει ο βοηθός.
		for (i = 0; i < 9; i++)
		{
			// Προσομοιώνουμε την κίνηση του βοηθού καλώντας τον προσομοιωτή helperAction(). Δουλεύουμε πάνω σε 
			// αντίγραφο για να μην αλλάξουμε την αρχική κατάσταση.
			gameState stateAfterHelperAction = initialState.Clone();
			stateAfterHelperAction = simulatedBattle.helperAction(i, stateAfterHelperAction, chosenEnemy, helperActions);

			// Αν η κίνηση απέτυχε λόγω counter, την αγνοούμε και προχωράμε στην επόμενη.
			if (stateAfterHelperAction.counterFlag == true) { continue; }

			number = 0; // Μετρητής έγκυρων απαντήσεων του εχθρού.
			sum = 0;    // Το άθροισμα των βαθμολογιών από τις απαντήσεις του εχθρού.
			if (chosenEnemy > 0 && chosenEnemy < 4) { k = 7; } else { k = 4; } // Πόσες κινήσεις έχει ο εχθρός. 

			// Για κάθε κίνηση του βοηθού, εξετάζουμε όλες τις πιθανές απαντήσεις του εχθρού.
			for (j = 0; j < k; j++)
			{
				// Προσομοιώνουμε την απάντηση του εχθρού.
				gameState finalState = stateAfterHelperAction.Clone();
				finalState = simulatedBattle.enemyAction(j, finalState, chosenEnemy, enemyActions);

				// Αν η κίνηση του εχθρού απέτυχε, την αγνοούμε.
				if (finalState.counterFlag == true) { continue; }

				// Καλούμε την συνάρτηση αξιολόγησης για να βαθμολογήσει την τελική κατάσταση.
				grade = Evaluator.evaluation(finalState, enemy);
				number = number + 1;
				sum = sum + grade;
			}

			// Υπολογίζουμε την αναμενόμενη αξία της κίνησης i. Είναι ο μέσος όρος των βαθμολογιών των πιθανών εκβάσεων.
			if (number > 0) { expectedValue = sum / number; }
			else { expectedValue = 0; }

			// Αποθηκεύουμε το αποτέλεσμα.
			moveScores.Add(new moveScore { index = i, finalGrade = expectedValue });
			GD.Print($"Η κίνηση {i} πηρε evaluation: {expectedValue}");
		}

		// Ταξινομούμε τις κινήσεις από αυτή με την υψηλότερη βαθμολογία προς τη χαμηλότερη.
		var sortedMoves = moveScores.OrderByDescending(move => move.finalGrade).ToList();

		if (sortedMoves.Count > 0)
		{
			// Εφαρμόζουμε μια λογική για την αντιμετώπιση ισοβαθμιών, ώστε το σύστημα να μην είναι προβλέψιμο.
			maxIndex = 0;
			if (sortedMoves.Count > 2 && sortedMoves[0].finalGrade == sortedMoves[2].finalGrade) { maxIndex = rng.Next(0, 3); }
			else if (sortedMoves.Count > 1 && sortedMoves[0].finalGrade == sortedMoves[1].finalGrade) { maxIndex = rng.Next(0, 2); }
			return helperActions[sortedMoves[maxIndex].index]; // Επιστρέφουμε το πραγματικό αναγνωρισιτκό της κίνησης.
		}
		else { return helperActions[0]; } // Αν καμία κίνηση δεν αξιολογήθηκε, επιστρέφουμε την πρώτη.
	}

}


/* Κινήσεις Αντιπάλων 1-3: 
	j=0: Κίνηση Στατιστικών 
	j=1: Βασική Επίθεση (στόχος: Βοηθός)
	j=2: Βασική Επίθεση (στόχος: Παίκτης)
	j=3: Δευτερεύουσα Επίθεση (μειωμένη, στόχος: Βοηθος)
	j=4: Δευτεύουσα Επίθεση (μειωμένη, στόχος: Παίκτης)
	j=5: Δευτερεύουσα Επίθεση (αυξημένη, στόχος: Βοηθός)
	j=6: Δευτερεύουσα Επίθεση (αυξημένη, στόχος: Παίκτης)

	Κινήσεις Αντίπαλου 4:
	j=0: Κίνηση Στατιστικών 
	j=1: Βασική Επίθεση (στόχος: Βοηθός)
	j=2: Βασική Επίθεση (στόχος: Παίκτης)
	j=3: Κίνηση Επαναφοράς Πόντων Ζωής

	Κινήσεις Βοηθού:
	i=0: Firebolt (Επίθεση)
	i=1: Thunderbolt (Επίθεση)
	i=2: Rock Smash (Επίθεση)
	i=3: Thunderbolt (Επίθεση)
	i=4: Encouragement Spell (Κίνηση Στατιστικών, στόχος: Παίκτης)
	i=5: Intimidation Spell (Κίνηση Στατιστικών, στόχος: Αντίπαλος)
	i=6: Distraction Spell (Κίνηση Στατιστικών, στόχος: Αντίπαλος)
	i=7: Heal Player (Κίνηση Επαναφοράς Πόντων Ζωής, στόχος: Παίκτης)
	i=8: Heal Self (Κίνηση Επαναφοράς Πόντων Ζωής, στόχος: Βοηθός) */
