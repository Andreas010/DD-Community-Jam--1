using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuStartButton : MonoBehaviour
{
    public void Yes()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }
}
