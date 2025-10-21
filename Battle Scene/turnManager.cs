using Godot;
using System;

public partial class turnManager : Node
{
	public Root root;
	public int [] counter = {0,0,0,0,0}; //Shield, ATKBoost, ATKDebuf, DEFDebuf, EnemyCounter
	public int[,] enemyAction = { { 110, 111, 205 }, { 112, 113, 206 }, { 114, 115, 207 }, { 116, 303, 208 } };
	public UIManager uim;

	public enum State{
		NS = 0,     //No State
		MD = 1,     //Move Delcaration
		AM = 2,     //Aftermath
		ST = 4      //Special State
	}

	public enum Losses{
		NL = 0, //No Losses
		PL = 1, //Player Lost
		EL = 2, //Enemy Lost
		HL = 4, //Helper Lost

	}

	public enum Turn{
		PT = 0,    //Player Turn
		HT = 1,    //Helper Turn
		ET = 2,    //Enemy Turn 
	}

	public Turn currentTurn = Turn.ET;
	public Turn previousTurn;
	public State state = State.NS;
	public Losses losses = Losses.NL;

	private Random rng = new Random();
	
	public override void _Ready()
	{
		root = GetTree().CurrentScene as Root;
		uim = GetNode<UIManager>($"../background/ui/UIManager");
		
	}
	public void TurnHandler(){
		previousTurn = currentTurn; 
		if (currentTurn == Turn.PT && losses == Losses.NL)
		{
			currentTurn = Turn.HT;
			uim.dialogueBoxText($"IT'S {root.helper.Name}'s TURN!");
		}
		else if (currentTurn == Turn.HT && losses == Losses.NL || currentTurn == Turn.PT && losses == Losses.HL)
		{
			enemyTurn();
		}
		else if (currentTurn == Turn.ET)
		{
			playerTurn();
		} 
	}
	
	public void playerTurn(){
		currentTurn = Turn.PT;
		root.chosenAction = 101;
		uim.menuVisible(true);

	}

	public void enemyTurn(){
		currentTurn = Turn.ET;
		uim.dialogueBoxText($"IT'S {root.enemy.Name}'s TURN!");

		do{
			int move = rng.Next(1,4);
			if (move == 1)
			{
				root.chosenAction = enemyAction[root.monsterId - 1, 0];
			}
			else if (move == 2)
			{
				root.chosenAction = enemyAction[root.monsterId - 1, 1];
			}
			else
			{
				if (counter[4] == 0)
				{
					root.chosenAction = enemyAction[root.monsterId - 1, 2];
				}
				else
				{
					root.chosenAction = 0;
				}
			}

			if(root.monsterId == 4 && root.chosenAction == 303 && root.enemy.HP == root.enemy.CHP){
				root.chosenAction = 0;
			}
		}while(root.chosenAction == 0);
	}

	public void counterHandler(){
		

		if (currentTurn == Turn.PT){
			if (counter[0]>0 && counter[0] < 4){
				counter[0] = counter[0] +1;
			}else if (counter[0] == 4){
				counter[0] = 0;
				root.player.DEF = root.player.DEF - 3;
			}
		}

		if (currentTurn == Turn.HT){
			if(counter[1]>0 && counter[1]<4){
				counter[1] = counter[1] +1;
			} else if (counter[1] == 4){
				counter[1] =0;
				root.player.ATK = root.player.ATK-3;
			}

			if (counter[2]>0 && counter[2]<4){
				counter[2] = counter[2] +1;
			} else if (counter[2]==4){
				counter[2]=0;
				root.enemy.ATK = root.enemy.ATK+3;
			}

			if (counter[3]>0 && counter[3] <4){
				counter[3] = counter[3] +1;
			} else if (counter[3] == 4){
				counter[3] =0;
				root.enemy.DEF = root.enemy.DEF +3;
			}
		}

		if (currentTurn == Turn.ET){
			if(counter[4]>0 && counter[4]<4){
				counter[4]=counter[4] +1;
			} else if (counter[4] == 4){
				counter[4]=0;
				if(root.monsterId == 1){
					root.enemy.SPD = root.enemy.SPD - 3;
				} else if (root.monsterId == 2){
					root.enemy.SP_DEF = root.enemy.SP_DEF -2;
					root.enemy.DEF = root.enemy.DEF -2;
				} else if (root.monsterId == 3){
					root.enemy.SP_ATK = root.enemy.SP_ATK -2;
					root.enemy.ATK = root.enemy.ATK -2;
				} else if (root.monsterId == 4){
					root.enemy.SP_ATK = root.enemy.SP_ATK -2;
					root.enemy.SPD = root.enemy.SPD - 2;
				}
			}
		}


		if(root.player.CHP >0 && root.helper.CHP>0 && root.enemy.CHP>0 || root.player.CHP>0 && root.enemy.CHP>0 && losses == Losses.HL ){
			state = State.NS;
		} else if (root.player.CHP<1){
			state = State.ST;
			losses = Losses.PL;
		} else if (root.helper.CHP<1 && losses == Losses.NL){
			state = State.ST;
			losses = Losses.HL;
		} else if (root.enemy.CHP<1){
			state = State.ST;
			losses = Losses.EL;
		} 

	}

}
