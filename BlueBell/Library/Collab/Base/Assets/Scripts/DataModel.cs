using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataModel : MonoBehaviour
{
    private List<BlueBell> _registeredBlueBells;

    private void Start()
    {
        _registeredBlueBells = new List<BlueBell>();
    }

    public void AddBlueBell(BlueBell blueBell)
    {
        _registeredBlueBells.Add(blueBell);
        
    } 
    private List<BlueBell> GetBlueBells()
    {
        return _registeredBlueBells;
    }
}