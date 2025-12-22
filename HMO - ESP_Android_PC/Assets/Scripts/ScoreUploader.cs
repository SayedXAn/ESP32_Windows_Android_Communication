using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;

public class ScoreUploader : MonoBehaviour
{
    private const string url = "https://rfid-scan.wskoly.xyz/api/game/score";

    public TMP_InputField rfid_IF;
    public TMP_InputField score_IF;
    private int gameID = 0;
    public Slider scoreSlider;
    public Button submitButton;
    public TMP_Text statusText;
    public TMP_Text scoreText;

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
            score_IF.text = scoreSlider.value.ToString();
        }
    }

    public void SendScore()
    {
        StartCoroutine(PostScore(rfid_IF.text, gameID, int.Parse(score_IF.text)));
    }

    IEnumerator PostScore(string rfid, int gID, int score)
    {
        var gameID = gID + 1;
        // Build JSON manually

        string jsonBody = $"{{\"rfid\": \"{rfid}\", \"scores\": {{\"{gameID}\": {score}}}}}";

        Debug.Log("Sending: " + jsonBody);
        statusText.text = "Posting score...";

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] jsonToSend = Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");
        //request.SetRequestHeader("x-token", token);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("POST Success: " + request.downloadHandler.text);
            statusText.text = "Score updated successfully!";
            statusText.color = Color.green;
            // Parse JSON response
            UpdateScoreResponse response = JsonUtility.FromJson<UpdateScoreResponse>(request.downloadHandler.text);
            scoreText.text = $"Your total score: {response.total_points}";
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

    public void ResetRFIDText()
    {
        rfid_IF.text = "";
        score_IF.text = "10";
        scoreSlider.value = 10;
        scoreText.text = "";
    }
}

[System.Serializable]
public class ScoreResponse
{
    public string rfid;
    public int game_id;
    public int score;
    public string user_name;
    public int total_points;
}

[System.Serializable]
public class UpdateScoreResponse
{
    public string rfid;
    public string user_name;
    public string updated_games;
    public int total_points;
    public string message;
}
