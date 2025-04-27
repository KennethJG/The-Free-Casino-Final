using UnityEngine;
using TMPro;

public class PokerInputUI : MonoBehaviour
{
    public GameObject promptTextPrefab;
    public Transform playerHandCanvas;
    public TextMeshProUGUI resultText;

    private GameObject currentPrompt;
    private bool isPlayerTurn = false;
    private TexasHoldemManager gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<TexasHoldemManager>();
    }

    void Update()
    {
        if (!isPlayerTurn) return;

        if (Input.GetKeyDown(KeyCode.B))
        {
            if (gameManager.IsBetActive())
                gameManager.PlayerRaises(50);
            else
                gameManager.PlayerBets(50);
            EndPlayerTurn();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            if (gameManager.IsBetActive())
                gameManager.PlayerCalls();
            else
                gameManager.PlayerChecks();
            EndPlayerTurn();
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            gameManager.PlayerFolds();
            EndPlayerTurn();
        }
    }

    public void BeginPlayerTurn(bool isBetActive)
    {
        if (gameManager.IsPlayerFolded())
        {
            Debug.Log("Player already folded, skipping their input.");
            isPlayerTurn = false;
            return;
        }

        isPlayerTurn = true;

        if (!isBetActive)
        {
            ShowPrompt("AI 2 Raised $50\n<color=green>B</color> = Raise($50)\n<color=black>C</color> = Call\n<color=red>F</color> = Fold");
        }
        else
        {
            ShowPrompt("Your Turn\n<color=green>B</color> = Bet($50)\n<color=black>C</color> = Check\n<color=red>F</color> = Fold");
        }
    }

    public void EndPlayerTurn()
    {
        isPlayerTurn = false;
        if (currentPrompt != null)
            Destroy(currentPrompt);
    }

    void ShowPrompt(string message)
    {
        if (currentPrompt != null)
            Destroy(currentPrompt);

        currentPrompt = Instantiate(promptTextPrefab, playerHandCanvas);
        RectTransform rt = currentPrompt.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(-300, -400);
        rt.localRotation = Quaternion.identity;
        rt.localScale = Vector3.one;

        TextMeshProUGUI tmp = currentPrompt.GetComponent<TextMeshProUGUI>();
        tmp.text = message;
    }
}
