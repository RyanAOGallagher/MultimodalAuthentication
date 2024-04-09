using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Diagnostics;
using System.Text;
using System.IO;

public class UIManager: MonoBehaviour {

  const string PASSWORD = "3579";

  private Stopwatch stopwatch = new Stopwatch();
  int correct = 0;
  int attempts = 0;

  public NetworkManager nm;
  public Transform TL;
  public Transform TR;
  public Transform BL;
  public Transform BR;
  public GameObject buttonPrefab;
  public TextMeshProUGUI userEntryText; // TextMeshPro field

  public string userEntry = "";
  public string quadrant = "";
  public string topLeft;
  public string topRight;
  public string bottomLeft;
  public string bottomRight;

  public Transform one;
  public Transform two;
  public Transform three;
  public Transform four;
  public Transform five;
  public Transform six;
  public Transform seven;
  public Transform eight;
  public Transform nine;
  public Transform ten;
  public Transform eleven;
  public Transform twelve;


  public List < string > elementsList = new List < string > ();

  private string previousData;
  private string prevRem;
  string prevList;
  public bool firstAttempt = true;

  private List < GameObject > instantiatedButtons = new List < GameObject > ();
  private List < GameObject > instantiatedInnerButtons = new List < GameObject > ();
  public List <float> times = new List <float>();
  public List <string> results = new List <string>();

  public GameObject loadText;
  public GameObject goText;
  public GameObject breakText;
  public GameObject finishedText;


  public string userInput = "";
  public List < string > userInputList = new List < string > ();
  public List < GameObject > UIElements = new List < GameObject > ();
  public List < string > elementsListList = new List < string > ();


  void Update() {

    string latestData = nm.GetLatestData();
    if (!string.IsNullOrEmpty(latestData)) {
      bool shouldUpdateUI = false;

      if (latestData.StartsWith("[")) {
        if (!stopwatch.IsRunning)
            {
                stopwatch.Start();
                
                loadText.SetActive(false);
            }

       
  if (prevList != latestData){
        elementsListList.Add(latestData);
        elementsList.Clear();
        AddElementsToList(latestData);
        shouldUpdateUI = true;
        goText.SetActive(true);

     
 if (!firstAttempt){
  
 
    if (userEntry.Length >= 0) {
         if (userEntry == PASSWORD) {
          correct++;
          results.Add("correct");
        } else {
          results.Add("incorrect");
        }
        userInputList.Add(userInput);

          UnityEngine.Debug.Log("Submitted!");
          attempts++;
          UnityEngine.Debug.Log(correct + "/" + attempts);
            UnityEngine.Debug.Log($"Time taken for this attempt: {stopwatch.ElapsedMilliseconds} milliseconds");
            times.Add(stopwatch.ElapsedMilliseconds);
            // Reset the stopwatch for the next entry
   
      
            stopwatch.Stop(); // Stop the stopwatch before break
            StartCoroutine(BreakCoroutine()); // Start the break coroutine
           
        
     
        }

        userEntry = "";
        userInput = "";

      } else if (elementsList.Count == 0) {
        AddElementsToList(latestData);
        shouldUpdateUI = true; 
      }

      if (shouldUpdateUI) {
       
        UpdateUI(); 

      }
      
      prevList = latestData;
        }
        firstAttempt = false;
      }
 if (attempts == 5){
    finishedText.SetActive(true);
    goText.SetActive(false);
    breakText.SetActive(false);
  }
      switch (latestData) {

      case "OK":
       
        goText.SetActive(false);
        elementsList.Clear();
        userEntry = "";
        attempts++;
        quadrant = "";
        if (instantiatedButtons.Count > 0) {
          foreach(GameObject button in instantiatedButtons) {
            Destroy(button);
          }
          instantiatedButtons.Clear();
        }

        if (userEntry == PASSWORD.ToString()) {
          correct++;
        }

        break;
      case "<-":
        if (!string.IsNullOrEmpty(userEntry)) {
          if (prevRem != latestData) {
            prevRem = latestData;
            userEntry = userEntry.Substring(0, userEntry.Length - 1);
          }
         

   
        }
         if (latestData != prevList){
          userInput = userInput+ "D";
          UpdateUI();
          }
         prevList = latestData;
        break;

      case "cancel":
      if (latestData != prevList){
        userInput = userInput+ "C";
          UpdateUI();
          }
          prevList = latestData;
        break;

      default:
        if (latestData.Contains("quadrant")) {
          previousData = latestData;
          prevRem = latestData;
          prevList = latestData;

          if (quadrant == "") {
            quadrant = latestData[latestData.Length - 1] + "";
                      userInput = userInput+ "Q"+quadrant;
            if (quadrant == "3") {
              topLeft = "";
              topRight = elementsList[10];
              bottomRight = elementsList[9];
              bottomLeft = elementsList[8];
            } else if (quadrant == "0") {
              topLeft = elementsList[11];
              topRight = "";
              bottomRight = elementsList[1];
              bottomLeft = elementsList[0];
            } else if (quadrant == "1") {
              topLeft = elementsList[3];
              topRight = elementsList[2];
              bottomRight = "";
              bottomLeft = elementsList[4];
            } else if (quadrant == "2") {
              topLeft = elementsList[7];
              topRight = elementsList[6];
              bottomRight = elementsList[5];
              bottomLeft = "";
            }
             if (instantiatedButtons.Count > 0) {
      foreach(GameObject button in instantiatedButtons) {
        Destroy(button);
      }
      instantiatedButtons.Clear();
    }
            CreateInnerButtonAtPosition(TL, topLeft);
            CreateInnerButtonAtPosition(TR, topRight);
            CreateInnerButtonAtPosition(BR, bottomRight);
            CreateInnerButtonAtPosition(BL, bottomLeft);
          }
        } else if (int.TryParse(latestData, out int number)) {
          if (previousData != latestData) {
            previousData = latestData;
            if (userEntry.Length <4){
            userEntry += number.ToString();

         
            }
            userInput = userInput+ number.ToString();
            UpdateUI();
          }

        }
        break;
      }
    }

  }

