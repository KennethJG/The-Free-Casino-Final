using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Make sure you have this if using TextMeshProUGUI

public class SlotManager : MonoBehaviour
{
    private int?[] results = new int?[3];
    private int wheelsReported = 0;

    public TextMeshProUGUI resultText;
    public TextMeshProUGUI betPromptText; // 🎯 new for betting prompt
    public int currentBet = 10; // Default starting bet

    private bool roundEnded = false;
    private bool awaitingBet = true; // 🎯 track if waiting for bet selection

    void Start()
    {
        ShowBetPrompt();
    }

    void Update()
    {
        if (awaitingBet)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                currentBet += 10;
                UpdateBetPrompt();
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (currentBet > 10)
                    currentBet -= 10;
                UpdateBetPrompt();
            }
            else if (Input.GetKeyDown(KeyCode.Return))
            {
                if (MoneyManager.instance.CanAfford(currentBet))
                {
                    awaitingBet = false;
                    betPromptText.text = "";
                    StartSpinning(); // 🎯 Start the wheels spinning
                }
                else
                {
                    betPromptText.text = "❌ Not enough money to bet!";
                }
            }
        }
        else if (roundEnded)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Play again
            }
            else if (Input.GetKeyDown(KeyCode.Q))
            {
                SceneManager.LoadScene("Casino"); // Go back to casino
            }
        }
    }

    public void ReportResult(WheelRotate wheel, int symbol)
    {
        if (awaitingBet) return; // 🛑 Don't accept spins before bet is placed

        if (wheel.name.Contains("1")) results[0] = symbol;
        else if (wheel.name.Contains("2")) results[1] = symbol;
        else if (wheel.name.Contains("3")) results[2] = symbol;

        wheelsReported++;

        if (wheelsReported >= 3)
        {
            CheckResults();
            wheelsReported = 0;
            results = new int?[3];
        }
    }

    void CheckResults()
    {
        int a = results[0].Value;
        int b = results[1].Value;
        int c = results[2].Value;

        Debug.Log($"RESULTS → Wheel1: {a}, Wheel2: {b}, Wheel3: {c}");

        if (a == b && b == c)
        {
            int payout = currentBet * 50;
            MoneyManager.instance.AddMoney(payout);
            ShowResult($"🎉 Jackpot! Three of a kind!\nYou won ${payout}!");
        }
        else if (a == b || b == c || a == c)
        {
            int payout = currentBet;
            MoneyManager.instance.AddMoney(payout);
            ShowResult($"⚡ Two symbols match!\nYou got your bet back (${payout})!");
        }
        else
        {
            ShowResult($"❌ No match. You lost your bet (${currentBet}).");
        }
    }

    void ShowResult(string message)
    {
        if (resultText != null)
        {
            resultText.text = $"{message}\n\nPress <color=yellow>(E)</color> to Play Again\nPress <color=red>(Q)</color> to Return to Casino";
        }
        else
        {
            Debug.LogWarning("⚠️ resultText not assigned!");
        }

        roundEnded = true;
    }

    void ShowBetPrompt()
    {
        awaitingBet = true;
        roundEnded = false;
        currentBet = 10; // Reset bet default

        if (betPromptText != null)
        {
            betPromptText.text = $"How much would you like to bet?\n<color=green>↑ / ↓</color> to adjust\n<color=yellow>ENTER</color> to confirm\n\nCurrent Bet: ${currentBet}";
        }
    }

    void UpdateBetPrompt()
    {
        if (betPromptText != null)
        {
            betPromptText.text = $"How much would you like to bet?\n<color=green>↑ / ↓</color> to adjust\n<color=yellow>ENTER</color> to confirm\n\nCurrent Bet: ${currentBet}";
        }
    }

    void StartSpinning()
    {
        MoneyManager.instance.SubtractMoney(currentBet);

        // Here you would call your wheels to start spinning
        // Example: 
        // wheel1.StartSpin();
        // wheel2.StartSpin();
        // wheel3.StartSpin();

        Debug.Log($"🎰 Started spinning with bet: ${currentBet}");
    }
}
