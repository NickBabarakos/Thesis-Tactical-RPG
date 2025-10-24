using Godot;
using System;

/// <summary>
/// Μια κλάση που λειτουργεί ως προσομοιωτής της μάχης. Παίρνει μια κατάσταση παιχνιδιού (gameState) και μια ενέργεια,
/// και επιστρέφει μια νέα κατάσταση του παιχνιδιού που αντικατοπτρίζει το αποτέλεσμα αυτής της ενέργειας. 
/// </summary>
public static class simulatedBattle
{

    /// <summary>
    /// Προσομοιώνει την εκτέλεση μιας κίνησης του βοηθού.
    /// </summary>
    public static gameState helperAction(int actionIndex, gameState newState, int chosenEnemy, int[] helperActions)
    {
        string attackPattern, moveNumber, counterSt = "", possibility, damageId;
        int j, damage = 0;

        // Ελέγχουμε αν η κίνηση είναι αλλαγής στατιστικών και αν μπορεί να εκτελεστεί.
        if (actionIndex > 3 && actionIndex < 7) { newState.counterFlag = checkCounter(actionIndex, newState.counterState); }
        // Αν το flag ενεργοποιηθεί, η κίνηση αποτυγχάνει. Επιστρέφουμε αμέσως την κατάσταση ως έχει.
        if (newState.counterFlag == true) { return newState; }

        // Διακρίνουμε τις περιπτώσεις με βάση το index της κίνησης.
        if (actionIndex >= 0 && actionIndex < 4){ // Επίθεση
            // Δημιουργούμε το μοναδικό αναγνωριστικό ζημιάς (damageId) συνδυάζοντας όλες τις παραμέτρους.
            attackPattern = "H" + chosenEnemy.ToString();
            moveNumber = helperActions[actionIndex].ToString();

            for (j = 0; j < 5; j++) { counterSt = counterSt + newState.counterState[j].ToString(); }

            possibility = "1";
            damageId = attackPattern + moveNumber + counterSt + possibility;

            // Ζητάμε από το dmgCatalogue την προ-υπολογισμένη ζημιά.
            damage = dmgCatalogue.searchDmgCatalogue(damageId);

            // Εφαρμόζουμε την αλλαγή στο gameState.
            newState.enemyHP = newState.enemyHP - damage;
            newState.dmg = damage; //Αποθηκεύουμε τη ζημιά για τη συνάρτηση αξιολόγησης.

        }
        else if (actionIndex >= 4 && actionIndex < 7){ // Κίνηση αλλαγής στατιστικών.
            newState.counterState[actionIndex - 3] = 1; // Ενεργοποιούμε τον αντίστοιχο counter.
        }
        else if (actionIndex == 7) { // Κίνηση επαναφοράς πόντων ζωής (με στόχο τον παίκτη)
            newState.playerHP = newState.playerHP + 4;
            newState.heal = 1; // Σημειώνουμε ότι έγινε επαναφορά πόντων ζωής του παίκτη.
        }
        else if (actionIndex == 8) { // Κίνηση επαναφοράς πόντων ζωής (με στόχο τον βοηθό).
            newState.helperHP = newState.helperHP + 4;
            newState.heal = 2; // Σημειώνουμε ότι έγινε επαναφορά πόντων ζωής του βοηθού.
        }

        return newState; // Επιστρέφουμε την τροποποιημένη κατάσταση.
    }

    /// <summary>
    /// Προσομοιώνει την εκτέλεση μιας κίνησης του εχθρού.
    /// </summary>
    public static gameState enemyAction(int actionIndex, gameState newState, int chosenEnemy, int[] enemyActions)
    {
        string attackPattern, moveNumber, counterSt = "", possibility, damageId;
        int i, dmg;

        // -- Έλεγχος Προυποθέσεων -- 
        if (actionIndex == 0) { newState.counterFlag = checkCounter(actionIndex, newState.counterState); }
        if (newState.counterFlag == true) { return newState; }
        // --Εφαρμογή της Κίνησης--
        if (actionIndex == 0) { newState.counterState[4] = 1; } // Κίνηση αλλαγής στατιστικών.
        else if (actionIndex == 3 && chosenEnemy == 4) { newState.enemyHP = newState.enemyHP + 8; } // Κίνηση Επαναφοράς πόντων ζωής (υπάρχει μόνο για τον αντίπαλο 4)
        else{ //Επίθεση
            // Καθορίζουμε τον στόχο (H=Helper, P=Player) με βάση το index.
            attackPattern = chosenEnemy.ToString() + "H";
            if (actionIndex == 2 || actionIndex == 4 || actionIndex == 6) { attackPattern = chosenEnemy.ToString() + "P"; }

            // Δημιουργούμε το damageId.
            moveNumber = enemyActions[actionIndex].ToString();
            for (i = 0; i < 5; i++) { counterSt = counterSt + newState.counterState[i].ToString(); }
            possibility = "1";

            // Παίρνουμε την ζημιά και την εφαρμόζουμε στον σωστό στόχο.
            if (actionIndex == 5 || actionIndex == 6) { possibility = "2"; }
            damageId = attackPattern + moveNumber + counterSt + possibility;
            dmg = dmgCatalogue.searchDmgCatalogue(damageId);
            if (actionIndex == 1 || actionIndex == 3 || actionIndex == 5) { newState.helperHP = newState.helperHP - dmg; }
            else { newState.playerHP = newState.playerHP - dmg; }
        }


        return newState;
    }

    /// <summary>
    /// Μια βοηθητική μέθοδος που ελέγχει αν μια κίνηση αλλαγής στατιστικών είναι ήδη ενεργοποιημένη.
    /// </summary>
    private static bool checkCounter(int i, int[] counterState)
    {
        if (i == 0)
        {
            if (counterState[4] == 1) { return true; }
        }
        else
        {
            if (counterState[i - 3] == 1) { return true; }
        }
        return false;
    }

}
