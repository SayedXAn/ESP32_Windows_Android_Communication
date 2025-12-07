using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Video;

public class Manager : MonoBehaviour
{
    public string espIP = "192.168.4.1"; // ESP32 AP IP
    public int port = 81;

    public VideoPlayer videoPlayer;
    public RenderTexture rt;

    TcpClient client;
    NetworkStream stream;
    public string brain, lungs, gut;

    async void Start()
    {

        await ConnectToESP32();
        ListenForMessages();
        brain = Application.dataPath + "/brain.mp4";
        lungs = Application.dataPath + "/lungs.mp4";
        gut = Application.dataPath + "/gut.mp4";
        //videoPlayer.url = brain;
        //videoPlayer.Play();
    }

    async Task ConnectToESP32()
    {
        client = new TcpClient();
        await client.ConnectAsync(espIP, port);
        stream = client.GetStream();
        Debug.Log("Connected to ESP32");
    }

    async void ListenForMessages()
    {
        byte[] buffer = new byte[256];

        while (true)
        {
            if (stream != null && stream.CanRead)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                string msg = Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim();

                Debug.Log("ESP32 sent: " + msg);

                ProcessCommand(msg);
            }
        }
    }

    void ProcessCommand(string cmd)
    {
        if (cmd == "LED_BRAIN")
            PlayVideo(brain);

        else if (cmd == "LED_LUNGS")
            PlayVideo(lungs);

        else if (cmd == "LED_GUT")
            PlayVideo(gut);

        else if (cmd == "RESET")
            videoPlayer.Stop();
    }

    void PlayVideo(string fileName)
    {
        //videoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, fileName);
        videoPlayer.Stop();
        rt.Release();
        videoPlayer.url = fileName;        
        videoPlayer.Play();
    }
}
