using Godot;
using System;

/// <summary>
/// Το κεντρικό class που διαχειρίζεται τη λογική της μάχης. Είναι υπεύθυνο για τη διαχείριση της κατάσταση της μάχης,
/// τη δημιουργία των χαρακτήρων και την επικοινωνία μεταξύ των διάφορων managers (UIManager, TurnManager).
/// </summary>
public partial class Root : Node
{

//-- Αναφορές στου managers -- 
	private UIManager uim;
	private turnManager TM;

//-- Αντικείμενα της μάχης --- 
	public character player;
	public character helper;
	public character enemy;

// --Εσωτερική Κατάσταση και Βοηθητικά Αντικείμενα -- 
	private move moveInstance;
	public movePrediction predictor;
	public int chosenAction = 101;  // Το ID της κίνησης που εκτελείται. Αρχικοποιείται στην πρώτη κίνηση του παίκτη.
	public int monsterId;           // Το ID του τέρατος για τη συγκεκριμένη μάχη.
	public int dialogueFlow = 0;    // Μεταβλητή για τη διαχείριση της ροής των διαλόγων στο τέλος της μάχης.
	private Random rng = new Random();

	/// <summary>
	/// Καλείται μια φορά όταν η σκηνή είναι έτοιμη. Εδώ κάνουμε την αρχικοποίηση.
	/// </summary>
	public override void _Ready()
	{
		// Παίρνουμε τις αναφοές στου κόμβους των managers από τη σκηνή.
		uim = GetNode<UIManager>("background/ui/UIManager");
		TM = GetNode<turnManager>("turnManager");

		// Δημιουργούμε τους χαρακτήρες και ετοιμάζουμε το UI.
		LoadFighters();
		uim.dialogueBoxText("THE BATTLE BEGINS. PRESS ENTER!");
	}

