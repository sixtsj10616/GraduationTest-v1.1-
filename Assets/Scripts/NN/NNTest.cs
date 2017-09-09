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
        net = new NeuralNet(8, 9, 1);
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

    public void startTrain(List<List<Dictionary<string, List<DataInfo>>>> trainDataList, List<int> ScoreList)
    {
        onTrainDone done = new onTrainDone(StyleMainVC.Instance.onNNDone);
        if (trainDataList.Count != ScoreList.Count)
        {
            print("In *startTrain* List Count Err");
        }
        else
        {
            for (int iIndex = 0; iIndex < trainDataList.Count; iIndex++)
            {
                double[] input = normalizeDataInfo(trainDataList[iIndex]);
                double[] output = { (double)ScoreList[iIndex] / 100 };
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
    public double tryData(List<Dictionary<string, List<DataInfo>>> data)
    {
        double[] input = normalizeDataInfo(data);
        double[] result = net.Compute(input);
        return result[0];
    }

    /**
     * 正規化參數(目前只有屋頂)
     * 這邊也決定了建築那些部分需要當NN 的 input
     * 注意 DataInfo 的定義， x、y、z 分別為為最小值、現在值、最大值
     */
    double[] normalizeDataInfo( List<Dictionary<string, List<DataInfo>>> trainDataList)
    {
        List<DataInfo> roofDataList = trainDataList[0][Define.RoofDataList];
        List<DataInfo> bodyDataList = trainDataList[0][Define.BodyDataList];
        double[] input = new double[roofDataList.Count + bodyDataList.Count];

        for (int iIndex = 0; iIndex < roofDataList.Count; iIndex++)
        {
            Vector3 v3Info = roofDataList[iIndex].Value;
            input[iIndex] = (v3Info.y - v3Info.x ) / (v3Info.z - v3Info.x);
        }
        for (int iIndex = 0; iIndex < bodyDataList.Count; iIndex++)
        {
            Vector3 v3Info = bodyDataList[iIndex].Value;
            input[iIndex] = (v3Info.y - v3Info.x) / (v3Info.z - v3Info.x);
        }
        return input;
    }

}
