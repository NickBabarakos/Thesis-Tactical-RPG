using Godot;
using System;
using System.Runtime.CompilerServices;

/// <summary>
/// Μια κλάση που περιέχει τη λογική για όλους τους υπολογισμούς που αφορούν τις κινήσεις επαναφοράς πόντων ζωής.
/// </summary>
public static class hpCalculator
{
    private static Random rng = new Random();

    /// <summary>
    /// Η μέθοδος που εφαρμόζει την επαναφορά πόντων ζωής στον χαρακτήρα-στόχο. 
    /// </summary>
    public static int ApplyHeal(character Target, statMoveData data)
    {
        // Υπολογίζουμε ένα τυχαίο βασικό ποσό επαναφοράς (2,4 ή 6 Hp)
        int initialHeal = (rng.Next(1, 4)) * 2;

        // Εάν ο στόχος της κίνησης είναι ο αντίπαλος το ποσό διπλασιάζεται.
        if (data.moveTarget == "Enemy") { initialHeal = initialHeal * 2; }

        // Αποθηκεύουμε το τρέχον HP του στόχου πριν την εφαρμογή της επαναφοράς. 
        int previousHP = Target.CHP;
        // Προσθέτουμε το ποσό της επαναφοράς στο τρέχον HP του στόχου.
        Target.CHP += initialHeal;

        // Ελέγχουμε αν το νεο HP ξεπέρασε το μέγιστο δυνατό. Αν το ξεπέρασε το επαναφέρουμε στο μέγιστο. 
        if (Target.CHP > Target.HP) { Target.CHP = Target.HP; }

        // Επιστρέφουμε τη διαφορά μεταξύ του τελικού και του αρχικού HP. Έτσι θα πάρουμε τον ακριβή αριθμών πόντων ζωής που
        // αποκαταστάθηκαν.
        return Target.CHP - previousHP;
    }
}
