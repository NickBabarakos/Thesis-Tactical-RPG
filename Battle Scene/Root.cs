using Godot;
using System;

public partial class Root : Node
{

	public Root rootNode;
	public UIManager uim;
	public turnManager TM;  
	public character player;
	public character helper;
	public character enemy;

	public int chosenAction = 101; //Η μεταβλητή που περιέχει τον αριθμό της κίνησης του παίκτη
	public move moveInstance;
	public movePrediction predictor;
	public int dmg, hp;
	public string dialogue1, dialogue2, attacker, defender;
	public int monsterId;
	public int dialogueFlow = 0;

	//public int helperChosenAction =0;
	//public int [] helperAction = {106, 107, 108, 109, 202, 203, 204, 301, 302};
	//public Button helper_action;
	public int turnNumber = 0; //for testing 
	private Random rng = new Random();

	public override void _Ready()
	{

		rootNode = GetNode<Root>(".");
		uim = GetNode<UIManager>("background/ui/UIManager");
		TM = GetNode<turnManager>("turnManager");
		LoadFighters();
		uim.dialogueBoxText("THE BATTLE BEGINS. PRESS ENTER!");
		
		//-- Για testing των κινήσεων του Helper -- 
		//helper_action = GetNode<Button>("background/ui/bot_action");
		//helper_action.FocusMode = Control.FocusModeEnum.None;
		//helper_action.Pressed += buttonPressed;
	}

	public void LoadFighters(){
		var gm = GameManager.Instance;
		monsterId = gm.Monster;
		enemy = new character(gm.enemyName, gm.monsterHp, gm.monsterCHp, gm.monsterAtk, gm.monsterDef, gm.monsterSpAtk, gm.monsterSpDef, gm.monsterSpd, gm.monsterVul, gm.monsterRes);
		player = new character(gm.playerName, 30, gm.playerCHp, 12, 10, 10, 12, 10, 0, 0 );
		helper = new character(gm.helperName, 30, gm.helperCHp, 10, 10, 12, 12, 10, 0, 0);

		uim.monsterSpriteSetUp(monsterId);
		uim.boxUiSetUp(gm.playerName, gm.helperName, gm.playerCHp, gm.helperCHp);
	}

	public override void _Input(InputEvent @event){
		if(@event.IsActionPressed("ui_cancel")){ GetTree().Quit(); }

		if (@event.IsActionPressed("ui_accept")){
			if (TM.state == turnManager.State.NS){
				uim.hpUIUpdate(player.CHP, helper.CHP);
				TM.TurnHandler();
				TM.state = turnManager.State.MD;
			} else if (TM.state == turnManager.State.MD) {
				if (TM.currentTurn == turnManager.Turn.PT)
				{
					moveInstance = new move(chosenAction, player, helper, enemy, 1, TM.counter);
					moveInstance.executeAction();
					uim.menuVisible(false);
				}
				else if (TM.currentTurn == turnManager.Turn.HT)
				{
					chosenAction = moveChooser(player, helper, enemy, TM.counter, monsterId);
					moveInstance = new move(chosenAction, player, helper, enemy, 2, TM.counter);
					moveInstance.executeAction();
				}
				else if (TM.currentTurn == turnManager.Turn.ET)
				{
					int op = rng.Next(1, 3);
					op = op + 2;
					if (TM.losses == turnManager.Losses.HL) { op = 3; }
					moveInstance = new move(chosenAction, player, helper, enemy, op, TM.counter);
					moveInstance.executeAction();
				}
				
				uim.dialogueBoxText(moveInstance.mdDialogue);
				if (moveInstance.counterWasActive == false){
					TM.state = turnManager.State.AM;
				} else {
					TM.state = turnManager.State.NS;
					TM.currentTurn = TM.previousTurn;
				}
			} else if (TM.state == turnManager.State.AM){
				uim.dialogueBoxText(moveInstance.amDialogue);
				TM.counterHandler();
				turnNumber = turnNumber + 1;
			} else if (TM.state == turnManager.State.ST){
				if (TM.losses == turnManager.Losses.PL){
					if (dialogueFlow == 0){
						uim.dialogueBoxText($"AFTER A HEROIC BATTLE {player.Name} LOST. BETTER LUCK NEXT TIME!");
						dialogueFlow = 1;
					} else if (dialogueFlow == 1) { GetTree().Quit(); }
				} else if (TM.losses == turnManager.Losses.HL) {
					if (dialogueFlow == 0){
						uim.dialogueBoxText($"{helper.Name} TRIED THEIR BEST BUT THE BATTLE WAS TO FIERCE FOR THEM!");
						TM.state = turnManager.State.NS;
					}
				} else if (TM.losses == turnManager.Losses.EL){
					if (dialogueFlow == 0){
						uim.dialogueBoxText($"YOUR PERSEVERANCE PAID OFF. YOU WIN!");
						dialogueFlow = 1;
					} else if (dialogueFlow == 1) { GoToEnemySelection(); }
				}
			}
		}


		if (TM.state == turnManager.State.MD){  //Αν το dialogue box = Move Decleration

			if (TM.currentTurn == turnManager.Turn.PT){    //Αν το Current Turn = Player Turn
				if (@event.IsActionPressed("ui_right")){ chosenAction = uim.menuHandler(chosenAction, "right"); }

				if (@event.IsActionPressed("ui_left")){ chosenAction = uim.menuHandler(chosenAction, "left"); }

				if (@event.IsActionPressed("ui_up")){ chosenAction = uim.menuHandler(chosenAction, "up");}

				if (@event.IsActionPressed("ui_down")){ chosenAction = uim.menuHandler(chosenAction, "down"); }
		 }  } }
		


