using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Linq;

public static class dmgCatalogue
{
    private static Dictionary<string, int> _catalogueCache;

    private static void InitializeCache()
    {
        GD.Print("Αρχικοποιούμε το Cache...");

        _catalogueCache = new Dictionary<string, int>();
        string fileName = "res://Meta/damageCatalogue.json";

        if (!FileAccess.FileExists(fileName))
        {
            GD.PushError($"Δεν βρέθηκε το αρχείο στο {fileName}");
            return;
        }

        try{
            string jsonString;
            using (var file = FileAccess.Open(fileName, FileAccess.ModeFlags.Read))
            {
                jsonString = file.GetAsText();
            }
            JsonNode catalogueNode = JsonNode.Parse(jsonString);

            if (catalogueNode is JsonObject catalogueObject)
            {
                foreach(var entry in catalogueObject)
                {
                    _catalogueCache[entry.Key] = entry.Value.GetValue<int>();
                }
            }
            } catch(Exception ex)
        {
            GD.PushError($"Error: Αποτυχία αρχικοποίησης. {ex.Message}");
        }
    }
    public static int searchDmgCatalogue(string damageId) {

        if (_catalogueCache == null) { InitializeCache(); }
        if (_catalogueCache.ContainsKey(damageId)) { return _catalogueCache[damageId]; }
        else
        {
            if (damageId.EndsWith("000001") || damageId.EndsWith("000002"))
            {
                GD.Print($"Δεν υπάρχει βασικό κλειδί για την ζημιά {damageId}");
                return 0;
            }
            int startIndex = damageId.Length - 6;
            string temp = damageId.Remove(startIndex, 5);
            string baseDamageId = temp.Insert(startIndex, "00000");

            return searchDmgCatalogue(baseDamageId);
        }

	}
}
