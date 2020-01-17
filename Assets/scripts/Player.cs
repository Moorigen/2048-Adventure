using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Player : MonoBehaviour {

    #region Singleton
    public static Player instance;

    void Awake() {
        if (instance != null) {
            Debug.LogWarning("More than 1 player");
        }
        instance = this;
        Load();
    }
    #endregion

    //public delegate Coords OnInteractableTrigger();
    //public OnInteractableTrigger onInteractableTriggerCallback;

    public static bool inCombat = false;

    public float moveSpeed = 3f;

    public Vector3 targetPos;
    public Vector2Int playerCoords;

    public Slider xpBar;
    public Text goldUI;

    public GameObject popUpNum;

    Vector3 velocity = Vector3.zero;
    bool hasMoved = true;

    Animator anim;

    [Header("Ingame Stats")]
    public int level = 0;
    int exp = 0;
    int maxExp = 10;

    public int gold = 0;

    void Start() {
        anim = GetComponent<Animator>();
        targetPos = transform.position;
        xpBar.transform.GetChild(2).GetComponent<Text>().text = level.ToString();
        GainExp(0);
        GainCoin(0);
    }

    public void Left() {
        if (inCombat)
            return;
        hasMoved = true;
        for (int i = 0; i < Game2048Sc.instance.steps; i++) {
            playerCoords.x--;
            if (MapController.instance.tileAllocation[playerCoords.x + "," + playerCoords.y] == "Mountain") {
                playerCoords.x++;
                return;
            } else if (MapController.instance.tileAllocation[playerCoords.x + "," + playerCoords.y] == "Object") {
                //onInteractableTriggerCallback.Invoke();
                MapController.instance.tileAllocation[playerCoords.x + "," + playerCoords.y] = "Grass";
                Interact();
            } else if (MapController.instance.tileAllocation[playerCoords.x + "," + playerCoords.y] == "Cave") {
                DungeonController.instance.EnterDungeon(playerCoords);
                return;
            }
        }
        MapController.instance.GenerateTiles();
    }

    public void Up() {
        if (inCombat)
            return;
        hasMoved = true;
        for (int i = 0; i < Game2048Sc.instance.steps; i++) {
            playerCoords.y++;
            if (MapController.instance.tileAllocation[playerCoords.x + "," + playerCoords.y] == "Mountain") {
                playerCoords.y--;
                return;
            } else if (MapController.instance.tileAllocation[playerCoords.x + "," + playerCoords.y] == "Object") {
                //onInteractableTriggerCallback.Invoke();
                MapController.instance.tileAllocation[playerCoords.x + "," + playerCoords.y] = "Grass";
                Interact();
            } else if (MapController.instance.tileAllocation[playerCoords.x + "," + playerCoords.y] == "Cave") {
                DungeonController.instance.EnterDungeon(playerCoords);
                return;
            }
        }
        MapController.instance.GenerateTiles();
    }

    public void Down() {
        if (inCombat)
            return;
        hasMoved = true;
        for (int i = 0; i < Game2048Sc.instance.steps; i++) {
            playerCoords.y--;
            if (MapController.instance.tileAllocation[playerCoords.x + "," + playerCoords.y] == "Mountain") {
                playerCoords.y++;
                return;
            } else if (MapController.instance.tileAllocation[playerCoords.x + "," + playerCoords.y] == "Object") {
                //onInteractableTriggerCallback.Invoke();
                MapController.instance.tileAllocation[playerCoords.x + "," + playerCoords.y] = "Grass";
                Interact();
            } else if (MapController.instance.tileAllocation[playerCoords.x + "," + playerCoords.y] == "Cave") {
                DungeonController.instance.EnterDungeon(playerCoords);
                return;
            }
        }
        MapController.instance.GenerateTiles();
    }

    public void Right() {
        if (inCombat)
            return;
        hasMoved = true;
        for (int i = 0; i < Game2048Sc.instance.steps; i++) {
            playerCoords.x++;
            if (MapController.instance.tileAllocation[playerCoords.x + "," + playerCoords.y] == "Mountain") {
                playerCoords.x--;
                return;
            } else if (MapController.instance.tileAllocation[playerCoords.x + "," + playerCoords.y] == "Object") {
                //onInteractableTriggerCallback.Invoke();
                MapController.instance.tileAllocation[playerCoords.x + "," + playerCoords.y] = "Grass";
                Interact();
            } else if (MapController.instance.tileAllocation[playerCoords.x + "," + playerCoords.y] == "Cave") {
                DungeonController.instance.EnterDungeon(playerCoords);
                return;
            }
        }
        MapController.instance.GenerateTiles();
    }

    void Update() {
        if (!hasMoved) {
            anim.SetBool("IsWalking", false);
            return;
        }
        anim.SetBool("IsWalking", true);
        anim.speed = Game2048Sc.instance.steps;
        targetPos = new Vector3(-15f, 0f, 5.93f) + playerCoords.x * Vector3.right * 2 + playerCoords.y * Vector3.forward * 2;
        transform.LookAt(targetPos);
        Vector3 smoothedPos = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, moveSpeed);
        transform.position = smoothedPos;
        if (Vector3.Distance(targetPos, transform.position) < 0.085f)
            hasMoved = false;
    }

    void Interact() {
        foreach (MapTileSc tile in FindObjectsOfType<MapTileSc>()) {
            if (tile.coords.x == playerCoords.x && tile.coords.y == playerCoords.y) {
                tile.interactable.GetComponent<Interactable>().Interact(this);
            }
        }
    }

    public void GainExp(int amount) {
        exp += amount;
        if (exp > maxExp) {
            GainLevel(1);
        }
        xpBar.value = exp;
        GameObject popUp = Instantiate(popUpNum, xpBar.transform);
        popUp.GetComponent<Text>().text = amount.ToString();
        popUp.GetComponent<Text>().color = Color.green;
        Destroy(popUp, 3f);
    }

    public void GainLevel(int one) {
        level += one;
        if (one > 0)
            exp -= maxExp;
        maxExp = Mathf.RoundToInt(0.6f * Mathf.Pow(level, 3) - 8f * Mathf.Pow(level, 2) + 40f * level);
        xpBar.maxValue = maxExp;
        xpBar.transform.GetChild(2).GetComponent<Text>().text = level.ToString();
        FightController.instance.LevelUp(level);
        GainExp(0);
    }

    public void GainCoin(int amount) {
        gold += amount;
        goldUI.text = gold.ToString();
    }

    public void GameOver() {
        playerCoords = Vector2Int.zero;
        targetPos = transform.position;
        hasMoved = true;
    }

    private void OnApplicationQuit() {
        Save();
    }

    private void OnApplicationPause(bool pause) {
        if (pause)
            Save();
    }

    void Save() {
        Debug.Log("Saving");
        List<int> stats = new List<int>();
        stats.Add(exp);
        stats.Add(gold);
        stats.Add(Game2048Sc.instance.gridSize);
        stats.Add(playerCoords.x);
        stats.Add(playerCoords.y);
        stats.Add(MapController.offsetX);
        stats.Add(MapController.offsetY);

        SaveLoad.Save(stats.ToArray());
    }

    void Load() {
        Debug.Log("Loading");
        int[] stats = SaveLoad.Load();
        if (stats[0] == -1) {
            return;
        }
        GainExp(stats[0]);
        GainCoin(stats[1]);
        for (int i = 4; i < stats[2]; i++) {
            Game2048Sc.instance.IncreaseGrid();
        }
        playerCoords = new Vector2Int(stats[3], stats[4]);
        MapController.offsetX = stats[5];
        MapController.offsetY = stats[6];
    }
}