	public void GoToEnemySelection(){
		if(helper.CHP < 1){
			helper.CHP = 1;
		}
		var gm = GameManager.Instance;
		gm.playerCHp = player.CHP;
		gm.helperCHp =  helper.CHP;
		gm.firstBattle = false;


		var nextScene = (PackedScene)ResourceLoader.Load("res://Enemy Selection/enemy_selection.tscn");

		if (nextScene != null) {
			GetTree().ChangeSceneToPacked(nextScene);
		}
	}

	//public void buttonPressed(){
	//	helperChosenAction = (helperChosenAction %9) +1;
	//	updateButtonText();
	//}

	/*public void updateButtonText(){
		switch(helperChosenAction){
			case 0:
				helper_action.Text = "FIREBOLT";
				break;
			case 1:
				helper_action.Text = "THUNDERBOLT";
				break;
			case 2:
				helper_action.Text = "ROCK SMASH";
				break;
			case 3:
				helper_action.Text = "THORNS";
				break;
			case 4:
				helper_action.Text = "ENCOURAGMENT";
				break;
			case 5:
				helper_action.Text = "INTIMIDATION";
				break;
			case 6: 
				helper_action.Text = "DISTRACTION";
				break;
			case 7:
				helper_action.Text = "HEAL PLAYER";
				break;
			case 8:
				helper_action.Text = "HEAL SELF";
				break;
		}
	} */

	public int moveChooser(character player, character helper, character enemy, int[] counters, int chosenEnemy){
		int [] helperActions = {106, 107, 108, 109, 202, 203, 204, 301, 302};
		int [] enemyActions;
		
		switch(chosenEnemy){
			case 1:
				enemyActions = new int[] { 205, 110, 110, 111, 111, 111, 111};
				break;
			case 2:
				enemyActions = new int[] { 206, 112, 112, 113, 113, 113, 113};
				break;
			case 3:
				enemyActions = new int[] { 207, 115, 115, 114, 114, 114, 114};
				break;
			default:
				enemyActions = new int[] { 208, 116, 116, 303};
				break;
		}
		
		predictor = new movePrediction(player, helper, enemy, chosenEnemy, counters, helperActions, enemyActions);
		int result = predictor.chooseBestAction();

		return result;

	}
}
	
