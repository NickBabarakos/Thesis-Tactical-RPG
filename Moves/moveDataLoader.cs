using Godot;
using System;
using System.Text.Json;
using System.Text.Json.Nodes;

/// <summary>
/// Μια κλάση που διαβάζει τα αρχεία JSON των κινήσεων και τα μετατρέπει σε MoveData αντικείμενα.
/// </summary>
public static class moveDataLoader
{
    // Ορίζουμε τα μονοπάτια των αρχείων σε σταθερές. 
    private const string ATTACKS_LIST_PATH = "res://Meta/AttacksList.json";
    private const string STATCHANGE_LIST_PATH = "res://Meta/StatChangeList.json";

    /// <summary>
    /// H κεντρική μέθοδος της κλάσης. Βρίσκει το σωστό αρχείο, το διαβάζει, και επιστρέφει ένα πλήρως συμπληρωμένο 
    /// moveData αντικείμενο.
    /// </summary>
    public static baseMoveData LoadMove(int moveNumber)
    {
        // Βρίσκουμε το πρώτο ψηφίο του αναγνωριστικού της κίνησης για να καταλάβουμε το τύπο της κίνησης.
        int moveType = moveNumber / 100;
        string filePath;

        // Επιλέγουμε το σωστό αρχείο για ανάγνωση.
        if (moveType == 1) { filePath = ATTACKS_LIST_PATH; }
        else { filePath = STATCHANGE_LIST_PATH; }

        // Έλεγχος Ασφάλειας: Αν το αρχείο δεν υπάρχει, σταματάμε.
        if (!FileAccess.FileExists(filePath))
        {
            GD.PushWarning($"File not found: {filePath}");
            return null;
        }

        // Διαβάζουμε όλο το περιεχόμενο του αρχείου ως ένα ενιαίο string.
        string jsonString = "";

        using (var file = FileAccess.Open(filePath, FileAccess.ModeFlags.Read))
        {
            // Με την εντολή using εξασφαλίζουμε ότι το αρχέιο θα κλείσει σωστά, ακόμα και αν έχουμε σφάλμα.
            jsonString = file.GetAsText();
        }

        // Χρησιμοποιούμε ένα try-catch block για να πιάσουμε σφάλματα κατα την επεξεργασία του JSON.
        try
        {
            // Μετατρέπουμε το string σε ένα δομημένο JsonNode αντικείμενο.
            JsonNode root = JsonNode.Parse(jsonString);

            // Τα JSON αρχεία μας είναι ένας πίνακας από αντικείμενα.
            if (root is JsonArray movesArray)
            {
                // Κάνουμε ένα loop για κάθε αντικείμενο-κίνηση μέσα στον πίνακα.
                foreach (var moveObject in movesArray)
                {
                    if (moveObject is JsonObject currentMove)
                    {
                        // Ελέγχουμε αν το τρέχον αντικείμενο έχει το Number που ψάχνουμε.
                        if (currentMove.TryGetPropertyValue("Number", out JsonNode numberNode) && numberNode.GetValue<int>() == moveNumber)
                        {
                            // Αφού βρεθεί η κίνηση, διαβάζουμε τα δεδομένα της.
                            if (moveType == 1)
                            {
                                string moveName = moveObject["Name"]?.GetValue<string>() ?? "";
                                int movePower = moveObject["Power"]?.GetValue<int>() ?? 0;
                                int moveDelay = moveObject["Delay"]?.GetValue<int>() ?? 0;
                                int moveDmgType = moveObject["Type"]?.GetValue<int>() ?? 0;
                                string dialogue1 = moveObject["dialogue1"]?.GetValue<string>() ?? "";
                                string dialogue2 = moveObject["dialogue2"]?.GetValue<string>() ?? "";

                                // Δημιουργούμε και επιστρέφουμε ένα νέο, πλήρες αντικείμενο attackMoveData. 
                                return new attackMoveData(moveNumber, moveName, movePower, dialogue1, dialogue2, moveDelay, moveDmgType);
                            }
                            else
                            {
                                string moveName = moveObject["Name"]?.GetValue<string>() ?? "";
                                string moveStat = moveObject["Stat"]?.GetValue<string>() ?? "";
                                string moveTarget = moveObject["Target"]?.GetValue<string>() ?? "";
                                int movePower = moveObject["Power"]?.GetValue<int>() ?? 0;
                                int moveCounter = moveObject["Counter"]?.GetValue<int>() ?? -1;
                                string dialogue1 = moveObject["dialogue1"]?.GetValue<string>() ?? "";
                                string dialogue2 = moveObject["dialogue2"]?.GetValue<string>() ?? "";
                                string dialogue3 = moveObject["dialogue3"]?.GetValue<string>() ?? "";

                                // Δημιουργούμε και επιστρέφουμε ένα νέο, πλήρες αντικείμενο statMoveData.
                                return new statMoveData(moveNumber, moveName, movePower, dialogue1, dialogue2, dialogue3, moveStat, moveTarget, moveCounter);
                            }
                        }
                    }
                }
            }
        }
        catch (JsonException e)
        {
            GD.PushError($"Error parsing Json in {filePath}: {e.Message}");
        }
        catch (Exception e)
        {
            GD.PushError($"An unexpected error occured while loading {filePath} : {e.Message}"); // Πιάνει, οποιοδήποτε άλλο σφάλμα.
        }
        // Αν φτάσουμε εδώ, σημαίνει ότι το loop τελείωσε χωρίς να βρεθεί το ID ή ότι υπήρξε κάποιο ασφάλμα.
        GD.PushWarning($"Η κίνηση με ID {moveNumber} δεν βρέθηκε στο {filePath}");
        return null;
    }

}
