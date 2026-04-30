using UnityEngine;
using UnityEngine.UI;

public class CenterMarker : MonoBehaviour
{
    public Sprite crosshairSprite;

    void Start()
    {
        GameObject canvasObj = new GameObject("CenterCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        GameObject markerObj = new GameObject("CenterMarker");
        markerObj.transform.SetParent(canvasObj.transform);

        Image markerImage = markerObj.AddComponent<Image>();
        markerImage.sprite = crosshairSprite;
        markerImage.rectTransform.sizeDelta = new Vector2(64, 64);
        markerImage.rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        markerImage.rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        markerImage.rectTransform.anchoredPosition = Vector2.zero;
    }
}
