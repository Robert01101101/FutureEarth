using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    ///////////////////////////////////////////////////////// PUBLIC INTERFACE /////////////////////////////////////////
    public static void AddTreeToList(Tree tree) { treeList.Add(tree);  }

    public static void AddWaterFilterToList (WaterFilter waterFilter) { 
        waterFilterList.Add(waterFilter);
        if (waterFilterList.Count == 1) PlayerCtrl.playerCtrl.PlayClippyAudio(16);
    }

    public static void AddPartToList(PartType part) { sparePartList.Add(part); }

    public static int GetTreeCount() { return treeList.Count; }

    public static int GetWaterFilterCount() { return waterFilterList.Count; }

    public static int GetPartCount() { return sparePartList.Count; }

    public static void RemovePartsFromList()
    {
        for (int i=0; i<5; i++)
        {
            sparePartList.RemoveAt(sparePartList.Count - 1);
        }
    }
}
