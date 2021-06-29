using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRoomThinker : MonoBehaviour
{
    public IBossChunkable bossThink;
    public bool canUpdate;

    void Update()
    {
        if (canUpdate)
            UpdateThinking();
    }

    public void StartThinking(Vector2 bossPosition, Vector2[] minionPositions, Vector2[] hazardPositions)
    {
        if (bossThink == null)
            return;
        Debug.Log("START THINKING");
        bossThink.OnBossChunkEnter(bossPosition, minionPositions, hazardPositions);
        canUpdate = true;
    }

    public void UpdateThinking()
    {
        if(bossThink == null)
            return;
        bossThink.OnBossChunkStay();
    }

    public void StopThinking()
    {
        if(bossThink == null)
            return;
        Debug.Log("STOP THINKING");
        bossThink.OnBossChunkExit();
    }
}
