using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.Notifications.Android;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

//Is attached to the BlueBell represented GameObject, to catch all corresponding Child GameObjects.  
public class BlueBellUiElement : MonoBehaviour
{
    private ConnectionHandler connectionHandler;
    private BlueBell _blueBell;
    private readonly string baseBlueBellURL = "https://mysterious-thicket-69990.herokuapp.com/";
    private DateTime lastDateTime;
    private DateTime lastUpdateSent;
    private int strikes = 0;

    public GameObject editButton; //Open InputField
    public Transform notificationScrollViewContent; //ScrollView for rings
    public GameObject notificationPrefab; //Ui Notification (No PushNotification)
    public GameObject connectionStatusActive; //GreenIcon
    public GameObject connectionStatusInactive; //RedIcon
    public GameObject overviewPanel;
    public GameObject settingsPanel;
    public Text nameText;
    public Text batteryNumber;
    public GameObject renamePanel;
    public GameObject namePanel;
    public GameObject deletePopUp;

    private void Start()
    {
        //Set up Push Notification
        AndroidNotificationChannel defaultNotificationChannel = new AndroidNotificationChannel()
        {
            Id = "default_channel",
            Name = "Default Channel",
            Description = "For Generic notification",
            Importance = Importance.Default,
        };
        AndroidNotificationCenter.RegisterNotificationChannel(defaultNotificationChannel);

        //Get the filled BlueBell Script from prefab, to know details for Ui Info
        _blueBell = GetComponent<BlueBell>();

        //Coroutine must called from other Script, to avoid errors, because this Script can be Destroyed 
        connectionHandler = FindObjectOfType<ConnectionHandler>();
        connectionHandler.CoroutineCallFromOutside(this); //Calls RepeatedRequestRoutine
    }

    public void ReName(Text newNameText)
    {
        if (newNameText.text != "")
        {
            //Rename Database
            StartCoroutine(PatchBlueBellCoroutine(_blueBell.GetDeviceId(), newNameText.text));

            //Rename UI and Script data
            var newName = newNameText.text;
            nameText.text = newName;
            _blueBell.name = newName;
        }
    }

    public void CloseReNameEditor()
    {
        editButton.SetActive(true);
        renamePanel.SetActive(false);
        namePanel.SetActive(true);
    }

    public void OpenReNameEditor()
    {
        editButton.SetActive(false);
        renamePanel.SetActive(true);
        namePanel.SetActive(false);
    }

    public void DeleteBlueBell()
    {
        deletePopUp.SetActive(false);
        StartCoroutine(DeleteBlueBellCoroutine(_blueBell.GetDeviceId())); //All Database calls in Coroutine 
    }

    IEnumerator DeleteBlueBellCoroutine(string _blueBellId)
    {
        string blueBellURL = baseBlueBellURL + "bluebells/bluebell/" + _blueBellId;

        UnityWebRequest blueBellRequest = UnityWebRequest.Delete(blueBellURL);

        yield return blueBellRequest.SendWebRequest();

        if (blueBellRequest.isNetworkError || blueBellRequest.isHttpError)
        {
            Debug.Log("OhOh " + blueBellRequest.error);
            yield break;
        }

        DataModel.Instance.RemoveBlueBell(_blueBell);
        DataModel.Instance.RemoveBlueBellUiElement(this);
        Destroy(this.gameObject);
    }

    IEnumerator PatchBlueBellCoroutine(string _blueBellId, string newName) //Rename
    {
        //Server
        string blueBellURL = baseBlueBellURL + "bluebells/bluebell/" + _blueBellId;

        string patchBody = "[{\"propName\": \"name\", \"value\": \"" + newName + "\"}]";

        var req = new UnityWebRequest(blueBellURL, "PATCH");

        byte[] data = Encoding.UTF8.GetBytes(patchBody);

        req.uploadHandler = new UploadHandlerRaw(data);
        req.uploadHandler.contentType = "application/json";
        req.downloadHandler = new DownloadHandlerBuffer();

        req.SetRequestHeader("Content-Type", "application/json");
        req.SetRequestHeader("accept", "application/json");

        yield return req.Send();

        if (req.isNetworkError || req.isHttpError)
        {
            Debug.Log("OhOh " + req.error);
            yield break;
        }

        //UI Update
        nameText.text = newName;
        _blueBell.name = newName;
    }

