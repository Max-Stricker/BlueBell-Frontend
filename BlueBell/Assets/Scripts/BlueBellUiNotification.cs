using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlueBellUiNotification : MonoBehaviour
{
    public Text timeText;

    public Image clockIcon;

    public Image bellIcon;

    public Image deleteIcon;


    // Start is called before the first frame update
    public void OnClick()
    {
        GetComponent<Image>().color = Color.white;
        timeText.color = Color.black;
        clockIcon.color = Color.black;
        bellIcon.color = Color.black;
        deleteIcon.color = Color.black;
    }

    public void DeleteButtonClick()
    {
        Destroy(gameObject);
    }
}