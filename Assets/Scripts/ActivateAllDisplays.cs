using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ActivateAllDisplays : MonoBehaviour
{
    public Canvas remoteCanvas;
    public GameObject remoteCanvasBackground;
    public GameObject remoteImage1;
    public GameObject remoteImage2;
    public GameObject localImage;

    void Start()
    {
        Debug.Log("displays connected: " + Display.displays.Length);
        // Display.displays[0] is the primary, default display and is always ON, so start at index 1.
        // Check if additional displays are available and activate each.

        for (int i = 1; i < Display.displays.Length; i++)
        {
            Display.displays[i].Activate();
        }

        print(Display.displays.Length);
        if (Display.displays.Length == 1)
        {
            remoteCanvas.targetDisplay = 0;
            remoteCanvasBackground.SetActive(false);
            remoteImage2.GetComponent<RawImage>().color = new Color(0, 0, 0, 0);

            RectTransform rt = remoteImage1.GetComponent<RectTransform>();
            rt.rotation = Quaternion.Euler(0, 0, 0);
            rt.anchorMin = new Vector2(0, 0);
            rt.anchorMax = new Vector2(1, 1);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.offsetMin = new Vector2(0, 0);
            rt.offsetMax = new Vector2(0, 0);

            // set local image to top right corner and set width 200, height 150
            rt = localImage.GetComponent<RectTransform>();
            rt.rotation = Quaternion.Euler(0, 0, 0);
            rt.anchorMin = new Vector2(1, 1);
            rt.anchorMax = new Vector2(1, 1);
            rt.pivot = new Vector2(1, 1);
            rt.offsetMin = new Vector2(-350, -200);
            rt.offsetMax = new Vector2(0, 0);
        }
    }

    void Update()
    {

    }
}