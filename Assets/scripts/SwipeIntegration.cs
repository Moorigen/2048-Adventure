/// Input registration
using UnityEngine;

public class SwipeIntegration : MonoBehaviour {

    public GameObject inventory;
    public GameObject store;
    public GameObject merchant;

    // only need start and end of a swipe
    private Vector3 startPos;
    private Vector3 endPos;
    float minDistance;  // difference between swipe and click

    void Start() {
        minDistance = Screen.height * 15 / 100; // 15% of screenheight
    }

    void Update() {
        if (inventory.activeSelf || store.activeSelf || merchant.activeSelf) {
            return;
        }
        if (Input.touchCount == 1) // if only touching with 1 finger
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began) {
                startPos = touch.position; // setting new variables
                endPos = touch.position;
            } else if (touch.phase == TouchPhase.Moved) {
                endPos = touch.position; // overwriting endpos
            } else if (touch.phase == TouchPhase.Ended) {
                // distance greater than threshhold?
                if (Mathf.Abs(endPos.x - startPos.x) > minDistance || Mathf.Abs(endPos.y - startPos.y) > minDistance) {
                    // generalising the direction
                    if (Mathf.Abs(endPos.x - startPos.x) > Mathf.Abs(endPos.y - startPos.y)) {   // mainly horizontal
                        if ((endPos.x > startPos.x)) {
                            Game2048Sc.instance.Right();
                            Player.instance.Right();
                        } else {
                            Game2048Sc.instance.Left();
                            Player.instance.Left();
                        }
                    } else {   // mainly vertical
                        if (endPos.y > startPos.y)  //Positiv oder Negativ Vertikal -> Hoch / Runter
                        {
                            Game2048Sc.instance.Up();
                            Player.instance.Up();
                        } else {
                            Game2048Sc.instance.Down();
                            Player.instance.Down();
                        }
                    }
                }
            }
        }

        // keyboard inputs
        if (Input.GetKeyDown("d")) {
            Game2048Sc.instance.Right();
            Player.instance.Right();
        } else if (Input.GetKeyDown("a")) {
            Game2048Sc.instance.Left();
            Player.instance.Left();
        } else if (Input.GetKeyDown("w")) {
            Game2048Sc.instance.Up();
            Player.instance.Up();
        } else if (Input.GetKeyDown("s")) {
            Game2048Sc.instance.Down();
            Player.instance.Down();
        }
    }
}