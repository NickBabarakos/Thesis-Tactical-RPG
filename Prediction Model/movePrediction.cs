using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class movePrediction : Node
{
	private character _player;
	private character _helper;
	private character _enemy;
	private int _chosenEnemy;
	private int[] _counter;
	private int[] _helperActions;
	private int[] _enemyActions;

	public character player {
		get { return _player; }
		set { _player = value; }
	}

	public character helper{
		get {return _helper; }
		set {_helper = value;}
	}

	public character enemy {
		get { return _enemy; }
		set { _enemy = value; }
	}

	public int  chosenEnemy {
		get { return _chosenEnemy; }
		set { _chosenEnemy = value; }

	}

	public int [] counter {
		get { return _counter;}
		set { _counter = value; }
	}

	public int [] helperActions {
		get { return _helperActions; }
		set { _helperActions = value;}
	}

	public int [] enemyActions {
		get { return _enemyActions; }
		set { _enemyActions = value; }
	}

	private Random rng = new Random();
	
	public struct moveScore {
		public int index { get; set; }
		public int finalGrade { get; set; }
	}



	public movePrediction (character Player, character Helper, character Enemy, int ChosenEnemy, int [] Counter, int [] HelperActions, int [] EnemyActions ){
		this.player = Player;
		this.helper = Helper;
		this.enemy = Enemy;
		this.chosenEnemy = ChosenEnemy;
		this.counter = Counter;
		this.helperActions = HelperActions;
		this.enemyActions = EnemyActions;
	}

	public int chooseBestAction(){
		int i,k, j, number, sum, expectedValue, maxIndex, grade;
		var moveScores = new List<moveScore>();
		bool flag; 
		gameState initialState = new gameState(player.CHP, helper.CHP, enemy.CHP, counter, 0, 0, false);

		for (i = 0; i < 9; i++)
		{
			gameState stateAfterHelperAction = initialState.Clone();
			stateAfterHelperAction = simulatedBattle.helperAction(i, stateAfterHelperAction, chosenEnemy, helperActions);
			if (stateAfterHelperAction.counterFlag == true) { continue; }

			number = 0;
			sum = 0;
			if (chosenEnemy > 0 && chosenEnemy < 4) { k = 7; } else { k = 4; }

			for (j = 0; j < k; j++)
			{
				gameState finalState = stateAfterHelperAction.Clone();
				finalState = simulatedBattle.enemyAction(j, finalState, chosenEnemy, enemyActions);
				if (finalState.counterFlag == true) { continue; }

				grade = Evaluator.evaluation(finalState, enemy);
				number = number + 1;
				sum = sum + grade;
			}

			//Η διαίρεση 2 ints στην C# επιστρέφει αποτέλεσμα int 
			if (number > 0) { expectedValue = sum / number; }
			else { expectedValue = 0; }
			moveScores.Add(new moveScore { index = i, finalGrade = expectedValue });
			GD.Print($"Η κίνηση {i} πηρε evaluation: {expectedValue}");
		}
		
		var sortedMoves = moveScores.OrderByDescending(move => move.finalGrade).ToList();

		if (sortedMoves.Count > 0){
			maxIndex = 0;
			if (sortedMoves.Count > 2 && sortedMoves[0].finalGrade == sortedMoves[2].finalGrade) { maxIndex = rng.Next(0, 3); }
			else if (sortedMoves.Count > 1 && sortedMoves[0].finalGrade == sortedMoves[1].finalGrade) { maxIndex = rng.Next(0, 2); } 
			return helperActions[sortedMoves[maxIndex].index];
		} else { return helperActions[0]; } 
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
