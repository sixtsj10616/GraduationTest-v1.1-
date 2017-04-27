using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

public class Building : EditorWindow
{
    string myGaneName = "Game_00";
    string remainTimeBunus = "100";
    string SL_BonusTime = "1000";
    string SL_BonusShot = "10000";
    int SimulationTimes = 10;

    public Transform cue_transform;
    public Transform ob_transform;
    public Object a;
    Vector3 cbPos;
    Vector3 opPos;
    float min_degree= 0.0f;
    float max_degree= 90.0f;
    float delay = 0.0f;

    bool showObeject = true;

    bool exam_btn;
    float examDis = 4.0f;
    bool random_btn = true;

	float defaultPower = 0.2f;
	float deltaPower = 0.2f;
	int addPowerTimes = 5;

	string CBFunction = "";
	string alphaFunction = "";

    [MenuItem("Window/Building")]
    static void Init()
    {
		Building window = (Building)EditorWindow.GetWindow(typeof(Building));
        window.Show();
    }

    void OnGUI()
    {
        cue_transform = EditorGUILayout.ObjectField("Cue Ball", cue_transform, typeof(Transform), true) as Transform;
        ob_transform = EditorGUILayout.ObjectField("Object Ball", ob_transform, typeof(Transform), true) as Transform;

        /*
        GUILayout.Label("Pool Settings", EditorStyles.boldLabel);
        myGaneName = EditorGUILayout.TextField("Game Name", myGaneName);

        if (GUILayout.Button("Load Pool Info"))
            Open();
        if (GUILayout.Button("Save Pool Info"))
            Save();

        GUILayout.Label("Remain Score", EditorStyles.boldLabel);
        remainTimeBunus = EditorGUILayout.TextField("Time Bonus:", remainTimeBunus);
      
        GUILayout.Label("Shot Limit Score", EditorStyles.boldLabel);
        SL_BonusTime = EditorGUILayout.TextField("Bonus Time:", SL_BonusTime);
        SL_BonusShot = EditorGUILayout.TextField("Bonus Shot:", SL_BonusShot);
        */
        /*if (GUILayout.Button("Set Score"))  SetScore();           */     

        GUILayout.Label("Simulation Times", EditorStyles.boldLabel);
        SimulationTimes = EditorGUILayout.IntField("#Times:", SimulationTimes);
        delay = EditorGUILayout.FloatField("Simulator Delay:", delay);

        GUILayout.Label("Cut Angle Range", EditorStyles.boldLabel);
        min_degree = EditorGUILayout.FloatField("Min Degree:", min_degree);
        max_degree = EditorGUILayout.FloatField("Max Degree:", max_degree);

        if (GUILayout.Button("Apply Times & Corner"))
            SetTimes();

        exam_btn = EditorGUILayout.Toggle("ExamTest", exam_btn);
        SetExam(exam_btn);
        if (exam_btn)
        {
            // setting in setpos
            examDis = EditorGUILayout.FloatField("CO Distance:",examDis);
            EditorGUILayout.BeginHorizontal();
            opPos = EditorGUILayout.Vector3Field("Object Ball:", opPos);
            EditorGUILayout.EndHorizontal();   

        }

        if (!exam_btn) { 
            random_btn = EditorGUILayout.Toggle("RandomTest", random_btn); 

        
            if (random_btn)
           {
              SetRandom(random_btn);///
            }
          else
           {
              SetRandom(random_btn);//
              EditorGUILayout.BeginHorizontal();
              cbPos = EditorGUILayout.Vector3Field("Cue Ball:", cbPos);   
              EditorGUILayout.EndHorizontal();

                 EditorGUILayout.BeginHorizontal();
               opPos = EditorGUILayout.Vector3Field("Object Ball:", opPos);   
               EditorGUILayout.EndHorizontal();   


             }
        }
        if (GUILayout.Button("Remember Position"))
            RememberPosition();
  
        if (GUILayout.Button("Apply Position"))
            SetPositino(cbPos, opPos);


		GUILayout.Label("Tangent Alpha Power setting", EditorStyles.boldLabel);

		defaultPower = EditorGUILayout.FloatField("Default Power:",defaultPower);
		deltaPower = EditorGUILayout.FloatField("Delta Power:", deltaPower);
		addPowerTimes = EditorGUILayout.IntField("Add Power times:", addPowerTimes);

		if (GUILayout.Button("Apply Power"))
			SetPowerExp(); 


		GUILayout.Label ("Prediction CueBall Position",EditorStyles.boldLabel);
		CBFunction = EditorGUILayout.TextField ("GC Model:",CBFunction);
		alphaFunction = EditorGUILayout.TextField ("Alpha Model:", alphaFunction);
		if (GUILayout.Button("ApplyModel"))
			SetGCAlphaModel();

        if (GUILayout.Button("ShowGrouping"))
            Grouping();
        
    }
    void Grouping()
    {
        // BallGrouping GP = (BallGrouping)GameObject.Find("GroupingGraph").gameObject.GetComponent(typeof(BallGrouping));
        // GP.MeasureDis();
        // GP.DrawGrouping();

        //** Destroy(MainController.Instance.building);
        for(int iIndex = 0; iIndex < MainController.Instance.Buildings.Count; iIndex++)
        {
            Destroy(MainController.Instance.Buildings[iIndex]);
        }
	   MainController.Instance.InitFunction();
    }

