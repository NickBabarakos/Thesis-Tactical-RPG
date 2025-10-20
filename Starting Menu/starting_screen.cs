using Godot;
using System;
using System.Text.RegularExpressions;

//Διαχειρίζεται τη λογική της αρχικής οθόνης του παιχνιδιού (Οθόνη Τίτλου, Βασικό Μενού, Οθόνη Επιλογής Ονόματος Παικτών).
public partial class starting_screen : Node
{
// --Μεταβλητές που συνδέονται με τα αντίστοιχα UI στοιχεία στον Editor της Godot--
	public TextureRect background;
	public Label press_enter;
	public Label new_game;
	public Label exit_game;
	public LineEdit name_input;
	public Label selection_text;
	private Texture2D selection_screen;

// --Μεταβλητές της κατάστασης (State)--
	private float timeElapsed = 0.0f;
	private bool isVisib = true;
	public bool firstNameDone = false;

// --Μεταβλητές Ρυθμίσεων -- 
	private Color yellow = new Color("ffeb3b");
	private Color white = new Color("ffffff");
	
// --Δημόσια Properties για την αποθήκευση των ονομάτων που θα δώσει ο χρήστης --
	[Export]
	public string player_name { get; private set; } = "";
	public string helper_name { get; private set; } = "";

	// --Enum που ορίζει τις διαφορετικές καταστάσεις της αρχικής οθόνης. Λειτουργεί ως μια απλή μηχανή καταστάσεων -- 
	[Flags]
	public enum State
	{
		ts = 0,  //Title Screen
		mn = 1,  //Menu
		ns = 2  //Name Selection
	}
	
// Η τρέχουσα κατάσταση της οθόνης. Αρχικοποιείται ως "Οθόνη Τίτλου".
	public State screen = State.ts;

// Η μέθοδος που καλείται αυτόματα από την Godot μια φορά όταν ο κόμβος είναι έτοιμος. Γίνεται αρχικοποίηση.
	public override void _Ready()
	{
		// Παίρνουμε τις αναφορές(refrences) στους κόμβους από το δένδρο σκηνής.
		background = GetNode<TextureRect>("background");
		press_enter = GetNode<Label>("background/UI/press_enter");
		new_game = GetNode<Label>("background/UI/new_game");
		exit_game = GetNode<Label>("background/UI/exit_game");
		name_input = GetNode<LineEdit>("background/UI/name_input");
		selection_text = GetNode<Label>("background/UI/selection_text");

		// Φορτώνουμε την εικόνα για την οθόνη επιλογής ονόματος
		selection_screen = GD.Load<Texture2D>("res://Assets/name_selection_screen.png");

		// Αρχικοποιούμε τις ιδιότητες των UI στοιχείων. 
		new_game.Modulate = yellow; //Το αρχικό χρώμα του lavel "new_game" είναι κίτρινο.
		selection_text.Text = "ENTER THE NAME OF YOUR HERO:";
		name_input.MaxLength = 7; //Ορίζουμε το μέγιστο μήκος ονόματος.

		//Συνδέουμε τα σήματα του LineEdit με τις αντίστοιχες μεθόδους.
		name_input.TextChanged += OnNameChanged;   //Καλείται σε κάθε αλλαγή κειμένου.
		name_input.TextSubmitted += OnNameEntered; //Καλείται όταν ο χρήστης πατήσει Enter.
	}

//Η μέθοδος _Input() καλείται κάθε φορά που υπάρχει ένα γεγονός εισόδου. Διαχείριση των ενεργειών του παίκτη.
	public override void _Input(InputEvent @event)
	{
		//Έλεγχος αν ο παίκτης πάτησε το πλήκρο "accept" (Enter ή Space).
		if (@event.IsActionPressed("ui_accept"))
		{
			if (screen == State.ts)
			{ //Ενέργειες που εκτελούνται αν είμαστε στην οθόνη τίτλου.
				timeElapsed = 10.0f;  //Σταματάμε το εφέ αναβοσβήσματος του lebel "press_enter".
				screen = State.mn;    //Αλλάζουμε την κατάσταση της οθόνης σε Menu.
				press_enter.Visible = false;
				new_game.Visible = true;
				exit_game.Visible = true;
			}
			else if (screen == State.mn)
			{ //Ενέργειες που εκτελούνται αν είμαστε στην οθόνη menu
				if (new_game.Modulate == yellow)
				{ //Αν έχουμε επιλεγμένο το "New Game"
					screen = State.ns;            //Αλλάζουμε την κατάσταση της οθόνης σε "Επιλογή Ονόματος"
					background.Texture = selection_screen;
					new_game.Visible = false;
					exit_game.Visible = false;
					selection_text.Visible = true;
					name_input.Visible = true;
					name_input.CallDeferred(LineEdit.MethodName.GrabFocus); //Καλούμε το GrabFocus() με καθυστέρηση για να προλάβει το UI να ενημερωθεί.
				}
				else { GetTree().Quit(); } //Αν έχουμε επιλεγμένο το "Exit Game" κλείνουμε το παιχνίδι. 
			}
		}
		
	// Ελέγχει αν ο παίκτης πάτησε το πάνω ή κάτω βελάκι
		if (@event.IsActionPressed("ui_up") || @event.IsActionPressed("ui_down")){
			if (screen == State.mn){ //Ενέργειες που εκτελούνται αν είμαστε στην οθόνη του Menu. Αλλάζει την επιλογή μεταξύ New Game και Exit Game
				if (new_game.Modulate == yellow){ 
					new_game.Modulate = white;
					exit_game.Modulate = yellow;
				}else{
					new_game.Modulate = yellow;
					exit_game.Modulate = white;}
			}

		}

	}

