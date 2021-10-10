using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.IO;

public class StartPannelController : MonoBehaviour
{
    [Header("Manager")]
    public GameObject GameManager;
    [Header("Panel相关预制体")]
    public GameObject CoursePanelPrefab;
    public GameObject OptionsPanelPrefab;

    #region debug

    private void Start()
    {
        FE_EventInfo.EventInfoManager.ManuallyInit();
        FE_EventInfo.EventInfoManager.DEBUG();

        
        //Debug.Log("streamingAssetsPath : " + Application.streamingAssetsPath);
        //Debug.Log("ApplicationBase : " + System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase);
        //Debug.Log(Application.dataPath);

        Debug.Log(System.Environment.CurrentDirectory);//E:\Unity\LudumDare48\Ludum Dare
    }

    //private void Start()
    //{
    //    EventInfo eventInfo = new EventInfo();

    //    //eventInfo.Id = "Discovery_of_oil";
    //    eventInfo.Eventtitle = "发现石油#Discovery of oil";
    //    eventInfo.Desciption = "你找到了一个人类遗留的石油精炼器，精炼器已经损坏，但你可以用自带的精炼器将其转化为能源，但这将消耗大量能源，并且这批石油的品质令人担忧#You find an oil refiner left by humans. The refiner is damaged, but you can use your own refiner to turn it into energy, but it will consume a lot of energy, and the quality of the oil is worrying";
    //    eventInfo.Icon = "Doo";


    //    eventInfo.Options = new List<Option>();
    //    {
    //        //选项1
    //        //固定部分
    //        Option option1 = new Option();
    //        option1.Label = "既然带了精炼器就没有不用的道理#Now that you have the refiner there's no reason not to use it";
    //        option1.Add_Energy = -10;


    //        //随机部分
    //        option1.RandomParts = new List<RandomPart>();

    //        //选项1第一轮随机判定
    //        RandomPart rp1 = new RandomPart();
    //        rp1.randomModifies = new List<RandomModify>();
    //        {
    //            RandomModify r1 = new RandomModify();
    //            r1.percentage = 0.2f;
    //            rp1.randomModifies.Add(r1);

    //            RandomModify r2 = new RandomModify();
    //            r2.percentage = 0.3f;
    //            r2.Add_Energy = 7;
    //            rp1.randomModifies.Add(r2);

    //            RandomModify r3 = new RandomModify();
    //            r3.percentage = 0.4f;
    //            r3.Add_Energy = 15;
    //            rp1.randomModifies.Add(r3);

    //            RandomModify r4 = new RandomModify();
    //            r4.percentage = 0.1f;
    //            r4.Add_Energy = 25;
    //            rp1.randomModifies.Add(r4);
    //        }
    //        option1.RandomParts.Add(rp1); //将第一轮加入option1的随机判定中

    //        eventInfo.Options.Add(option1);

    //        Option option2 = new Option();
    //        option2.Label = "精炼器放着不用也不会生锈#The refiner does not rust when left unused";
    //        eventInfo.Options.Add(option2);
    //    }

    //    Dictionary<string, EventInfo> EventInfoList = new Dictionary<string, EventInfo>();
    //    EventInfoList.Add("Discovery_of_oil", eventInfo);

    //    Debug.Log(LitJson.JsonMapper.ToJson(EventInfoList));
    //    File.WriteAllText(Application.persistentDataPath + "/fallendless_event.json", LitJson.JsonMapper.ToJson(EventInfoList));


    //}
    #endregion

    public void StartPannelCallback()
    {
        Instantiate<GameObject>(GameManager, Vector3.zero, Quaternion.identity);
    }

    public void Options()
    {
        //实例化
        GameObject tutorialsPanel = Instantiate<GameObject>(OptionsPanelPrefab, new Vector3((float)Screen.width * 1.5f, (float)Screen.height / 2, 0), Quaternion.identity, transform);
        //改名字
        tutorialsPanel.name = CoursePanelPrefab.name;

        //动画
        transform.DOLocalMoveX(-1920, 0.2f);
    }

    public void Course()
    {
        //实例化
        GameObject tutorialsPanel = Instantiate<GameObject>(CoursePanelPrefab, new Vector3((float)Screen.width * 1.5f, (float)Screen.height / 2, 0), Quaternion.identity, transform);
        //改名字
        tutorialsPanel.name = CoursePanelPrefab.name;

        //动画
        transform.DOLocalMoveX(-1920, 0.2f);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
