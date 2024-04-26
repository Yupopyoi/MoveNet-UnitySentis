using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR;

namespace MoveNet
{
    sealed class Marker : MonoBehaviour
    {
        RectTransform _xform;
        Image _panel;
        TextMeshProUGUI _label;

        public void Setup()
        {
            _xform = GetComponent<RectTransform>();
            _panel = GetComponentInChildren<Image>();
            _label = GetComponentInChildren<TextMeshProUGUI>();
        }

        public void SetAttributes(in Detection d, int w, int h, float rawImageWidth)
        {
            // Local Position
            // The range of values for d.x and d.y is [0.0, 1.0]
            // Match this to the size of the RawImage.
            var x = (d.x * 2 - 1) * rawImageWidth * 0.5f;
            var y = (d.y * 2 - 1) * rawImageWidth * 0.5f;

            _xform.localPosition = new Vector2(x, y);

            // Label
            _label.text = $"{d.labelName}";

            // Panel color
            var hue = d.labelIndex * 0.073f % 1.0f;
            var color = Color.HSVToRGB(hue, 1, 1);
            color.a = 0.4f;
            _panel.color = color;

            // Enable
            gameObject.SetActive(true);
        }

        public void Hide() => gameObject.SetActive(false);
    }
}