	//Μια συνάρτηση όταν ενεργοποιείται το σημα "TextChanged" του LineEdit. Φιλτράρει την είσοδο του χρήστη ώστε να
	//επιτρέπει μόνο αλφαβητικούς χαρακτήρες και τους μετατρέπει σε κεφαλαία.
	private void OnNameChanged(string newText)
	{
		// Με ένα Regular Expression αφαιρούμε όλους τους χαρακτήρες που δεν είναι γράμματα (A-Z, a-z).
		var filtered = Regex.Replace(newText, "[^A-Za-z]", "");

		// Περιορίζουμε το μήκος της συμβολοσειράς σε 7 χαρακτήρες
		if (filtered.Length > 7) { filtered = filtered.Substring(0, 7); }
		
		//Μετατρέπουμε το κείμενο σε κεφαλαία.	
		var upper = filtered.ToUpper();

		//Ενημερώνουμε το TextEdit μόνο αν έχει γίνει κάποια αλλαγή, για να αποφύγουμε άπειρο loop.
		if (upper != newText)
		{
			name_input.Text = upper;
			name_input.SetCaretColumn(upper.Length); //Τοποθετούμε τον κέρσορα στο τέλος του κειμένου.
		}
	}

	//Μια συνάρτηση που εκτελείται όταν ενεργοποιείται το σήμα "TextSubmitted" (με πάτημα του Enter).
	//Αποθηκεύει το όνομα και προχωράει στο επόμενο βήμα της διαδικασίας.
	private void OnNameEntered(string enteredText){
		//Αν το κείμενο είναι κενό, δεν γίνεται τίποτα.
		if (string.IsNullOrEmpty(name_input.Text)) { return; }

		if (!firstNameDone)
		{ //Οι παρακάτω ενέργειες εκτελούνται αν δεν έχουμε πάρει το πρώτο όνομα
			player_name = name_input.Text;  //Αποθηκεύουμε το όνομα του παίκτη.
			firstNameDone = true;           //Σημειώνουμε ότι το πρώτο όνομα δόθηκε
			selection_text.AddThemeFontSizeOverride("font_size", 10); //Μικραίνουμε τη γραμματοσειρά για το επόμενο μήνυμα.
			selection_text.Text = "ENTER THE NAME OF YOUR COMPANION:"; //Αλλάζουμε το μήνυμα για τη εισαγωγή του ονόματος του βοηθού.

			//Καθαρίζουμε το πεδίο εισαγωγής για την επόμενη χρήση.
			name_input.Text = "";
			name_input.CallDeferred(LineEdit.MethodName.GrabFocus);
		}
		else
		{ //Οι παρακάτω ενέργειες εκτελούνται αν έχει ήδη δοθεί το πρώτο όνομα.
			helper_name = name_input.Text; //Αποθηκεύουμε το όνομα του βοηθού.
			GoToNextScene();               //Καλούμε μια συνάρτηση για να πάμε στην επόμενη σκηνή του παιχνιδιού.
		} 
		

	}

	//Αποθηκεύει τα δεδομένα στον GameManager και αλλάζει την σκηνή. 
	private void GoToNextScene()
	{
		//Παίρνουμε μια αναφορά στο GameManager.
		var gm = GameManager.Instance;
		//Αποθηκεύουμε τα ονόματα και ότι πρόκειται για την πρώτη μάχη του παιχνιδιού στον GameManager.
		gm.playerName = player_name;
		gm.helperName = helper_name;
		gm.firstBattle = true;

		//Φορτώνουμε το πόρο της επόμενης σκηνής.
		var nextScene = (PackedScene)ResourceLoader.Load("res://Enemy Selection/enemy_selection.tscn");

		//Αν η φόρτωση ήταν επιτυχής, αλλάζουμε τη σκηνή.
		if (nextScene != null) { GetTree().ChangeSceneToPacked(nextScene); }
	}
	
	//H μέθοδος _Process καλείται σε κάθε frame. Την χρησιμοποιούμε για να δημιουργήσουμε ένα εφέ για το label "press_enter".
	public override void _Process(double delta){
		//Αυξάνουμε τον μετρητή.
		timeElapsed += (float)delta;

		//Ελέγχουμε αν έχει περάσει μισό δευτερόλεπτο. Ο έλεγχος "timeElapsed <10.0f" υπάρχει για να σταματήσουμε το εφέ
		//όταν ο παίκτης προχωρήσει στην οθόνη του menu.
		if (timeElapsed >= 0.5f && timeElapsed < 10.0f){
			//Αντιστρέφουμε την ορατότητα του κειμένου.
			if (isVisib){
				press_enter.Visible = false;
				}else{
				press_enter.Visible = true;
			}

			isVisib = !isVisib; //Εναλλαγή της κατάστασης ορατότητας.
			timeElapsed = 0.0f; //Μηδενισμός του χρονομέτρου για το επόμενο μισό δευτερόλεπτο.
		}
	}
}
