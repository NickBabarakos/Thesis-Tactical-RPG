using Godot;
using System;

public class statMoveData : baseMoveData
{
    public string moveStat { get; }
    public string moveTarget { get; }
    public int moveCounter { get; }

    public statMoveData(int MoveNumber, string MoveName, int MovePower, string Dialogue1, string Dialogue2, string Dialogue3, string MoveStat, string MoveTarget, int MoveCounter ) 
    : base (MoveNumber, MoveName, MovePower, Dialogue1, Dialogue2, Dialogue3)
    {
        moveStat = MoveStat;
        moveTarget = MoveTarget;
        moveCounter = MoveCounter;
    }
}
