using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using Unity.Sentis;

namespace MoveNet
{
    //Execute inferences using machine learning model
    public class Detector : IDisposable
    {
        public const int CLASSIFICATION_NUMBER = 17;

        public Detector(ResourceSet resources)
          => AllocateObjects(resources);

        public void Run(Texture source, float Threshold = 0.5f)
        {
            // Not used in this sample
            if (Threshold < 0.0f) _scoreThreshold = 0.0f;
            else if (Threshold > 1.0f) _scoreThreshold = 1.0f;
            else _scoreThreshold = Threshold;

            RunModel(source);
        }

        public IEnumerable<Detection> Detections()
        {
            /*
            Return Detections Result (IEnumerable<Detection>)

            Length of the IEnumerable = 17 (Classification)

            return [struct Detection (position_x, position_y, score)] * 17

            Data structure of <Detection> : See Detection.cs
            */
            return _detectionCache.Cached(new ReadOnlyCollection<float>(_detections), _scoreThreshold);
        }

        public float Threshold => _scoreThreshold;

        public void Dispose()
        {
            _worker?.Dispose();
            _worker = null;

            _buffers.preprocess?.Dispose();
            _buffers.preprocess = null;
        }

        // -------------------------------------------------------------------------------------

        ResourceSet _resources;
        Config _config;
        IWorker _worker;

        (GraphicsBuffer preprocess,
         RenderTexture processedImage) _buffers;

        DetectionCache _detectionCache;
        static float _scoreThreshold;
        static float[] _detections;

        void AllocateObjects(ResourceSet resources)
        {
            // NN model loading
            var nnmodel = ModelLoader.Load(resources.model);

            // Edit a Model
            var editedModel = Functional.Compile(
                RGB =>
                {
                    var sRGB = Functional.Pow(RGB, Functional.Tensor(1 / 2.2f));

                    // Transform values from the range [0, 1] to the range [0, 255].
                    var RGB_255 = Functional.Mul(sRGB, Functional.Tensor(255.0f));

                    return nnmodel.ForwardWithCopy(RGB_255)[0];
                },
                nnmodel.inputs[0]);

            // Private object initialization
            _resources = resources;
            _config = new Config(editedModel);
            _worker = WorkerFactory.CreateWorker(BackendType.GPUCompute, editedModel);

            _buffers.preprocess = new GraphicsBuffer(
                GraphicsBuffer.Target.Structured, _config.InputFootprint, sizeof(float));

            _buffers.processedImage = new RenderTexture(_config.InputWidth, _config.InputWidth, 3);
            _buffers.processedImage.enableRandomWrite = true;
            _buffers.processedImage.Create();

            _detectionCache = new DetectionCache(CLASSIFICATION_NUMBER);
        }

        void RunModel(Texture source)
        {
            // Preprocessing (Sampling)
            var pre = _resources.preprocess;
            pre.SetInt("Size", _config.InputWidth);
            pre.SetTexture(0, "Image", source);
            pre.SetBuffer(0, "Tensor", _buffers.preprocess); 
            pre.SetTexture(0, "processedImage", _buffers.processedImage);
            pre.Dispatch(0, _config.InputWidth / 8, _config.InputWidth / 8, 1);

            float[] ft = new float[_config.InputFootprint];
            _buffers.preprocess.GetData(ft);

            // NN worker invocation
            TensorShape newShape = new TensorShape(_config.InputWidth, _config.InputWidth, 3);
            using (var tensor = new TensorFloat(newShape, ft))
            {
                tensor.Reshape(newShape);

                _worker.Execute(tensor);
            }

            var output = _worker.PeekOutput(_config.OutputName) as TensorFloat;
            output.CompleteOperationsAndDownload();
            _detections = output.ToReadOnlyArray();

            output.Dispose();
            _detectionCache.Invalidate();
        }
    }
}//namespace MoveNet
