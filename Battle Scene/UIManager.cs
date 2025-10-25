using Godot;
using System;

/// <summary>
///  Διαχειρίζεται όλα τα στοιχεία του UI της σκηνής μάχης.
/// </summary>
public partial class UIManager : Node
{
	// Αναφορές στους managers.
	private turnManager TM;
	private Root root;

	// Sprites
	private Texture2D Sprite1, Sprite2, Sprite3, Sprite4;
	public Sprite2D monsterSprite;

	// Labels για την εμφάνιση των στατιστικών των χαρακτήρων στην οθόνη.
	public Label helper_name;
	public Label player_name;
	public Label player_hp_1,  player_hp_2;
	public Label helper_hp_1, helper_hp_2;

	// Labels για το menu κινήσεων του παίκτη.
	public Label sword_slash, shield, hydro_pump, arrows, fire_pierce, electric_hammer;
	
	// To κύριο Label για τους διαλόγους.
	public Label dialogue;

	// -- Μεταβλητές Ρυθμίσεων -- 
	public Color red = new Color(1, 0, 0); //To χρώμα που παίρνει μια κίνηση όταν είναι επιλεγμένη
	public Color black = new Color(0, 0, 0); //Το χρώμα που παίρνει μια κίνηση όταν δεν είναι επιλέγμένη


	/// <summary>
	/// Καλείται μια φορά όταν ο κόμβος είναι έτοιμος.
	/// </summary>
	public override void _Ready()
	{
		// Λήψη αναφορών στους άλλους managers.
		TM = GetNode<turnManager>($"../../../turnManager");
		root = GetTree().CurrentScene as Root;

		// Λήψη αναφορών και φόρτωση των textures για τα sprites των τεράτων.
		monsterSprite = GetNode<Sprite2D>("%monster");
		Sprite1 = GD.Load<Texture2D>("res://Assets/Monster 1.png");
		Sprite2 = GD.Load<Texture2D>("res://Assets/Monster 2.png");
		Sprite3 = GD.Load<Texture2D>("res://Assets/Monster 3.png");
		Sprite4 = GD.Load<Texture2D>("res://Assets/Monster 4(old).png");

		// Λήψη αναφοράων για όλα τα labels από τη σκηνή.
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

	/// <summary>
	/// Ορίζει το sprite του εχθρού με βάση το ID του.
	/// </summary>
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
	
	/// <summary>
	/// Αρχικοποιεί τα UI στοιχεία που δείχνουν τα ονόματα και το HP στην αρχή της μάχης.
	/// </summary>
	public void boxUiSetUp(string playerName, string helperName, int playerCHP, int helperCHP)
	{
		player_name.Text = playerName;
		helper_name.Text = helperName;
		hpUIUpdate(playerCHP, helperCHP);

	}

	/// <summary>
	/// Ενημερώνει τα labels που δείχνουν τους τρέχοντες πότνους ζωής του παίκτη και του βοηθού.
	/// </summary>
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

	/// <summary>
	/// Ελέγχει την ορατότητα του menu κινήσεων του παίκτη.
	/// </summary>
	public void menuVisible(bool visible)
	{
		if (visible)
		{
			// Όταν το menu είναι ορατό, κρύβουμε το διαλόγο.
			dialogue.Visible = false;

			// Εμφανίζουμε όλα τα labels των κινήσεων και ορίζουμε τα αρχικά τους χρώματα.
			sword_slash.Visible = true;
			sword_slash.AddThemeColorOverride("font_color", red); // Η πρώτη κίνηση είναι πάντα η αρχική.
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
		}
		else
		{
			sword_slash.Visible = false;
			shield.Visible = false;
			hydro_pump.Visible = false;
			arrows.Visible = false;
			fire_pierce.Visible = false;
			electric_hammer.Visible = false;
			dialogue.Visible = true;        // Όταν κρύβουμε το menu εμφανίζεται ο διάλογος.
		}
	}

	/// <summary>
	/// Διαχειρίζεται τις αλλαγές επιλογής κινήσεων του menu κινήσεων.
	/// </summary>
	public int menuHandler(int chosenAction, string Key)
	{
		// H μέθοδος περιέχει όλη τη λογική για την πλοήγηση στο 2D menu των κινήσεων. Αλλάζουμε το χρώμα των labels για 
		// να δείξουμε την τρέχουσα επιλογή. Όταν ένα label είναι κόκκινο αποθηκεύεται το αναγνωριστικό της κίνησης που
		// αντιπροσωπεύει στην μεταβλητή chosenAction.
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
		return chosenAction; // Επιστρέφουμε το αναγνωριστικό.
	}
	/// <summary>
	/// Μια μέθοδος που ορίζει το κείμενο του κουτιού του διαλόγου.
	/// </summary>
	public void dialogueBoxText(string dial)
	{
		dialogue.Text = dial;
	}
	
	/// <summary>
	/// Διαχειρίζεται την είσοδο του παίκτη που σχετίζεται αποκλειστικά με το UI.
	/// </summary>
	public override void _Input(InputEvent @event)
	{
		// Ενεργοποιείται μόνο όταν είμαστε στην φάση MD και είναι η σειρά του παίκτη.
		if (TM.state == turnManager.State.MD)
		{
			// Για κάθε πάτημα πλήκτρου καλούμε τη menuHandler για να αλλάξει το χρώμα και να μας δώσει το νέο αναγνωριστικό,
			// και ενημερώνουμε την κεντρική μεταβλητή 'chosenAction' του root.
			if (TM.currentTurn == turnManager.Turn.PT)
			{
				if (@event.IsActionPressed("ui_right")) { root.chosenAction = menuHandler(root.chosenAction, "right"); }

				if (@event.IsActionPressed("ui_left")) { root.chosenAction = menuHandler(root.chosenAction, "left"); }

				if (@event.IsActionPressed("ui_up")) { root.chosenAction = menuHandler(root.chosenAction, "up"); }

				if (@event.IsActionPressed("ui_down")) { root.chosenAction = menuHandler(root.chosenAction, "down"); }
			}
		}
	}
}
