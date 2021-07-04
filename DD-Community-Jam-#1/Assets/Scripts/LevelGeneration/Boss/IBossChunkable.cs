using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IBossChunkable : MonoBehaviour
{
    public abstract void OnBossChunkEnter(Vector2 bossPosition, Vector2[] minionPosition, Vector2[] hazardPosition);
    public abstract void OnBossChunkStay();
    public abstract void OnBossChunkExit();
}