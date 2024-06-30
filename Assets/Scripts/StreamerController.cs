using System.Linq;
using Unity.RenderStreaming;
using UnityEngine;
using UnityEngine.UI;

class StreamerController : MonoBehaviour
{
#pragma warning disable 0649
    [SerializeField] private SignalingManager renderStreaming;
    [SerializeField] private RawImage localVideoImage;
    [SerializeField] private RawImage remoteVideoImage;
    [SerializeField] private RawImage remoteVideoImageRight;
    [SerializeField] private AudioSource receiveAudioSource;
    [SerializeField] private VideoStreamSender webCamStreamer;
    [SerializeField] private VideoStreamReceiver receiveVideoViewer;
    [SerializeField] private AudioStreamSender microphoneStreamer;
    [SerializeField] private AudioStreamReceiver receiveAudioViewer;
    [SerializeField] private SingleConnection singleConnection;
    [SerializeField] private Button hangUpButton;

    [SerializeField] private InputField serverAddressInputField;
#pragma warning restore 0649

    private string connectionId = "10";
    private Unity.RenderStreaming.Samples.RenderStreamingSettings settings;

    public void StartAndConnect()
    {
        // Select the first available webcam and microphone
        webCamStreamer.sourceDeviceIndex = 0;
        microphoneStreamer.sourceDeviceIndex = 0;

        // Enable webcam and microphone streamers
        webCamStreamer.enabled = true;
        microphoneStreamer.enabled = true;

        // Assign event handlers
        webCamStreamer.OnStartedStream += id => receiveVideoViewer.enabled = true;
        webCamStreamer.OnStartedStream += _ => localVideoImage.texture = webCamStreamer.sourceWebCamTexture;
        receiveVideoViewer.OnUpdateReceiveTexture += texture => { print("Got texture"); remoteVideoImage.texture = texture; remoteVideoImageRight.texture = texture; };

        microphoneStreamer.OnStartedStream += id => microphoneStreamer.loopback = false;
        receiveAudioViewer.targetAudioSource = receiveAudioSource;
        receiveAudioViewer.OnUpdateReceiveAudioSource += source =>
        {
            source.loop = true;
            source.Play();
        };

        // Set up hangup button
        hangUpButton.onClick.AddListener(HangUp);

        // Load settings if available
        settings = Unity.RenderStreaming.Samples.SampleManager.Instance.Settings;
        if (settings == null) settings = new Unity.RenderStreaming.Samples.RenderStreamingSettings();
        settings.SignalingType = Unity.RenderStreaming.Samples.SignalingType.WebSocket;
        settings.SignalingAddress = (serverAddressInputField.text == "Charity-Cyrango" ? "48.217.242.173:3000" : serverAddressInputField.text);

        if (settings != null)
        {
            webCamStreamer.width = (uint)settings.StreamSize.x;
            webCamStreamer.height = (uint)settings.StreamSize.y;
        }

        if (renderStreaming.runOnAwake)
            return;
        if (settings != null)
            renderStreaming.useDefaultSettings = settings.UseDefaultSettings;
        if (settings?.SignalingSettings != null)
            renderStreaming.SetSignalingSettings(settings.SignalingSettings);
        renderStreaming.Run();

        // Automatically set up the connection
        Invoke("SetUp", 3);
    }

    void Start()
    {
        //if (renderStreaming.runOnAwake)
        //    return;
        //if (settings != null)
        //    renderStreaming.useDefaultSettings = settings.UseDefaultSettings;
        //if (settings?.SignalingSettings != null)
        //    renderStreaming.SetSignalingSettings(settings.SignalingSettings);
        //renderStreaming.Run();

        //// Automatically set up the connection
        //Invoke("SetUp", 3);
    }

    private void SetUp()
    {
        if (settings != null)
        {
            receiveVideoViewer.SetCodec(settings.ReceiverVideoCodec);
            webCamStreamer.SetCodec(settings.SenderVideoCodec);
        }

        singleConnection.CreateConnection(connectionId);
    }

    private void HangUp()
    {
        singleConnection.DeleteConnection(connectionId);

        remoteVideoImage.texture = null;
        remoteVideoImageRight.texture = null;
        localVideoImage.texture = null;
    }
}