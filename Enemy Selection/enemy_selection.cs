using Godot;
using System;

// Διαχειρίζεται τη λογική της οθόνης επιλογής αντιπάλου. Ο παίκτης μπορεί να επιλέγει ανάμεσα σε 4 τέρατα.
public partial class enemy_selection : Node
{
	// -- Μεταβλητή που συνδέεται με το κόμβο δείκτη επιλογής αντιπάλου --
	public TextureRect enemy_selector;

	// -- Μεταβλητές που αποθηκεύουν τις προκαθορθισμένες συντεταγμένες για κάθε μια από τις 4 επιλογές στην οθόνη -- 
	public Vector2 upRight = new Vector2(155, 4);
	public Vector2 upLeft = new Vector2(18, 4);
	public Vector2 downRight = new Vector2(155, 94);
	public Vector2 downLeft = new Vector2(18, 94);

	//-- Μεταβλητή για να κρατάμε την αναφορά στον GameManager -- 
	private GameManager gm;

	// Η μέθοδος _Read() καλείται αυτόματα μια φορά όταν η σκηνή είναι έτοιμη. 
	public override void _Ready(){
		//Παίρνουμε την αναφορά στον κόμβο του δείκτη από τη σκηνή.
		enemy_selector = GetNode<TextureRect>("background/enemy_selector");

		//Τοποθετούμε τον δείκτη στην αρχική του θέση, που είναι η επιλογή "Πάνω Αριστερά".
		enemy_selector.Position = upLeft;

		//Παίρνουμε την αναφορά στον GameManager και την αποθηκεύουμε.
		gm = GameManager.Instance;
	}

	//Η μέθοδος _Input() καλείται κάθε φορά που υπάρχει ένα γεγονός εισόδου. Διαχείριση των ενεργειών του παίκτη.
	public override void _Input(InputEvent @event)
	{
		//--Λογική Κίνησης του Δείκτη --
		//Οι παρακάτω έλεγχοι μετακινουν τον δείκτη ανάλογα με το πλήκρο που πάτησε ο παίκτης και την τρέχουσα θέση του.

		if (@event.IsActionPressed("ui_left"))
		{         //Ενέργειες που θα εκτελεστούν αν πατήσει το αριστερό βελάκι.
			if (enemy_selector.Position == upRight) { enemy_selector.Position = upLeft; }
			else if (enemy_selector.Position == downRight) { enemy_selector.Position = downLeft; }
		}
		else if (@event.IsActionPressed("ui_right"))
		{ //Ενέργειες που θα εκτελεστούν αν πατήσει το δεξί βελάκι.
			if (enemy_selector.Position == upLeft) { enemy_selector.Position = upRight; }
			else if (enemy_selector.Position == downLeft) { enemy_selector.Position = downRight; }
		}
		else if (@event.IsActionPressed("ui_down"))
		{  //Ενέργειες που θα εκτελεστούν αν πατήσει το κάτω βελάκι.
			if (enemy_selector.Position == upLeft) { enemy_selector.Position = downLeft; }
			else if (enemy_selector.Position == upRight) { enemy_selector.Position = downRight; }
		}
		else if (@event.IsActionPressed("ui_up"))
		{    //Ενέργειες που θα εκτελεστούν αν πατήσει το πάνω βελάκι.
			if (enemy_selector.Position == downLeft) { enemy_selector.Position = upLeft; }
			else if (enemy_selector.Position == downRight) { enemy_selector.Position = upRight; }
		}

		//-- Λογική Επιβεβαίωση Επιλογής -- 
		// Αν πατηθεί το κουμπί επιβεβαίωσης (Enter)
		if (@event.IsActionPressed("ui_accept"))
		{

			//Ελέγχουμε σε ποια θέση βρίσκεται ο δείκτης και φορτώνουμε τα αντίστοιχα στατιστικά
			if (enemy_selector.Position == upLeft)
			{ //Στατιστικά τέρατος 1 (LOGMON)
				gm.Monster = 1;
				gm.enemyName = "LOGMON";
				gm.monsterHp = 70;
				gm.monsterCHp = 70;
				gm.monsterAtk = 13;
				gm.monsterDef = 11;
				gm.monsterSpAtk = 10;
				gm.monsterSpDef = 13;
				gm.monsterSpd = 11;
				gm.monsterRes = 24;
				gm.monsterVul = 167;
			}
			else if (enemy_selector.Position == upRight)
			{ //Στατιστικά τέρατος 2 (ROCKRAB)
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
			}
			else if (enemy_selector.Position == downLeft)
			{ //Στατιστικά τέρατος 3 (FROGIC)
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
			}
			else if (enemy_selector.Position == downRight)
			{ //Στατιστικά τέρατος 4 (MYSTOX)
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
			//Αφού αποθηκεύσουμε τα δεδομένα, καλούμε τη μέθοδο για να αλλάξουμε σκηνή.
			GoToNextScene();
		}
	}
	
	//Μια συνάρτηση που προετοιμάζει την μάχη και αλλάζει σκηνή.
	private void GoToNextScene(){
		
		// Ελέγχουμε αν αυτή είναι η πρώτη μάχη.
		if (gm.firstBattle)
		{
			//Αν είναι, ορίζουμε τους τρέχοντες πόντους ζωής του παίκτη και του βοηθού ίσους με 30. 
			gm.playerCHp = 30;
			gm.helperCHp = 30;
		}

		//Φορτώνουμε το αρχείο της επόμενης σκηνής (της μάχης).
		var nextScene = (PackedScene)ResourceLoader.Load("res://Battle Scene/battle_scene.tscn");

		//Αν η φόρτωση του αρχέιου ήταν επιτυχής, αλλάζουμε την τρέχουσα σκηνή, στη σκηνή μάχης.
		if (nextScene != null) { GetTree().ChangeSceneToPacked(nextScene); }
	}
}
