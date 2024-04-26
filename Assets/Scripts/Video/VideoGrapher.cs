using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.UI;

namespace Video
{
    public class VideoGrapher : MonoBehaviour
    {
        public enum VideoSource { @Texture, @Camera}

        [Header("[Input]")]
        [SerializeField] VideoSource _videoSource = VideoSource.@Texture;

        [Tooltip("No attachment required when using camera")]
        [SerializeField] Texture _texture = null;

        [Tooltip("The number and device name can be confirmed by using DisplayAvailableDevice()")]
        [SerializeField] int _cameraDeviceNumber = 0;

        [Header("[Output]")]
        [SerializeField] RawImage rawImage;

        WebCamTexture webCamTexture;
        WebCamDevice[] webCamDevice;

        int _imageHeight;
        int _imageWidth;

        #region Propaties

        public RawImage CameraImage => rawImage;
        public Texture CameraTexture => rawImage.texture;
        public int ImageHeight => _imageHeight;
        public int ImageWidth => _imageWidth;

        #endregion

        void Start()
        {
            webCamDevice = WebCamTexture.devices;

            //DisplayAvailableDevice();

            DisplayVideo();
        }

        public void DisplayAvailableDevice()
        {
            for(int deviceNumber = 0; deviceNumber < webCamDevice.Length; deviceNumber++)
            {
                Debug.Log(deviceNumber + " / " + webCamDevice[deviceNumber].name);
            }
        }

        public void DisplayVideo()
        {
            if (_videoSource == VideoSource.Texture)
            {
                rawImage.texture = _texture;
                _imageHeight = _texture.height;
                _imageWidth = _texture.width;

                return;
            }

            if (_videoSource == VideoSource.Camera)
            {
                if(_cameraDeviceNumber >= webCamDevice.Length)
                {
                    Debug.LogError("The device with the specified number does not exist.");
                    return;
                }

                webCamTexture = new WebCamTexture(webCamDevice[_cameraDeviceNumber].name);
                rawImage.texture = webCamTexture;
                webCamTexture.Play();
                
                _imageHeight = webCamTexture.height;
                _imageWidth = webCamTexture.width;
                return;
            }
        }
    }
}//namespace Video