using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;

public class UIManager: MonoBehaviour {

  const string PASSWORD = "3579";


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

  public List < string > elementsList = new List < string > ();

  private string previousData;
  private string prevRem;

  private List < GameObject > instantiatedButtons = new List < GameObject > ();



  void Update() {

    string latestData = nm.GetLatestData();
    if (!string.IsNullOrEmpty(latestData)) {
      bool shouldUpdateUI = false;

      if (latestData.StartsWith("[")) {
        elementsList.Clear();
        AddElementsToList(latestData);
        shouldUpdateUI = true; 
        if (userEntry.Length > 0) {
          Debug.Log("Submitted!");
        }


        if (userEntry == PASSWORD) {
          Debug.Log("Correct");
        }

        userEntry = "";

      } else if (elementsList.Count == 0) {
        AddElementsToList(latestData);
        shouldUpdateUI = true; 
      }

      if (shouldUpdateUI) {
        UpdateUI(); 

      }

      switch (latestData) {

      case "OK":

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
          UpdateUI();
        }
        break;

      case "cancel":
        UpdateUI();
        break;

      default:
        if (latestData.Contains("quadrant")) {
          previousData = latestData;
          prevRem = latestData;

          if (quadrant == "") {
            quadrant = latestData[latestData.Length - 1] + "";
            if (quadrant == "3") {
              topLeft = "";
              topRight = elementsList[8];
              bottomRight = elementsList[9];
              bottomLeft = elementsList[10];
            } else if (quadrant == "0") {
              topLeft = elementsList[1];
              topRight = "";
              bottomRight = elementsList[11];
              bottomLeft = elementsList[0];
            } else if (quadrant == "1") {
              topLeft = elementsList[4];
              topRight = elementsList[2];
              bottomRight = "";
              bottomLeft = elementsList[3];
            } else if (quadrant == "2") {
              topLeft = elementsList[7];
              topRight = elementsList[5];
              bottomRight = elementsList[6];
              bottomLeft = "";
            }

            CreateButtonAtPosition(TL, topLeft);
            CreateButtonAtPosition(TR, topRight);
            CreateButtonAtPosition(BR, bottomRight);
            CreateButtonAtPosition(BL, bottomLeft);
          }
        } else if (int.TryParse(latestData, out int number)) {
          if (previousData != latestData) {
            previousData = latestData;
            if (userEntry.Length <4){
            userEntry += number.ToString();//////////////////////////////////////////////////////////////////

            UpdateUI();
            }
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

  void UpdateUI() {
    if (userEntryText != null) {
      userEntryText.text = userEntry;
    }
    quadrant = "";
    if (instantiatedButtons.Count > 0) {
      foreach(GameObject button in instantiatedButtons) {
        Destroy(button);
      }
      instantiatedButtons.Clear();
    }

  }
}