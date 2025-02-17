using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverCutscene : Cutscene
{
    public override void End()
    {
        base.End();
        GameManager.Instance.GameOver();
    }
}