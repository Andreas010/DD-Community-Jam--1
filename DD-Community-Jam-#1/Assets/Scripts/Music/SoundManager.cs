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
        Shopping
    }

    public Musics curMusic;

    void Start()
    {
        volumes = new float[2]; //Musics Count

        for (int i = 0; i < volumes.Length; i++)
        {
            volumes[i] = -80;
        }
        curMusic = Musics.Cave_Calm;
    }

    public void SetMusic(Musics music)
    {
        curMusic = music;
    }

    void Update()
    {
        //INDEXES
        // 0 = Cave_Calm
        // 1 = Shopping

        if (curMusic == Musics.Cave_Calm)
            volumes[0] = Calc(volumes[0], true);
        else
            volumes[0] = Calc(volumes[0], false);

        if (curMusic == Musics.Shopping)
            volumes[1] = Calc(volumes[1], true);
        else
            volumes[1] = Calc(volumes[1], false);

        mixer.SetFloat("Volume_Cave_Calm", volumes[0]);
        mixer.SetFloat("Volume_Shopping", volumes[1]);
    }

    private float Calc(float input, bool up)
    {
        if (up)
            return Mathf.Clamp(input + interpolation * Time.deltaTime * goUpMultiply, -80, 0);
        else
            return Mathf.Clamp(input - interpolation * Time.deltaTime * goDownMultiply, -80, 0);
    }
}