using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Reflection;

public partial class move : Node
{

	private int _chosenAction;
	private character _player;
	private character _helper;
	private character _enemy;
	private int _attack_pattern;
	private int[] _counter;


	public int moveNumber
	{
		get { return _chosenAction; }
		set { _chosenAction = value; }
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

	private int movePower, moveDelay, moveDmgType, moveCounter, moveDmg, moveHP; 
	private string moveName, moveStat, moveTarget, dialogue1, dialogue2, dialogue3;
	public string mdDialogue, amDialogue;
	private character Attacker, Defender;
	public bool counterWasActive;
	private Random rng = new Random();

	public move(int chosen_action, character player, character helper, character enemy, int attack_pattern, int[] counter)
	{
		moveNumber = chosen_action;
		Player = player;
		Helper = helper;
		Enemy = enemy;
		attackPattern = attack_pattern;
		Counter = counter;
	}

	public void executeAction(){
		int moveType = moveNumber/100;

			switch (attackPattern){
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
		if (moveType == 1){

			LoadMove(1, "res://Meta/AttacksList.json" );
			attackCalculations();
		}else {
			LoadMove(2, "res://Meta/StatChangeList.json");
			if (movePower == 0){
				hpCalculations();

			} else {
				statCalculations();
			}
		}
	}

	public void LoadMove(int moveType, string fileName){
		if (!FileAccess.FileExists(fileName)){
			GD.PushWarning($"File not found: {fileName}");
			return;
		}

		string jsonString = "";

		using (var file = FileAccess.Open(fileName, FileAccess.ModeFlags.Read)){
			jsonString = file.GetAsText();
		}

		try{
			JsonNode root = JsonNode.Parse(jsonString);

			if (root is JsonArray movesArray)
			{
				foreach (var moveObject in movesArray)
				{
					if (moveObject is JsonObject currentMove)
					{
						if (currentMove.TryGetPropertyValue("Number", out JsonNode numberNode) && numberNode.GetValue<int>() == moveNumber)
						{
							if (moveType == 1){
								moveName = moveObject["Name"]?.GetValue<string>() ?? "";
								movePower = moveObject["Power"]?.GetValue<int>() ?? 0;
								moveDelay = moveObject["Delay"]?.GetValue<int>() ?? 0;
								moveDmgType = moveObject["Type"]?.GetValue<int>() ?? 0;
								dialogue1 = moveObject["dialogue1"]?.GetValue<string>() ?? "";
								dialogue2 = moveObject["dialogue2"]?.GetValue<string>() ?? "";
							}else {
								moveName = moveObject["Name"]?.GetValue<string>() ?? "";
								moveStat = moveObject["Stat"]?.GetValue<string>() ?? "";
								moveTarget = moveObject["Target"]?.GetValue<string>() ?? "";
								movePower = moveObject["Power"]?.GetValue<int>() ?? 0;
								moveCounter = moveObject["Counter"]?.GetValue<int>() ?? -1;
								dialogue1 = moveObject["dialogue1"]?.GetValue<string>() ?? "";
								dialogue2 = moveObject["dialogue2"]?.GetValue<string>() ?? "";
								dialogue3 = moveObject["dialogue3"]?.GetValue<string>() ?? "";
							}
						}
					}
				}
			}
		}
		catch (JsonException e)
		{
			GD.PushError($"Error parsing Json in {fileName}: {e.Message}");
		}
		catch (Exception e)
		{
			GD.PushError($"An unexpected error occured while loading {fileName} : {e.Message}");
		}
	}

	public void attackCalculations(){
		int spdMod = speedComparison();
		float vulResMod = vulResCalc();
		int moveDmg = atkPow(spdMod, vulResMod);

		if (moveNumber == 111 || moveNumber == 113 || moveNumber == 114)
		{
			int rn = rng.Next(1,3);
			if (rn == 1)
			{
				moveDmg = (int)Math.Ceiling(moveDmg + 0.25 * moveDmg);
				if (moveNumber == 113)
				{
					moveDmg = moveDmg + 1;
				} 
			}
			else if (rn == 2)
			{
				moveDmg = (int)Math.Ceiling(moveDmg - 0.25 * moveDmg);
			}
		}
		
		mdDialogue = $"{Attacker.Name} {dialogue1} {Defender.Name} {dialogue2}";

		if (moveNumber == 103)
		{
			int arrows = rng.Next(1,4);
			moveDmg = moveDmg * arrows;
			mdDialogue = $"{Attacker.Name} {dialogue1} {Defender.Name} {arrows} {dialogue2}";
		}

			
		Defender.CHP = Defender.CHP - moveDmg;
		amDialogue = $"{Defender.Name} TOOK {moveDmg} POINTS OF DAMAGE!";
		counterWasActive = false;
	}

	public void hpCalculations(){
		counterWasActive = false;
			moveHP = (rng.Next(1,4)) * 2;

			if (moveTarget == "Player")
			{
				Player.CHP = Player.CHP + moveHP;
				if (Player.CHP > Player.HP)
				{
					moveHP = Player.HP - Player.CHP + moveHP;
					Player.CHP = Player.HP;
				}
				mdDialogue = $"{Helper.Name} {dialogue1} {Player.Name}";
				amDialogue = $"{Player.Name} {dialogue2} {moveHP} HP!";
			}
			else if (moveTarget == "Helper")
			{
				Helper.CHP = Helper.CHP + moveHP;
				if (Helper.CHP > Helper.HP)
				{
					moveHP = Helper.HP - Helper.CHP + moveHP;
					Helper.CHP = Helper.HP;
				}
				mdDialogue = $"{Helper.Name} {dialogue1}";
				amDialogue = $"{Helper.Name} {dialogue2} {moveHP} HP!";
			}
			else if (moveTarget == "Enemy")
			{
				moveHP = moveHP * 2;
				Enemy.CHP = Enemy.CHP + moveHP;
				if (Enemy.CHP > Enemy.HP)
				{
					moveHP = Enemy.HP - Enemy.CHP + moveHP;
					Enemy.CHP = Enemy.HP;
				}
				mdDialogue = $"{Enemy.Name} {dialogue1}";
				amDialogue = $"{Enemy.Name} {dialogue2} {moveHP} HP!";
			}
	}

	public void statCalculations()
	{
		string stat0 = "";
		string stat1 = "";

		character Target = null;
		if (moveTarget == "Player")
		{
			Target = Player;
		}
		else if (moveTarget == "Enemy")
		{
			Target = Enemy;
		}
		else if (moveTarget == "Helper")
		{
			Target = Helper;
		}

		string[] stats = moveStat.Split(' ');
		stat0 = stats[0];
		if (Counter[moveCounter] == 0)
		{
			PropertyInfo propertyInfo = typeof(character).GetProperty(stat0);
			int currentValue = (int)propertyInfo.GetValue(Target);
			propertyInfo.SetValue(Target, currentValue + movePower);
			Counter[moveCounter] = 1;
			counterWasActive = false;

			if (stats.Length == 2)
			{
				stat1 = stats[1];
				propertyInfo = typeof(character).GetProperty(stat1);
				currentValue = (int)propertyInfo.GetValue(Target);
				propertyInfo.SetValue(Target, currentValue + movePower);
			}
		}
		else
		{
			counterWasActive = true;
		}


		if (counterWasActive == true)
		{
			mdDialogue = dialogue3;
		}
		else
		{
			mdDialogue = $"{Attacker.Name} {dialogue1}";
			amDialogue = $"{Target.Name} {dialogue2}";
			}
	}
	
	private int speedComparison()
	{
		if (Attacker.SPD > (Defender.SPD + moveDelay))
		{
			return 1;
		}
		else if (Attacker.SPD == (Defender.SPD + moveDelay))
		{
			return 0;
		}
		else if (Attacker.SPD < (Defender.SPD + moveDelay))
		{
			return -1;
		}
		GD.Print("Error in SpeedComparison");
		return 0;
	}

	private float vulResCalc()
	{
		int x, y;
		string vul = Defender.VUL.ToString();
		string res = Defender.RES.ToString();
		string atk = moveDmgType.ToString();
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

	private int atkPow(int spdMod, float vulResMod)
	{
		int atk_pow = 0;
		int x;
		string atk = moveDmgType.ToString();
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
			atk_pow = (int)Math.Ceiling((Attacker.ATK - 0.5 * Defender.DEF) + movePower);
		}
		else if (moveDmgCategory == 1)
		{
			atk_pow = (int)Math.Ceiling((Attacker.SP_ATK - 0.5 * Defender.SP_DEF) + movePower);
		}
		else if (moveDmgCategory == 3)
		{
			atk_pow = (int)Math.Ceiling(0.5 * (Attacker.ATK - 0.5 * Defender.DEF) + 0.5 * (Attacker.SP_ATK - 0.5 * Defender.SP_DEF) + movePower);
		}

		atk_pow = (int)Math.Ceiling((atk_pow * vulResMod + 0.25 * (atk_pow * spdMod)) * 0.5);

		if (atk_pow == 0){
			atk_pow = 1;
		}
		
		return atk_pow;
	}


	 }
