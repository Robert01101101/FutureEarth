using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

/// <summary>
/// Responsible for all logic related to the game. 
/// Uses singleton pattern - all properties & methods are accessibly through: GameCtrl.gameCtrl or for static methods: GameCtrl.GetPartCount() (example)
/// 
/// Features:
/// - Track game variables such as amount of trees grown (move to GameCtrl)
/// - Control non-spatial audio
/// - Control clippy priming sequence (adio & things like enabling interactions at the right time)
/// </summary> 

public class GameCtrl : MonoBehaviour
{
    //Singleton gameCtrl instance
    public static GameCtrl gameCtrl;
    

    //Tree health management
    private static List<Tree> treeList = new List<Tree>();
    private static List<WaterFilter> waterFilterList = new List<WaterFilter>();
    private static List<PartType> sparePartList = new List<PartType>();
    private bool firstTreeDeath = true;

    //Other
    [HideInInspector] public static lb_BirdController birdCtrl;
    [HideInInspector] public static SpawnEnemy spawnEnemy;
    private static List<GameObject> garbagePiles;

    [Space(10)]
    [Header("Environment Mood")]
    public Material skyBox, waterMat;
    public Color[] startColors, endColors, waterColors;
    public ParticleSystem dust1, dust2;
    public PostProcessVolume ppBefore, ppAfter;
    public Light sun;


    //Singleton pattern - only one instance that is accessible from anywhere though PlayerCtrl.playerCtrl
    //from: https://riptutorial.com/unity3d/example/14518/a-simple-singleton-monobehaviour-in-unity-csharp
    void Awake()
    {
        if (gameCtrl == null)
        {
            gameCtrl = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else { Destroy(this); }
    }

    private void Start()
    {
        birdCtrl = GetComponent<lb_BirdController>();
        spawnEnemy = GetComponent<SpawnEnemy>();
        ResetEnvironmentMood();
        //Find all garbage piles
        garbagePiles = new List <GameObject>(GameObject.FindGameObjectsWithTag("Garbage"));
    }

    private void Update()
    {
        MonitorTreeHealth();
    }

    ///////////////////////////////////////////////////////// TREE HEALTH /////////////////////////////////////////
    private void MonitorTreeHealth()
    {
        int unsustainableTreesCount = treeList.Count - waterFilterList.Count*10;
        if (unsustainableTreesCount > 0)
        {
            Debug.Log("tooManyTrees");

            //Too many trees for the water filters -> begin killing off extra trees (starting with the most recent ones)
            int count = treeList.Count;
            for (int i=0; i<unsustainableTreesCount; i++)
            {
                Tree curTree = treeList[count - 1 - i];
                curTree.StartDeath();
                treeList.Remove(curTree);

                if (firstTreeDeath)
                {
                    firstTreeDeath = false;
                    StartCoroutine(FirstTreeDeath());
                }
            }
        }
    }

    IEnumerator FirstTreeDeath()
    {
        yield return new WaitForSeconds(9);
        PlayerCtrl.playerCtrl.PlayClippyAudio(11);
    }

    ///////////////////////////////////////////////////////// Environment Change /////////////////////////////////////////
    private void ResetEnvironmentMood() {
        SetEnvironment(0);
        StartCoroutine(SkyLerp());
    }

    IEnumerator SkyLerp()
    {
        yield return new WaitForSeconds(30);

        float time = 60;
        float elapsedTime = 0;

        while (elapsedTime < time)
        {
            SetEnvironment(elapsedTime / time);

            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    //Sets the mood of the environment (skybox, fog, dust, water). Input a value between 0 (beginning: grim) and 1 (ending: bright).
    int startDust = 600;
    int endDust = 0;
    int startFogLimit = 300;
    int endFogLimit = 1250;
    int startFog = 0;
    int endFog = 50;
    private void SetEnvironment (float lerp)
    {
        Color horizonCol = Color.Lerp(startColors[1], endColors[1], lerp);
        //sky
        skyBox.SetColor("_TopColor", Color.Lerp(startColors[0], endColors[0], lerp));
        skyBox.SetColor("_HorizonColor", horizonCol);
        skyBox.SetColor("_BottomColor", Color.Lerp(startColors[2], endColors[2], lerp));
        //fog
        RenderSettings.fogColor = horizonCol;
        RenderSettings.fogEndDistance = Util.mapVal(lerp, 0, 1, startFogLimit, endFogLimit);
        RenderSettings.fogStartDistance = Util.mapVal(lerp, 0, 1, startFog, endFog);
        //dust
        int dustCount = (int)Mathf.SmoothStep(startDust, endDust, lerp);
        dust1.maxParticles = dustCount;
        dust2.maxParticles = dustCount / 12;
        //water
        waterMat.SetColor("_Color", Color.Lerp(waterColors[0], waterColors[1], lerp));
        //Post Processing
        ppBefore.weight = Util.mapVal(lerp, 0, 1, 1, 0);
        ppAfter.weight = Util.mapVal(lerp, 0, 1, 0, .4f);
        //sun
        sun.intensity = Util.mapVal(lerp, 0, 1, 0.5f, 1.1f);
    }

    ///////////////////////////////////////////////////////// PUBLIC INTERFACE /////////////////////////////////////////
    public static void AddTreeToList(Tree tree) { treeList.Add(tree);  }

    public static void AddWaterFilterToList (WaterFilter waterFilter) { 
        waterFilterList.Add(waterFilter);
        if (waterFilterList.Count == 1) PlayerCtrl.playerCtrl.PlayClippyAudio(16);
    }

    public static void AddPartToList(PartType part) { sparePartList.Add(part); }

    public static int GetTreeCount() { return treeList.Count; }

    public static int GetWaterFilterCount() { return waterFilterList.Count; }

    public static int GetPartCount(PartType part) {
        int count = 0;
        foreach(PartType type in sparePartList){ if (type == part) count++; }
        return count; 
    }

    public static GameObject GetClosestGarbagePile(Transform botLocation)
    {
        float distance = 1000;
        GameObject output = null;

        //find closest garbage pile
        foreach (GameObject garbagePile in garbagePiles)
        {
            float curDistance = Vector3.Distance(botLocation.position, garbagePile.transform.position);
            if (curDistance < distance)
            {
                distance = curDistance;
                output = garbagePile;
            }
        }

        return output;
    }

    public static void RemovePileFromList(GameObject pile)
    {
        if (garbagePiles.Contains(pile)) garbagePiles.Remove(pile);
    }

    public static void RemovePartsFromList()
    {
        for (int i=0; i<5; i++)
        {
            sparePartList.RemoveAt(sparePartList.Count - 1);
        }
    }
}
