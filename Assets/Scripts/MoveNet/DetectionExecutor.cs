using MoveNet;
using System.Collections.Generic;
using UnityEngine;

//Execute inference & Intermediary for delivery of result
public class DetectionExecutor : MonoBehaviour
{
    Detector _detector;
    [SerializeField] ResourceSet _resources = null;
    [SerializeField] Video.VideoGrapher _grapher = null;

    int _imageWidth;
    int _imageHeight;

    public int ImageWidth => _imageWidth;
    public int ImageHeight => _imageHeight;

    void Start()
    {
        _detector = new Detector(_resources);
    }

    public void Run()
    {
        _imageWidth = _grapher.ImageWidth;
        _imageHeight = _grapher.ImageHeight;
        _detector.Run(_grapher.CameraTexture);
    }

    public IEnumerable<Detection> LatestDetection() => _detector.Detections();

    void OnDisable() => _detector.Dispose();
}

