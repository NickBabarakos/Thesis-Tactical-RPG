using Godot;
using System;

/// <summary>
///  Διαχειρίζεται τις σειρές, την κατάσταση της μάχης και τους μετρητές για τα buffs/debuffs.
/// </summary>
public partial class turnManager : Node {

	//-- Αναφορές σε άλλους Managers-- 
	public Root root;
	public UIManager uim;

	//-- Ένας πίνακας που λειτουργεί ως χρονόμετρο για τις κινήσεις αλλαγής στατιστικών --- 
	public int[] counter = { 0, 0, 0, 0, 0 }; //Shield, ATKBoost, ATKDebuf, DEFDebuf, EnemyCounter

	//-- Ένας πίνακας που καταγράφει τις κινήσεις των 4 αντιπάλων. Για κάθε αντίπαλο ισχύει monserId -1 -- 
	public int[,] enemyAction = { { 110, 111, 205 }, { 112, 113, 206 }, { 114, 115, 207 }, { 116, 303, 208 } };
	
	// Καθορίζει τις φάσεις ενός γύρου.
	public enum State
	{
		NS = 0,     //No State
		MD = 1,     //Move Delcaration
		AM = 2,     //Aftermath
		ST = 4      //Special State
	}

	// Καταγράφει ποιος χαρακτήρας έχει ηττηθεί.
	public enum Losses
	{
		NL = 0, //No Losses
		PL = 1, //Player Lost
		EL = 2, //Enemy Lost
		HL = 4, //Helper Lost

	}

	// Καθορίζει ποιος χαρακτήρας παίζει στην τρέχουσα σειρα ή έπαιζε στην προηγούμενη σειρά.
	public enum Turn
	{
		PT = 0,    //Player Turn
		HT = 1,    //Helper Turn
		ET = 2,    //Enemy Turn 
	}

	public Turn currentTurn = Turn.ET; // Η τρέχουσα σειρά. Αρχικοποιείται στον εχθρό αλλά αλλάζει αμέσως.
	public Turn previousTurn;          // Αποθηκεύει την προηγούμενη σειρά, χρήσιμο αν μια κίνηση αποτύχει.
	public State state = State.NS;     // Η τρέχουσα κατάσταση της μηχανής καταστάσεων.
	public Losses losses = Losses.NL;  // Η τρέχουσα κατάσταση χαρακτήρων που έχουν ηττηθεί.

	private Random rng = new Random();

	/// <summary>
	/// Καλείται μια φορά όταν ο κόμβος είναι έτοιμος.
	/// </summary>
	public override void _Ready()
	{
		// Παίρνουμε τις απαραίτητες αναφορές στους άλλους managers.
		root = GetTree().CurrentScene as Root;
		uim = GetNode<UIManager>($"../background/ui/UIManager");

	}
	
	/// <summary>
    /// H κεντρική λογική που καθορίζει τις σειρές. Καλείται στην αρχή κάθε νέας σειράς.
    /// </summary>
	public void TurnHandler(){
		previousTurn = currentTurn; //Αποθηκεύουμε την τρέχουσα σειρά πριν την αλλάξουμε.

		if (currentTurn == Turn.PT && losses == Losses.NL) {   //Οι συνθήκες που οδηγούν στην σειρά του βοηθού.
			currentTurn = Turn.HT;
			uim.dialogueBoxText($"IT'S {root.helper.Name}'s TURN!");
		}
		else if (currentTurn == Turn.HT && losses == Losses.NL || currentTurn == Turn.PT && losses == Losses.HL) { enemyTurn(); }//Η συνθήκες που οδηγούν στη σειρά του αντιπάλου.
		else if (currentTurn == Turn.ET) { 	playerTurn(); }// Η συνθήκη που οδηγεί στην σειρά του παίκτη.
	}

	/// <summary>
    /// Προετοιμάζει τη σκηνή για την σειρά του παίκτη.
    /// </summary>
	public void playerTurn() {
		currentTurn = Turn.PT;
		root.chosenAction = 101; // Επαναφέρουμε την επιλεγμένη κίνηση στην πρώτη της λίστας.
		uim.menuVisible(true);   // Δίνουμε εντολή στο UIManager να εμφανίσει το menu κινήσεων του παίκτη.
	}

	/// <summary>
	/// Προετοιμάζει και εκτελεί τη λογική για την σειρά του εχθρού.
	/// </summary>
	public void enemyTurn()
	{
		currentTurn = Turn.ET;
		uim.dialogueBoxText($"IT'S {root.enemy.Name}'s TURN!");

		// Ένας βρόγχος για να διασφαλίσουμε ότι ο εχθρός θα επιλέξει μια έγκυρη κίνηση.
		do
		{
			// Επιλέγεται τυχαία μια από τις 3 έγκυρες κινήσεις του αντιπάλου.
			int move = rng.Next(1, 4);
			if (move == 1) { root.chosenAction = enemyAction[root.monsterId - 1, 0]; }
			else if (move == 2) { root.chosenAction = enemyAction[root.monsterId - 1, 1]; }
			else
			{
				// Αν επιλεγεί η 3η κίνηση (αλλαγής στατιστικών), ελέγχουμε αν μπορεί να χρησιμοποιηθεί.
				if (counter[4] == 0) { root.chosenAction = enemyAction[root.monsterId - 1, 2]; }
				else { root.chosenAction = 0; }
			}
			// Ειδικός κανόνας για το τέρας 4: δεν μπορεί να χρησιμοποιήσει τη κίνηση 303 αν έχει πλήρη τρέχοντες πόντους ζωής.
			if (root.monsterId == 4 && root.chosenAction == 303 && root.enemy.HP == root.enemy.CHP) { root.chosenAction = 0; }
		} while (root.chosenAction == 0);
	}