    void SetPositino(Vector3 cuePos,Vector3 obPos)
    {      
       // Exhaustive et = (Exhaustive)GameObject.Find("ET").gameObject.GetComponent(typeof(Exhaustive));
       // et.cbPos = Fix64Vec3.FromVector3(cbPos);
//et.obPos = Fix64Vec3.FromVector3(opPos);
       // et.examDis = examDis;
       // SetExam(exam_btn);
    }
    void RememberPosition()
    {
        //cbPos = Pools.Instance.CueBall.Position.ToVector3();
       // opPos = Pools.Instance.Balls[9].Position.ToVector3();
       // random_btn = false;

    }
    void SetExam(bool a)
    {
      //  Exhaustive et = (Exhaustive)GameObject.Find("ET").gameObject.GetComponent(typeof(Exhaustive));
      // et.exam_btn = a;
    }
    void SetRandom(bool a)
    {
     //   Exhaustive et = (Exhaustive)GameObject.Find("ET").gameObject.GetComponent(typeof(Exhaustive));
      //  et.random_btn = a;         
    }

    void SetTimes()
    {
      //  Exhaustive et = (Exhaustive)GameObject.Find("ET").gameObject.GetComponent(typeof(Exhaustive));
      //  et.maxCA = max_degree;
       // et.time_delay = delay;
     //  et.SetSimulatationTimes(SimulationTimes);
    }

    void SetScore()
    {
        //int rtb,slbt,slbs;
       // int.TryParse(remainTimeBunus, out rtb );     
      //  int.TryParse(SL_BonusTime,    out slbt);      
     //   int.TryParse(SL_BonusShot,    out slbs);

      //  MyGame a = (MyGame)GameObject.Find("GameSetting").gameObject.GetComponent(typeof(MyGame));
      //  a.SetBonusScore(rtb,slbt,slbs);
    }

    void Open()
    {
       // string path = EditorUtility.OpenFilePanel("Load json", "", "json");
       // if (path != null)
      //  {
      //      a.loadJsonFormat_str(path);
      //  }
     //   Debug.Log(path);
    }

    void Save()
    {
      //  string path = EditorUtility.SaveFilePanel("Save Json", "", myGaneName + ".json", "json");
//
      //  if (path != null)
      //  {
      //      SaveJson a = (SaveJson)GameObject.Find("GameJson").gameObject.GetComponent(typeof(SaveJson));
       //     a.saveJsonFormat_Name(path);
      //  }        
    }

	void SetPowerExp()
	{
		// et = (Exhaustive)GameObject.Find("ET").gameObject.GetComponent(typeof(Exhaustive));
		/////et.defaultPower = defaultPower;
		///et.deltaPower = deltaPower;
		//et.powerTimes = addPowerTimes;
	}
	void SetGCAlphaModel()
	{
     //   Exhaustive et = (Exhaustive)GameObject.Find("ET").gameObject.GetComponent(typeof(Exhaustive));

       // PredictCuePosition.Instance.SetModel(CBFunction, alphaFunction);
      //  PredictCuePosition.Instance.calculatePredictPosition(et.currentSU);
		
	}

}
#endif