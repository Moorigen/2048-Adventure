using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game2048Sc : MonoBehaviour {

    #region Singleton
    public static Game2048Sc instance;

    void Awake() {
        if (instance != null) {
            Debug.LogWarning("Multiple MapControllers");
        }
        instance = this;
    }
    #endregion

    public int gridSize = 4;

    public float tileSpawnBonus = 0f;

    public GameObject gridComponent;

    public int steps = 1;

    GridLayoutGroup gridLO;

    int[,] grid = null;

    public string gridInteraction;
    Coords combineFrom;

    public Image fatigueFlash;
    int fatigue = 0;

    void Start() {
        gridLO = transform.GetComponentInChildren<GridLayoutGroup>();
        if (grid == null) // If no grid exists
        {
            grid = new int[gridSize, gridSize];
            for (int i = 0; i < gridSize; i++) {
                for (int j = 0; j < gridSize; j++) {
                    grid[i, j] = 0; // we create an empty (logic) grid
                }
            }
            for (int i = 1; i < transform.GetChild(0).childCount; i++) {
                Destroy(transform.GetChild(0).GetChild(i).gameObject); // remove any existing objects from the old grid
            }
            float temp = 240f / gridSize; // calculate how big the tiles can be
            gridLO.cellSize = new Vector2(temp, temp);
            temp = 20f / (gridSize - 1);
            gridLO.spacing = new Vector2(temp, temp);
            for (int y = 1; y <= gridSize; y++) {
                for (int x = 1; x <= gridSize; x++) {
                    GameObject gridComp = Instantiate(gridComponent, transform.GetChild(0)); // and create new objects for our grid
                    gridComp.GetComponent<GridButton>().gridCoords = new Coords(x - 1, y - 1);
                }
            }
        }
        SpawnRandomNew();
        StartCoroutine(UpdateGridUI());
    }

    public void ChooseGridComponent(GameObject comp) {
        GridButton gbSc = comp.GetComponent<GridButton>();
        switch (gridInteraction) {
            case "Double":
                grid[gbSc.gridCoords.x, gbSc.gridCoords.y] *= 2;
                gridInteraction = null;
                break;
            case "Remove":
                grid[gbSc.gridCoords.x, gbSc.gridCoords.y] = 0;
                gridInteraction = null;
                break;
            case "Combine":
                if (combineFrom == null) {
                    combineFrom = gbSc.gridCoords;
                } else if (combineFrom.x != gbSc.gridCoords.x || combineFrom.y != gbSc.gridCoords.y) {
                    if (grid[combineFrom.x, combineFrom.y] == grid[gbSc.gridCoords.x, gbSc.gridCoords.y]) {
                        grid[combineFrom.x, combineFrom.y] = 0;
                        grid[gbSc.gridCoords.x, gbSc.gridCoords.y] *= 2;
                        gridInteraction = null;
                    }
                }
                break;
            case "Combat":
                FightController.instance.damageInput = grid[gbSc.gridCoords.x, gbSc.gridCoords.y];
                grid[gbSc.gridCoords.x, gbSc.gridCoords.y] = 0;
                break;
            default:
                break;
        }
        if (Player.inCombat) {
            gridInteraction = "Combat";
        }
        StartCoroutine(UpdateGridUI());
    }

    #region Input Management
    public void Left() {
        steps = 1;
        FightController.instance.damageInput = 0;
        for (int x = 0; x < gridSize; x++) {
            for (int y = 0; y < gridSize; y++) {
                if (grid[x, y] != 0) { //if the field value is 0 it's ignored
                    int tempX = x;
                    while (tempX > 0) // moving left we can ignore the leftmost column
                    {
                        if (grid[tempX - 1, y] == grid[x, y]) // is left of us the same value?
                        {
                            grid[tempX - 1, y] += grid[x, y]; // then we combine
                            grid[x, y] = 0; // and clear the tile we came from
                            tempX = -1; // and exit
                            steps++;
                        } else if (grid[tempX - 1, y] == 0) // is left of us 0
                        {
                            tempX--; // we move through and check the next
                        } else // is left of us something different we stop
                          {
                            grid[tempX, y] = grid[x, y]; // set our value on the destination
                            if (tempX != x) // and if destination is different from origin
                                grid[x, y] = 0; // we clear our origin
                            tempX = -1; // and exit
                        }
                    }
                    if (x != 0 && tempX == 0) // if everything left of us was just 0, we move to the edge
                    {
                        grid[0, y] = grid[x, y];
                        grid[x, y] = 0;
                    }
                }
            }
        }
        SpawnRandomNew(); // after every move spawn a new number
        StartCoroutine(UpdateGridUI());
        /*Debug.Log(grid[0, 0] + " " + grid[0, 1] + " " + grid[0, 2] + " " + grid[0, 3] + "\n" +
            grid[1, 0] + " " + grid[1, 1] + " " + grid[1, 2] + " " + grid[1, 3] + "\n" +
            grid[2, 0] + " " + grid[2, 1] + " " + grid[2, 2] + " " + grid[2, 3] + "\n" +
            grid[3, 0] + " " + grid[3, 1] + " " + grid[3, 2] + " " + grid[3, 3]);*/
    }

    public void Right() {
        steps = 1;
        FightController.instance.damageInput = 0;
        for (int x = gridSize - 1; x >= 0; x--) {
            for (int y = gridSize - 1; y >= 0; y--) {
                if (grid[x, y] != 0) {
                    int tempX = x;
                    while (tempX < gridSize - 1) {
                        if (grid[tempX + 1, y] == grid[x, y]) {
                            grid[tempX + 1, y] += grid[x, y];
                            grid[x, y] = 0;
                            tempX = gridSize;
                            steps++;
                        } else if (grid[tempX + 1, y] == 0) {
                            tempX++;
                        } else {
                            grid[tempX, y] = grid[x, y];
                            if (tempX != x)
                                grid[x, y] = 0;
                            tempX = gridSize;
                        }
                    }
                    if (x != gridSize - 1 && tempX == gridSize - 1) {
                        grid[tempX, y] = grid[x, y];
                        grid[x, y] = 0;
                    }
                }
            }
        }
        SpawnRandomNew();
        StartCoroutine(UpdateGridUI());
    }

    public void Up() {
        steps = 1;
        FightController.instance.damageInput = 0;
        for (int y = 0; y < gridSize; y++) {
            for (int x = 0; x < gridSize; x++) {
                if (grid[x, y] != 0) {
                    int tempY = y;
                    while (tempY > 0) {
                        if (grid[x, tempY - 1] == grid[x, y]) {
                            grid[x, tempY - 1] += grid[x, y];
                            grid[x, y] = 0;
                            tempY = -1;
                            steps++;
                        } else if (grid[x, tempY - 1] == 0) {
                            tempY--;
                        } else {
                            grid[x, tempY] = grid[x, y];
                            if (tempY != y)
                                grid[x, y] = 0;
                            tempY = -1;
                        }
                    }
                    if (y != 0 && tempY == 0) {
                        grid[x, 0] = grid[x, y];
                        grid[x, y] = 0;
                    }
                }
            }
        }
        SpawnRandomNew();
        StartCoroutine(UpdateGridUI());
    }

    public void Down() {
        steps = 1;
        FightController.instance.damageInput = 0;
        for (int y = gridSize - 1; y >= 0; y--) {
            for (int x = gridSize - 1; x >= 0; x--) {
                if (grid[x, y] != 0) {
                    int tempY = y;
                    while (tempY < gridSize - 1) {
                        if (grid[x, tempY + 1] == grid[x, y]) {
                            grid[x, tempY + 1] += grid[x, y];
                            grid[x, y] = 0;
                            tempY = gridSize;
                            steps++;
                        } else if (grid[x, tempY + 1] == 0) {
                            tempY++;
                        } else {
                            grid[x, tempY] = grid[x, y];
                            if (tempY != y)
                                grid[x, y] = 0;
                            tempY = gridSize;
                        }
                    }
                    if (y != gridSize - 1 && tempY == gridSize - 1) {
                        grid[x, tempY] = grid[x, y];
                        grid[x, y] = 0;
                    }
                }
            }
        }
        SpawnRandomNew();
        StartCoroutine(UpdateGridUI());
    }
    #endregion

    int GetGridID(int line, int column) {
        return column * gridSize + line;
    }

    IEnumerator UpdateGridUI() {
        yield return null;
        for (int x = 0; x < gridSize; x++) {
            for (int y = 0; y < gridSize; y++) {
                transform.GetChild(0).GetChild(GetGridID(x, y) + 1).GetChild(0).GetComponent<Text>().text = grid[x, y].ToString();
            }
        }
    }

    void SpawnRandomNew() {
        bool spawned = false;
        List<Vector2Int> allPositions = new List<Vector2Int>();
        for (int x = 0; x < gridSize; x++) {
            for (int y = 0; y < gridSize; y++) {
                allPositions.Add(new Vector2Int(x, y));
            }
        }

        while (allPositions.Count > 0 && spawned == false) {
            int rng = Random.Range(0, allPositions.Count);
            Vector2Int position = allPositions[rng];
            allPositions.RemoveAt(rng);
            if (grid[position.x, position.y] == 0) {
                int spawnAmount = Mathf.RoundToInt(Mathf.Pow(2f, Mathf.Round(Random.Range(0f + tileSpawnBonus, 2f + tileSpawnBonus))));
                grid[position.x, position.y] = spawnAmount;
                //BonusStuffManager.instance.AddScore(spawnAmount);
                spawned = true;
            }
        }
        if (!spawned) {
            fatigue++;
            FightController.instance.TakeDamage(fatigue);
            StartCoroutine(FatigueFlash());
        }
    }

    IEnumerator FatigueFlash() {
        for (int i = 200; i > 0; i -= 5) {
            fatigueFlash.color = new Color(1f, 0f, 0f, i / 255f);
            yield return null;
        }
    }

    public void ClearGrid() {
        grid = null;
        Start();
    }

    public void IncreaseGrid() {
        int[,] oldGrid = grid;
        gridSize++;
        grid = new int[gridSize, gridSize];
        for (int i = 0; i < gridSize; i++) {
            for (int j = 0; j < gridSize; j++) {
                int val = i < gridSize - 1 && j < gridSize - 1 ? oldGrid[i, j] : 0;
                grid[i, j] = val;
            }
        }

        for (int i = 1; i < transform.GetChild(0).childCount; i++) {
            Destroy(transform.GetChild(0).GetChild(i).gameObject);
        }
        float temp = 240f / gridSize;
        gridLO.cellSize = new Vector2(temp, temp);
        temp = 20f / (gridSize - 1);
        gridLO.spacing = new Vector2(temp, temp);
        for (int y = 1; y <= gridSize; y++) {
            for (int x = 1; x <= gridSize; x++) {
                GameObject gridComp = Instantiate(gridComponent, transform.GetChild(0));
                gridComp.GetComponent<GridButton>().gridCoords = new Coords(x - 1, y - 1);
            }
        }
        StartCoroutine(UpdateGridUI());
    }
}
