using Godot;
using System;

public partial class enemy_selection : Node
{
	public TextureRect enemy_selector;
	public Vector2 upRight = new Vector2(155,4);
	public Vector2 upLeft = new Vector2(18,4);
	public Vector2 downRight = new Vector2(155, 94);
	public Vector2 downLeft = new Vector2(18,94);
	public int monster = 1;
	
	public override void _Ready()
	{
		enemy_selector = GetNode<TextureRect>("background/enemy_selector");
		enemy_selector.Position = upLeft;
	}

	public override void _Input(InputEvent @event){
		

		if(@event.IsActionPressed("ui_left")){
			if (enemy_selector.Position == upRight){
				enemy_selector.Position = upLeft;
			} else if (enemy_selector.Position == downRight){
				enemy_selector.Position = downLeft;
			}
		}

		else if (@event.IsActionPressed("ui_right")){
			if(enemy_selector.Position == upLeft){
				enemy_selector.Position = upRight;
			} else if (enemy_selector.Position == downLeft){
				enemy_selector.Position = downRight;
			}
		}

		else if(@event.IsActionPressed("ui_down")){
			if(enemy_selector.Position == upLeft){
				enemy_selector.Position = downLeft;
			}else if (enemy_selector.Position == upRight){
				enemy_selector.Position = downRight;
			}
		}

		else if(@event.IsActionPressed("ui_up")){
			if(enemy_selector.Position == downLeft){
				enemy_selector.Position = upLeft;
			} else if (enemy_selector.Position == downRight){
				enemy_selector.Position = upRight;
			}
		}

		if (@event.IsActionPressed("ui_accept")){
			var gm = GameManager.Instance;
			if(enemy_selector.Position == upLeft){
				gm.Monster = 1;
				gm.enemyName= "LOGMON";
				gm.monsterHp = 70;
				gm.monsterCHp =  70;
				gm.monsterAtk = 13;
				gm.monsterDef = 11;
				gm.monsterSpAtk = 10;
				gm.monsterSpDef = 13;
				gm.monsterSpd = 11;
				gm.monsterRes = 24;
				gm.monsterVul = 167;

			} else if (enemy_selector.Position == upRight){
				gm.Monster = 2;
				gm.enemyName = "ROCKRAB";
				gm.monsterHp = 100;
				gm.monsterCHp = 100;
				gm.monsterAtk = 5;
				gm.monsterDef = 14;
				gm.monsterSpAtk = 5;
				gm.monsterSpDef = 14;
				gm.monsterSpd = 8;
				gm.monsterRes = 1478;
				gm.monsterVul = 26;
			} else if (enemy_selector.Position == downLeft){
				gm.Monster = 3;
				gm.enemyName = "FROGIC";
				gm.monsterHp = 50;
				gm.monsterCHp = 50;
				gm.monsterAtk = 16;
				gm.monsterDef = 10;
				gm.monsterSpAtk = 15;
				gm.monsterSpDef = 10;
				gm.monsterSpd = 10;
				gm.monsterRes = 158;
				gm.monsterVul = 347;
			} else if (enemy_selector.Position == downRight){
				gm.Monster = 4;
				gm.enemyName = "MYSTOX";
				gm.monsterHp = 50;
				gm.monsterCHp = 50;
				gm.monsterAtk = 10;
				gm.monsterDef = 15;
				gm.monsterSpAtk = 13;
				gm.monsterSpDef = 13;
				gm.monsterSpd = 10;
				gm.monsterRes = 369;
				gm.monsterVul = 258;
			}
			GoToNextScene();
		}
	}
	private void GoToNextScene(){
		var gm = GameManager.Instance;
		if (gm.firstBattle){
		gm.playerCHp = 30;
		gm.helperCHp = 30;}

		var nextScene = (PackedScene)ResourceLoader.Load("res://Battle Scene/battle_scene.tscn");

		if (nextScene != null) {
			GetTree().ChangeSceneToPacked(nextScene);
		}
	}
}
