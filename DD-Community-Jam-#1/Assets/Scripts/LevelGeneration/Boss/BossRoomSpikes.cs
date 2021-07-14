using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BossRoomSpikes : IBossChunkable
{
    public GameObject bossObject;
    public GameObject[] minionTypes;
    public GameObject[] spikeTypes;

    public float minionSpawnTime;
    public float spikeSpawnTime;
    private float curMinionSpawnTime;
    private float curSpikeSpawnTime;

    private GameObject currentBoss;
    private List<GameObject> minions;
    private List<GameObject> spikes;

    private Vector2[] minionPosition;
    private Vector2[] hazardPosition;

    public override void OnBossChunkEnter(Vector2 bossPosition, Vector2[] minionPosition, Vector2[] hazardPosition)
    {
        if(bossObject != null)
            currentBoss = Instantiate(bossObject, bossPosition, Quaternion.identity);
        curMinionSpawnTime = minionSpawnTime;
        curSpikeSpawnTime = spikeSpawnTime;

        this.minionPosition = minionPosition;
        this.hazardPosition = hazardPosition;

        minions = new List<GameObject>();
        spikes = new List<GameObject>();
    }

    public override void OnBossChunkExit()
    {
        if(minions != null)
        {
            for (int i = 0; i < minions.Count; i++)
            {
                if (minions[i] == null)
                    minions.RemoveAt(i);
                else
                    Destroy(minions[i]);
            }
        }
        if (spikes != null)
        {
            for (int i = 0; i < spikes.Count; i++)
            {
                if (spikes[i] == null)
                    spikes.RemoveAt(i);
                else
                    Destroy(spikes[i]);
            }
        }

        if(currentBoss != null)
            Destroy(currentBoss);
    }

    public override void OnBossChunkStay()
    {
        if (curMinionSpawnTime <= 0f)
        {
            //Spawn Minions
            curMinionSpawnTime = minionSpawnTime;
            if (minionTypes.Length != 0)
            {
                Vector3 minionPos = minionPosition[Random.Range(0, minionPosition.Length)];
                minions.Add(Instantiate(minionTypes[Random.Range(0, minionTypes.Length)], minionPos, Quaternion.identity));
            }
        }

        if (curSpikeSpawnTime <= 0f)
        {
            //Spawn Spikes
            curSpikeSpawnTime = spikeSpawnTime;
            if (spikeTypes.Length != 0)
            {
                Vector3 spikePos = hazardPosition[Random.Range(0, hazardPosition.Length)];
                minions.Add(Instantiate(spikeTypes[Random.Range(0, spikeTypes.Length)], spikePos, Quaternion.identity));
            }
        }
        //Remove minions if they don't exist
        for (int i = 0; i < minions.Count; i++)
        {
            if (minions[i] == null)
                minions.RemoveAt(i);
        }
        //Remove Spikes if they don't exist
        for (int i = 0; i < spikes.Count; i++)
        {
            if (spikes[i] == null)
                spikes.RemoveAt(i);
        }

        curMinionSpawnTime -= Time.deltaTime;
        curSpikeSpawnTime -= Time.deltaTime;
    }
}