	/// <summary>
    /// Διαχειρίζεται όλους τους μετρτητές στο τέλος μιας ενέργειας. Μειώνει τη διάρκεια τους και επαναφέρει τα στατιστικά όταν λήξουν.
	/// Επίσης ελέγχει για συνθήκες ειδικών καταστάσεων του παιχνιδιού.
    /// </summary>
	public void counterHandler()
	{

		//-- ΕΝΗΜΕΡΩΣΗ ΜΕΤΡΗΤΩΝ --

		// Αν έπαιξε μόλις ο παίκτης, ελέγχουμε το πρώτο μετρητή.
		if (currentTurn == Turn.PT)
		{
			if (counter[0] > 0 && counter[0] < 4) { counter[0] = counter[0] + 1; } //Αν ο μετρητής έχει τιμή 1-3 τότε τον αυξάνουμε κατά 1.
			else if (counter[0] == 4)
			{  //Αν ο μετρητής έχει τιμή 4, τον μηδενίζουμε και επαναφέρουμε το στατιστικό.
				counter[0] = 0;
				root.player.DEF = root.player.DEF - 3;
			}
		}

		// Αν έπαιξε μόλις ο βοηθός, ελέγχουμε τον δεύτερο, τρίτο και τέταρτο μετρητή.
		if (currentTurn == Turn.HT)
		{
			if (counter[1] > 0 && counter[1] < 4) { counter[1] = counter[1] + 1; }
			else if (counter[1] == 4)
			{
				counter[1] = 0;
				root.player.ATK = root.player.ATK - 3;
			}

			if (counter[2] > 0 && counter[2] < 4) { counter[2] = counter[2] + 1; }
			else if (counter[2] == 4)
			{
				counter[2] = 0;
				root.enemy.ATK = root.enemy.ATK + 3;
			}

			if (counter[3] > 0 && counter[3] < 4) { counter[3] = counter[3] + 1; }
			else if (counter[3] == 4)
			{
				counter[3] = 0;
				root.enemy.DEF = root.enemy.DEF + 3;
			}
		}

		// Αν έπαιξε μόλις ο αντίπαλος ελέγχουμε τον τελευταίο μετρητή.
		if (currentTurn == Turn.ET)
		{
			if (counter[4] > 0 && counter[4] < 4) { counter[4] = counter[4] + 1; }
			else if (counter[4] == 4)
			{
				counter[4] = 0;
				// Ανάλογα τον αντίπαλο, εκτελέστηκε διαφορετική κίνηση και επαναφέρονται διαφορετικά στατιστικά/
				if (root.monsterId == 1) { root.enemy.SPD = root.enemy.SPD - 3; }
				else if (root.monsterId == 2)
				{
					root.enemy.SP_DEF = root.enemy.SP_DEF - 2;
					root.enemy.DEF = root.enemy.DEF - 2;
				}
				else if (root.monsterId == 3)
				{
					root.enemy.SP_ATK = root.enemy.SP_ATK - 2;
					root.enemy.ATK = root.enemy.ATK - 2;
				}
				else if (root.monsterId == 4)
				{
					root.enemy.SP_ATK = root.enemy.SP_ATK - 2;
					root.enemy.SPD = root.enemy.SPD - 2;
				}
			}
		}

		// -- ΈΛΕΓΧΟΣ ΕΙΔΙΚΗΣ ΚΑΤΑΣΤΑΣΗΣ -- 

		// H κατάσταση που πρέπει να βρίσκεται το παιχνίδι για να συνεχιστεί κανονικά.
		if (root.player.CHP > 0 && root.helper.CHP > 0 && root.enemy.CHP > 0 || root.player.CHP > 0 && root.enemy.CHP > 0 && losses == Losses.HL)
		{
			state = State.NS;
		}
		// Η συνθήκη που οδηγεί το παιχνίδι στην ειδική κατάσταση "Ήττα Παίκτη". 
		else if (root.player.CHP < 1)
		{
			state = State.ST;
			losses = Losses.PL;
		}
		// Η συνθήκη που οδηγεί το παιχνίδι στην ειδική κατάσταση "Απομάκρυνση Βοηθού". Το παιχνίδι συνεχίζεται χωρίς αυτόν.
		else if (root.helper.CHP < 1 && losses == Losses.NL)
		{
			state = State.ST;
			losses = Losses.HL;
		}
		// Η συνθήκη που οδηγεί το παιχνίδι στην ειδική κατάσταση "Ήττα Αντιπάλου". Ο παίκτης νίκησε και οδηγείται στην επόμενη μάχη.
		else if (root.enemy.CHP < 1)
		{
			state = State.ST;
			losses = Losses.EL;
		}

	}

}
