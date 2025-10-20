using Godot;
using System;
using System.Text.RegularExpressions;

public partial class starting_screen : Node
{
	public TextureRect background; 
	public Label press_enter;
	public Label new_game;
	public Label exit_game;
	public LineEdit name_input;
	public Label selection_text;
	private float timeElapsed = 0.0f; 
	private bool isVisib = true;
	private Color yellow = new Color("ffeb3b");
	private Color white = new Color("ffffff"); 
	private Texture2D selection_screen;

	[Export]
	public string player_name {get; private set; } = "";
	public string helper_name {get; private set; } = "";

	public bool firstNameDone = false;

	[Flags]
	public enum state{
		ts = 0,  //Title Screen
		mn = 1,  //Menu
		ns = 2  //Name Selection
	}
	public state screen = state.ts;

	public override void _Ready()
	{
		background = GetNode<TextureRect>("background");
		press_enter = GetNode<Label>("background/UI/press_enter");
		new_game = GetNode<Label>("background/UI/new_game");
		new_game.Modulate = yellow;
		exit_game = GetNode<Label>("background/UI/exit_game");
		name_input = GetNode<LineEdit>("background/UI/name_input");
		selection_text = GetNode<Label>("background/UI/selection_text");
		selection_text.Text = "ENTER THE NAME OF YOUR HERO:";
		selection_screen = GD.Load<Texture2D>("C:/Users/User/Desktop/Godot/Projects/Untitled Game V0.2/untitled-v0.2/Starting Menu/name_selection_screen.png");



		name_input.MaxLength = 7;
		name_input.TextChanged += OnNameChanged;
		name_input.TextSubmitted += OnNameEntered;
	}

	
	public override void _Input(InputEvent @event){
		if(@event.IsActionPressed("ui_accept")){
			if(screen == state.ts){
				timeElapsed = 10.0f;
				screen = state.mn;
				press_enter.Visible = false;
				new_game.Visible = true;
				exit_game.Visible = true;
			}else if (screen == state.mn){
				if (new_game.Modulate == yellow){
					screen = state.ns;
					background.Texture = selection_screen;
					new_game.Visible = false;
					exit_game.Visible = false;
					name_input.GrabFocus();
					selection_text.Visible = true;
					name_input.Visible = true;
				} else{
					GetTree().Quit();
				}
			}
		}

		if (@event.IsActionPressed("ui_up") || @event.IsActionPressed("ui_down")){
			if (screen == state.mn){
				if(new_game.Modulate == yellow){
					new_game.Modulate = white;
					exit_game.Modulate = yellow;
				}else {
					new_game.Modulate = yellow;
					exit_game.Modulate = white;

				}
			}

		}

	} 

	private void OnNameChanged(string newText){

		var filtered = Regex.Replace(newText, "[^A-Za-z]", "");
		if (filtered.Length > 7){
			filtered = filtered.Substring(0,7);
		}
		var upper = filtered.ToUpper();

		if(upper != newText){
			name_input.Text = upper;
			name_input.SetCaretColumn(upper.Length);
		}
	}

	private void OnNameEntered(string enteredText){

		if (string.IsNullOrEmpty(name_input.Text)){
			return;
		}

		if(!firstNameDone){
			player_name = name_input.Text;
			firstNameDone = true;
			selection_text.AddThemeFontSizeOverride("font_size", 10);
			selection_text.Text = "ENTER THE NAME OF YOUR COMPANION:";
		} else {
			helper_name = name_input.Text;
			GoToNextScene();
		}
		name_input.Text = "";
		name_input.GrabFocus();
		
	}

	private void GoToNextScene(){
		var gm = GameManager.Instance;
		gm.playerName = player_name; 
		gm.helperName = helper_name;
		gm.firstBattle = true;

		var nextScene = (PackedScene)ResourceLoader.Load("res://Enemy Selection/enemy_selection.tscn");

		if (nextScene != null) {
			GetTree().ChangeSceneToPacked(nextScene);
		}
	}
	public override void _Process(double delta)
	{
		timeElapsed += (float)delta;

		if (timeElapsed >= 0.5f && timeElapsed < 10.0f){
			if (isVisib){
				press_enter.Visible = false;
			} else {
				
				press_enter.Visible = true;  
			}

			isVisib = !isVisib;
			timeElapsed = 0.0f; 
		} 
	}
}
