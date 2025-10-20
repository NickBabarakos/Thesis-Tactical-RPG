using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
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
		gameState state0 = new gameState(player.CHP, helper.CHP, enemy.CHP, counter, 0, 0);

		for (i=0; i<9; i++){
			if (i>3 && i<7){
				flag = checkCounter(i, state0.counterState);
				if (flag == true){
					continue;
				} 
			}

			gameState state1 = state0.Clone();
			helperAction(state1, chosenEnemy, helperActions,i);

			number = 0;
			sum = 0;
			if (chosenEnemy >0 && chosenEnemy < 4){
				k = 7;
			} else {
				k= 4;
			}

			for (j=0; j<k ; j++){
				if (j==0){
					flag = checkCounter(j, state1.counterState);
					if (flag == true){
						continue; 
					}
				}

				gameState state2 = state1.Clone();
				enemyAction(state2, chosenEnemy, enemyActions, j);
				grade = Evaluation(state2, enemy);
				number = number + 1;
				sum = sum + grade;
			}

			if (number > 0){
			//Η διαίρεση 2 ints στην C# επιστρέφει αποτέλεσμα int 
				expectedValue = sum/number;
			}else {
				expectedValue = 0;
			}
			moveScores.Add(new moveScore{index = i, finalGrade = expectedValue });
			GD.Print($"Η κίνηση {i} πηρε evaluation: {expectedValue}");
			

		}
		var sortedMoves = moveScores.OrderByDescending(move => move.finalGrade).ToList();


		if (sortedMoves.Count > 0){
			maxIndex = 0;
			if (sortedMoves.Count >2 && sortedMoves[0].finalGrade == sortedMoves[2].finalGrade){
				maxIndex = rng.Next(0,3);
			} else if (sortedMoves.Count > 1 && sortedMoves[0].finalGrade == sortedMoves[1].finalGrade){
				maxIndex = rng.Next(0,2);
			} 
			return helperActions[sortedMoves[maxIndex].index];
		} else {
			return helperActions[0];
		} 


	}

	public bool checkCounter(int i, int [] counterState){
		if (i==0){
			if (counterState[4] == 1){
				return true;
			}
		} else {
			if (counterState[i-3] == 1){
				return true;
			}
		}
		return false;
	}

	public void helperAction(gameState state1,  int chosenEnemy, int [] helperActions, int i){
		string attackPattern, moveNumber, counterSt="", possibility, damageId;
		int j, damage=0;

		if (i>=0 && i<4){
			attackPattern = "H" + chosenEnemy.ToString();
			moveNumber = helperActions[i].ToString();
		   
			for (j=0; j<5; j++){
			 counterSt = counterSt + state1.counterState[j].ToString();
			}

			possibility = "1";
			damageId = attackPattern + moveNumber + counterSt + possibility; 

			damage = searchDmgCatalogue(damageId,"res://Meta/damageCatalogue.json");
			state1.enemyHP = state1.enemyHP -damage;
			state1.dmg = damage;
			
		} else if (i>=4 && i<7){
			state1.counterState[i-3] = 1;
		} else if ( i == 7){
			state1.playerHP = state1.playerHP + 4;
			state1.heal = 1;
		} else if ( i == 8){
			state1.helperHP = state1.helperHP + 4;
			state1.heal=2;
		}
	}

	public void enemyAction(gameState state2, int chosenEnemy, int[] enemyActions, int j){
		string attackPattern, moveNumber, counterSt="", possibility, damageId;
		int i, dmg;
		if (j==0){
			state2.counterState[4] = 1;
		} else if (j== 3 && chosenEnemy == 4){
			state2.enemyHP = state2.enemyHP + 8;
		} else {
			attackPattern = chosenEnemy.ToString() + "H";
			if (j==2 || j==4 || j==6){
				attackPattern = chosenEnemy.ToString() + "P";
			}
			moveNumber = enemyActions[j].ToString();
			for (i=0; i<5; i++){
				counterSt = counterSt + state2.counterState[i].ToString();
			}
			possibility = "1";
			if (j==5 || j==6){
				possibility = "2";
			}
			damageId = attackPattern + moveNumber + counterSt + possibility; 
			dmg = searchDmgCatalogue(damageId,"res://Meta/damageCatalogue.json" );
			if (j==1 || j==3 || j==5){
				state2.helperHP = state2.helperHP -dmg;
			} else {
				state2.playerHP = state2.playerHP - dmg;
			}
		}
	}

	public int Evaluation(gameState state2, character enemy){
		int grade =0, sum=0, i;

		if (state2.playerHP > 20){
			grade = grade + 100;
		} else if (state2.playerHP >10 && state2.playerHP < 21){
			grade = grade + 50;
		} else if (state2.playerHP >0 && state2.playerHP < 11){
			grade = grade +10;
		} else {
			grade = grade - 1000;
		}

		if (state2.helperHP > 20){
			grade = grade + 25;
		} else if (state2.helperHP > 10 && state2.helperHP < 21){
			grade = grade + 15;
		} else if (state2.helperHP >0 && state2.helperHP < 11){
			grade = grade + 5;
		} else {
			grade = grade - 50;
		}

		if (state2.enemyHP > (enemy.HP*0.33) && state2.enemyHP <= (enemy.HP*0.66) ){
			grade = grade + 20;
		} else if (state2.enemyHP >0 && state2.enemyHP < (enemy.HP*0.33)){
			grade = grade + 50;
		} else if (state2.enemyHP == 0){
			grade = grade + 1000;
		}

		for (i=0; i<4; i++){  if (state2.counterState[i] >0){ sum = sum +1;}}
		
		if (state2.counterState[0] == 1 && state2.counterState[1] == 1){grade = grade + 10;}
		if (state2.counterState[2] == 1 && state2.counterState[3] == 1) {grade = grade + 10;}
		if (state2.counterState[0] == 0 && sum == 1 || state2.counterState[0] == 1 && sum==2){ grade = grade + 17;}
		if ( sum>0 && sum<4){ grade = grade + sum*5;} else {grade = grade + 15;}

		if (state2.dmg > 0){
			float dmgMul = 0.4f;

			if(state2.playerHP > 0 && state2.playerHP <11) { dmgMul = dmgMul*0.01f;}
			if (state2.helperHP >0 && state2.helperHP < 11) { dmgMul = dmgMul*0.5f;}
			if (state2.enemyHP < (enemy.HP*0.33)) { dmgMul = dmgMul*3f;}
			else if (state2.enemyHP <= (enemy.HP*0.66f)) {dmgMul = dmgMul*1.5f;}

			grade = grade + (int)Math.Floor(state2.dmg * dmgMul);
		}

		if (state2.heal == 1){
			if (state2.playerHP >0 && state2.playerHP < 11) { grade = grade +20;}
			else if (state2.playerHP > 10 && state2.playerHP <21) {grade = grade + 5;}
		} else if (state2.heal == 2){
			if (state2.helperHP >0 && state2.helperHP < 11) { grade = grade +10;}
		}


		return grade; 

	}

	public int searchDmgCatalogue(string damageId, string fileName){
		int dmg;
		if (!FileAccess.FileExists(fileName))
		{
			GD.PushWarning($"File not found: {fileName}");
			return 0;
		}

		try{
			string jsonString;
			using (var file = FileAccess.Open(fileName, FileAccess.ModeFlags.Read)){
				jsonString = file.GetAsText();
			}
			
			JsonNode catalogueNode = JsonNode.Parse(jsonString);

			if (catalogueNode is JsonObject catalogueObject){
				if (catalogueObject.ContainsKey(damageId)){
					dmg = (int)catalogueObject[damageId];
					return dmg; 
				} else {
					if (damageId.EndsWith("000001") || damageId.EndsWith("000002")){
						GD.Print($"Δεν υπάρχει βασικό κλειδί για την ζημιά {damageId}");
						return 0;
					}

					int startIndex = damageId.Length - 6;
					string temp = damageId.Remove(startIndex, 5);
					string baseDamageId = temp.Insert(startIndex, "00000");
					dmg = searchDmgCatalogue(baseDamageId, "res://Meta/damageCatalogue.json");
					return dmg;
				}
			} else {
				GD.PushWarning("Δεν είναι έγκυρο JSON Object");
			}

		}
		catch (Exception ex){
			GD.PushError($"Error processing file {fileName}: {ex.Message}");
			return 0;
		}
		return 0;

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
