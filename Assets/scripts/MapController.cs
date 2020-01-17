using UnityEngine;
using System.Collections.Generic;

public class MapController : MonoBehaviour {

    public static MapController instance;

    void Awake() {
        if (instance != null) {
            Debug.LogWarning("Multiple MapControllers");
        }
        instance = this;
    }

    public GameObject mapTile;

    [Header("MapgenerationValues")]
    public float scale = 25f;

    public float grassChance = 0.7f;
    public float caveChance = 0.72f;
    public float mountainChance = 1f;

    public static int offsetX = 0;
    public static int offsetY = 0;

    public Dictionary<string, string> tileAllocation;

    public void Restart() {
        Start();

        offsetX = Random.Range(-999999, 999999);
        offsetY = Random.Range(-999999, 999999);

    }

    private void Start() {
        for (int i = 0; i < transform.childCount; i++) {
            Destroy(transform.GetChild(i).gameObject);
        }
        tileAllocation = new Dictionary<string, string>();
        GenerateTiles();
    }

    void Update() {
        Vector2 playerCords = Player.instance.playerCoords;
        GenerateTiles();
        if (tileAllocation[playerCords.x + "," + playerCords.y] == "Mountain") {
            Debug.Log("moving player out of mountain");
            Player.instance.playerCoords.x += Mathf.RoundToInt(Random.Range(-3f, 3f));
            Player.instance.playerCoords.y += Mathf.RoundToInt(Random.Range(-3f, 3f));
            playerCords = Player.instance.playerCoords;
            Player.instance.gameObject.transform.position = new Vector3(-15f, 0f, 5.93f) + playerCords.x * Vector3.right * 2 + playerCords.y * Vector3.forward * 2;
            Player.instance.targetPos = Player.instance.gameObject.transform.position;
            GenerateTiles();
        }
    }

    public void GenerateTiles() {
        Vector2 playerCords = Player.instance.playerCoords;
        for (int x = (int)playerCords.x - 5; x <= playerCords.x + 5; x++) {
            for (int y = (int)playerCords.y - 5; y <= playerCords.y + 5; y++) {

                if (!tileAllocation.ContainsKey(x + "," + y)) {
                    Vector3 pos = new Vector3(2f * x - 15f, -0.5f, 2f * y + 5.93f);
                    GameObject temp = Instantiate(mapTile, pos, Quaternion.identity, transform);
                    temp.transform.position = pos;
                    temp.name = "MapTile(" + x + ", " + y + ")";
                    float alloc = Mathf.PerlinNoise((x + offsetX) / scale, (y + offsetY) / scale);
                    string allocName = "Grass";
                    if (alloc > grassChance) {
                        allocName = "Cave";
                    }
                    if (alloc > caveChance) {
                        allocName = "Mountain";
                    }
                    tileAllocation[x + "," + y] = allocName;
                    temp.GetComponent<MapTileSc>().type = allocName;
                    temp.GetComponent<MapTileSc>().coords = new Coords(x, y);
                }
            }
        }
    }
}
