using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StyleMainVC : Singleton<StyleMainVC> {

    public Camera PicCamera;
    public Camera PreViewCamera;
    public GameObject SelectView;
    public GameObject Alert;
    public List<List<Dictionary<string, List<DataInfo>>>> BuildingsInfo = new List<List<Dictionary<string, List<DataInfo>>>>();

    private List< Texture2D > PicList = new List<Texture2D>();
    private RenderTexture PreViewTexture;
    private int nowSelect = -1;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /**
     * 初始化建築資料
     * 
     */
    public void initBuildingsInfo()
    {
        BuildingsInfo.Add(DataCenter.Instance.BuildingDataToArrayMethod3(MainController.Instance.Buildings));
        for (int iIndex = 1 ; iIndex < SelectView.transform.childCount ; iIndex++)
        {
            BuildingsInfo.Add( EvolutionCenter.Instance.MutateData(BuildingsInfo[0] , 0.5f ,0.0f));
        }
        print("StyleMainVC initBuildingsInfo OK!!");
        initSelectView();  //*** 目前由MainController呼叫，此時已有一棟建築所以直接拍照
    }
    void initSelectView()
    {
        PicCamera.GetComponent<TakePic>().StartScreenshot();
    }

    /**
     * 測試拍照用
     */
    public void OnTestTakePic()
    {
        if (PicCamera.isActiveAndEnabled == false)
        {
            print("open Camera");
            PicCamera.GetComponent<Camera>().enabled = true;
        }
        PicCamera.GetComponent<TakePic>().StartScreenshot(); 
    }

    public void OnClickSelectCell(GameObject cell)
    {
        print("OnClickSelectCell : " + cell.transform.GetSiblingIndex());
        List<Dictionary<string, List<DataInfo>>> selectInfo = BuildingsInfo[cell.transform.GetSiblingIndex()];
        if (nowSelect != cell.transform.GetSiblingIndex())
        {
            nowSelect = cell.transform.GetSiblingIndex();
            MainController.Instance.UpdateALL(selectInfo);
        }
        else
        {
            PicList.Clear();
            BuildingsInfo.Clear();
            BuildingsInfo.Add(selectInfo);
            for (int iIndex = 1; iIndex < SelectView.transform.childCount; iIndex++)
            {
                BuildingsInfo.Add(EvolutionCenter.Instance.MutateData(BuildingsInfo[0], 0.5f, 0.0f));
            }
            checkSelectViewDone();
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
     * 當照片處理好 (給TakePic呼叫用)
     * 將照片存到一個新的 Texture2D 後存入List中
     */
    public void onPicDone()
    {

        //SelectView.transform.GetChild(iIndex).GetComponent<RawImage>().texture = PicCamera.GetComponent<TakePic>().ShotTexture;
        int iIndex = PicList.Count;
        Texture2D pic = new Texture2D(130, 130);
        pic.SetPixels(PicCamera.GetComponent<TakePic>().ShotTexture.GetPixels());
        pic.Apply();
        PicList.Add(pic);
        //BuildingsInfo.Add(DataCenter.Instance.BuildingDataToArrayMethod3(MainController.Instance.Buildings));
        checkSelectViewDone();
        //SelectView.transform.GetChild(iIndex).GetComponent<RawImage>().texture = pic;

    }
    /**
     * 檢查九宮格是否都有照片
     * 若有缺就給予 MainController 新的資料後再拍
     */
    private void checkSelectViewDone()
    {
        if (PicList.Count != 9)
        {
            //EvolutionCenter.Instance.MutateData(BuildingsInfo[0], 0.5f, 0.0f);
            //DataCenter.Instance.BuildingDataToArrayMethod3(BuildingsInfo);

            //MainController.Instance.tmpUpdateRoof();
            MainController.Instance.UpdateALL(BuildingsInfo[PicList.Count]);

            PicCamera.GetComponent<TakePic>().StartScreenshot();
        }
        else
        {
            print("Done !!");
            reloadSelectView();
            if (Alert != null)
            {
              //  closeAlert();
            }
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
