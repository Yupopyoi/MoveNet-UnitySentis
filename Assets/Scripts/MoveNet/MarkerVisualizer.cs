using MoveNet;
using UnityEngine;
using UnityEngine.UI;

public class MarkerVisualizer : MonoBehaviour
{
    [SerializeField] DetectionExecutor executor;

    [SerializeField] RawImage _preview = null;
    [SerializeField] Marker _marker = null;

    Marker[] _markers = new Marker[17];
    float _rawImageWidth;

    void Start()
    {
        for (var i = 0; i < _markers.Length; i++)
        {
            _markers[i] = Instantiate(_marker, _preview.transform);
            _markers[i].Setup();
            _markers[i].Hide();
        }

        _rawImageWidth = _preview.rectTransform.sizeDelta.x;
    }

    void OnDestroy()
    {
        foreach (var marker in _markers) Destroy(marker);
    }

    public void DisplayMarkers()
    {
        var latestDetections = executor.LatestDetection();

        var i = 0;
        foreach (var detection in latestDetections)
        {
            if (i == _markers.Length) break;

            Debug.Log(detection.ToString());
            _markers[i++].SetAttributes(detection, executor.ImageWidth, executor.ImageHeight, _rawImageWidth);
        }

        for (; i < _markers.Length; i++) _markers[i].Hide();
    }
}

