using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Linq;

/// <summary>
/// Μια κλάση που διαχειρίζεται την πρόσβαση στο κατάλογο των προ-υπολογισμένων ζημιών. 
/// </summary>
public static class dmgCatalogue
{
    // Ένα dictionary που λειτουργεί ως η γρήγορη, προσωρινή μνήμη (cache). Το string είναι το κλειδί 
    // το μοναδικό αναγνωρισιτκό (damageId) και το int η τιμή της ζημιάς.
    private static Dictionary<string, int> _catalogueCache;

    /// <summary>
    /// H μέθοδος που φορτώνει το αρχείο JSON και αποθηκεύει τα περιεχόμενα του στο cache.
    /// </summary>
    private static void InitializeCache()
    {
        GD.Print("Αρχικοποιούμε το Cache...");

        // Δημιουργούμε το αντικείμενο Dictionary.
        _catalogueCache = new Dictionary<string, int>();
        string fileName = "res://Meta/damageCatalogue.json";

        if (!FileAccess.FileExists(fileName))
        {
            GD.PushError($"Δεν βρέθηκε το αρχείο στο {fileName}");
            return;
        }

        try
        {
            // Διαβάζουμε όλο το περιεχόμενο του αρχείου σε ένα string.
            string jsonString;
            using (var file = FileAccess.Open(fileName, FileAccess.ModeFlags.Read))
            {
                jsonString = file.GetAsText();
            }
            // Μετατρέπουμε το string σε ένα δομημένο αντικείμενο JSON.
            JsonNode catalogueNode = JsonNode.Parse(jsonString);

            if (catalogueNode is JsonObject catalogueObject)
            {
                // Κάνουμε ένα loop σε όλα τα ζευγάρια "κλειδί-τιμή" του JSON αντικειμένου.
                foreach (var entry in catalogueObject)
                {
                    // Για κάθε ζεύγος, προσθέτουμε μια νέα εγγραφή στο Dictionary μας.
                    _catalogueCache[entry.Key] = entry.Value.GetValue<int>();
                }
            }
        }
        catch (Exception ex)
        {
            GD.PushError($"Error: Αποτυχία αρχικοποίησης. {ex.Message}");
        }
    }

    /// <summary>
    /// H κύρια μέθοδος. Αναζητά ένα damageId και επιστρέφει την αντίστοιχη ζημιά. 
    /// </summary>
    public static int searchDmgCatalogue(string damageId)
    {
        // Αν το cache είναι null, σημαίνει ότι δεν έχει γίνει αρχικοποίηση. Καλούμε την InitializeCache για να διαβάσει
        // το αρχείο και να γεμίσει το cache.
        if (_catalogueCache == null) { InitializeCache(); } 
        
        // Ψάχνουμε γρήγορα το Dictionary για την τιμή της ζημιάς.
        if (_catalogueCache.ContainsKey(damageId)) { return _catalogueCache[damageId]; }
        else
        {
            // Αν δεν βρεθεί το ακριβές κλειδί, ψάχνουμε μια εκδοχή του κλειδιού αλλά με όλους τους μετρητές απενεργοποιημένους.
            // Την βασική ζημιά.
            if (damageId.EndsWith("000001") || damageId.EndsWith("000002"))
            {
                GD.Print($"Δεν υπάρχει βασικό κλειδί για την ζημιά {damageId}");
                return 0;
            }
            int startIndex = damageId.Length - 6;
            string temp = damageId.Remove(startIndex, 5);
            string baseDamageId = temp.Insert(startIndex, "00000");

            // Η αναδρομική κλήση θα ψάξει πάλι στο cache.
            return searchDmgCatalogue(baseDamageId);
        }

    }
}
