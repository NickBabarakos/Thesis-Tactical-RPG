using Godot;
using System;

public partial class GameManager : Node
{
	public static GameManager Instance {get; private set; }

	public int Monster {get; set; }
	public int monsterHp {get; set; }
	public int monsterCHp {get; set; }
	public int monsterAtk {get; set; }
	public int monsterDef {get; set; }
	public int monsterSpAtk {get; set; }
	public int monsterSpDef {get; set; }
	public int monsterSpd {get; set; }
	public int monsterRes {get; set; }
	public int monsterVul {get; set; }

	public string playerName {get; set; }
	public string helperName {get; set; }
	public string enemyName {get; set;}

	public bool firstBattle {get; set;}
	public int playerCHp {get; set; }
	public int helperCHp {get; set; }
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Instance = this;
	}

}
