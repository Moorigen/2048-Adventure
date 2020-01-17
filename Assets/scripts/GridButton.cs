using UnityEngine;

public class GridButton : MonoBehaviour {

    public Coords gridCoords;

    public void OnClick() {
        transform.parent.parent.GetComponent<Game2048Sc>().ChooseGridComponent(gameObject);
    }

}
