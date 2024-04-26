using System.Collections.ObjectModel;

namespace MoveNet
{
    sealed class DetectionCache
    {
        Detection[] _cached;
        int _classNumber;

        public DetectionCache(int classNumber)
        {
            _classNumber = classNumber;
            _cached = new Detection[classNumber];
        }

        public Detection[] Cached(ReadOnlyCollection<float> detectData, float t) => _cached ?? UpdateCache(detectData, t);

        public void Invalidate() => _cached = null;

        public static string[] labels = new[]
        {
            "Nose", "L_eye", "R_eye", "L_ear", "R_ear", "L_shoulder", "R_shoulder",
            "L_elbow", "R_elbow", "L_wrist", "R_wrist", "L_hip", "R_hip",
            "L_knee", "R_knee", "L_ankle", "R_ankle",
        };

        public Detection[] UpdateCache(ReadOnlyCollection<float> detectData, float threshold)
        {
            if(_cached == null) _cached = new Detection[_classNumber];

            for(int index = 0; index < _classNumber; index++)
            {
                var detectDatum = new float[3]{ detectData[index * 3 + 0],
                                                detectData[index * 3 + 1],
                                                detectData[index * 3 + 2]};

                var detection = new Detection(detectDatum, index, labels[index]);

                _cached[index] = detection;
            }

            return _cached;
        }
    }
}
