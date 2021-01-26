//using System;
//using System.Collections;
//using System.Collections.Generic;
//using RestSharp;
//using UnityEngine;
//using UnityEngine.Networking;
//using SimpleJSON;
//
//
//public class BackendController : MonoBehaviour
//{
//    public static BackendControler Instance;
//    private readonly string baseBlueBellURL = "localhost:3000/";
//    private List<BlueBellScript> _blueBells;
//    private bool deleted = false;
//    public bool posted = false;
//
//    private void OnEnable()
//    {
//        Instance = this;
//        _blueBells = new List<BlueBellScript>();
//    }
//
//    public List<BlueBellScript> GetBlueBells()
//    {
//        StartCoroutine(GetBlueBellsCoroutine());
//        return _blueBells;
//    }
//
//    IEnumerator GetBlueBellsCoroutine()
//    {
//        string blueBellURL = baseBlueBellURL + "bluebells/";
//
//        UnityWebRequest blueBellRequest = UnityWebRequest.Get(blueBellURL);
//
//        yield return blueBellRequest.SendWebRequest();
//
//        if (blueBellRequest.isNetworkError || blueBellRequest.isHttpError)
//        {
//            Debug.Log("OhOh " + blueBellRequest.error);
//            yield break;
//        }
//
//        JSONNode blueBellInfo = JSON.Parse(blueBellRequest.downloadHandler.text);
//
//        string name;
//        string _id;
//        string blueBellId;
//        int batteryPercent;
//
//        for (int i = 0; i < blueBellInfo.Count; i++)
//        {
//            JSONNode blueBellObject = blueBellInfo[i];
//
//            name = blueBellObject["name"];
//            blueBellId = blueBellObject["blueBellId"];
//            batteryPercent = blueBellObject["batteryPercent"];
//
//            Debug.Log("Name: " + name + "    ID: " + blueBellId + "    BatteryPercent: " + batteryPercent);
//
//            BlueBellScript blueBell = new BlueBellScript();
//            blueBell.SetName(name);
//            blueBell.SetDeviceId(blueBellId);
//            blueBell.SetBattery(batteryPercent);
//            if (!_blueBells.Contains(blueBell))
//            {
//                _blueBells.Add(blueBell);
//            }
//        }
//    }
//
//    public void PostBlueBell(string blueBellName, string blueBellId)
//    {
//        StartCoroutine(PostBlueBellCoroutine(blueBellName, blueBellId));
//    }
//
//    IEnumerator PostBlueBellCoroutine(string _blueBellName, string _blueBellId)
//    {
//        string blueBellURL = baseBlueBellURL + "bluebells/";
//
//        WWWForm form = new WWWForm();
//
//        form.AddField("name", _blueBellName);
//        form.AddField("id", _blueBellId);
//
//        UnityWebRequest blueBellRequest = UnityWebRequest.Post(blueBellURL, form);
//
//        yield return blueBellRequest.SendWebRequest();
//
//        if (blueBellRequest.isNetworkError || blueBellRequest.isHttpError)
//        {
//            Debug.Log("OhOh " + blueBellRequest.error);
//            posted = false;
//            yield break;
//        }
//
//        posted = true;
//        
//        JSONNode blueBellInfo = JSON.Parse(blueBellRequest.downloadHandler.text);
//
//        string name;
//        string _id;
//        string blueBellId;
//        int batteryPercent;
//
//        for (int i = 0; i < blueBellInfo.Count; i++)
//        {
//            JSONNode blueBellObject = blueBellInfo[i];
//
//            name = blueBellObject["name"];
//            blueBellId = blueBellObject["blueBellId"];
//            batteryPercent = blueBellObject["batteryPercent"];
//
//            Debug.Log("Name: " + name + "    ID: " + blueBellId + "    BatteryPercent: " + batteryPercent);
//        }
//        
//    }
//
//    public bool DeleteBlueBell(string blueBellId)
//    {
//        StartCoroutine(DeleteBlueBellCoroutine(blueBellId));
//        return deleted;
//    }
//
//    IEnumerator DeleteBlueBellCoroutine(string _blueBellId)
//    {
//        string blueBellURL = baseBlueBellURL + "bluebells/bluebell/" + _blueBellId;
//
//
//        UnityWebRequest blueBellRequest = UnityWebRequest.Delete(blueBellURL);
//
//        yield return blueBellRequest.SendWebRequest();
//
//        if (blueBellRequest.isNetworkError || blueBellRequest.isHttpError)
//        {
//            Debug.Log("OhOh " + blueBellRequest.error);
//            deleted = false;
//            yield break;
//        }
//
//        deleted = true;
//    }
//}