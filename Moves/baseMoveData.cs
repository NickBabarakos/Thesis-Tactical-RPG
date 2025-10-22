using Godot;
using System;

public class baseMoveData 
{
    public int moveNumber { get; }
    public string moveName { get; }
    public int movePower { get; }
    public string dialogue1 { get; }
    public string dialogue2 { get;}
    public string dialogue3 { get; }

    public baseMoveData (int MoveNumber, string MoveName, int MovePower, string Dialogue1, string Dialogue2, string Dialogue3 )
    {
        moveNumber = MoveNumber;
        moveName = MoveName;
        movePower = MovePower;
        dialogue1 = Dialogue1;
        dialogue2 = Dialogue2;
        dialogue3 = Dialogue3;
        
    }
}
