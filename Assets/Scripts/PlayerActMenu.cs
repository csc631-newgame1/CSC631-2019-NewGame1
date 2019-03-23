using UnityEngine;
using UnityEngine.UI;

public class PlayerActMenu : MonoBehaviour
{
    private GameObject actMenu;
    private GameObject[] playerStats;
    private Button[] buttons;
    // Button indexes
    private const int MOVE = 0;
    private const int ACT = 1;
    private const int POTION = 2;
    private const int WAIT = 3;
    private const int ACTION1 = 4;
    private const int ACTION2 = 5;
    private const int BLANK = 6;
    private const int BACK = 7;

    private bool isPlayerActMenuActive = false;

    public void SetPlayerActMenuActive(bool active, GameAgentAction[] actions = null) {
        if (!active || actions == null) {
            SetButtonsToBattleMenu();
            isPlayerActMenuActive = false;
        } else if (active && actions != null) {
            SetButtons(actions);
            isPlayerActMenuActive = true;
        }
    }

    public bool IsPlayerActMenuActive() {
        return isPlayerActMenuActive;
    }

    public void init() {
        actMenu = GameObject.FindGameObjectWithTag("PlayerActMenu");
        playerStats = GameObject.FindGameObjectsWithTag("PlayerStats");
        buttons = actMenu.GetComponentsInChildren<Button>(true);
        SetPlayerActMenuActive(false);
    }

    public void UpdatePlayerStatsMenu(int position, string name, GameAgentStats stats) {
        if (position < playerStats.Length) {
            string hpString = stats.currentHealth.ToString() + "/" + stats.maxHealth.ToString();
            string mpString = stats.currentMagicPoints.ToString() + "/" + stats.maxMagicPoints.ToString();

            playerStats[position].GetComponentInChildren<Text>().text = name;
            FindObjectwithTag("Level", playerStats[position]).transform.GetChild(0).gameObject.GetComponentInChildren<Text>().text = stats.level.ToString();
            FindObjectwithTag("HP", playerStats[position]).transform.GetChild(0).gameObject.GetComponentInChildren<Text>().text = hpString;
            FindObjectwithTag("MP", playerStats[position]).transform.GetChild(0).gameObject.GetComponentInChildren<Text>().text = mpString;
        }
    }

    public void SetButtonsToBattleMenu() {
        buttons[MOVE].gameObject.SetActive(true);
        buttons[ACT].gameObject.SetActive(true);
        buttons[POTION].gameObject.SetActive(true);
        buttons[WAIT].gameObject.SetActive(true);

        buttons[ACTION1].gameObject.SetActive(false);
        buttons[ACTION2].gameObject.SetActive(false);
        buttons[BLANK].gameObject.SetActive(false);
        buttons[BACK].gameObject.SetActive(false);
    }

    public void SetActButtons() {
        buttons[MOVE].gameObject.SetActive(false);
        buttons[ACT].gameObject.SetActive(false);
        buttons[POTION].gameObject.SetActive(false);
        buttons[WAIT].gameObject.SetActive(false);

        buttons[ACTION1].gameObject.SetActive(true);
        buttons[ACTION2].gameObject.SetActive(true);
        buttons[BLANK].gameObject.SetActive(true);
        buttons[BACK].gameObject.SetActive(true);
    }

    private void SetButtons(GameAgentAction[] actions) {
        int buttonIndex = ACTION1;
        foreach (GameAgentAction action in actions) {
            switch (action) {
                case GameAgentAction.MeleeAttack:
                    if (buttonIndex < buttons.Length) {
                        buttons[buttonIndex].GetComponentInChildren<Text>().text = "ATTACK";
                        buttonIndex++;
                            }
                    break;
                case GameAgentAction.Taunt:
                    if (buttonIndex < buttons.Length) {
                        buttons[buttonIndex].GetComponentInChildren<Text>().text = "TAUNT";
                        buttonIndex++;
                    }
                    break;
                case GameAgentAction.RangedAttack:
                    if (buttonIndex < buttons.Length) {
                        buttons[buttonIndex].GetComponentInChildren<Text>().text = "SHOOT";
                        buttonIndex++;
                    }
                    break;
                case GameAgentAction.RangedAttackMultiShot:
                    if (buttonIndex < buttons.Length) {
                        buttons[buttonIndex].GetComponentInChildren<Text>().text = "MULTISHOT";
                        buttonIndex++;
                    }
                    break;
                case GameAgentAction.MagicAttackSingleTarget:
                    if (buttonIndex < buttons.Length) {
                        buttons[buttonIndex].GetComponentInChildren<Text>().text = "L BOLT";
                        buttonIndex++;
                    }
                    break;
                case GameAgentAction.MagicAttackAOE:
                    if (buttonIndex < buttons.Length) {
                        buttons[buttonIndex].GetComponentInChildren<Text>().text = "L STORM";
                        buttonIndex++;
                    }
                    break;
                case GameAgentAction.Heal:
                    if (buttonIndex < buttons.Length) {
                        buttons[buttonIndex].GetComponentInChildren<Text>().text = "HEAL";
                        buttonIndex++;
                    }
                    break;
            }
        }

        buttons[BLANK].GetComponentInChildren<Text>().text = "";
        buttons[BACK].GetComponentInChildren<Text>().text = "BACK";

        SetActButtons();
    }

    private GameObject FindObjectwithTag(string _tag, GameObject parent) {
        return GetChildObject(parent, _tag);
    }

    private GameObject GetChildObject(GameObject parent, string _tag) {
        for (int i = 0; i < parent.transform.childCount; i++) {
            Transform child = parent.transform.GetChild(i);
            if (child.tag == _tag) {
                return child.gameObject;
            }
            if (child.childCount > 0) {
                GetChildObject(child.gameObject, _tag);
            }
        }

        return null;
    }
}
