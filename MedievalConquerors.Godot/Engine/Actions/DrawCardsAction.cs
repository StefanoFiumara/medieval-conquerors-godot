﻿using System.Collections.Generic;
using MedievalConquerors.Engine.Data;

namespace MedievalConquerors.Engine.Actions;

public class DrawCardsAction : GameAction
{
    public IPlayer Target { get; }
    public int Amount { get; }
    public List<Card> DrawnCards { get; set; }

    public DrawCardsAction(int amount, IPlayer target)
    {
        Amount = amount;
        Target = target;
        DrawnCards = new List<Card>();
    }
}