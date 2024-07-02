using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

public class ActivateAllDisplays : MonoBehaviour
{
    public Canvas remoteCanvas;
    public GameObject remoteCanvasBackground;
    public GameObject remoteImage1;
    public GameObject remoteImage2;
    public GameObject localImage;

    public List<RawImage> imagesToBeTransformed;

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

        if (Display.displays.Length == 2)
        {
            LoadTransforms();
        }
    }

    private void LoadTransforms()
    {
        string path;
#if UNITY_EDITOR
        path = Path.Combine(Application.dataPath, "transforms.json");
#else
        path = Path.Combine(Application.dataPath, @"..\transforms.json");
#endif

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            TransformCollection transformCollection = JsonUtility.FromJson<TransformCollection>(json);
            ApplyTransforms(transformCollection);
        }
        else
        {
            Debug.LogError("Transform file not found at: " + path);
        }
    }

    private void ApplyTransforms(TransformCollection transformCollection)
    {
        foreach (var transformData in transformCollection.images)
        {
            RawImage img = imagesToBeTransformed.Find(i => i.name == transformData.name);
            if (img != null)
            {
                img.rectTransform.localPosition = transformData.position.ToVector3();
                img.rectTransform.localRotation = transformData.rotation.ToQuaternion();
                img.rectTransform.localScale = transformData.scale.ToVector3();
                img.rectTransform.sizeDelta = new Vector2(transformData.width, transformData.height);
            }
            else
            {
                Debug.LogError("Image with name " + transformData.name + " not found in the canvas.");
            }
        }
    }

    [System.Serializable]
    public class TransformData
    {
        public string name;
        public Vector3Data position;
        public QuaternionData rotation;
        public Vector3Data scale;
        public float width;
        public float height;
    }

    [System.Serializable]
    public class Vector3Data
    {
        public float x;
        public float y;
        public float z;

        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }
    }

    [System.Serializable]
    public class QuaternionData
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public Quaternion ToQuaternion()
        {
            return new Quaternion(x, y, z, w);
        }
    }

    [System.Serializable]
    public class TransformCollection
    {
        public List<TransformData> images;
    }
}