using Godot;
using System;

public partial class Root : Node
{

	public Root rootNode;
	public Label dialogue;  //Reference στο Label που γράφεται ο διάλογος
	public Label sword_slash; //Reference στο Label που επιλέγει την κίνηση sword slash
	public Label shield;
	public Label hydro_pump;
	public Label arrows;
	public Label fire_pierce;
	public Label electric_hammer;
	public Label helper_name;
	public Label player_name;
	public Label player_hp_1;
	public Label player_hp_2;
	public Label helper_hp_1;
	public Label helper_hp_2;
	public Button helper_action;
	public Sprite2D monsterSprite;

	public turnManager TM;  
	public character player;
	public character helper;
	public character enemy;
	public Color red = new Color(1,0,0); //To χρώμα που παίρνει μια κίνηση όταν είναι επιλεγμένη
	public Color black = new Color(0,0,0); //Το χρώμα που παίρνει μια κίνηση όταν δεν είναι επιλέγμένη
	public int chosenAction = 101; //Η μεταβλητή που περιέχει τον αριθμό της κίνησης του παίκτη
	public move moveInstance;
	public movePrediction predictor;
	public int dmg, hp;
	public string dialogue1, dialogue2, attacker, defender;
	public int monsterId;
	public int dialogueFlow = 0;
	private Texture2D Sprite1;
	private Texture2D Sprite2;
	private Texture2D Sprite3;
	private Texture2D Sprite4;
	//public int helperChosenAction =0;
	//public int [] helperAction = {106, 107, 108, 109, 202, 203, 204, 301, 302};
	public int turnNumber = 0; //for testing 
	private Random rng = new Random();

	public override void _Ready()
	{

		rootNode = GetNode<Root>(".");
		TM = GetNode<turnManager>("turnManager");
		dialogue = GetNode<Label>("background/ui/dialogue");
		dialogue.Text = "THE BATTLE BEGINS. PRESS ENTER!";
		sword_slash = GetNode<Label>("background/ui/sword_slash");
		shield = GetNode<Label>("background/ui/shield");
		hydro_pump = GetNode<Label>("background/ui/hydro_pump");
		arrows = GetNode<Label>("background/ui/arrows");
		fire_pierce = GetNode<Label>("background/ui/fire_pierce");
		electric_hammer = GetNode<Label>("background/ui/electric_hammer");
		player_name = GetNode<Label>("background/ui/player_name");
		helper_name = GetNode<Label>("background/ui/bot_name");
		player_hp_1 = GetNode<Label>("background/ui/player_hp_1");
		player_hp_2 = GetNode<Label>("background/ui/player_hp_2");
		helper_hp_1 = GetNode<Label>("background/ui/bot_hp_1");
		helper_hp_2 = GetNode<Label>("background/ui/bot_hp_2");
		monsterSprite = GetNode<Sprite2D>("background/monster");
		Sprite1 = GD.Load<Texture2D>("res://Battle Scene/Monster 1.png");
		Sprite2 = GD.Load<Texture2D>("res://Battle Scene/Monster 2.png");
		Sprite3 = GD.Load<Texture2D>("res://Battle Scene/Monster 3.png");
		Sprite4 = GD.Load<Texture2D>("res://Battle Scene/Monster 4(old).png");
		helper_action = GetNode<Button>("background/ui/bot_action");
		helper_action.FocusMode = Control.FocusModeEnum.None;



		LoadFighters();
		hpUi(player.CHP, helper.CHP);
		//helper_action.Pressed += buttonPressed;
	}

	public void LoadFighters(){
		var gm = GameManager.Instance;
		monsterId = gm.Monster;
		enemy = new character(gm.enemyName, gm.monsterHp, gm.monsterCHp, gm.monsterAtk, gm.monsterDef, gm.monsterSpAtk, gm.monsterSpDef, gm.monsterSpd, gm.monsterVul, gm.monsterRes);
		player = new character(gm.playerName, 30, gm.playerCHp, 12, 10, 10, 12, 10, 0, 0 );
		helper = new character(gm.helperName, 30, gm.helperCHp, 10, 10, 12, 12, 10, 0, 0);
		player_name.Text = gm.playerName;
		helper_name.Text = gm.helperName;


		if(monsterId == 1){
			monsterSprite.Texture = Sprite1;
		} else if (monsterId == 2){
			monsterSprite.Texture = Sprite2;
		} else if (monsterId == 3){
			monsterSprite.Texture = Sprite3;
		}else {
			monsterSprite.Texture = Sprite4;
		}
			
	}

