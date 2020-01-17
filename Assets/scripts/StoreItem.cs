using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class StoreItem : MonoBehaviour {

    public GameObject inventoryItem;
    public Transform inventory;

    public string buyName;
    public int price;
    public string tooltip;
    public GameObject tooltipGO;

    public void OnBuy() {

        if (buyName == "GridSizePlus") {
            CheckGridBonusPrice();
            if (Player.instance.level >= price) {
                Player.instance.GainLevel(-price);
                Game2048Sc.instance.IncreaseGrid();
                CheckGridBonusPrice();
            }
            return;
        } else if (buyName == "GridSpawnBonus") {
            CheckSpawnPlusPrice();
            if (Player.instance.level >= price) {
                Player.instance.GainLevel(-price);
                Game2048Sc.instance.tileSpawnBonus += 0.75f;
                CheckSpawnPlusPrice();
            }
            return;
        }
        if (buyName == "GridSpawnBonus" || buyName == "GridSizePlus")
            return;

        if (Player.instance.gold < price)
            return;
        if (buyName == "GoldMulti") {
            Player.instance.GainCoin(-price);
            Interactable.goldMultiplier++;
        } else if (inventory.childCount < 19) {
            Player.instance.GainCoin(-price);
            GameObject temp = Instantiate(inventoryItem, inventory);
            temp.GetComponent<InventoryItem>().type = buyName;
            temp.name = buyName;
        }
    }

    IEnumerator InventoryUpdate() {
        inventory.gameObject.SetActive(true);
        yield return null;
        inventory.gameObject.SetActive(false);
    }

    public void GetTooltip() {
        tooltipGO.SetActive(true);
        tooltipGO.GetComponentInChildren<Text>().text = tooltip;
        StopAllCoroutines();
        StartCoroutine(RemoveTooltip());
    }

    IEnumerator RemoveTooltip() {
        yield return new WaitForSeconds(3f);
        tooltipGO.SetActive(false);
    }

    public void CheckSpawnPlusPrice() {
        price = (int)(Game2048Sc.instance.tileSpawnBonus / 0.75f) * 3;
        transform.GetComponentInChildren<Text>().text = price + " Levels";
    }

    public void CheckGridBonusPrice() {
        price = (Game2048Sc.instance.gridSize - 3) * 5;
        transform.GetComponentInChildren<Text>().text = price + " Levels";
    }
}
