using Godot;
using System;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Nodes;

public static class moveDataLoader
{
    private const string ATTACKS_LIST_PATH = "res://Meta/AttacksList.json";
    private const string STATCHANGE_LIST_PATH = "res://Meta/StatChangeList.json";

    public static baseMoveData LoadMove(int moveNumber)
    {
        int moveType = moveNumber / 100;
        string filePath;

        if (moveType == 1) { filePath = ATTACKS_LIST_PATH; }
        else { filePath = STATCHANGE_LIST_PATH; }

        if (!FileAccess.FileExists(filePath))
        {
            GD.PushWarning($"File not found: {filePath}");
            return null;
        }


        string jsonString = "";

        using (var file = FileAccess.Open(filePath, FileAccess.ModeFlags.Read))
        {
            jsonString = file.GetAsText();
        }

        try
        {
            JsonNode root = JsonNode.Parse(jsonString);

            if (root is JsonArray movesArray)
            {
                foreach (var moveObject in movesArray)
                {
                    if (moveObject is JsonObject currentMove)
                    {
                        if (currentMove.TryGetPropertyValue("Number", out JsonNode numberNode) && numberNode.GetValue<int>() == moveNumber)
                        {
                            if (moveType == 1)
                            {
                                string moveName = moveObject["Name"]?.GetValue<string>() ?? "";
                                int movePower = moveObject["Power"]?.GetValue<int>() ?? 0;
                                int moveDelay = moveObject["Delay"]?.GetValue<int>() ?? 0;
                                int moveDmgType = moveObject["Type"]?.GetValue<int>() ?? 0;
                                string dialogue1 = moveObject["dialogue1"]?.GetValue<string>() ?? "";
                                string dialogue2 = moveObject["dialogue2"]?.GetValue<string>() ?? "";

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
            GD.PushError($"An unexpected error occured while loading {filePath} : {e.Message}");
        }
        GD.PushWarning($"Η κίνηση με ID {moveNumber} δεν βρέθηκε στο {filePath}");
        return null;
    }
    
}
