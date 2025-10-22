using Godot;
using System;
using System.Text.RegularExpressions;
using System.Reflection;

public class move 
{

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
 
	public string mdDialogue, amDialogue;
	private character Attacker, Defender;
	public bool counterWasActive;
	private Random rng = new Random();

	public move(baseMoveData MoveData,  character player, character helper, character enemy, int attack_pattern, int[] counter)
	{
		moveData = MoveData;
		Player = player;
		Helper = helper;
		Enemy = enemy;
		attackPattern = attack_pattern;
		Counter = counter;
	}

	public void executeAction(){

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
		
		if (moveData is attackMoveData attackData)
		{
			attackCalculations(attackData);
		} else if (moveData is statMoveData statData)
		{
			if (statData.movePower == 0)
			{
				hpCalculations(statData);
			} else
			{
				statCalculations(statData);
			}
		}
	}

	public void attackCalculations(attackMoveData data){

		int moveDmg = dmgCalculator.GetFinalDmg(Attacker, Defender, data);

		if (data.moveNumber == 111 || data.moveNumber == 113 || data.moveNumber == 114)
		{
			int rn = rng.Next(1,3);
			if (rn == 1) {
				moveDmg = (int)Math.Ceiling(moveDmg + 0.25 * moveDmg);
				if (data.moveNumber == 113) { moveDmg = moveDmg + 1; } 
			} else if (rn == 2) { moveDmg = (int)Math.Ceiling(moveDmg - 0.25 * moveDmg); }
		}
		
		mdDialogue = $"{Attacker.Name} {data.dialogue1} {Defender.Name} {data.dialogue2}";

		if (data.moveNumber == 103)
		{
			int arrows = rng.Next(1,4);
			moveDmg = moveDmg * arrows;
			mdDialogue = $"{Attacker.Name} {data.dialogue1} {Defender.Name} {arrows} {data.dialogue2}";
		}

			
		Defender.CHP = Defender.CHP - moveDmg;
		amDialogue = $"{Defender.Name} TOOK {moveDmg} POINTS OF DAMAGE!";
		counterWasActive = false;
	}

	public void hpCalculations(statMoveData data){
		counterWasActive = false;

		character Target = null;

		if (data.moveTarget == "Player") { Target = Player; }
		else if (data.moveTarget == "Helper") { Target = Helper; }
		else if (data.moveTarget == "Enemy") { Target = Enemy; }
		else { GD.PushWarning("Δεν βρέθηκε χαρακτήρας"); }

		int moveHP = hpCalculator.ApplyHeal(Target, data);

		if (data.moveTarget == "Player") {
			mdDialogue = $"{Helper.Name} {data.dialogue1} {Player.Name}";
			amDialogue = $"{Player.Name} {data.dialogue2} {moveHP} HP!"; 
		} else if (data.moveTarget == "Helper") {
			mdDialogue = $"{Helper.Name} {data.dialogue1}";
			amDialogue = $"{Helper.Name} {data.dialogue2} {moveHP} HP!"; 
		} else if (data.moveTarget == "Enemy") {
			mdDialogue = $"{Enemy.Name} {data.dialogue1}";
			amDialogue = $"{Enemy.Name} {data.dialogue2} {moveHP} HP!";
		} else { GD.PushWarning("Δεν βρέθηκε χαρακτήρας"); }
	}

	public void statCalculations(statMoveData data){
		character Target = null;

		if (data.moveTarget == "Player") { Target = Player; }
		else if (data.moveTarget == "Helper") { Target = Helper; }
		else if (data.moveTarget == "Enemy") { Target = Enemy; }
		else { GD.PushWarning("Δεν βρέθηκε χαρακτήρας"); }

		GD.Print($"DEF before {Player.DEF}");
		bool success = statChangeCalculator.ApplyStatChange(Target, data, Counter);
		GD.Print($"DEF after {Player.DEF}");

		if (success) {
			counterWasActive = false;
			mdDialogue = $"{Attacker.Name} {data.dialogue1}";
			amDialogue = $"{Target.Name} {data.dialogue2}";
		} else {
			counterWasActive = true;
			mdDialogue = data.dialogue3;
		}
	}

 }
