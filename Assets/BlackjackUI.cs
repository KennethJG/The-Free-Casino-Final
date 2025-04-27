using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BlackjackUI : MonoBehaviour
{
    public GameObject promptTextPrefab;
    public Transform canvasTarget;
    public BlackjackManager manager;
    public GameObject actionButtons;
    public TextMeshProUGUI resultText;

    private GameObject currentPrompt;
    private int currentBet = 50;
    private bool awaitingBet = true;
    private bool playerCanAct = false;

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
                UpdatePromptText();
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (currentBet > 10)
                    currentBet -= 10;
                UpdatePromptText();
            }
            else if (Input.GetKeyDown(KeyCode.Return))
            {
                if (MoneyManager.instance.CanAfford(currentBet))
                {
                    awaitingBet = false;
                    ClearPrompt();
                    manager.StartRound(currentBet);
                }
                else
                {
                    resultText.text = "❌ Not enough money!";
                }
            }
        }
        else if (playerCanAct)
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                manager.PlayerHit();
            }
            else if (Input.GetKeyDown(KeyCode.X))
            {
                playerCanAct = false;
                resultText.text = "";
                manager.PlayerStand();
            }
        }
        else if (!awaitingBet && !playerCanAct)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
            }
            else if (Input.GetKeyDown(KeyCode.Q))
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("Casino_Scene"); // <-- Replace "Casino" with your actual casino scene name
            }
        }
    }

    public void ShowBetPrompt()
    {
        awaitingBet = true;
        playerCanAct = false;
        ClearPrompt();

        currentPrompt = Instantiate(promptTextPrefab, canvasTarget);
        RectTransform rt = currentPrompt.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(-300, -400);
        rt.localScale = Vector3.one;

        UpdatePromptText();
    }

    void UpdatePromptText()
    {
        if (currentPrompt == null) return;

        TextMeshProUGUI tmp = currentPrompt.GetComponent<TextMeshProUGUI>();
        tmp.text = $"How much would you like to bet?\n<color=green>↑ / ↓</color> to adjust\n<color=yellow>ENTER</color> to confirm\n\nCurrent Bet: ${currentBet}";
    }

    void ClearPrompt()
    {
        if (currentPrompt != null)
            Destroy(currentPrompt);
    }

    public void ShowResult(string message, int playerTotal = -1, int dealerTotal = -1)
    {
        if (playerTotal >= 0 && dealerTotal >= 0)
        {
            resultText.text = $"{message}\nPlayer: {playerTotal} | Dealer: {dealerTotal}\n\nPress <color=yellow>(E)</color> to Play Again\nPress <color=red>(Q)</color> to Return to Casino";
        }
        else
        {
            resultText.text = $"{message}\n\nPress <color=yellow>(E)</color> to Play Again\nPress <color=red>(Q)</color> to Return to Casino";
        }

        playerCanAct = false;
        actionButtons.SetActive(false);

        // ❌ No automatic reset anymore
    }

    void ResetForNextRound()
    {
        resultText.text = "";
        ShowBetPrompt();
    }

    public void ShowPlayerOptions()
    {
        playerCanAct = true;
        UpdateActionPrompt();
        actionButtons.SetActive(false);
    }

    public void HidePlayerOptions()
    {
        playerCanAct = false;
        resultText.text = "";
        actionButtons.SetActive(false);
    }

    public void UpdateActionPrompt()
    {
        resultText.text = "Press <color=yellow>H</color> to Hit or <color=yellow>X</color> to Stand";
    }
}
