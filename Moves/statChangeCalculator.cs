using Godot;
using System;
using System.Reflection;

/// <summary>
/// Μια κλάση υπεύθυνση για την εφαρμογή αλλαγών στα στατιστικά των χαρακτήρων. 
/// </summary>
public static class statChangeCalculator
{
    /// <summary>
    /// Εφαρμόζει μια αλλαγή στατιστικών σε έναν χαρακτήρα-στόχο, ελέγχοντας πρώτα αν μπορεί να εκτελεστεί.
    /// </summary>
    public static bool ApplyStatChange(character Target, statMoveData data, int[] counters)
    {
        // Κάθε κίνηση αλλαγής στατιστικών αντιστοιχεί σε έναν counter. Αν η τιμή του δεν είναι 0, σημαίνει ότι η κίνηση 
        // έχει ήδη χρησιμοποιθεί και δεν μπορεί να χρησιμοποιηθεί μέχρι να μηδενιστεί ο counter.
        if (counters[data.moveCounter] != 0) { return false; }

        // Το πεδίο 'moveStat' στο JSON μπορεί να περιέχει ένα ή περισσότερα στατιστικά, χωρισμένα με κενό.
        string[] stats = data.moveStat.Split(' ');

        // Κάνουμε ένα loop για κάθε όνομα στατιστικού που βρήκαμε.
        foreach (string statName in stats)
        {
            // Ψάχνουμε μια ιδιότητα στην κλάση 'character' που να έχει το ίδιο όνομα που διαβάσαμε από το JSON.
            PropertyInfo propertyInfo = typeof(character).GetProperty(statName);
            // Ελέγχουμε αν βρέθηκε ιδιότητα.
            if (propertyInfo != null)
            {
                // Αν βρέθηκε παίνρουμε την τρέχουσα τιμή της ιδιότητας από τον χαρακτήρα-στόχο.
                int currentValue = (int)propertyInfo.GetValue(Target);
                // Ορίζουμε την νέα τιμή, προσθέτοντας το 'movePower' της κίνησης.
                propertyInfo.SetValue(Target, currentValue + data.movePower);
            }
        }

        // Αφού η κίνηση εκτελέστηκε με επιτυχία, θέτουμε τον counter της αντίστοιχης κίνησης ενεργό (ίσο με 1).
        counters[data.moveCounter] = 1;
        return true;
    }
}
