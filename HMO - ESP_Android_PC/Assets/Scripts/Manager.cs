using UnityEngine;
using UnityEngine.Video;
using System.Net.Http;
using System.Collections;

public class Manager : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public RenderTexture rt;

    private static readonly HttpClient client = new HttpClient();
    public string flaskURL = "http://127.0.0.1:5000/get_command";

    public string brain, lungs, gut;
    private string curr = "";

    void Start()
    {
        InvokeRepeating(nameof(PollFlask), 1f, 1f);
        brain = Application.dataPath + "/brain.mp4";
        lungs = Application.dataPath + "/lungs.mp4";
        gut = Application.dataPath + "/gut.mp4";
    }
    async void PollFlask()
    {
        try
        {
            string json = await client.GetStringAsync(flaskURL);

            // Convert JSON → C# object
            CommandResponse data = JsonUtility.FromJson<CommandResponse>(json);

            if (data != null && !string.IsNullOrEmpty(data.command))
            {
                ProcessCommand(data.command.Trim().ToLower());
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Flask error: " + e.Message);
        }
    }


    void ProcessCommand(string cmd)
    {
        //Debug.Log(cmd + "   "+curr);
        if(cmd == curr)
        {
            //Debug.Log("ferot jacche?");
            return;
        }
        if (cmd == "led_brain" )
        {
            //Debug.Log("Dhuke brain a?");
            curr = cmd;
            PlayVideo(brain);
        }
        else if (cmd == "led_lungs")
        {
            //Debug.Log("Dhuke lungs a?");
            curr = cmd;
            PlayVideo(lungs);
        }
        else if (cmd == "led_gut")
        {
            //Debug.Log("Dhuke gut a?");
            curr = cmd;
            PlayVideo(gut);
        }
        else if (cmd == "reset")
        {
            //Debug.Log("Dhuke reset a?");
            curr = cmd;
            ResetVideo();

        }
        //Debug.Log(cmd);
    }
    void PlayVideo(string fileName)
    {
        videoPlayer.Stop();
        rt.Release();
        videoPlayer.url = fileName;
        videoPlayer.Play();
        StartCoroutine(CheckIfVideoEnds());
    }

    void ResetVideo()
    {
        curr = "reset";
        videoPlayer.Stop();
        rt.Release();
    }
    IEnumerator CheckIfVideoEnds()
    {
        yield return new WaitForSeconds(0.1f);
        if(videoPlayer.isPlaying)
        {
            StartCoroutine(CheckIfVideoEnds());
        }
        else
        {
            ResetVideo();
        }
    }


    [System.Serializable]
    public class CommandResponse
    {
        public string command;
    }
}
