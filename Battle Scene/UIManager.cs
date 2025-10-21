using Godot;
using System;

public partial class UIManager : Node
{
	private Texture2D Sprite1;
	private Texture2D Sprite2;
	private Texture2D Sprite3;
	private Texture2D Sprite4;
	public Sprite2D monsterSprite;
	
	public Label helper_name;
	public Label player_name;
	public Label player_hp_1;
	public Label player_hp_2;
	public Label helper_hp_1;
	public Label helper_hp_2;

	public Color red = new Color(1,0,0); //To χρώμα που παίρνει μια κίνηση όταν είναι επιλεγμένη
	public Color black = new Color(0,0,0); //Το χρώμα που παίρνει μια κίνηση όταν δεν είναι επιλέγμένη
	
	public Label sword_slash; //Reference στο Label που επιλέγει την κίνηση sword slash
	public Label shield;
	public Label hydro_pump;
	public Label arrows;
	public Label fire_pierce;
	public Label electric_hammer;
	
	public Label dialogue;  //Reference στο Label που γράφεται ο διάλογος

	public override void _Ready()
	{
		monsterSprite = GetNode<Sprite2D>("%monster");
		Sprite1 = GD.Load<Texture2D>("res://Assets/Monster 1.png");
		Sprite2 = GD.Load<Texture2D>("res://Assets/Monster 2.png");
		Sprite3 = GD.Load<Texture2D>("res://Assets/Monster 3.png");
		Sprite4 = GD.Load<Texture2D>("res://Assets/Monster 4(old).png");

		player_name = GetNode<Label>($"../player_name");
		helper_name = GetNode<Label>($"../bot_name");
		player_hp_1 = GetNode<Label>($"../player_hp_1");
		player_hp_2 = GetNode<Label>($"../player_hp_2");
		helper_hp_1 = GetNode<Label>($"../bot_hp_1");
		helper_hp_2 = GetNode<Label>($"../bot_hp_2");
		
		sword_slash = GetNode<Label>($"../sword_slash");
		shield = GetNode<Label>($"../shield");
		hydro_pump = GetNode<Label>($"../hydro_pump");
		arrows = GetNode<Label>($"../arrows");
		fire_pierce = GetNode<Label>($"../fire_pierce");
		electric_hammer = GetNode<Label>($"../electric_hammer");

		dialogue = GetNode<Label>($"../dialogue");
		
	}
	public void monsterSpriteSetUp(int monsterId)
	{
	   
		if (monsterId == 1)
		{
			monsterSprite.Texture = Sprite1;
		}
		else if (monsterId == 2)
		{
			monsterSprite.Texture = Sprite2;
		}
		else if (monsterId == 3)
		{
			monsterSprite.Texture = Sprite3;
		}
		else
		{
			monsterSprite.Texture = Sprite4;
		}

	}

	public void boxUiSetUp(string playerName, string helperName, int playerCHP, int helperCHP)
	{
		player_name.Text = playerName;
		helper_name.Text = helperName;
		hpUIUpdate(playerCHP, helperCHP);

	}

	public void hpUIUpdate(int playerCHP, int helperCHP)
	{
		if (playerCHP > 0)
		{
			player_hp_1.Text = (playerCHP / 10).ToString();
			player_hp_2.Text = (playerCHP % 10).ToString();
		}
		else
		{
			player_hp_1.Text = "0";
			player_hp_2.Text = "0";
		}

		if (helperCHP > 0)
		{
			helper_hp_1.Text = (helperCHP / 10).ToString();
			helper_hp_2.Text = (helperCHP % 10).ToString();
		}
		else
		{
			helper_hp_1.Text = "0";
			helper_hp_2.Text = "0";
		}
	}

