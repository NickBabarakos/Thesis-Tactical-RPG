using Godot;
using System;

public class attackMoveData : baseMoveData
{
    public int moveDelay { get; }
    public int moveDmgType { get; }

    public attackMoveData(int MoveNumber, string MoveName, int MovePower, string Dialogue1, string Dialogue2, int MoveDelay, int MoveDmgType)
    : base(MoveNumber, MoveName, MovePower, Dialogue1, Dialogue2, "")
    {
        moveDelay = MoveDelay;
        moveDmgType = MoveDmgType;
    }
}
