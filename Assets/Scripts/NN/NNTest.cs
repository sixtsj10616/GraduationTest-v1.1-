using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NeuralNetwork;

public delegate void onTrainDone();

public class NNTest : MonoBehaviour {

    //Neural Network Variables
    private const double MinimumError = 0.01;
    private const TrainingType TrType = TrainingType.MinimumError;
    private static NeuralNet net;
    private static List<DataSet> dataSets;

    // Use this for initialization
    void Start ()
    {
        net = new NeuralNet(2, 3, 1);
        dataSets = new List<DataSet>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void startTrain(List<Vector2> CulInfoList , List<int> ScoreList)
    {
        onTrainDone done = new onTrainDone(StyleMainVC.Instance.onNNDone);
        if (CulInfoList.Count != ScoreList.Count)
        {
            print("In *startTrain* List Count Err");
        }
        else
        {
            for (int iIndex = 0; iIndex < CulInfoList.Count; iIndex++)
            {
                double[] input = { (double)(CulInfoList[iIndex].x - 0.5f)/3, (double)CulInfoList[iIndex].y };
                double[] output = { (double)ScoreList[iIndex]/100 };
                dataSets.Add(new DataSet(input, output));

                //float[] input = { CulInfoList[iIndex].x, CulInfoList[iIndex].y };
                //int[] output = { ScoreList[iIndex] };
                //dataSets.Add(new DataSet2(input, output));

            }
            net.Train(dataSets, MinimumError);
            done();
        }
    }

    public double tryData(Vector2 vals)
    {
        double[] input = { (double)vals.x, (double)vals.y };
        //float[] input = { vals.x, vals.y };
        double[] result = net.Compute(input);
        return result[0];
    }

}