	public override void _Input(InputEvent @event){
		if(@event.IsActionPressed("ui_cancel")){
			GetTree().Quit();
		}

		if (@event.IsActionPressed("ui_accept")){


			if (TM.state == turnManager.State.NS)
			{
				hpUi(player.CHP, helper.CHP);
				TM.TurnHandler();
				TM.state = turnManager.State.MD;

			}
			else if (TM.state == turnManager.State.MD)
			{
				//GD.Print($"Turn: {turnNumber} \n Before Move: \n Move: Shield, Counter{TM.counter[0]}, Player Def= {player.DEF} \n Move: Encouragement, Counter:{TM.counter[1]}, Player ATK={player.ATK} \n Move:Discouregement, Counter={TM.counter[2]} Enemy Atk={enemy.ATK} \n Move:Distraction, Counter={TM.counter[3]}, Enemy Def= {enemy.DEF} \n Move: Evanecence, Counter={TM.counter[4]}, Enemy Spd: {enemy.SPD} ");
				
				if (TM.currentTurn == turnManager.Turn.PT)
				{
					moveInstance = new move(chosenAction, player, helper, enemy, 1, TM.counter);
					moveInstance.executeAction();
					sword_slash.Visible = false;                                         //Κάνουμε το menu κινήσεων μη ορατό 
					shield.Visible = false;
					hydro_pump.Visible = false;
					arrows.Visible = false;
					fire_pierce.Visible = false;
					electric_hammer.Visible = false;
					dialogue.Visible = true;
				}
				else if (TM.currentTurn == turnManager.Turn.HT)
				{
					chosenAction = moveChooser(player, helper, enemy, TM.counter, monsterId);
					moveInstance = new move(chosenAction, player, helper, enemy, 2, TM.counter);
					moveInstance.executeAction();


				}
				else if (TM.currentTurn == turnManager.Turn.ET)
				{
					int op = rng.Next(1,3);
					op = op + 2;
					//GD.Print($"OP:{op}");
					if (TM.losses == turnManager.Losses.HL)
					{
						op = 3;
					}

					moveInstance = new move(chosenAction, player, helper, enemy, op, TM.counter);
					moveInstance.executeAction();
				}
				
				if (moveInstance.counterWasActive == false)
				{
					dialogue.Text = moveInstance.mdDialogue;
					TM.state = turnManager.State.AM;
				}
				else
				{
					dialogue.Text = moveInstance.mdDialogue;
					TM.state = turnManager.State.NS;
					TM.currentTurn = TM.previousTurn;
				}


			}
			else if (TM.state == turnManager.State.AM)
			{
				dialogue.Text = moveInstance.amDialogue;
				TM.counterHandler();
				//GD.Print($"Turn: {turnNumber} \n After counterHandler: \n Move: Shield, Counter{TM.counter[0]}, Player Def= {player.DEF} \n Move: Encouragement, Counter:{TM.counter[1]}, Player ATK={player.ATK} \n Move:Discouregement, Counter={TM.counter[2]} Enemy Atk={enemy.ATK} \n Move:Distraction, Counter={TM.counter[3]}, Enemy Def= {enemy.DEF} \n Move: Evanecence, Counter={TM.counter[4]}, Enemy Spd: {enemy.SPD} ");
				turnNumber = turnNumber + 1;
			}
			else if (TM.state == turnManager.State.ST)
			{
				if (TM.losses == turnManager.Losses.PL)
				{
					if (dialogueFlow == 0)
					{
						dialogue.Text = $"AFTER A HEROIC BATTLE {player.Name} LOST. BETTER LUCK NEXT TIME!";
						dialogueFlow = 1;
					}
					else if (dialogueFlow == 1)
					{
						GetTree().Quit();
					}
				}
				else if (TM.losses == turnManager.Losses.HL)
				{
					if (dialogueFlow == 0)
					{
						dialogue.Text = $"{helper.Name} TRIED THEIR BEST BUT THE BATTLE WAS TO FIERCE FOR THEM!";
						TM.state = turnManager.State.NS;
					}
				}
				else if (TM.losses == turnManager.Losses.EL)
				{
					if (dialogueFlow == 0)
					{
						dialogue.Text = $"YOUR PERSEVERANCE PAID OFF. YOU WIN!";
						dialogueFlow = 1;
					}
					else if (dialogueFlow == 1)
					{
						GoToEnemySelection();
					}
				}
			}
		}


		if (TM.state == turnManager.State.MD){  //Αν το dialogue box = Move Decleration

			if (TM.currentTurn == turnManager.Turn.PT){    //Αν το Current Turn = Player Turn
				if (@event.IsActionPressed("ui_right")){
			
					if (chosenAction == 101){
						sword_slash.AddThemeColorOverride("font_color", black);
						hydro_pump.AddThemeColorOverride("font_color", red);
						chosenAction = 102;	
					} else if (chosenAction == 102){
						hydro_pump.AddThemeColorOverride("font_color", black);
						fire_pierce.AddThemeColorOverride("font_color", red);
						chosenAction = 104;
					} else if (chosenAction == 201){
						shield.AddThemeColorOverride("font_color", black);
						arrows.AddThemeColorOverride("font_color", red);
						chosenAction = 103;
					} else if (chosenAction == 103){
						arrows.AddThemeColorOverride("font_color", black);
						electric_hammer.AddThemeColorOverride("font_color", red);
						chosenAction = 105;
					}
				}

				if (@event.IsActionPressed("ui_left")){

					if (chosenAction == 102){
						sword_slash.AddThemeColorOverride("font_color", red);
						hydro_pump.AddThemeColorOverride("font_color", black);
						chosenAction = 101;
					} else if (chosenAction == 104){
						hydro_pump.AddThemeColorOverride("font_color", red);
						fire_pierce.AddThemeColorOverride("font_color", black);
						chosenAction = 102;
					} else if (chosenAction == 105){
						arrows.AddThemeColorOverride("font_color", red);
						electric_hammer.AddThemeColorOverride("font_color", black);
						chosenAction = 103;
					} else if (chosenAction == 103){
						shield.AddThemeColorOverride("font_color", red);
						arrows.AddThemeColorOverride("font_color", black);
						chosenAction = 201;
					}
				}

				if (@event.IsActionPressed("ui_up")){

					if (chosenAction == 201){
						sword_slash.AddThemeColorOverride("font_color", red);
						shield.AddThemeColorOverride("font_color", black);
						chosenAction = 101;
					} else if (chosenAction == 103){
						hydro_pump.AddThemeColorOverride("font_color", red);
						arrows.AddThemeColorOverride("font_color", black);
						chosenAction = 102;
					} else if (chosenAction == 105){
						fire_pierce.AddThemeColorOverride("font_color", red);
						electric_hammer.AddThemeColorOverride("font_color", black);
						chosenAction = 104;
					}
				}

				if (@event.IsActionPressed("ui_down")){
					if (chosenAction == 101){
						shield.AddThemeColorOverride("font_color", red);
						sword_slash.AddThemeColorOverride("font_color", black);
						chosenAction = 201;
					} else if (chosenAction == 102){
						arrows.AddThemeColorOverride("font_color", red);
						hydro_pump.AddThemeColorOverride("font_color", black);
						chosenAction = 103;
					} else if (chosenAction == 104){
						electric_hammer.AddThemeColorOverride("font_color", red);
						fire_pierce.AddThemeColorOverride("font_color", black);
						chosenAction = 105;
					}
				}
		 }  } }
		


	public void hpUi(int playerHp,int helperHp){
		if (playerHp > 0){
			player_hp_1.Text = (playerHp/10).ToString();
			player_hp_2.Text = (playerHp % 10).ToString();
		} else {
			player_hp_1.Text = "0";
			player_hp_2.Text = "0";}

		if (helperHp > 0){
			helper_hp_1.Text = (helperHp/10).ToString();
			helper_hp_2.Text = (helperHp%10).ToString();
		}else{
			helper_hp_1.Text ="0";
			helper_hp_2.Text ="0";} 
	}

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
	
