using UnityEngine;

public class MerchantShop : MonoBehaviour {

    public void OpenShop() {
        GameObject.Find("PlayerHpSlider").SetActive(false);
        Transform merchWindow = transform.GetChild(0);
        merchWindow.gameObject.SetActive(true);

        int shownItems = 0;
        for (int i = 1; i < merchWindow.childCount; i++) {
            Transform merchItem = merchWindow.GetChild(i);
            if (Random.value > Mathf.Pow(0.8f, shownItems)) {
                merchItem.gameObject.SetActive(false);
            } else {
                merchItem.gameObject.SetActive(true);
                shownItems++;
                int standartPrice = merchItem.GetComponent<StoreItem>().price;
                int discountPrice = Mathf.RoundToInt(Random.Range(standartPrice * 0.65f, standartPrice));
                merchItem.GetComponent<StoreItem>().price = discountPrice;
                merchItem.GetChild(0).GetComponent<UnityEngine.UI.Text>().text = discountPrice.ToString();
            }
        }
    }
}
