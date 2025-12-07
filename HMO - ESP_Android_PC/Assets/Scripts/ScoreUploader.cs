using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;

public class ScoreUploader : MonoBehaviour
{
    private const string url = "https://rfid-scan.mern.singularitybd.net/users/set-point";
    private const string token = "9b1de5f407f1463e7b2a921bbce364";

    public TMP_InputField rfid_IF;
    public TMP_InputField score_IF;
    private int gameID = 0;
    public Slider scoreSlider;
    public TMP_Dropdown gameIDDropdown;
    public Button submitButton;
    public TMP_Text statusText;

    private void Start()
    {
        rfid_IF.ActivateInputField();
        score_IF.interactable = false;
        scoreSlider.interactable = false;
        submitButton.interactable = false;
    }

    public void RFID_InputCheck()
    {
        if(rfid_IF.text.Length == 10)
        {
            score_IF.interactable = true;
            //score_IF.ActivateInputField();
            scoreSlider.interactable = true;
            submitButton.interactable = true;
        }
    }

    public void SendScore()
    {
        StartCoroutine(PostScore(rfid_IF.text, gameID, int.Parse(score_IF.text)));
    }

    IEnumerator PostScore(string rfid, int gID, int score)
    {
        // Build JSON manually
        string jsonBody = "{";
        jsonBody += "\"RFID\":\"" + rfid + "\",";

        if (gID == 0) jsonBody += "\"game1\":" + score;
        if (gID == 1) jsonBody += "\"game2\":" + score;
        if (gID == 2) jsonBody += "\"game3\":" + score;
        if (gID == 3) jsonBody += "\"game4\":" + score;
        if (gID == 4) jsonBody += "\"game5\":" + score;
        if (gID == 5) jsonBody += "\"game6\":" + score;

        jsonBody += "}";

        Debug.Log("Sending: " + jsonBody);
        statusText.text = "Posting score...";

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] jsonToSend = Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("x-token", token);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("POST Success: " + request.downloadHandler.text);
            statusText.text = "Score updated successfully!";
            statusText.color = Color.green;
        }
        else
        {
            Debug.LogError("POST Failed: " + request.error + "\nResponse: " + request.downloadHandler.text);

            statusText.text = "Failed to update score!";
            statusText.color = Color.red;
        }

        // Optional: fade out after 3 seconds
        yield return new WaitForSeconds(3);
        statusText.text = "";
    }



    public void SyncSliderWithIF()
    {
        score_IF.text = scoreSlider.value.ToString();
    }
    public void SyncDropDownWithGID()
    {
        gameID = gameIDDropdown.value;
    }
}

[System.Serializable]
public class ScoreData
{
    public string RFID;
    public int game1;
    public int game2;
    public int game3;
    public int game4;
    public int game5;
    public int game6;
    public int gameId;

    public ScoreData(string rfid, int gID, int score)
    {
        RFID = rfid;
        if(gID == 0)
        {
            game1 = score;
        }
        else if(gID == 1)
        {
            game2 = score;
        }
        else if(gID == 2)
        {
            game3 = score;
        }
        else if (gID == 3)
        {
            game4 = score;
        }
        else if (gID == 4)
        {
            game5 = score;
        }
        else if (gID == 5)
        {
            game6 = score;
        }
    }    
}
