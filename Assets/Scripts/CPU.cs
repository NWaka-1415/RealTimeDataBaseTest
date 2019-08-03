using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPU : Player
{
    public override void Initialize(OverAllManager.UserData userData)
    {
        SetUp(userData);
        SetCards(false);
    }
}