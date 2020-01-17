using UnityEngine;
using UnityEngine.UI;

public class Interactable : MonoBehaviour {

    [System.Serializable]
    public struct typeList {
        public string type;
        public Sprite img;
        public int chance;
    }
    public typeList[] types;

    public static int goldMultiplier = 1;

    public string type = "Coin";
    public GameObject self;

    void Start() {
        GetComponent<Canvas>().worldCamera = Camera.main;
        int rngMax = 0;
        foreach (typeList variation in types) {
            rngMax += variation.chance;
        }
        int rngGen = Random.Range(0, rngMax);
        foreach (typeList variation in types) {
            if (rngGen >= 0) {
                type = variation.type;
                GetComponent<Image>().sprite = variation.img;
            }
            rngGen -= variation.chance;
        }
    }

    public void Interact(Player plSc) {
        switch (type) {
            case "Coin":
                int gain = Mathf.RoundToInt(Mathf.Pow(Random.Range(2f, 15f), goldMultiplier * Mathf.Sqrt(Player.instance.level) / 10));
                plSc.GainCoin(gain);
                Destroy(self);
                break;

            case "Enemy":
                FightController.instance.StartFight();
                Destroy(self);
                break;

            case "Merchant":
                GameObject.Find("Merchant").GetComponent<MerchantShop>().OpenShop();
                Destroy(self);
                break;

            default:
                Debug.LogWarning("No Interaction selected");
                break;
        }
    }
}
