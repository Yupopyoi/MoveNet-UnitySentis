namespace MoveNet
{
    public readonly struct Detection
    {
        public Detection(float[] detectData, int index, string label)
        {
            // Note : The first two data represent yx coordinates.
            x = detectData[1];
            y = detectData[0];
            score = detectData[2];

            labelIndex = index;
            labelName = label;
        }

        public readonly float x, y;
        public readonly int labelIndex;
        public readonly string labelName;
        public readonly float score;

        public override string ToString()
            => $"({x},{y}) : {labelIndex} / {labelName} ({score})";
    }
}
