using Godot;
using System;

public partial class gameState : Node
{
    private int _playerHP;
    private int _helperHP;
    private int _enemyHP;
    private int [] _counterState;
    private int _dmg;
    private int _heal;

    public int playerHP {
        get { return _playerHP; }
        set { _playerHP = value; }
    }

    public int helperHP {
        get { return _helperHP; }
        set { _helperHP = value; }
    }

    public int enemyHP {
        get { return _enemyHP; }
        set { _enemyHP = value; }
    }

    public int [] counterState {
        get { return _counterState;}
        set {_counterState = value;}
    }

    public int dmg {
        get { return _dmg;}
        set { _dmg = value;}
    }

    public int heal {
        get { return _heal;}
        set {_heal = value;}
    }

    public gameState(int PlayerHP, int HelperHP, int EnemyHP, int [] CounterState, int Dmg, int Heal){
        playerHP = PlayerHP;
        helperHP = HelperHP;
        enemyHP = EnemyHP;
        counterState = (int[])CounterState.Clone(); 
        dmg = Dmg;
        heal = Heal;
    }

    public gameState Clone(){
        return new gameState (this.playerHP, this.helperHP, this.enemyHP, this.counterState, this.dmg, this.heal);
    }

}
