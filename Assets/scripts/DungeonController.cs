using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonController : MonoBehaviour {

    #region Singleton
    public static DungeonController instance;
    private void Awake() {
        if (instance != null) {
            Debug.LogWarning("multiple dungeon controllers");
        }
        instance = this;
    }

    #endregion

    public float lootChance = 0.25f;

    public GameObject tooltipGO;

    Player plSc;
    Transform merchWindow;
    GameObject[] merchItems;

    bool waitForFight;

    void Start() {
        plSc = GameObject.FindObjectOfType<Player>();
        merchWindow = GameObject.Find("Merchant").transform.GetChild(0);
        merchItems = new GameObject[merchWindow.childCount - 1];
        for (int i = 1; i < merchWindow.childCount; i++) {
            merchItems[i - 1] = merchWindow.GetChild(i).gameObject;
        }
    }

    public void EnterDungeon(Vector2 coords) {
        int difficulty = (int)Mathf.Clamp(coords.magnitude / 10f, 1f, Mathf.Infinity);
        Debug.Log(difficulty);
        StartCoroutine(Dungeoneering(difficulty));
    }

    public void WonBattle() {
        waitForFight = false;
    }

    IEnumerator Dungeoneering(int difficulty) {
        FightController.instance.inDungeon = true;
        for (int i = 1; i <= difficulty; i++) {
            waitForFight = true;
            FightController.instance.StartFight();
            while (waitForFight) {
                yield return null;
            }
        }

        Debug.Log("Cleared Dungeon..");

        int coinfind = Mathf.RoundToInt(Mathf.Pow(Random.Range(2f, 5f), difficulty));
        plSc.GainCoin(coinfind);
        string loot = "";
        if (Random.value * difficulty / 3f > 1f - lootChance) {
            merchWindow.gameObject.SetActive(true);
            int lootID = (int)Mathf.Clamp(Random.value * difficulty, 0, merchItems.Length);
            StoreItem itemSc = merchItems[lootID].GetComponent<StoreItem>();
            int temp = itemSc.price;
            itemSc.price = 0;
            itemSc.OnBuy();
            itemSc.price = temp;
            merchWindow.gameObject.SetActive(false);
            loot = itemSc.buyName;
            if (loot.Contains("Passive"))
                loot = loot.Substring(7);
        }
        FightController.instance.inDungeon = false;

        string tt = "You found " + coinfind + " Gold";
        tt += loot != "" ? " and " + loot : "";

        tooltipGO.SetActive(true);
        tooltipGO.GetComponentInChildren<UnityEngine.UI.Text>().text = tt;

        yield return new WaitForSeconds(2.5f);
        tooltipGO.SetActive(false);
    }

}
