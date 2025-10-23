using Godot;
using System;

public static class simulatedBattle
{

    public static gameState helperAction(int actionIndex, gameState newState, int chosenEnemy, int[] helperActions)
    {
        string attackPattern, moveNumber, counterSt = "", possibility, damageId;
        int j, damage = 0;

        if (actionIndex > 3 && actionIndex < 7) { newState.counterFlag = checkCounter(actionIndex, newState.counterState); }
        if (newState.counterFlag == true) { return newState; }

        if (actionIndex >= 0 && actionIndex < 4)
        {
            attackPattern = "H" + chosenEnemy.ToString();
            moveNumber = helperActions[actionIndex].ToString();

            for (j = 0; j < 5; j++) { counterSt = counterSt + newState.counterState[j].ToString(); }

            possibility = "1";
            damageId = attackPattern + moveNumber + counterSt + possibility;

            damage = dmgCatalogue.searchDmgCatalogue(damageId);
            newState.enemyHP = newState.enemyHP - damage;
            newState.dmg = damage;

        }
        else if (actionIndex >= 4 && actionIndex < 7)
        {
            newState.counterState[actionIndex - 3] = 1;
        }
        else if (actionIndex == 7)
        {
            newState.playerHP = newState.playerHP + 4;
            newState.heal = 1;
        }
        else if (actionIndex == 8)
        {
            newState.helperHP = newState.helperHP + 4;
            newState.heal = 2;
        }

        return newState;
    }

    public static gameState enemyAction (int actionIndex, gameState newState, int chosenEnemy, int[] enemyActions)
    {
        string attackPattern, moveNumber, counterSt="", possibility, damageId;
        int i, dmg;
        
        if (actionIndex == 0) { newState.counterFlag = checkCounter(actionIndex, newState.counterState); }
        if (newState.counterFlag == true) { return newState; }

        if (actionIndex == 0) { newState.counterState[4] = 1; }
        else if (actionIndex == 3 && chosenEnemy == 4) { newState.enemyHP = newState.enemyHP + 8; }
        else{
            attackPattern = chosenEnemy.ToString() + "H";
            if (actionIndex == 2 || actionIndex == 4 || actionIndex == 6) { attackPattern = chosenEnemy.ToString() + "P"; }
           
            moveNumber = enemyActions[actionIndex].ToString();
            for (i = 0; i < 5; i++) { counterSt = counterSt + newState.counterState[i].ToString(); }
            possibility = "1";
            
            if (actionIndex == 5 || actionIndex == 6) { possibility = "2"; }
            damageId = attackPattern + moveNumber + counterSt + possibility;
            dmg = dmgCatalogue.searchDmgCatalogue(damageId);
            if (actionIndex == 1 || actionIndex == 3 || actionIndex == 5) { newState.helperHP = newState.helperHP - dmg; }
            else { newState.playerHP = newState.playerHP - dmg; }
        }


        return newState;
    }
    
public static bool checkCounter(int i, int [] counterState){
		if (i==0){
			if (counterState[4] == 1){ return true; }
		} else {
			if (counterState[i-3] == 1){ return true; }
		}
		return false;
	}
    
}
