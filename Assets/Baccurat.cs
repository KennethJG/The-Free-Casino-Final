using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class BaccaratManager : MonoBehaviour
{
    public GameObject cardPrefab;
    public Sprite[] cardSprites;
    public Transform playerHand, bankerHand;
    public Transform table;
    public BaccaratUIInput uiInput;
    public TextMeshProUGUI resultText;
    public TextMeshProUGUI betAmountText;


    private List<Sprite> deck;
    private List<int> playerValues = new List<int>();
    private List<int> bankerValues = new List<int>();

    public enum BetSide { None, Player, Banker, Tie }
    public BetSide playerBet = BetSide.None;

    private int betAmount = 0;
    private bool choosingBetAmount = false;

    void Start()
    {
        if (MoneyManager.instance != null)
            MoneyManager.instance.UpdateUI();

        uiInput.ShowBetPrompt();
    }

    public void SetPlayerBet(BetSide bet)
    {
        playerBet = bet;
        choosingBetAmount = true;
        UpdateBetUI();
    }

    void Update()
    {
        if (choosingBetAmount)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                betAmount += 10;
                UpdateBetUI();
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                betAmount = Mathf.Max(10, betAmount - 10);
                UpdateBetUI();
            }
            else if (Input.GetKeyDown(KeyCode.Return))
            {
                if (MoneyManager.instance != null)
                {
                    if (MoneyManager.instance.GetBalance() >= betAmount)
                    {
                        MoneyManager.instance.SubtractMoney(betAmount);
                        choosingBetAmount = false;
                        StartGame();
                    }
                    else
                    {
                        resultText.text = "❌ Not enough money!";
                    }
                }
            }
        }
    }
    public void SetPlayerBet(BetSide bet, int amount)
    {
        playerBet = bet;
        betAmount = amount;
        StartGame();
    }

    void UpdateBetUI()
    {
        if (betAmountText != null)
        {
            betAmountText.text = $"Betting ${betAmount} on {playerBet}";
        }
    }

    void StartGame()
    {
        if (playerBet == BetSide.None)
        {
            Debug.LogWarning("⚠️ Please place a bet before playing.");
            return;
        }

        resultText.text = "";
        ClearHands();
        deck = new List<Sprite>(cardSprites);

        DealCard(playerHand, playerValues);
        DealCard(bankerHand, bankerValues);
        DealCard(playerHand, playerValues);
        DealCard(bankerHand, bankerValues);

        StartCoroutine(ResolveRound());
    }

    void ClearHands()
    {
        foreach (Transform card in playerHand) Destroy(card.gameObject);
        foreach (Transform card in bankerHand) Destroy(card.gameObject);
        playerValues.Clear();
        bankerValues.Clear();
    }

    void DealCard(Transform hand, List<int> values)
    {
        int index = Random.Range(0, deck.Count);
        Sprite chosen = deck[index];
        deck.RemoveAt(index);

        GameObject card = Instantiate(cardPrefab, hand);
        card.GetComponent<SpriteRenderer>().sprite = chosen;

        float xOffset = (hand == playerHand) ? -0.5f : 0.25f;
        float spacing = 0.1f * hand.childCount;

        Vector3 cardBase = table.position + new Vector3(xOffset + spacing, 0.86f, 0);
        card.transform.position = cardBase;
        card.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        card.transform.rotation = Quaternion.Euler(-90, 0, 0);

        values.Add(GetCardValue(chosen));
    }

    int GetCardValue(Sprite card)
    {
        string name = card.name.ToLower();
        if (name.Contains("01")) return 1;
        if (name.Contains("02")) return 2;
        if (name.Contains("03")) return 3;
        if (name.Contains("04")) return 4;
        if (name.Contains("05")) return 5;
        if (name.Contains("06")) return 6;
        if (name.Contains("07")) return 7;
        if (name.Contains("08")) return 8;
        if (name.Contains("09")) return 9;
        return 0;
    }

    int HandTotal(List<int> hand)
    {
        int sum = 0;
        foreach (int v in hand) sum += v;
        return sum % 10;
    }

    IEnumerator ResolveRound()
    {
        yield return new WaitForSeconds(1f);

        int playerTotal = HandTotal(playerValues);
        int bankerTotal = HandTotal(bankerValues);

        if ((playerTotal == 8 || playerTotal == 9) || (bankerTotal == 8 || bankerTotal == 9))
        {
            yield return ShowResults(playerTotal, bankerTotal);
            yield break;
        }

        if (playerTotal <= 5)
        {
            DealCard(playerHand, playerValues);
            playerTotal = HandTotal(playerValues);
        }

        yield return new WaitForSeconds(1f);

        if (bankerValues.Count == 2)
        {
            int thirdPlayerCard = (playerValues.Count == 3) ? playerValues[2] : -1;
            if (bankerTotal <= 2) DealCard(bankerHand, bankerValues);
            else if (bankerTotal == 3 && thirdPlayerCard != 8) DealCard(bankerHand, bankerValues);
            else if (bankerTotal == 4 && thirdPlayerCard >= 2 && thirdPlayerCard <= 7) DealCard(bankerHand, bankerValues);
            else if (bankerTotal == 5 && thirdPlayerCard >= 4 && thirdPlayerCard <= 7) DealCard(bankerHand, bankerValues);
            else if (bankerTotal == 6 && thirdPlayerCard >= 6 && thirdPlayerCard <= 7) DealCard(bankerHand, bankerValues);
        }

        yield return new WaitForSeconds(1f);
        yield return ShowResults(HandTotal(playerValues), HandTotal(bankerValues));
    }

    IEnumerator ShowResults(int playerTotal, int bankerTotal)
    {
        string outcome;
        Debug.Log($"PLAYER total: {playerTotal}, BANKER total: {bankerTotal}");

        if (playerTotal > bankerTotal) outcome = "Player";
        else if (bankerTotal > playerTotal) outcome = "Banker";
        else outcome = "Tie";

        // 🧮 Apply money logic and show UI result
        if ((playerBet == BetSide.Player && outcome == "Player") ||
            (playerBet == BetSide.Banker && outcome == "Banker"))
        {
            MoneyManager.instance.AddMoney(betAmount * 2);
            Debug.Log($"✅ You won your bet on {outcome}!");
            if (resultText != null)
                resultText.text = $"✅ You won your bet on <b>{outcome}</b>!\n+${betAmount * 2}";
        }
        else if (outcome == "Tie")
        {
            MoneyManager.instance.AddMoney(betAmount);
            Debug.Log("🟡 It's a tie. Your bet is returned.");
            if (resultText != null)
                resultText.text = "🟡 It's a tie. Your bet is returned.";
        }
        else
        {
            Debug.Log("❌ You lost your bet.");
            if (resultText != null)
                resultText.text = $"❌ You lost your bet on <b>{playerBet}</b>.";
        }

        yield return new WaitForSeconds(2f);
        playerBet = BetSide.None;
        uiInput.ShowBetPrompt(); // Restart with prompt
    }
}
