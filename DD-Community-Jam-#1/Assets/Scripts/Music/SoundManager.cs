using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public AudioMixer mixer;

    public float interpolation;
    public float goDownMultiply;
    public float goUpMultiply;

    public float[] volumes;

    public enum Musics
    {
        Cave_Calm,
        Cave_Tense,
        Boss,
        Shopping
    }

    public Musics curMusic;

    void Start()
    {
        volumes = new float[4]; //Musics Count

        for (int i = 0; i < volumes.Length; i++)
        {
            volumes[i] = -80;
        }
        curMusic = Musics.Cave_Calm;
    }

    void Update()
    {
        //Update Music in events
        if (ShopManager.instance.isOpen)
            curMusic = Musics.Shopping;
        else if (PlayerHealth.instance.curHealth == 1)
            curMusic = Musics.Cave_Tense;
        else if (DD_JAM.LevelGeneration.ChunkGeneration.instance.playerIsInBossChunk)
            curMusic = Musics.Boss;
        else
            curMusic = Musics.Cave_Calm;

        //INDEXES
        // 0 = Cave_Calm
        // 1 = Cave_Tense
        // 2 = Shopping
        // 3 = Boss

        if (curMusic == Musics.Cave_Calm)
            volumes[0] = Calc(volumes[0], true);
        else
            volumes[0] = Calc(volumes[0], false);

        if (curMusic == Musics.Cave_Tense)
            volumes[1] = Calc(volumes[1], true);
        else
            volumes[1] = Calc(volumes[1], false);

        if (curMusic == Musics.Shopping)
            volumes[1] = Calc(volumes[2], true);
        else
            volumes[1] = Calc(volumes[2], false);

        if (curMusic == Musics.Boss)
            volumes[1] = Calc(volumes[3], true);
        else
            volumes[1] = Calc(volumes[3], false);

        mixer.SetFloat("Volume_Cave_Calm", volumes[0]);
        mixer.SetFloat("Volume_Cave_Tense", volumes[1]);
        mixer.SetFloat("Volume_Shopping", volumes[2]);
        mixer.SetFloat("Volume_Boss", volumes[3]);
    }

    private float Calc(float input, bool up)
    {
        if (up)
            return Mathf.Clamp(input + interpolation * Time.deltaTime * goUpMultiply, -80, 0);
        else
            return Mathf.Clamp(input - interpolation * Time.deltaTime * goDownMultiply, -80, 0);
    }
}