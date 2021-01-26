using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueBell : MonoBehaviour
{
    private string _deviceId;
    private string _name;
    private int _battery;
    private bool _connected;

    public string GetDeviceId()
    {
        return _deviceId;
    }

    public void SetDeviceId(string i)
    {
        _deviceId = i;
    }  
    public string GetName()
    {
        return _name;
    }

    public void SetName(string i)
    {
        _name = i;
    }

    public int GetBattery()
    {
        return _battery;
    }

    public void SetBattery(int i)
    {
        _battery = i;
    }

    public bool GetConnectionStatus()

    {
        return _connected;
    }

    public void SetConnectionStatus(bool b)
    {
        _connected = b;
    }
}