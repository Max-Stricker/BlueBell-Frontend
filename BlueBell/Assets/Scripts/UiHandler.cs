using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UiHandler : MonoBehaviour
{
    public GameObject dataProtectionPanel;
    public GameObject settingsPanel;
    public GameObject overviewPanel;
    public GameObject diagnosePanel;
    public GameObject contactPanel;
    public GameObject connectionPanel;
    public GameObject addPanel;
    public GameObject errorPanel;
    public GameObject loadingPanel;
    public GameObject scrollView;
    public Dropdown diagnoseDropdownBlueBells;

    public void LoadDataProtectionPanel()
    {
        DisableAllPanels();
        dataProtectionPanel.SetActive(true);
    }

    public void LoadOverviewPanel()
    {
        DisableAllPanels();
        overviewPanel.SetActive(true);
        scrollView.SetActive(true);
        foreach (var blueBellUiElement in DataModel.Instance.GetBlueBellUiElements())
        {
            blueBellUiElement.overviewPanel.SetActive(true);
            blueBellUiElement.settingsPanel.SetActive(false);
        }
    }

    public void LoadSettingsPanel()
    {
        DisableAllPanels();
        settingsPanel.SetActive(true);
        scrollView.SetActive(true);
        foreach (var blueBellUiElement in DataModel.Instance.GetBlueBellUiElements())
        {
            blueBellUiElement.overviewPanel.SetActive(false);
            blueBellUiElement.settingsPanel.SetActive(true);
        }
    }

    
    public void LoadDiagnosePanel()
    {
                
        DisableAllPanels();
        diagnosePanel.SetActive(true);
        
        Text text;
        diagnoseDropdownBlueBells.options.Clear();
        foreach (string blueBellId in DataModel.Instance.GetBlueBellIds())
        {
            diagnoseDropdownBlueBells.options.Add(new Dropdown.OptionData(){ text = blueBellId });
        }

    }
    public void LoadContactPanel()
    {
        DisableAllPanels();
        contactPanel.SetActive(true);
    }

    public void LoadConnectionPanel()
    {
        addPanel.SetActive(true);
        errorPanel.SetActive(false);
        loadingPanel.SetActive(false);
        connectionPanel.SetActive(true);
    }
    
    private void DisableAllPanels()
    {
        dataProtectionPanel.SetActive(false);
        settingsPanel.SetActive(false);
        overviewPanel.SetActive(false);
        contactPanel.SetActive(false);
        diagnosePanel.SetActive(false);
        addPanel.SetActive(false);
        scrollView.SetActive(false);
    }
 
    
    
    public void ShowLoadingPanel()
    {
        loadingPanel.SetActive(true);
        connectionPanel.SetActive(false);
    }

    public void ShowConnectionPanel()
    {
        loadingPanel.SetActive(false);
        connectionPanel.SetActive(true);
    }

}