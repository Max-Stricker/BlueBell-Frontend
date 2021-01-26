using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.Utils;
using SimpleJSON;
using TMPro;
using UnityEngine.Networking;

//Graph for Diagnostics 
public class WindowGraph : MonoBehaviour
{
    private readonly string baseBlueBellURL = "https://mysterious-thicket-69990.herokuapp.com/";
    private List<GameObject> _gameObjectsToDelete; // To delete old Graph, if a new one is Drawn

    [SerializeField] private Sprite circleSprite;
    private RectTransform graphContainer;

    //X axis of Graph (last 7 Days of LASTRing)
    public TextMeshProUGUI weekDay1;
    public TextMeshProUGUI weekDay2;
    public TextMeshProUGUI weekDay3;
    public TextMeshProUGUI weekDay4;
    public TextMeshProUGUI weekDay5;
    public TextMeshProUGUI weekDay6;
    public TextMeshProUGUI weekDay7;

    private void Awake()
    {
        _gameObjectsToDelete = new List<GameObject>();
        graphContainer = transform.Find("graphContainer").GetComponent<RectTransform>();

        //Default last 7 Days, because someBells haven't documented Rings
        weekDay1.text = DateTime.Today.ToString("dd/MM/yyyy");
        weekDay2.text = DateTime.Today.AddDays(-1).ToString("dd/MM/yyyy");
        weekDay3.text = DateTime.Today.AddDays(-2).ToString("dd/MM/yyyy");
        weekDay4.text = DateTime.Today.AddDays(-3).ToString("dd/MM/yyyy");
        weekDay5.text = DateTime.Today.AddDays(-4).ToString("dd/MM/yyyy");
        weekDay6.text = DateTime.Today.AddDays(-5).ToString("dd/MM/yyyy");
        weekDay7.text = DateTime.Today.AddDays(-6).ToString("dd/MM/yyyy");
    }


    public void ShowDiagnose(Text blueBell) //Text from chosen BlueBell from UI Dropdown 
    {
        StartCoroutine(GetAllDatesAndShowDiagnose(blueBell.text)); //Coroutine, because we need Data from Server
    }

    private GameObject CreateCircle(Vector2 anchoredPosition)
    {
        GameObject gameObject = new GameObject("circle", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().sprite = circleSprite;
        gameObject.GetComponent<Image>().color = new Color(0.9764706f, 0.6313726f, 0.282353f);
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = new Vector2(20, 20);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);

        return gameObject;
    }

    private void ShowGraph(List<int> valueList)
    {
        float graphHeight = graphContainer.sizeDelta.y;
        float yMaximum = 10f;
        float xSize = graphContainer.sizeDelta.x / 8f;

        GameObject lastCircleGameObject = null;
        for (int i = 0;
            i < valueList.Count;
            i++)
        {
            float xPosition = xSize + i * xSize;
            float yPosition = (valueList[i] / yMaximum) * graphHeight;

            GameObject circleGameObject = CreateCircle(new Vector2(xPosition, yPosition));
            if (lastCircleGameObject != null)
            {
                CreateDotConnection(lastCircleGameObject.GetComponent<RectTransform>().anchoredPosition,
                    circleGameObject.GetComponent<RectTransform>().anchoredPosition);
            }

            _gameObjectsToDelete.Add(circleGameObject);

            lastCircleGameObject = circleGameObject;
        }
    }

