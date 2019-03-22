using UnityEngine;
using UnityEngine.UI;

public class PlayerActMenu : MonoBehaviour
{
    private GameObject actMenu;
    private Button[] buttons;

    public void SetPlayerActMenuActive(bool active) {
        actMenu.gameObject.SetActive(active);
        Debug.Log("Active state: " + actMenu.gameObject.activeSelf);
    }

    public void init() {
        actMenu = GameObject.FindGameObjectWithTag("PlayerActMenu");
        SetPlayerActMenuActive(false);
        buttons = actMenu.GetComponentsInChildren<Button>(true);
    }

    public void SetButtons(GameAgentAction[] actions) {
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
