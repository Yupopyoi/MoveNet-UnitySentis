using System.Collections.Generic;
using Unity.Sentis;

namespace MoveNet
{
    // Input/output configuration for machine learning (onnx) model
    struct Config
    {
        public string InputName { get; private set; }
        public SymbolicTensorShape InputShape { get; private set; }
        public string OutputName { get; private set; }
        public int InputFootprint => InputShape[0].value * InputShape[1].value * InputShape[2].value * InputShape[3].value;
        public int InputWidth => InputShape[1].value;

        public Config(Model nnmodel)
        {
            List<Model.Input> inputs = nnmodel.inputs;
            InputName = inputs[0].name;
            InputShape = inputs[0].shape;

            List<Model.Output> outputs = nnmodel.outputs;
            OutputName = outputs[0].name;
        }
    }
}
