using System.Collections;
using UnityEngine;

public class InventoryItem : MonoBehaviour {

    [System.Serializable]
    public struct itemDataList {
        public string itemType;
        public Sprite image;
        public string tooltip;
    }

    public itemDataList[] items;

    public string type;
    string bonus;
    string tooltip;

    GameObject tooltipGO;

    void Start() {
        tooltipGO = GameObject.Find("GlobalCanvas").transform.GetChild(0).gameObject;
        foreach (itemDataList id in items) {
            if (id.itemType.Contains(type)) {
                GetComponent<UnityEngine.UI.Image>().sprite = id.image;
                tooltip = id.tooltip;
            }
        }
        if (type.Contains("Passive")) {
            bonus = type.Substring(7);
            type = "Passive";
            switch (bonus) {
                case "DualSwords":
                    FightController.damageBonus += 1;
                    break;
                case "Greatsword":
                    FightController.damageAmplifier += 0.1f;
                    break;
                case "Armor":
                    FightController.damageResitance += 0.05f;
                    break;
                case "Boots":
                    FightController.dodgeChance += 0.1f;
                    break;
                default:
                    break;
            }
        }
    }

    public void UseItem() {
        switch (type) {
            case "HealthPot":
                FightController.instance.TakeDamage(-15);
                break;
            case "Passive":
                return;
            default:
                Game2048Sc.instance.gridInteraction = type;
                break;
        }
        Destroy(gameObject);
    }

    public void RemoveItem() {
        switch (bonus) {
            case "DualSwords":
                FightController.damageBonus -= 1;
                break;
            case "Greatsword":
                FightController.damageAmplifier -= 0.1f;
                break;
            case "Armor":
                FightController.damageResitance -= 0.05f;
                break;
            case "Boots":
                FightController.dodgeChance -= 0.1f;
                break;
            default:
                break;
        }
        Destroy(gameObject);
    }

    public void GetTooltip() {
        tooltipGO.SetActive(true);
        tooltipGO.GetComponentInChildren<UnityEngine.UI.Text>().text = tooltip;
        StopCoroutine(RemoveTooltip());
        StartCoroutine(RemoveTooltip());
    }

    IEnumerator RemoveTooltip() {
        yield return new WaitForSeconds(3f);
        tooltipGO.SetActive(false);
    }
}
