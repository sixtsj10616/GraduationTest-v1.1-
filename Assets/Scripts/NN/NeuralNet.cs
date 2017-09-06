using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NeuralNetwork
{
	public class NeuralNet
	{
		public double LearnRate { get; set; }
		public double Momentum { get; set; }
		public List<Neuron> InputLayer { get; set; }
		public List<Neuron> HiddenLayer { get; set; }
		public List<Neuron> OutputLayer { get; set; }

		private static readonly System.Random Random = new System.Random();

        /**
         * 承翰給的
         */
    //     public NeuralNet nnCreater(int inputLayerSize, int hiddenLayerSize, int outputLayerSize, string wJson, string bJson)
    //     {
    //        NeuralNet nn = new NeuralNet(inputLayerSize, hiddenLayerSize, outputLayerSize);

    //         List<object> Model_Weights = MiniJSON.Json.Deserialize(wJson) as List<object>;
    //         List<object> Model_Biases = MiniJSON.Json.Deserialize(bJson) as List<object>;

    //         List<object> in_hidden_weights = Model_Weights[0] as List<object>;
    //         List<object> in_hidden_biases = Model_Biases[0] as List<object>;

    //        for (int i = 0; i < nn.InputLayer.Count; i++)
    //        {
    //             Neuron input = nn.InputLayer[i];
    //            List<object> row = in_hidden_weights[i] as List<object>;
    //            for (int j = 0; j < nn.HiddenLayer.Count; j++)
    //            {
    //                Neuron hidden = nn.HiddenLayer[j];
    //                hidden.Bias = Convert.ToDouble(in_hidden_biases[j]);

    //                foreach (Synapse syn in input.OutputSynapses)
    //                {
    //                    if (syn.OutputNeuron == hidden)
    //                    {
    //                        syn.Weight = Convert.ToDouble(row[j]);
    //                        break;
    //                    }
    //                }
    //            }
    //        }

    //        List<object> hidden_output_weights = Model_Weights[1] as List<object>;
    //        List<object> hidden_output_biases = Model_Biases[1] as List<object>;

    //        for (int i = 0; i < nn.HiddenLayer.Count; i++)
    //        {
    //            Neuron hidden = nn.HiddenLayer[i];
    //             List<object> row = hidden_output_weights[i] as List<object>;

    //        for (int j = 0; j < nn.OutputLayer.Count; j++)
    //        {
    //            Neuron output = nn.OutputLayer[j];
    //            output.Bias = Convert.ToSingle(hidden_output_biases[j]);

    //            foreach (Synapse syn in hidden.OutputSynapses)
    //            {
    //                if (syn.OutputNeuron == output)
    //                {
    //                    syn.Weight = Convert.ToSingle(row[j]);
    //                    break;
    //                }
    //            }
    //        }
    //    }
    //    return nn;
    //}


		public NeuralNet(int inputSize, int hiddenSize, int outputSize, double? learnRate = null, double? momentum = null)
		{
			LearnRate = learnRate ?? .4;
			Momentum = momentum ?? .9;
			InputLayer = new List<Neuron>();
			HiddenLayer = new List<Neuron>();
			OutputLayer = new List<Neuron>();

			for (var i = 0; i < inputSize; i++)
				InputLayer.Add(new Neuron());

			for (var i = 0; i < hiddenSize; i++)
				HiddenLayer.Add(new Neuron(InputLayer));

			for (var i = 0; i < outputSize; i++)
				OutputLayer.Add(new Neuron(HiddenLayer));
		}

		public void Train(List<DataSet> dataSets, int numEpochs)
		{
			for (var i = 0; i < numEpochs; i++)
			{
				foreach (var dataSet in dataSets)
				{
					ForwardPropagate(dataSet.Values);
					BackPropagate(dataSet.Targets);
				}
			}
		}

		public void Train(List<DataSet> dataSets, double minimumError)
		{
			var error = 1.0;
			var numEpochs = 0;

			while (error > minimumError && numEpochs < 100000)
			{
				var errors = new List<double>();
				foreach (var dataSet in dataSets)
				{
					ForwardPropagate(dataSet.Values);
					BackPropagate(dataSet.Targets);
					errors.Add(CalculateError(dataSet.Targets));
				}
				error = errors.Average();
				numEpochs++;
			}
		}
        /**
         * 自己測試用
         */
        public void Train(List<DataSet2> dataSets, double minimumError)
        {
            var error = 1.0;
            var numEpochs = 0;

            while (error > minimumError && numEpochs < 100000)
            {
                var errors = new List<double>();
                foreach (var dataSet in dataSets)
                {
                    ForwardPropagate(dataSet.Values);
                    BackPropagate(dataSet.Targets);
                    errors.Add(CalculateError(dataSet.Targets));
                }
                error = errors.Average();
                numEpochs++;
            }
        }

        private void ForwardPropagate(params double[] inputs)
		{
			var i = 0;
			InputLayer.ForEach(a => a.Value = inputs[i++]);
			HiddenLayer.ForEach(a => a.CalculateValue());
			OutputLayer.ForEach(a => a.CalculateValue());
		}
        /**
         * 自己測試用
         */
        private void ForwardPropagate(params float[] inputs)
        {
            var i = 0;
            InputLayer.ForEach(a => a.Value = inputs[i++]);
            HiddenLayer.ForEach(a => a.CalculateValue());
            OutputLayer.ForEach(a => a.CalculateValue());
        }

        private void BackPropagate(params double[] targets)
		{
			var i = 0;
			OutputLayer.ForEach(a => a.CalculateGradient(targets[i++]));
			HiddenLayer.ForEach(a => a.CalculateGradient());
			HiddenLayer.ForEach(a => a.UpdateWeights(LearnRate, Momentum));
			OutputLayer.ForEach(a => a.UpdateWeights(LearnRate, Momentum));
		}
        /**
         * 自己測試用
         */
        private void BackPropagate(params int[] targets)
        {
            var i = 0;
            OutputLayer.ForEach(a => a.CalculateGradient(targets[i++]));
            HiddenLayer.ForEach(a => a.CalculateGradient());
            HiddenLayer.ForEach(a => a.UpdateWeights(LearnRate, Momentum));
            OutputLayer.ForEach(a => a.UpdateWeights(LearnRate, Momentum));
        }

        public double[] Compute(params double[] inputs)
		{
			ForwardPropagate(inputs);
			return OutputLayer.Select(a => a.Value).ToArray();
		}
        /**
        * 自己測試用
        */
        public double[] Compute(params float[] inputs)
        {
            ForwardPropagate(inputs);
            return OutputLayer.Select(a => a.Value).ToArray();
        }
        private double CalculateError(params double[] targets)
		{
			var i = 0;
			return OutputLayer.Sum(a => Mathf.Abs((float)a.CalculateError(targets[i++])));
		}
        /**
        * 自己測試用
        */
        private double CalculateError(params int[] targets)
        {
            var i = 0;
            return OutputLayer.Sum(a => Mathf.Abs((float)a.CalculateError(targets[i++])));
        }

        public static double GetRandom()
		{
			return 2 * Random.NextDouble() - 1;
		}
	}

	public enum TrainingType
	{
		Epoch,
		MinimumError
	}

}