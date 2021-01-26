using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Static access for other Classes to important Data
public class DataModel : MonoBehaviour
{
    public static DataModel Instance; //Reference to itself
    
    private List<BlueBell> _registeredBlueBells;
    private List<BlueBellUiElement> _blueBellUiElements;

    private void OnEnable()
    {
        Instance = this;
        _registeredBlueBells = new List<BlueBell>();
        _blueBellUiElements = new List<BlueBellUiElement>();
    }

    public void AddBlueBell(BlueBell blueBell)
    {
        _registeredBlueBells.Add(blueBell);
    } 
    public void RemoveBlueBell(BlueBell blueBell)
    {
        _registeredBlueBells.Remove(blueBell);
    }

    public List<BlueBell> GetBlueBells()
    {
        return _registeredBlueBells;
    }  
    
    public List<string> GetBlueBellIds()
    {
        List<string> blueBellIds = new List<string>();
        foreach (var blueBell in _registeredBlueBells)
        {
            blueBellIds.Add(blueBell.GetDeviceId());
        }
        return blueBellIds;
    }
    
    public void AddBlueBellUiElement(BlueBellUiElement blueBellUiElement)
    {
        _blueBellUiElements.Add(blueBellUiElement);
    } 
    public void RemoveBlueBellUiElement(BlueBellUiElement blueBellUiElement)
    {
        _blueBellUiElements.Remove(blueBellUiElement);
    }

    public List<BlueBellUiElement> GetBlueBellUiElements()
    {
        return _blueBellUiElements;
    }
    
    
}