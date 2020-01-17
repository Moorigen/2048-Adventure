using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FightController : MonoBehaviour {

    public static FightController instance;
    private void Awake() {
        if (instance != null) {
            Debug.LogWarning("multiple fight controllers");
        }
        instance = this;
    }

    public GameObject inventory;
    public Slider enemyHpSl;
    public Slider playerHpSl;
    public GameObject popUpNum;

    public int damageInput = -1;

    public static int damageBonus = 0;
    public static float damageAmplifier = 1f;
    public static float damageResitance = 0f;
    public static float dodgeChance = 0f;

    public bool inDungeon = false;

    int enemyLevel;
    int enemyMaxHP;
    int enemyHP;

    int playerHP;
    int playerMaxHP = 25;

    void Start() {
        LevelUp(1);
    }

    public void StartFight() {
        Player.inCombat = true;
        Game2048Sc.instance.gridInteraction = "Combat";

        enemyLevel = Mathf.RoundToInt(Player.instance.level * Mathf.Pow(Vector2.Distance(Player.instance.playerCoords, Vector2.zero), Player.instance.level * Random.Range(0.8f, 1.5f) / 10));
        enemyMaxHP = enemyLevel * 10;
        enemyHpSl.maxValue = enemyMaxHP;
        enemyHP = enemyMaxHP;
        enemyHpSl.value = enemyHP;
        enemyHpSl.transform.GetChild(2).GetComponent<Text>().text = enemyHP.ToString();
        damageInput = -1;

        transform.GetChild(0).gameObject.SetActive(true);
        enemyHpSl.gameObject.SetActive(true);
        StartCoroutine("CombatProcess");
    }

    IEnumerator CombatProcess() {
        while (true) {
            while (damageInput == -1) {
                yield return null;
            }
            DealDamage(damageInput);
            yield return new WaitForSeconds(0.5f);
            TakeDamage(Mathf.RoundToInt(Random.Range(1f, 3f) * enemyLevel));
            damageInput = -1;
        }
    }

    void EndFight() {
        StopCoroutine("CombatProcess");
        Player.inCombat = false;
        enemyHpSl.gameObject.SetActive(false);
        transform.GetChild(0).gameObject.SetActive(false);
        Player.instance.GainExp(Mathf.RoundToInt(enemyLevel * 1.5f));
        damageInput = -1;
        if (inDungeon)
            DungeonController.instance.WonBattle();
    }

    public void TakeDamage(int amount) {
        float dodgeAbility = dodgeChance;
        if (damageInput == 0) {
            dodgeAbility += 0.25f;
        }
        if (amount > 0 && Random.value < dodgeAbility) {
            amount = 0;
        }
        playerHP -= Mathf.RoundToInt(Mathf.Clamp(amount * (1f - damageResitance), 0f, Mathf.Infinity));
        if (playerHP <= 0) {
            GameOver();
        } else if (playerHP > playerMaxHP) {
            playerHP = playerMaxHP;
        }
        playerHpSl.value = playerHP;
        playerHpSl.transform.GetChild(2).GetComponent<Text>().text = playerHP.ToString();
        GameObject popUp = Instantiate(popUpNum, playerHpSl.transform);
        popUp.GetComponent<Text>().text = amount.ToString();
        if (amount > 0)
            popUp.GetComponent<Text>().color = Color.red;
        else
            popUp.GetComponent<Text>().color = Color.green;
        Destroy(popUp, 3f);
    }

    void DealDamage(int amount) {
        amount = Mathf.RoundToInt(amount * damageAmplifier + damageBonus);
        enemyHP -= amount;
        if (enemyHP <= 0) {
            EndFight();
        }
        enemyHpSl.value = enemyHP;
        enemyHpSl.transform.GetChild(2).GetComponent<Text>().text = enemyHP.ToString();
        GameObject popUp = Instantiate(popUpNum, enemyHpSl.transform);
        popUp.GetComponent<Text>().text = amount.ToString();
        popUp.GetComponent<Text>().color = Color.red;
        Destroy(popUp, 3f);
    }

    public void LevelUp(int lvl) {
        playerMaxHP = 10 + 15 * lvl;
        playerHP = playerMaxHP;
        playerHpSl.maxValue = playerMaxHP;
        TakeDamage(0);
    }

    void GameOver() {
        StopCoroutine("CombatProcess");
        Player.inCombat = false;
        enemyHpSl.gameObject.SetActive(false);
        transform.GetChild(0).gameObject.SetActive(false);
        Player.instance.GainExp(Mathf.RoundToInt(Vector2.Distance(Player.instance.playerCoords, Vector2.zero) / 10f));
        Player.instance.GainExp(Player.instance.gold / 2);
        Player.instance.GainCoin(-Player.instance.gold);
        Player.instance.playerCoords = Vector2Int.zero;
        Player.instance.GameOver();
        MapController.instance.Restart();
        if (inventory.transform.childCount > 1) {
            for (int i = 1; i < inventory.transform.childCount; i++) {
                Destroy(inventory.transform.GetChild(i).gameObject);
            }
        }
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Interactable")) {
            Destroy(go);
        }
        Game2048Sc.instance.ClearGrid();
        playerHP = playerMaxHP;
        TakeDamage(0);
    }

}
