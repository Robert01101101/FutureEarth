using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCtrl : MonoBehaviour
{
    //Singleton gameCtrl instance
    public static GameCtrl gameCtrl;

    //Tree health management
    private static List<Tree> treeList = new List<Tree>();
    private static List<WaterFilter> waterFilterList = new List<WaterFilter>();

    //Other
    [HideInInspector] public static lb_BirdController birdCtrl;


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
            }
        }
    }

    ///////////////////////////////////////////////////////// PUBLIC INTERFACE /////////////////////////////////////////
    public static void AddTreeToList(Tree tree) { treeList.Add(tree);  }

    public static void AddWaterFilterToList (WaterFilter waterFilter) { waterFilterList.Add(waterFilter); }

}
