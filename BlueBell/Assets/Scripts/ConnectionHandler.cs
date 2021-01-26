using System;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

/*
 * Communicates with Server and create Ui Elements for every BlueBell found on Server at the App start.
 * Also create new BlueBells and add to the Server
 */

//TODO: Shift UI to UiHandler
public class ConnectionHandler : MonoBehaviour
{
    public GameObject blueBellUiElementPrefab;
    public Transform scrollViewContent; //Place new BlueBell UiElement
    public UiHandler uiHandler;

    //New BlueBell
    public InputField idInputField;
    public InputField nameInputField;

    //Panels
    public GameObject errorPanel;
    public GameObject startPanel;
    public GameObject mainPanel;

    private readonly string baseBlueBellURL = "https://mysterious-thicket-69990.herokuapp.com/";
    private IEnumerator co; //Able to reference a Coroutine, to make it possible to Stop Coroutine

    private void Start()
    {
        // Ask Server for all existing BlueBells, to create for each of one a UI Element
        StartCoroutine(GenerateAllBlueBellUiElementsFromDbBlueBells());
    }

    IEnumerator GenerateAllBlueBellUiElementsFromDbBlueBells()
    {
        string blueBellURL = baseBlueBellURL + "bluebells/";

        UnityWebRequest blueBellRequest = UnityWebRequest.Get(blueBellURL);

        yield return blueBellRequest.SendWebRequest();

        if (blueBellRequest.isNetworkError || blueBellRequest.isHttpError)
        {
            Debug.Log("OhOh " + blueBellRequest.error);
            yield break;
        }

        JSONNode blueBellInfo = JSON.Parse(blueBellRequest.downloadHandler.text);

        Debug.Log(blueBellInfo);

        //All BlueBells Information 
        string name;
        string _id;
        string blueBellId;
        int batteryPercent;

        for (int i = 0; i < blueBellInfo.Count; i++)
        {
            JSONNode blueBellObject = blueBellInfo[i];

            name = blueBellObject["name"];
            blueBellId = blueBellObject["blueBellId"];
            batteryPercent = blueBellObject["batteryPercent"];

            Debug.Log("Name: " + name + "    ID: " + blueBellId + "    BatteryPercent: " + batteryPercent);

            //Create new BlueBell UI with all Data from Server
            var newBlueBellUiElementGameObject = Instantiate(blueBellUiElementPrefab, scrollViewContent);

            var newBlueBellScript =
                newBlueBellUiElementGameObject
                    .GetComponent<BlueBell>(); //Prefab does have BlueBell Script attached, to store BlueBell Server Data
            newBlueBellScript.SetDeviceId(blueBellId);
            newBlueBellScript.SetName(name);
            newBlueBellScript.SetBattery(batteryPercent);

            DataModel.Instance.AddBlueBell(newBlueBellScript); //Additional store BlueBell at DataModel

            //Edit Ui 
            var newBlueBellUiElementScript = newBlueBellUiElementGameObject.GetComponent<BlueBellUiElement>();
            newBlueBellUiElementScript.nameText.text = newBlueBellScript.GetName();
            newBlueBellUiElementScript.batteryNumber.text = newBlueBellScript.GetBattery().ToString();

            DataModel.Instance.AddBlueBellUiElement(
                newBlueBellUiElementScript); //Additional store UiElement at DataModel
        }
    }

    public void ConnectionButtonClick()
    {
        if (!CheckWlanConnection()) return;
        if (!CheckIdInputField()) return;
        uiHandler.ShowLoadingPanel();
        TryConnect();
    }

    //TODO: Check for internet Connection
    private bool CheckWlanConnection()
    {
        return true;
//        if ()
//        {
//            errorPanel.SetActive(false);
//        }
//        else
//        {
//            errorPanel.SetActive(true);
//        }
    }

    private bool CheckIdInputField()
    {
        return idInputField.text != "";
        //TODO: Noch bessere Abfrage machen, ist die Form der eingebebenen Id überhaupt richtig (kann das eine richtige id sein)
    }

    public void StopConnecting()
    {
        StopCoroutine(co);
        uiHandler.ShowConnectionPanel();
    }

    private void TryConnect()
    {
        co = PostBlueBellCoroutine(nameInputField.text, idInputField.text);
        StartCoroutine(co);
    }

    //Create new BlueBell and add to Server
    //Closely the same from "GenerateAllBlueBellUiElementsFromDbBlueBells" but just for one BlueBell and add Data to Server
    IEnumerator PostBlueBellCoroutine(string _blueBellName, string _blueBellId)
    {
        string blueBellURL = baseBlueBellURL + "bluebells/";

        WWWForm form = new WWWForm();

        form.AddField("name", _blueBellName);
        form.AddField("id", _blueBellId);

        UnityWebRequest blueBellRequest = UnityWebRequest.Post(blueBellURL, form); //Add to Server

        yield return blueBellRequest.SendWebRequest();

        if (blueBellRequest.isNetworkError || blueBellRequest.isHttpError)
        {
            Debug.Log("OhOh " + blueBellRequest.error);
            yield break;
        }

        JSONNode blueBellInfo = JSON.Parse(blueBellRequest.downloadHandler.text);

        string name = null;
        string _id;
        string blueBellId = null;
        int batteryPercent = 100;

        for (int i = 0; i < blueBellInfo.Count; i++)
        {
            JSONNode blueBellObject = blueBellInfo[i];

            name = blueBellObject["name"];
            blueBellId = blueBellObject["blueBellId"];
            batteryPercent = blueBellObject["batteryPercent"];

            Debug.Log("Name: " + name + "    ID: " + blueBellId + "    BatteryPercent: " + batteryPercent);
        }

        var newBlueBellUiElementGameObject = Instantiate(blueBellUiElementPrefab, scrollViewContent);

        var newBlueBellScript = newBlueBellUiElementGameObject.GetComponent<BlueBell>();
        newBlueBellScript.SetDeviceId(blueBellId);
        newBlueBellScript.SetName(name);

        newBlueBellScript.SetConnectionStatus(true);
        newBlueBellScript.SetBattery(batteryPercent);

        DataModel.Instance.AddBlueBell(newBlueBellScript);

        var newBlueBellUiElementScript = newBlueBellUiElementGameObject.GetComponent<BlueBellUiElement>();
        newBlueBellUiElementScript.nameText.text = newBlueBellScript.GetName();
        newBlueBellUiElementScript.batteryNumber.text = newBlueBellScript.GetBattery().ToString();


        DataModel.Instance.AddBlueBellUiElement(newBlueBellUiElementScript);

        uiHandler.LoadSettingsPanel();
    }
    
    //Just for BlueBellUiElement
    // weil coroutine sonst gestoppt wird, wenn sie vom übergebenen script direct aufgerufen wird //TODO: refactor
    public void CoroutineCallFromOutside(BlueBellUiElement bellUiElement)
    {
        StartCoroutine(bellUiElement.RepeatedRequestRoutine(5));
    }
}