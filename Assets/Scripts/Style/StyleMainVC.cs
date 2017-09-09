using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class StyleMainVC : Singleton<StyleMainVC> {

    public Camera PicCamera;
    public Camera PreViewCamera;
    public GameObject SelectView;
    public GameObject ResultView;
    public GameObject PreView;
    public GameObject Alert;
    public GameObject SelectPanel;
    public GameObject ResultPanel;
    public List<List<Dictionary<string, List<DataInfo>>>> BuildingsInfo = new List<List<Dictionary<string, List<DataInfo>>>>();         //** 當前選擇視窗上的 
    public List<List<Dictionary<string, List<DataInfo>>>> allBuildingsInfo = new List<List<Dictionary<string, List<DataInfo>>>>();      //** 全部幾輪的
    public List<List<Dictionary<string, List<DataInfo>>>> resultBuildingsInfo = new List<List<Dictionary<string, List<DataInfo>>>>();   //** 結果視窗上的
    public List<int> ScoreList = new List<int>();
    public Button passBtn;
    public Button nextBtn;
    public Button RefreshBtn;

    private List< Texture2D > PicList = new List<Texture2D>();
    private List<Texture2D> ResultPicList = new List<Texture2D>();
    private List<int> SelectOrderList = new List<int>();
    private RenderTexture PreViewTexture;
    private bool isSelectPic = true;            //** 是否還在選擇喜歡的風格照片
    private bool isLoadFile = false;             //** 是否由讀檔開始
    private float fTrainThreshold = 0.9f;
    private string strFileName = "ResultData.json";     //** ResultData_Avg、ResultData
    NNTest nn;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /**
     * 初始化建築資料 (目前給MainController呼叫)
     * 將目前的Main中的建築物資訊存入 BuildingsInfo，並依照剩下的格數產出建築變異資訊
     * 完成後開始拍照
     */
    public void initBuildingsInfo()
    {
        if (isLoadFile)
        {
            string strFileText = "";
            try
            {
                strFileText = File.ReadAllText(Application.dataPath + "/Resources/"+ strFileName);
            }
            catch
            {
                print("!!! The file could not be read or find");
            }
            var dict = MiniJSON.Json.Deserialize(strFileText) as Dictionary<string, object>;
            List< object > listData = (List<object>)dict["data"] ;

            for (int i = 0; i < listData.Count; i++)
            {
                //DataCenter.Instance.changeFileDataToBuildingData(listData, i);
                BuildingsInfo.Add(DataCenter.Instance.changeFileDataToBuildingData(listData, i));
            }
            checkSelectViewDone();
        }
        else
        {
            BuildingsInfo.Add(DataCenter.Instance.BuildingDataToArrayMethod3(MainController.Instance.Buildings));
            for (int iIndex = 1; iIndex < SelectView.transform.childCount; iIndex++)
            {
                //BuildingsInfo.Add(EvolutionCenter.Instance.MutateData(BuildingsInfo[0], 0.5f, 0.0f));
                BuildingsInfo.Add(EvolutionCenter.Instance.RamdomMutateData(1.0f, 1.0f));
            }
            print("StyleMainVC initBuildingsInfo OK!!");
            initSelectView();  //*** 目前由MainController呼叫，此時已有一棟建築所以直接拍照
        }
        
    }
    /**
     * 開始初始化九宮格要顯示的資料
     * 拍照
     */
    void initSelectView()
    {
        PicCamera.GetComponent<TakePic>().StartScreenshot();
    }
    /**
     * 開始初始化九宮格資料
     */
    void initSelectData(int iStartNum)
    {
        for (int iIndex = iStartNum; iIndex < SelectView.transform.childCount; iIndex++)
        {
            BuildingsInfo.Add(EvolutionCenter.Instance.RamdomMutateData(1.0f, 1.0f));
        }
    }
    /**
     * 當照片處理好 (給TakePic呼叫用)
     * 將照片存到一個新的 Texture2D 後存入List中
     */
    public void onPicDone()
    {
        Texture2D pic = new Texture2D(130, 130);
        pic.SetPixels(PicCamera.GetComponent<TakePic>().ShotTexture.GetPixels());
        pic.Apply();
        if (isSelectPic)
        {
            PicList.Add(pic);
            checkSelectViewDone();
        }
        else
        {
            ResultPicList.Add(pic);
            checkResultViewDone();
        }
    }
    /**
     * 當nn跑完
     */
    public void onNNDone()
    {
        print("NN Done");
        //if (ResultView.activeInHierarchy == false)
        if (ResultPanel.activeInHierarchy == false)
        {
            ResultPanel.SetActive(true);
            SelectPanel.SetActive(false);
            //PreView.SetActive(false);
            RefreshBtn.transform.gameObject.SetActive(true);
            
            //*** ----------------------------------------------
            //*** 這便要能直接依照設定黨的最大最小值產出建築資訊
            //*** ----------------------------------------------
            resultBuildingsInfo.Clear();
            while (resultBuildingsInfo.Count < ResultView.transform.childCount)
            {
                List<Dictionary<string, List<DataInfo>>> newData = EvolutionCenter.Instance.RamdomMutateData(1.0f, 1.0f);
                double result = nn.tryData(newData);
                if (result > fTrainThreshold)
                {
                    resultBuildingsInfo.Add(newData);
                }
            }
                
            isSelectPic = false;
            checkResultViewDone();
        }
    }

    
    /**
     * 測試用按鈕事件
     */
    public void OnTestNNResult()
    {
        EvolutionCenter.Instance.saveRamdomData();
        //EvolutionCenter.Instance.createAverageDistributedData(10);
    }
    /**
     * 點擊Select Cell
     */
    public void OnClickSelectCell(GameObject cell)
    {
        print("OnClickSelectCell : " + cell.transform.GetSiblingIndex());
        //MainController.Instance.UpdateTmpCul(tmpCulsInfo[cell.transform.GetSiblingIndex()]);
        List < Dictionary<string, List<DataInfo>> > selectInfo = BuildingsInfo[cell.transform.GetSiblingIndex()];
        MainController.Instance.UpdateALL(selectInfo);

        int iSelOrder = checkIfSelect(cell.transform.GetSiblingIndex());
        if (iSelOrder == -1)
        {
            SelectOrderList.Add(cell.transform.GetSiblingIndex());
            cell.transform.Find("Text").GetComponent<Text>().text = SelectOrderList.Count.ToString();
        }
        else
        {
            SelectOrderList.RemoveAt(checkIfSelect(cell.transform.GetSiblingIndex()));
            resetOrderNum();
        }
        //** 原先GA進化邏輯
        //List<Dictionary<string, List<DataInfo>>> selectInfo = BuildingsInfo[cell.transform.GetSiblingIndex()];
        //if (nowSelect != cell.transform.GetSiblingIndex())
        //{
        //    nowSelect = cell.transform.GetSiblingIndex();
        //    MainController.Instance.UpdateALL(selectInfo);
        //}
        //else
        //{
        //    PicList.Clear();
        //    BuildingsInfo.Clear();
        //    BuildingsInfo.Add(selectInfo);
        //    for (int iIndex = 1; iIndex < SelectView.transform.childCount; iIndex++)
        //    {
        //        BuildingsInfo.Add(EvolutionCenter.Instance.MutateData(BuildingsInfo[0], 0.5f, 0.0f));
        //    }
        //    checkSelectViewDone();
        //}
    }
    /**
     * 檢查是否在選擇的序列中
     * 輸入 : 要查詢的九宮格順序數(0 ~ count)
     * 輸出 : 在列表中的順序 (0~count) (也代表著排名)
     */
    int checkIfSelect(int num)
    {
        for (int iIndex = 0; iIndex < SelectOrderList.Count; iIndex++)
        {
            if (SelectOrderList[iIndex] == num)
            {
                return iIndex;  //** 有就回傳在列表中的順序 (0~count)
            }
        }
        return -1;              //** 沒找到就回傳-1
    }

    /**
     * 略過此輪
     */
    public void OnClickPassBtn()
    {
        PicList.Clear();  
        SelectOrderList.Clear();
        
        //BuildingsInfo.Clear();
        for (int iIndex = 0; iIndex < SelectView.transform.childCount; iIndex++)
        {   
            BuildingsInfo.Add(EvolutionCenter.Instance.RamdomMutateData(1.0f, 1.0f));
        }
        BuildingsInfo.RemoveRange(0, SelectView.transform.childCount);
        
        checkSelectViewDone();
        resetOrderNum();
    }

    /**
     * 按下一輪
     * 若有選東西就要記錄，沒有選東西就全部捨棄(同pass)
     */
    public void OnClickNextBtn()
    {
        if (SelectOrderList.Count != 0)
        {
            saveSelesctViewInfo();
        }
        OnClickPassBtn();
    }

    /**
     * 開始訓練
     */
    public void OnClickStartBtn()
    {
        if (SelectOrderList.Count != 0)
        {
            saveSelesctViewInfo();
        }
        nn = transform.GetComponent<NNTest>();   
        nn.startTrain(allBuildingsInfo, ScoreList);
        
    }

    public void OnClickRestartBtn()
    {
        PicList.Clear();
        ScoreList.Clear();
        ResultPicList.Clear();       
        SelectOrderList.Clear();
        BuildingsInfo.Clear();
        allBuildingsInfo.Clear();
        resultBuildingsInfo.Clear();
        ResultPanel.SetActive(false);
        SelectPanel.SetActive(true);
        isSelectPic = true;

        initSelectData(0);
        checkSelectViewDone();
        resetOrderNum();
    }

    public void reloadResult()
    {
        resultBuildingsInfo.Clear();
        ResultPicList.Clear();
        while (resultBuildingsInfo.Count < ResultView.transform.childCount)
        {
            List<Dictionary<string, List<DataInfo>>> newData = EvolutionCenter.Instance.RamdomMutateData(1.0f, 1.0f);
            double result = nn.tryData(newData);
            if (result > fTrainThreshold)
            {
                resultBuildingsInfo.Add(newData);
            }
        }

        checkResultViewDone();
        print(" Reload Result Done !!!");
    }

    /**
     * 儲存selectView上的資訊
     * 有被選的格子依照順序給予分數，沒選的格子一率零分
     */
    void saveSelesctViewInfo()
    {    
        for (int iIndex = 0; iIndex < SelectView.transform.childCount; iIndex++)
        {
            int selectOrder = checkIfSelect(iIndex);
            
            allBuildingsInfo.Add(BuildingsInfo[iIndex]);
            
            if (selectOrder != -1)
            {
                ScoreList.Add(100 - selectOrder * 5);
            }
            else
            {
                ScoreList.Add(0);
            }
        }
    }
    /**
     * 重設選擇視窗中的所有cell的順序編號
     * 
     */
    void resetOrderNum()
    {
        int iOrder; 
        for (int iIndex = 0; iIndex < SelectView.transform.childCount; iIndex++)
        {
            iOrder = checkIfSelect(iIndex);
            if (iOrder != -1)
            {
                SelectView.transform.GetChild(iIndex).Find("Text").GetComponent<Text>().text = (iOrder + 1).ToString();
                
            }
            else
            {
                SelectView.transform.GetChild(iIndex).Find("Text").GetComponent<Text>().text = "";
            }
        }
    }

    /**
     * 設定 Preview 攝影機的目標紋理
     */
    public void SetPreView()
    {
        PreViewTexture = Resources.Load("PreView") as RenderTexture;
        PreViewCamera.GetComponent<Camera>().targetTexture = PreViewTexture;
    }

    /**
     * 檢查九宮格是否都有照片 (也可看作資料處理好後開始通知 MainVC 做模型)
     * 若有缺就給予 MainController 新的資料後再拍
     */
    private void checkSelectViewDone()
    {
        if (PicList.Count != SelectView.transform.childCount)
        {
            //EvolutionCenter.Instance.MutateData(BuildingsInfo[0], 0.5f, 0.0f);
            //DataCenter.Instance.BuildingDataToArrayMethod3(BuildingsInfo);
            //MainController.Instance.tmpUpdateRoof();
          
            MainController.Instance.UpdateALL(BuildingsInfo[PicList.Count]);
            PicCamera.GetComponent<TakePic>().StartScreenshot();
        }
        else
        {
            //*** 全部拍照完成，重load selectview
            print("Done !!");
            reloadSelectView();
            if (Alert != null)
            {
              //  closeAlert();
            }
        }
    }
    /**
     * 檢查結果區是否都有照片 (也可看作資料處理好後開始通知 MainVC 做模型)
     * 若有缺就給予 MainController 新的資料後再拍
     */
    private void checkResultViewDone()
    {
        if (ResultPicList.Count != ResultView.transform.childCount)
        {
            MainController.Instance.UpdateALL(resultBuildingsInfo[ResultPicList.Count]);
            PicCamera.GetComponent<TakePic>().StartScreenshot();
        }
        else
        {
            //*** 全部拍照完成，重load selectview
            print("ResultView Done !!");
            reloadResultView();
        }
    }

   /**
    * 九宮格重新讀取圖片
    */
    private void reloadSelectView()
    {
        for (int iIndex = 0; iIndex < SelectView.transform.childCount; iIndex++)
        {
            SelectView.transform.GetChild(iIndex).GetComponent<RawImage>().texture = PicList[iIndex];
        }
    }
    /**
     * 結果區九宮格重新讀取圖片
     */
    private void reloadResultView()
    {
        for (int iIndex = 0; iIndex < ResultView.transform.childCount; iIndex++)
        {
            ResultView.transform.GetChild(iIndex).GetComponent<RawImage>().texture = ResultPicList[iIndex];
        }
    }
    /**
     * 出現Alert (目前還有問題，出現位置不正確)
     * 
     */
    void showAlert()
    {
        Alert = Instantiate(Resources.Load("UI/AlertView"),new Vector3(0,768,0), Quaternion.Euler(0, 0, 0)) as GameObject;
        Alert.transform.parent = transform;
        Alert.transform.localPosition = Vector3.zero;
        //Alert.GetComponent<RectTransform>().localPosition = new Vector3(0,0,0);
        Alert.GetComponent<AlertView>().Show();
    }
    void closeAlert()
    {
        Alert.GetComponent<AlertView>().Close();
        Destroy(Alert);
    }


    
}