    private void CreateDotConnection(Vector2 dotPositionA, Vector2 dotPositionB)
    {
        GameObject gameObject = new GameObject("dotConnection", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().color = new Color(0.9764706f, 0.6313726f, 0.282353f, 0.5f);
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        Vector2 dir = (dotPositionB - dotPositionA).normalized;
        float distance = Vector2.Distance(dotPositionA, dotPositionB);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.sizeDelta = new Vector2(distance, 3f);
        rectTransform.anchoredPosition = dotPositionA + dir * distance * 0.5f;
        rectTransform.localEulerAngles = new Vector3(0, 0, UtilsClass.GetAngleFromVectorFloat(dir));

        _gameObjectsToDelete.Add(gameObject);
    }


    //Get Ring Array from Server and calculate rings per day
    IEnumerator GetAllDatesAndShowDiagnose(string blueBellIdForDiagnose)
    {
        string blueBellURL = baseBlueBellURL + "bluebells/";
        var blueBellRequest = UnityWebRequest.Get(baseBlueBellURL + "bluebells/bluebell/" + blueBellIdForDiagnose);
        yield return blueBellRequest.SendWebRequest();
        if (blueBellRequest.isNetworkError || blueBellRequest.isHttpError)
        {
            Debug.Log("OhOh " + blueBellRequest.error);
            Debug.Log(baseBlueBellURL + "bluebells/bluebell/" + "333");

            yield break;
        }

        JSONNode blueBellInfo = JSON.Parse(blueBellRequest.downloadHandler.text);

        DateTime[] allDates = null;
        for (int i = 0;
            i < blueBellInfo.Count;
            i++)
        {
            JSONNode blueBellObject = blueBellInfo[i];

            allDates = new DateTime[blueBellObject["allDates"].Count];

            Debug.Log("All Dates: ");

            for (int index = 0; index < blueBellObject["allDates"].Count; index++)
            {
                allDates[index] = System.DateTime.Parse(blueBellObject["allDates"][index]);
                Debug.Log(allDates[index]);
                Debug.Log("|");
            }
        }

        //Calculate rings per day
        Dictionary<int, int> ringsPerDay = new Dictionary<int, int>();//first int: Day | Second int: rings per day

        foreach (var date in allDates)
        {
            //If a Day is already in the Diagnostic List, count for this specific day, else add a new day to the list
            if (ringsPerDay.ContainsKey(date.Day))
            {
                if (ringsPerDay[date.Day] + 1 < 10) //10 Upper Limit for Diagnostic Graph //TODO: Dynamic
                {
                    ringsPerDay[date.Day]++;
                }
            }
            else
            {
                ringsPerDay.Add(date.Day, 1);
            }
        }

        List<int> valueList = new List<int>(); //values for Graph drawing 
        for (int day = DateTime.Now.Day - 6; day <= DateTime.Now.Day; day++) // 6 Because of 1 Week 
        {
            //if rang, add the value of rings for selected day to value list
            if (ringsPerDay.ContainsKey(day))
            {
                valueList.Add(ringsPerDay[day]);
            }
            else
            {
                valueList.Add(0);
            }
        }

        //Start Diagnostic x Axis from last ring entry else, took last 7 Days
        if (allDates.Any())
        {
            weekDay1.text = allDates.Last().Date.ToString("dd/MM/yyyy");
            weekDay2.text = allDates.Last().Date.AddDays(-1).AddDays(-1).ToString("dd/MM/yyyy");
            weekDay3.text = allDates.Last().Date.AddDays(-2).ToString("dd/MM/yyyy");
            weekDay4.text = allDates.Last().Date.AddDays(-3).ToString("dd/MM/yyyy");
            weekDay5.text = allDates.Last().Date.AddDays(-4).ToString("dd/MM/yyyy");
            weekDay6.text = allDates.Last().Date.AddDays(-5).ToString("dd/MM/yyyy");
            weekDay7.text = allDates.Last().Date.AddDays(-6).ToString("dd/MM/yyyy");
        }
        else
        {
            weekDay1.text = DateTime.Today.ToString("dd/MM/yyyy");
            weekDay2.text = DateTime.Today.AddDays(-1).ToString("dd/MM/yyyy");
            weekDay3.text = DateTime.Today.AddDays(-2).ToString("dd/MM/yyyy");
            weekDay4.text = DateTime.Today.AddDays(-3).ToString("dd/MM/yyyy");
            weekDay5.text = DateTime.Today.AddDays(-4).ToString("dd/MM/yyyy");
            weekDay6.text = DateTime.Today.AddDays(-5).ToString("dd/MM/yyyy");
            weekDay7.text = DateTime.Today.AddDays(-6).ToString("dd/MM/yyyy");
        }

        //Destroy old Graph
        for (int i = _gameObjectsToDelete.Count - 1; i >= 0; i--)
        {
            Destroy(_gameObjectsToDelete[i].gameObject);
        }

        _gameObjectsToDelete = new List<GameObject>();
        
        //Draw Graph
        ShowGraph(valueList);
    }
}