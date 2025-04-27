using UnityEngine;
using TMPro;

public class BaccaratUIInput : MonoBehaviour
{
    public GameObject promptTextPrefab;
    public Transform canvasTarget;
    public BaccaratManager manager;

    private GameObject currentPrompt;
    private bool awaitingBetAmount = false;
    private BaccaratManager.BetSide selectedBet;
    private int betAmount = 100;

    void Update()
    {
        if (manager.playerBet != BaccaratManager.BetSide.None && !awaitingBetAmount) return;

        if (!awaitingBetAmount)
        {
            if (Input.GetKeyDown(KeyCode.B))
                PrepareBet(BaccaratManager.BetSide.Banker);
            else if (Input.GetKeyDown(KeyCode.P))
                PrepareBet(BaccaratManager.BetSide.Player);
            else if (Input.GetKeyDown(KeyCode.T))
                PrepareBet(BaccaratManager.BetSide.Tie);
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                betAmount += 50;
                ShowBetAdjustPrompt();
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) && betAmount > 50)
            {
                betAmount -= 50;
                ShowBetAdjustPrompt();
            }
            else if (Input.GetKeyDown(KeyCode.Return))
            {
                if (MoneyManager.instance != null && MoneyManager.instance.CanAfford(betAmount))
                {
                    MoneyManager.instance.SubtractMoney(betAmount);
                    manager.SetPlayerBet(selectedBet, betAmount);
                    awaitingBetAmount = false;
                    ClearPrompt();
                }
                else
                {
                    ShowPrompt("❌ Not enough funds!");
                }
            }
        }
    }

    void PrepareBet(BaccaratManager.BetSide side)
    {
        selectedBet = side;
        awaitingBetAmount = true;
        betAmount = 100;
        ShowBetAdjustPrompt();
    }

    void ShowBetAdjustPrompt()
    {
        ShowPrompt($"Bet on <b>{selectedBet}</b>\nUse ↑ ↓ to set amount\nCurrent: ${betAmount}\nPress ENTER to confirm");
    }

    void ShowPrompt(string message)
    {
        if (currentPrompt != null)
            Destroy(currentPrompt);

        currentPrompt = Instantiate(promptTextPrefab, canvasTarget);
        RectTransform rt = currentPrompt.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(-600, -400);
        rt.localScale = Vector3.one;

        TextMeshProUGUI tmp = currentPrompt.GetComponent<TextMeshProUGUI>();
        tmp.text = message;
    }

    void ClearPrompt()
    {
        if (currentPrompt != null)
            Destroy(currentPrompt);
    }

    public void ShowBetPrompt()
    {
        ClearPrompt();

        currentPrompt = Instantiate(promptTextPrefab, canvasTarget);
        RectTransform rt = currentPrompt.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(-600  , -400);
        rt.localScale = Vector3.one;

        TextMeshProUGUI tmp = currentPrompt.GetComponent<TextMeshProUGUI>();
        tmp.text = "<color=green>B</color> = Banker\n<color=blue>P</color> = Player\n<color=yellow>T</color> = Tie\n<color=gray>↑↓</color> = Adjust Bet\n<color=white>Enter</color> = Confirm";
    }
}
