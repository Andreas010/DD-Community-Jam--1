using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth instance;

    public int maxHealth;
    private int curHealth;

    public Transform healthCanvas;
    public Image healthRadial;
    public GameObject heartCanvasObject;

    public bool invincible;
    public bool godMode;

    private float curInvincibleTime;

    void Awake() => instance = this;

    void Start()
    {
        curHealth = maxHealth;
        curInvincibleTime = -1;
        UpdateHealthDisplay();
    }

    void Update()
    {
        if (curInvincibleTime > 0)
            curInvincibleTime -= Time.deltaTime;
    }

    public void ModifyHealth(int value)
    {
        if (godMode || curInvincibleTime >= 0)
            return;

        curHealth += value;
        if (curHealth > maxHealth)
            curHealth = maxHealth;

        UpdateHealthDisplay();

        if (curHealth <= 0)
            Kill();
    }

    public void MakeInvincible(float time)
    {
        if(curInvincibleTime <= 0)
            curInvincibleTime = time;
    }

    public void SetInvincible(float time)
    {
        curInvincibleTime = time;
    }

    public void Kill()
    {
        SimpleLoadScene.LoadScene("Died");
    }

    public void Heal()
    {
        curHealth = maxHealth;
    }

    public void UpdateHealthDisplay()
    {
        for (int i = 0; i < healthCanvas.transform.childCount; i++)
        {
            Destroy(healthCanvas.transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < curHealth; i++)
        {
            Instantiate(heartCanvasObject, healthCanvas);
        }

        healthRadial.fillAmount = (maxHealth - curHealth) / (float)maxHealth;
        healthRadial.transform.parent.GetComponent<Image>().fillAmount = curHealth / (float)maxHealth;
    }

    public void UpgradeHealth(int count)
    {
        maxHealth += count;
        UpdateHealthDisplay();
    }
}
