using UnityEngine;
using UnityEngine.UI;

public class MapTileSc : MonoBehaviour {

    public string type = "Emptyness";
    public Coords coords;

    public Sprite[] icons;
    public GameObject interactable;

    static readonly Vector3 interactableScale = new Vector3(0.1f, 0.1f, 0.1f);

    void Start() {
        GetComponent<Canvas>().worldCamera = Camera.main;
        if (type == "Emptyness") {
            Debug.LogWarning(name + "not allocated");
        } else if (type == "Grass") {
            transform.GetChild(0).GetComponent<Image>().sprite = icons[0];
            Color darkening = new Color(0f, 1f, 0f, 1f);
            darkening.g -= Mathf.Clamp(Vector2.Distance(Player.instance.playerCoords, Vector2.zero) / 250f, 0f, 0.55f);
            transform.GetChild(0).GetComponent<Image>().color = darkening;
            if (Random.value > 0.915f) {
                GameObject temp = Instantiate(interactable, transform.position + Vector3.up * 0.5f, Quaternion.Euler(30f, 40f, 320f), transform);
                temp.transform.localPosition = Vector3.up * 0.5f;
                temp.transform.localScale = interactableScale;
                temp.GetComponent<Interactable>().self = temp;
                interactable = temp;
                type = "Object";
                MapController.instance.tileAllocation[coords.x + "," + coords.y] = "Object";
            }
        } else if (type == "Mountain") {
            transform.GetChild(0).GetComponent<Image>().sprite = icons[1];
        } else if (type == "Cave") {
            transform.GetChild(0).GetComponent<Image>().sprite = icons[2];
        }
    }

    public GameObject Interact() {
        return interactable;
    }
}
