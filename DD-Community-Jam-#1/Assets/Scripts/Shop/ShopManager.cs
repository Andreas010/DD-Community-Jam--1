using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public static ShopManager instance;

    public ShopOption[] options;
    private int curIndex;
    private int oldIndex;
    public bool isOpen;
    private Animator animator;

    //UI stuff
    private TMP_Text description;

    private TMP_Text option1_name;
    private TMP_Text option2_name;
    private TMP_Text option3_name;

    private TMP_Text option1_price;
    private TMP_Text option2_price;
    private TMP_Text option3_price;

    private RawImage option1_icon;
    private RawImage option2_icon;
    private RawImage option3_icon;

    private Button upArrow;
    private Button downArrow;

    void Awake() => instance = this;

    void Start()
    {
        curIndex = 0;
        oldIndex = -1;
        animator = GetComponent<Animator>();

        description = transform.GetChild(1).GetChild(2).GetComponent<TMP_Text>();

        upArrow = transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<Button>();
        downArrow = transform.GetChild(1).GetChild(0).GetChild(1).GetComponent<Button>();

        option1_name = transform.GetChild(1).GetChild(1).GetChild(0).GetChild(1).GetComponent<TMP_Text>();
        option2_name = transform.GetChild(1).GetChild(1).GetChild(1).GetChild(1).GetComponent<TMP_Text>();
        option3_name = transform.GetChild(1).GetChild(1).GetChild(2).GetChild(1).GetComponent<TMP_Text>();

        option1_icon = transform.GetChild(1).GetChild(1).GetChild(0).GetChild(2).GetComponent<RawImage>();
        option2_icon = transform.GetChild(1).GetChild(1).GetChild(1).GetChild(2).GetComponent<RawImage>();
        option3_icon = transform.GetChild(1).GetChild(1).GetChild(2).GetChild(2).GetComponent<RawImage>();

        option1_price = transform.GetChild(1).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetComponent<TMP_Text>();
        option2_price = transform.GetChild(1).GetChild(1).GetChild(1).GetChild(0).GetChild(0).GetComponent<TMP_Text>();
        option3_price = transform.GetChild(1).GetChild(1).GetChild(2).GetChild(0).GetChild(0).GetComponent<TMP_Text>();

        UpdateUI();
    }

    public void Open()
    {
        isOpen = true;
        animator.SetBool("IsOpen", true);
    }

    public void Close()
    {
        isOpen = false;
        animator.SetBool("IsOpen", false);
    }

    public void ClickOption(int localIndex)
    {
        int newIndex = curIndex + localIndex;

        if (newIndex >= options.Length)
            return;

        description.text = options[newIndex].description;
        if(oldIndex == newIndex)
            options[newIndex].onBuy.Invoke();
        oldIndex = newIndex;
    }

    public void PressArrow(bool up)
    {
        if (up)
        {
            if (curIndex > 0)
                curIndex--;
        }
        else
        {
            if (curIndex < options.Length-3)
                curIndex++;
        }

        if (curIndex > 0)
            upArrow.interactable = true;
        else
            upArrow.interactable = false;

        if (curIndex < options.Length - 3)
            downArrow.interactable = true;
        else
            downArrow.interactable = false;

        UpdateUI();
    }

    void UpdateUI()
    {
        int newIndex = curIndex + 0;

        option1_name.text = options[newIndex].name;
        option1_price.text = options[newIndex].price.ToString();
        option1_icon.texture = options[newIndex].image;

        newIndex = curIndex + 1;

        option2_name.text = options[newIndex].name;
        option2_price.text = options[newIndex].price.ToString();
        option2_icon.texture = options[newIndex].image;

        newIndex = curIndex + 2;

        option3_name.text = options[newIndex].name;
        option3_price.text = options[newIndex].price.ToString();
        option3_icon.texture = options[newIndex].image;
    }
}