    //Is called every 5 Seconds, to Get all information of connected BlueBell (invoke Time is changeable at last yield statement)
    public IEnumerator RepeatedRequestRoutine(float timeOffset)
    {
        // No worries this still looks like a while loop (well it is ^^)
        // but the yield makes sure it doesn't block Unity's UI thread
        while (true)
        {
            // Configure your request
            var request = UnityWebRequest.Get(baseBlueBellURL + "bluebells/bluebell/" + _blueBell.GetDeviceId());

            // Make the request and wait until the it has finished (has a response or timeout)
            // a yield kind of tells this routine to leave, let Unity render this frame
            // and continue from here in the next frame
            yield return request.SendWebRequest();

            if (request.isNetworkError || request.isHttpError)
            {
                Debug.Log("OhOh " + request.error);
                yield break;
            }

            JSONNode blueBellInfo = JSON.Parse(request.downloadHandler.text);

            for (int i = 0; i < blueBellInfo.Count; i++)
            {
                //Get Information
                JSONNode blueBellObject = blueBellInfo[i];
                //To know if BlueBell is still alive (server changes every minut from hardware)
                DateTime updateLastSent;
                //From Server if someone rang
                DateTime dateLastRang;

                //Update Battery status in UI
                if (blueBellObject["batteryPercent"] != null)
                {
                    batteryNumber.text = blueBellObject["batteryPercent"];
                }

                // Update Active Status (Green or Red Icon)
                if (blueBellObject["lastUpdateSent"] != null)
                {
                    updateLastSent = System.DateTime.Parse(blueBellObject["lastUpdateSent"]);

                    // Bluebell is alive, if its a different input
                    if (updateLastSent == lastUpdateSent && lastUpdateSent != null)
                    {
                        //Same as last time, so something is wrong... Strike system, at to much strikes something is really wrong
                        strikes++;

                        print("BlueBell: " + _blueBell.GetName() + "| TimeoutStrikes: " + strikes);
                        if (strikes >= 3) //something is really wrong
                        {
                            connectionStatusInactive.SetActive(true);
                            connectionStatusActive.SetActive(false);
                            strikes = 0;
                        }
                    }
                    else // everything fine
                    {
                        connectionStatusInactive.SetActive(false);
                        connectionStatusActive.SetActive(true);
                        strikes = 0;
                    }

                    lastUpdateSent = updateLastSent; //To Check next Time
                }
                else
                {
                    connectionStatusInactive.SetActive(true);
                    connectionStatusActive.SetActive(false);
                }

                //Check for Ring
                if (blueBellObject["dateLastRang"] != null)
                {
                    dateLastRang = System.DateTime.Parse(blueBellObject["dateLastRang"]);

                    Debug.Log("dateLastRang: " + dateLastRang);

                    if (dateLastRang != lastDateTime && lastDateTime != null) //It rang!
                    {
                        //Prepare Push Notification
                        AndroidNotification androidNotification = new AndroidNotification()
                        {
                            Title = "Someone is at your Door!",
                            Text = "BlueBell: " + blueBellObject["name"] + "| Hour: " + dateLastRang.Hour +
                                   "| Minute: " + dateLastRang.Minute,
                            SmallIcon = "smallIcon",
                            LargeIcon = "largeIcon",
                            FireTime = System.DateTime.Now.AddSeconds(1),
                        };

                        //Fire Push Notification
                        AndroidNotificationCenter.SendNotification(androidNotification, "default_channel");

                        //Ui Notification
                        GameObject notification = Instantiate(notificationPrefab, notificationScrollViewContent);
                        notification.GetComponent<BlueBellUiNotification>().timeText.text = dateLastRang.ToString();

                        notification.transform.SetAsFirstSibling(); // To Set it on Top of ScrollView
                    }

                    lastDateTime = dateLastRang; // To Check next Time
                }

                // Now wait for the timeOffset
            }

            yield return new WaitForSeconds(timeOffset);
        }
    }
}