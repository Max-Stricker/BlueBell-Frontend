using System;
using System.Collections;
using System.Collections.Generic;
using RestSharp;
using UnityEngine;
using UnityEngine.Networking;


public class BackendControler : MonoBehaviour
{

    private readonly string baseBlueBellURL = "localhost:3000/";


    public void GetBlueBell()
    {
        //var client = new RestClient("localhost:3000/bluebells/");
        //client.Timeout = -1;
        //var request = new RestRequest(Method.GET);
        //IRestResponse response = client.Execute(request);
        //Console.WriteLine(response.Content);

        StartCoroutine(GetBlueBells());

    }

    IEnumerator GetBlueBells()
    {
        string blueBellURL = baseBlueBellURL + "bluebells/";

        UnityWebRequest blueBellRequest = UnityWebRequest.Get(blueBellURL);

        yield return blueBellRequest.SendWebRequest();

        if(blueBellRequest.isNetworkError || blueBellRequest.isHttpError)
        {
            Debug.Log(blueBellRequest.error);
            yield break;
        }

        Debug.Log(blueBellRequest); 
    }
}