	/// <summary>
	/// Δημιουργεί τα αντικείμενα των χαρακτήρων (player, helper, enemy) αντλώντας τα δεδομένα από τον GameManager και 
	/// δίνει εντολή στο UIManager να εμφανίσει πληροφορίες γι' αυτούς στο UI.
	/// </summary>
	public void LoadFighters()
	{
		var gm = GameManager.Instance;
		monsterId = gm.Monster;        // Παίρνουμε το ID του τέρατος.

		// Δημιουργούμε τα αντικείμενα των χαρακτήρων με τα στατιστικά τους.
		enemy = new character(gm.enemyName, gm.monsterHp, gm.monsterCHp, gm.monsterAtk, gm.monsterDef, gm.monsterSpAtk, gm.monsterSpDef, gm.monsterSpd, gm.monsterVul, gm.monsterRes);
		player = new character(gm.playerName, 30, gm.playerCHp, 12, 10, 10, 12, 10, 0, 0);
		helper = new character(gm.helperName, 30, gm.helperCHp, 10, 10, 12, 12, 10, 0, 0);

		// Δίνουμε εντολή στο UIManager να ρυθμίσει το UI και το Sprite του τέρατος.
		uim.monsterSpriteSetUp(monsterId);
		uim.boxUiSetUp(gm.playerName, gm.helperName, gm.playerCHp, gm.helperCHp);
	}

/// <summary>
/// Η κεντρική μέθοδος που διαχειρίζεται την είσοδο του παίκτη και τη ροή της μάχης. Λειτουργεί ως μια μηχανή καταστάσεων
/// με βάση το state του turnManager.
/// </summary>
	public override void _Input(InputEvent @event)
	{
		// Επιλογή για έξοδο από το παιχνίδι.
		if (@event.IsActionPressed("ui_cancel")) { GetTree().Quit(); }

		// Ο κύριος έλεγχος γίνεται όταν ο παίκτης πατήσει "Enter".
		if (@event.IsActionPressed("ui_accept"))
		{
			// -- Κατάσταση NS (No State ) -- 
			// Ο παίκτης πατάει Enter για να προχωρήσει στον επόμενο γύρο.
			if (TM.state == turnManager.State.NS)
			{
				uim.hpUIUpdate(player.CHP, helper.CHP);  //Ενημερώνουμε το UI που δείχνει το HP πριν ξεκινήσει η σειρά.
				TM.TurnHandler();                       // Λέμε στον turnManager να αποφασίσει ποιος παίζει.
				TM.state = turnManager.State.MD;        // Αλλάζουμε την κατάσταση σε "Move Delcaration"
			}

			// -- Κατάσταση MD (Move Declaration) --
			// H κίνηση έχει επιλαγεί (ή θα επιλεχθεί) και την εκτελούμε
			else if (TM.state == turnManager.State.MD)
			{
				// Καθορίζουμε ποιος επιτίθεται σε ποιον (attackPattern)
				int attackPattern = 0;
				if (TM.currentTurn == turnManager.Turn.PT) { attackPattern = 1; }
				else if (TM.currentTurn == turnManager.Turn.HT)
				{
					//Καλούμε το σύστημα επιλογής κινήσεων για να επιλέξει την καλύτερη κίνηση για το βοηθό.
					chosenAction = moveChooser(player, helper, enemy, TM.counter, monsterId);
					attackPattern = 2;
				}
				else if (TM.currentTurn == turnManager.Turn.ET)
				{
					// Για τον εχθρό επιλέγουμε τυχαία τον στόχο του (3: Παίκτης, 4: Βοηθός)
					int op = rng.Next(3, 5);
					if (TM.losses == turnManager.Losses.HL) { op = 3; } //Αν έχει ηττηθεί ο βοηθός επιλέγουμε τον παίκτη.
					attackPattern = op;
				}

				// Δημιουργούμε ένα αντικείμενο moveData, η μέθοδος LoadMove() διαβάζει το αντίστοιχο json αρχείο και 
				// συμπληρώνει τις τιμές της κίνησης που θα εκτελεστεί.
				baseMoveData moveData = moveDataLoader.LoadMove(chosenAction);
				if (moveData != null)
				{
					// Δημιουργούμε ένα αντικείμενο move, που θα εκτελέσει την κίνηση, δίνοντας του τις απαραίτητες πληροφορίες.
					moveInstance = new move(moveData, player, helper, enemy, attackPattern, TM.counter);
					// Καλούμε την μέθοδο της move που θα ξεκινήσει την εκτέλεση της κίνησης. 
					moveInstance.executeAction();
					// Ενημερώνουμε το UI μόνο αν έπαιζε ο παίκτης για να κρύψουμε το menu επιλογών.
					if (TM.currentTurn == turnManager.Turn.PT) { uim.menuVisible(false); }
				}
				else{
					// Αν δεν φορτώθηκαν σωστά τα δεδομένα της κίνησης επαναφέρουμε την σειρά.
					GD.PushError($"Root Error: Αποτυχία φόρτωσης κίνησης. Επαναφοράς σειράς");
					TM.state = turnManager.State.NS;
					return;
				}
				
				// Εμφανίζεται ο διάλογος που ανακοινώνει την κίνηση που εκτελείται.
				uim.dialogueBoxText(moveInstance.mdDialogue);

				// Ελέγχουμε αν η κίνηση απέτυχε λόγω ενεργού counter.
				if (moveInstance.counterWasActive == false) { TM.state = turnManager.State.AM; } //Αν πέτυχε, πάμε στην κατάσταση "Aftermath"
				else
				{
					//Αν απέτυχε, γυρνάμε στην αρχή και ο γύρος επανλαμβάνεται.
					TM.state = turnManager.State.NS;
					TM.currentTurn = TM.previousTurn;
				}
			}

			// -- Κατάσταση AM (Aftermath) -- 
			// Εμφανίζουμε το αποτέλεσμα της κίνησης.
			else if (TM.state == turnManager.State.AM)
			{
				uim.dialogueBoxText(moveInstance.amDialogue);
				TM.counterHandler();  // Λέμε στο turnManager να ενημερώσει τους μετρητές και να ελέγξει για ειδικές καταστάσεις.
			}

			// -- Κατάσταση ST (Special State) --
			// Διαχειριζόμαστε τα σενάρια που αλλάζουν δραστικά το παιχνίδι (νίκη, ήττα, απομάκρυνση του βοηθού)
			else if (TM.state == turnManager.State.ST)
			{
				if (TM.losses == turnManager.Losses.PL)
				{
					if (dialogueFlow == 0)
					{
						uim.dialogueBoxText($"AFTER A HEROIC BATTLE {player.Name} LOST. BETTER LUCK NEXT TIME!");
						dialogueFlow = 1;
					}
					else if (dialogueFlow == 1) { GetTree().Quit(); } // Το παιχνίδι τελειώνει.
				}
				else if (TM.losses == turnManager.Losses.HL)
				{
					if (dialogueFlow == 0)
					{
						uim.dialogueBoxText($"{helper.Name} TRIED THEIR BEST BUT THE BATTLE WAS TO FIERCE FOR THEM!");
						TM.state = turnManager.State.NS; 
					}
				}
				else if (TM.losses == turnManager.Losses.EL)
				{
					if (dialogueFlow == 0)
					{
						uim.dialogueBoxText($"YOUR PERSEVERANCE PAID OFF. YOU WIN!");
						dialogueFlow = 1;
					}
					else if (dialogueFlow == 1) { GoToEnemySelection(); } // Πάμε στη επιλογή αντιπάλου για την εκκίνηση της επόμενης μάχης.
				}
			}
		}
	}


/// <summary>
/// Προετοιμάζει την κατάσταση του παιχνιδιού για την επόμενη μάχη και αλλάζει την σκηνή στην σκηνή επιλογής αντιπάλου.
/// </summary>
	public void GoToEnemySelection()
	{
		if (helper.CHP < 1) { helper.CHP = 1; } //Αν ο βοηθός έχει ηττηθεί, τον επαναφέρουμε στους 1 πόντους ζωής 

		var gm = GameManager.Instance;
		// Ενημερώνουμε τις τιμές του GameManager για τους τρέχοντες πόντους ζωής του παίκτη και του βοηθού και ορίζει το 
		// flag που σηματοδοτεί αν πρόκειται για την πρώτη μάχη του παιχνιδιού ως false.
		gm.playerCHp = player.CHP;
		gm.helperCHp = helper.CHP;
		gm.firstBattle = false;

		// Φορτώνουμε την σκηνή επιλογής αντιπάλου και αν φορτώθηκε σωστά αλλάζουμε την σκηνή.
		var nextScene = (PackedScene)ResourceLoader.Load("res://Enemy Selection/enemy_selection.tscn");
		if (nextScene != null) { GetTree().ChangeSceneToPacked(nextScene); }
	}

/// <summary>
/// Η συνάρτηση που θα δημιουργήσει το αντικείμενο επιλογής κινήσεων. Επιστρέφει την την κίνηση που κρίνεται ως η βέλτιστη.
/// </summary>
	public int moveChooser(character player, character helper, character enemy, int[] counters, int chosenEnemy)
	{
		// Ορίζουμε τους πίνακες με όλα τα αναγνωριστικά των κινήσεων του βοηθού και των 4 αντιπάλων.
		int[] helperActions = { 106, 107, 108, 109, 202, 203, 204, 301, 302 };
		int[] enemyActions;

		switch (chosenEnemy)
		{
			case 1:
				enemyActions = new int[] { 205, 110, 110, 111, 111, 111, 111 };
				break;
			case 2:
				enemyActions = new int[] { 206, 112, 112, 113, 113, 113, 113 };
				break;
			case 3:
				enemyActions = new int[] { 207, 115, 115, 114, 114, 114, 114 };
				break;
			default:
				enemyActions = new int[] { 208, 116, 116, 303 };
				break;
		}

		// Δημιουργούμε το αντικείμενο της κλάσης 'movePrediction' δίνοντας του την τρέχουσα κατάσταση της μάχης.
		predictor = new movePrediction(player, helper, enemy, chosenEnemy, counters, helperActions, enemyActions);
		// Καλούμε την μέθοδο του συστήματος που θα κάνει τους υπολογισούμε και θα επιστρέψει την καλύτερη κίνηση.
		int result = predictor.chooseBestAction();
		//Επιστρέφουμε το αποτέλεσμα.
		return result;

	}
}

//-- ΚΩΔΙΚΑΣ ΓΙΑ TESTING: ΚΙΝΗΣΕΙΣ ΤΟΥ ΒΟΗΘΟΥ-- 
/*
	public void buttonPressed(){
		helperChosenAction = (helperChosenAction %9) +1;
		updateButtonText();
	}

	public void updateButtonText(){
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
	} 
	
-- ΕΝΤΟΛΕΣ ΠΟΥ ΜΠΑΙΝΟΥΝ ΣΤΗΝ _Ready() --
		helper_action = GetNode<Button>("background/ui/bot_action");
		helper_action.FocusMode = Control.FocusModeEnum.None;
		helper_action.Pressed += buttonPressed;

-- ΔΗΛΩΣΕΙΣ -- 
	public int helperChosenAction =0;
	public int [] helperAction = {106, 107, 108, 109, 202, 203, 204, 301, 302};
	public Button helper_action;
	
	*/
