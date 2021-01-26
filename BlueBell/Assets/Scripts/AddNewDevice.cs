using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class AddNewDevice : MonoBehaviour
{

    [FormerlySerializedAs("uihandler")] [FormerlySerializedAs("panelLoader")] public UiHandler uiHandler;
    private bool deviceAccepted;

    async void CheckServerForDevice(string deviceID)
    {
        //TODO: Implement Backend
        deviceAccepted = true;
    }

    void AddDevice(string deviceID)
    {
        //TODO: Activate Loading Panel

        CheckServerForDevice(deviceID);

        if (deviceAccepted)
        {

            //TODO: Deactivate Loading Panel?
            uiHandler.LoadOverviewPanel();
        }
        else
        {
            //TODO: Error Meldung je nach Server Feedback
        }
    }
}
