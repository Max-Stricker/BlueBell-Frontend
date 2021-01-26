using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Handel the HamburgerMenu in Top Right Corner
public class MenuHandler : MonoBehaviour
{
    public GameObject menuPanel;
    public GameObject menuButtonClose;
    public GameObject menuButtonOpen;

    public void CloseMenu()
    {
        menuButtonClose.SetActive(false);
        menuButtonOpen.SetActive(true);
        menuPanel.SetActive(false);
    }

    public void OpenMenu()
    {
        menuButtonClose.SetActive(true);
        menuButtonOpen.SetActive(false);
        menuPanel.SetActive(true);
    }
}