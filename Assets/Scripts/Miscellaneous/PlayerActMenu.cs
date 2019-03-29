using UnityEngine;
using UnityEngine.UI;

public class PlayerActMenu_DEPRECATED
{
    static private GameObject actMenu;
    static private Button[] buttons;

    static public void SetPlayerActMenuActive(bool active) {
        actMenu.gameObject.SetActive(active);
    }

    static public bool IsPlayerActMenuActive() {
        return actMenu.gameObject.activeSelf;
    }

    static public void init() {
		if (actMenu == null)
			actMenu = GameObject.FindWithTag("PlayerActMenu");
		
        SetPlayerActMenuActive(false);
        buttons = actMenu.GetComponentsInChildren<Button>(true);
    }

    static public void SetButtons(GameAgentAction[] actions) {
        int buttonIndex = 0;
        foreach (GameAgentAction action in actions) {
            switch (action) {
                case GameAgentAction.MeleeAttack:
                    if (buttonIndex < buttons.Length) {
                        buttons[buttonIndex].gameObject.SetActive(true);
                        buttons[buttonIndex].GetComponentInChildren<Text>().text = "Attack";
                        buttonIndex++;
                            }
                    break;
                case GameAgentAction.Taunt:
                    if (buttonIndex < buttons.Length) {
                        buttons[buttonIndex].gameObject.SetActive(true);
                        buttons[buttonIndex].GetComponentInChildren<Text>().text = "Taunt";
                        buttonIndex++;
                    }
                    break;
                case GameAgentAction.RangedAttack:
                    if (buttonIndex < buttons.Length) {
                        buttons[buttonIndex].gameObject.SetActive(true);
                        buttons[buttonIndex].GetComponentInChildren<Text>().text = "Shoot";
                        buttonIndex++;
                    }
                    break;
                case GameAgentAction.RangedAttackMultiShot:
                    if (buttonIndex < buttons.Length) {
                        buttons[buttonIndex].gameObject.SetActive(true);
                        buttons[buttonIndex].GetComponentInChildren<Text>().text = "Multishot";
                        buttonIndex++;
                    }
                    break;
                case GameAgentAction.MagicAttackSingleTarget:
                    if (buttonIndex < buttons.Length) {
                        buttons[buttonIndex].gameObject.SetActive(true);
                        buttons[buttonIndex].GetComponentInChildren<Text>().text = "L Bolt";
                        buttonIndex++;
                    }
                    break;
                case GameAgentAction.MagicAttackAOE:
                    if (buttonIndex < buttons.Length) {
                        buttons[buttonIndex].gameObject.SetActive(true);
                        buttons[buttonIndex].GetComponentInChildren<Text>().text = "L Storm";
                        buttonIndex++;
                    }
                    break;
                case GameAgentAction.Heal:
                    if (buttonIndex < buttons.Length) {
                        buttons[buttonIndex].gameObject.SetActive(true);
                        buttons[buttonIndex].GetComponentInChildren<Text>().text = "Heal";
                        buttonIndex++;
                    }
                    break;
            }
        }

        // Deactivate the rest of the buttons
        while (buttonIndex < buttons.Length) {
            buttons[buttonIndex].gameObject.SetActive(false);
            buttonIndex++;
        }
    }
}
