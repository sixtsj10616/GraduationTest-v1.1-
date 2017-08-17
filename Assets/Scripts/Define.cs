using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define : MonoBehaviour {

    //***           底座          ***//
    public const float initPlatWidth    = 50;
    public const float initPlatLength   = 40;
    public const float initPlatHeight   = 5;
    public const float initStairWidth   = 5;
    public const float initStairLength  = 5;

    //***           屋身          ***//
    public const float initEaveColumnHeight   = 11;
    public const int initColRadSegment = 7;


    //***           屋頂          ***//
    public const float initJijaHeight = 13;


    //***           主脊          ***//
    public const string TopPoint = "TopPoint";
    public const string SecPoint = "SecPoint";
    public const string ThirdPoint = "ThirdPoint";
    public const string LastPoint = "LastPoint";
    public const float mainRidgeTileHeight = 0.3f;
    public const float Do_Kun_Height = 0.5f;

    //***           線段          ***//
    public const int Large = 1000;
    public const int Medium = 500;
    public const int Low = 250;

    //***           資料          ***//
    public const string RoofDataList = "RoofDataList";
    public const string BodyDataList = "BodyDataList";
    public const string PlatformDataList = "PlatformDataList";

}
