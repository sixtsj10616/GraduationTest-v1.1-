using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StyleMainVC : Singleton<StyleMainVC> {

    public Camera PicCamera;
    public Camera PreViewCamera;
    public GameObject SelectView;
    public GameObject ResultView;
    public GameObject PreView;
    public GameObject Alert;
    public List<List<Dictionary<string, List<DataInfo>>>> BuildingsInfo = new List<List<Dictionary<string, List<DataInfo>>>>();
    public List<List<Dictionary<string, List<DataInfo>>>> allBuildingsInfo = new List<List<Dictionary<string, List<DataInfo>>>>();
    public List<List<Dictionary<string, List<DataInfo>>>> resultBuildingsInfo = new List<List<Dictionary<string, List<DataInfo>>>>();
    public List<int> ScoreList = new List<int>();
    public Button passBtn;
    public Button nextBtn;

    private List< Texture2D > PicList = new List<Texture2D>();
    private List<Texture2D> ResultPicList = new List<Texture2D>();
    private List<int> SelectOrderList = new List<int>();
    private RenderTexture PreViewTexture;
    private int nowSelect = -1;
    private bool bTest = true;
    private bool isSelectPic = true;

    //*** !tmp
    public List<Vector2> tmpCulsInfo = new List<Vector2>();
    public List<Vector2> allCulsInfo = new List<Vector2>();
    public List<Vector2> resultCulsInfo = new List<Vector2>();
    NNTest nn;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /**
     * 初始化建築資料 (目前給MainController呼叫)
     * 將目前的Main中的建築物資訊存入 BuildingsInfo，並依照剩下的格數由剛才的建築變異資訊
     * 完成後開始拍照
     */
    public void initBuildingsInfo()
    {
        if (!bTest)
        {
            BuildingsInfo.Add(DataCenter.Instance.BuildingDataToArrayMethod3(MainController.Instance.Buildings));
            for (int iIndex = 1; iIndex < SelectView.transform.childCount; iIndex++)
            {
                BuildingsInfo.Add(EvolutionCenter.Instance.MutateData(BuildingsInfo[0], 0.5f, 0.0f));
            }
            print("StyleMainVC initBuildingsInfo OK!!");
            initSelectView();  //*** 目前由MainController呼叫，此時已有一棟建築所以直接拍照
        }
        else
        {
            //*** !!暫用  柱子***//
            tmpCulsInfo.Add(MainController.Instance.tmpCulInfo);
            for (int iIndex = 1; iIndex < SelectView.transform.childCount; iIndex++)
            {
                tmpCulsInfo.Add(EvolutionCenter.Instance.MutateData(tmpCulsInfo[0], 0.5f, 0.0f));
            }
            initSelectView();
        }
    }
    /**
     * 開始初始化九宮格資料
     * 拍照
     */
    void initSelectView()
    {
        PicCamera.GetComponent<TakePic>().StartScreenshot();
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
        if (ResultView.activeInHierarchy == false)
        {
            ResultView.SetActive(true);
            PreView.SetActive(false);

            resultCulsInfo.Clear();
            while (resultCulsInfo.Count < ResultView.transform.childCount)
            {
                Vector2 newData = EvolutionCenter.Instance.MutateData(new Vector2(2.0f, 1.0f), 0.5f, 0.0f);
                double result = nn.tryData(new Vector2((newData.x - 0.5f) / 3, 1.0f));
                if (result > 0.7)
                {
                    resultCulsInfo.Add(newData);
                }
                else
                {

                }
            }
            isSelectPic = false;
            checkResultViewDone();
        }
    }

    /**
     * 測試用按鈕事件
     */
    public void OnTestNNResult(float fValue)
    {
        double result = nn.tryData(new Vector2((fValue - 0.5f)/3, 1.0f));
        if (result > 0.7)
        {
            print("這是我要的 nn result : " + result);
        }
        else
        {
            print("不要的 nn result : " + result);
        }
        
    }
    /**
     * 點擊Select Cell
     */
    public void OnClickSelectCell(GameObject cell)
    {
        print("OnClickSelectCell : " + cell.transform.GetSiblingIndex());
        MainController.Instance.UpdateTmpCul(tmpCulsInfo[cell.transform.GetSiblingIndex()]);

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

    /***/
    public void OnClickPassBtn()
    {
        PicList.Clear();
        tmpCulsInfo.Clear();
        SelectOrderList.Clear();
        //BuildingsInfo.Clear();
        for (int iIndex = 0; iIndex < SelectView.transform.childCount; iIndex++)
        {
            tmpCulsInfo.Add(EvolutionCenter.Instance.MutateData(new Vector2(2.0f,1.0f), 0.5f, 0.0f));
        }
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
        nn.startTrain(allCulsInfo, ScoreList);
    }

    /**
     * 儲存selectView上的資訊
     * 有被選的格子依照順序給予分數，沒選的格子一率零分
     */
    void saveSelesctViewInfo()
    {
        for (int iIndex = 0; iIndex < tmpCulsInfo.Count; iIndex++)
        {
            int selectOrder = checkIfSelect(iIndex);
            allCulsInfo.Add(tmpCulsInfo[iIndex]);

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
            if (!bTest)
            {
                MainController.Instance.UpdateALL(BuildingsInfo[PicList.Count]);
                PicCamera.GetComponent<TakePic>().StartScreenshot();
            }
            else
            {
                MainController.Instance.UpdateTmpCul(tmpCulsInfo[PicList.Count]);
                PicCamera.GetComponent<TakePic>().StartScreenshot();
            }
            
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
        if (ResultPicList.Count != SelectView.transform.childCount)
        {
            MainController.Instance.UpdateTmpCul(resultCulsInfo[ResultPicList.Count]);
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
        for (int iIndex = 0; iIndex < 9; iIndex++)
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