	public void menuVisible(bool visible)
	{
		if (visible)
		{
		dialogue.Visible = false;

		sword_slash.Visible = true;
		sword_slash.AddThemeColorOverride("font_color", red);
		shield.Visible = true;
		shield.AddThemeColorOverride("font_color", black);
		hydro_pump.Visible = true;
		hydro_pump.AddThemeColorOverride("font_color", black);
		arrows.Visible = true;
		arrows.AddThemeColorOverride("font_color", black);
		fire_pierce.Visible = true;
		fire_pierce.AddThemeColorOverride("font_color", black);
		electric_hammer.Visible = true;
		electric_hammer.AddThemeColorOverride("font_color", black);
		} else {
			sword_slash.Visible = false;                                         //Κάνουμε το menu κινήσεων μη ορατό 
			shield.Visible = false;
			hydro_pump.Visible = false;
			arrows.Visible = false;
			fire_pierce.Visible = false;
			electric_hammer.Visible = false;
			dialogue.Visible = true;
		}
	}

	public int menuHandler(int chosenAction, string Key)
	{
		if (Key == "right")
		{
			if (chosenAction == 101)
			{
				sword_slash.AddThemeColorOverride("font_color", black);
				hydro_pump.AddThemeColorOverride("font_color", red);
				chosenAction = 102;
			}
			else if (chosenAction == 102)
			{
				hydro_pump.AddThemeColorOverride("font_color", black);
				fire_pierce.AddThemeColorOverride("font_color", red);
				chosenAction = 104;
			}
			else if (chosenAction == 201)
			{
				shield.AddThemeColorOverride("font_color", black);
				arrows.AddThemeColorOverride("font_color", red);
				chosenAction = 103;
			}
			else if (chosenAction == 103)
			{
				arrows.AddThemeColorOverride("font_color", black);
				electric_hammer.AddThemeColorOverride("font_color", red);
				chosenAction = 105;
			}
		}
		else if (Key == "left")
		{
			if (chosenAction == 102)
			{
				sword_slash.AddThemeColorOverride("font_color", red);
				hydro_pump.AddThemeColorOverride("font_color", black);
				chosenAction = 101;
			}
			else if (chosenAction == 104)
			{
				hydro_pump.AddThemeColorOverride("font_color", red);
				fire_pierce.AddThemeColorOverride("font_color", black);
				chosenAction = 102;
			}
			else if (chosenAction == 105)
			{
				arrows.AddThemeColorOverride("font_color", red);
				electric_hammer.AddThemeColorOverride("font_color", black);
				chosenAction = 103;
			}
			else if (chosenAction == 103)
			{
				shield.AddThemeColorOverride("font_color", red);
				arrows.AddThemeColorOverride("font_color", black);
				chosenAction = 201;
			}
		}
		else if (Key == "up")
		{
			if (chosenAction == 201)
			{
				sword_slash.AddThemeColorOverride("font_color", red);
				shield.AddThemeColorOverride("font_color", black);
				chosenAction = 101;
			}
			else if (chosenAction == 103)
			{
				hydro_pump.AddThemeColorOverride("font_color", red);
				arrows.AddThemeColorOverride("font_color", black);
				chosenAction = 102;
			}
			else if (chosenAction == 105)
			{
				fire_pierce.AddThemeColorOverride("font_color", red);
				electric_hammer.AddThemeColorOverride("font_color", black);
				chosenAction = 104;
			}
		}
		else if (Key == "down")
		{
			if (chosenAction == 101)
			{
				shield.AddThemeColorOverride("font_color", red);
				sword_slash.AddThemeColorOverride("font_color", black);
				chosenAction = 201;
			}
			else if (chosenAction == 102)
			{
				arrows.AddThemeColorOverride("font_color", red);
				hydro_pump.AddThemeColorOverride("font_color", black);
				chosenAction = 103;
			}
			else if (chosenAction == 104)
			{
				electric_hammer.AddThemeColorOverride("font_color", red);
				fire_pierce.AddThemeColorOverride("font_color", black);
				chosenAction = 105;
			}
		}
		return chosenAction;
	}
	
	public void dialogueBoxText(string dial)
	{
		dialogue.Text = dial;
	}
}