  void AddElementsToList(string data) {
    string[] tempArray = data.Trim(new char[] {
      '[',
      ']'
    }).Split(',');
    foreach(string element in tempArray) {
      elementsList.Add(element.Trim().Trim(new char[] {
        '\'',
        '"'
      }));
    }

  }

  void CreateButtonAtPosition(Transform position, string buttonText) {
    GameObject buttonObj = Instantiate(buttonPrefab, position.position, Quaternion.identity, position);
    instantiatedButtons.Add(buttonObj);
    Text textComponent = buttonObj.GetComponentInChildren <Text> ();
    if (textComponent != null) {
      textComponent.text = buttonText;
    }
  }

  void CreateInnerButtonAtPosition(Transform position, string buttonText) {
    GameObject buttonObj = Instantiate(buttonPrefab, position.position, Quaternion.identity, position);
    instantiatedInnerButtons.Add(buttonObj);
    Text textComponent = buttonObj.GetComponentInChildren <Text> ();
    if (textComponent != null) {
      textComponent.text = buttonText;
    }
  }

  void UpdateUI() {
      UnityEngine.Debug.Log("UI update");

        CreateButtonAtPosition(one, elementsList[0]);
        CreateButtonAtPosition(two, elementsList[1]);
        CreateButtonAtPosition(three, elementsList[2]);
        CreateButtonAtPosition(four, elementsList[3]);
        CreateButtonAtPosition(five, elementsList[4]);
        CreateButtonAtPosition(six, elementsList[5]);
        CreateButtonAtPosition(seven, elementsList[6]);
        CreateButtonAtPosition(eight, elementsList[7]);
        CreateButtonAtPosition(nine, elementsList[8]);
        CreateButtonAtPosition(ten, elementsList[9]);
        CreateButtonAtPosition(eleven, elementsList[10]);
        CreateButtonAtPosition(twelve, elementsList[11]);
    
    if (userEntryText != null) {
      userEntryText.text = userEntry;
    }
    quadrant = "";
    if (instantiatedInnerButtons.Count > 0) {
      foreach(GameObject button in instantiatedInnerButtons) {
        Destroy(button);
      }
      instantiatedInnerButtons.Clear();
    }

  }





private IEnumerator BreakCoroutine()
{
    UnityEngine.Debug.Log("Starting 3-second break...");
    breakText.SetActive(true);
    goText.SetActive(false);
    foreach(GameObject UIElement in UIElements) {
    UIElement.SetActive(false);
    }
    yield return new WaitForSeconds(3f);
    UnityEngine.Debug.Log("Break over, resuming...");
     foreach(GameObject UIElement in UIElements) {
    UIElement.SetActive(true);
    }
    breakText.SetActive(false);
    goText.SetActive(true);
    stopwatch.Reset();
    stopwatch.Start();
}

void OnApplicationQuit()
    {
        SaveDataToCSV();
    }

    void SaveDataToCSV()
    {
        StringBuilder csvContent = new StringBuilder();

        // Assuming times and results have the same count
        for (int i = 0; i < times.Count; i++)
        {
            csvContent.AppendLine(times[i] + "," + results[i] + "," + userInputList[i] + ',' + elementsListList[i]);
        }

        string filePath = Path.Combine(Application.persistentDataPath, "data.csv");
        File.WriteAllText(filePath, csvContent.ToString());

        UnityEngine.Debug.Log("Data saved to " + filePath);
    }
    

}