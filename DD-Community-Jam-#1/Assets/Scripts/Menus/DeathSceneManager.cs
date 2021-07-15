using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathSceneManager : MonoBehaviour
{
    public void Revive()
    {
        SimpleLoadScene.LoadScene("SampleScene");
    }
}
