using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TexasHoldemManager : MonoBehaviour
{
    [Header("Card Prefabs")]
    public GameObject cardPrefab;
    public GameObject uiCardPrefab;

    [Header("Card Assets")]
    public Sprite[] cardSprites;
    public Sprite cardBackSprite;

    [Header("Card Locations")]
    public Transform playerHandCanvas;
    public Transform communityCards;
    public Transform table;
    public List<Transform> aiSpots;

    [Header("Game Systems")]
    public PokerInputUI pokerUI;
    public PokerBank pokerBank;

    private List<Sprite> deck;
    private List<Sprite> playerHoleCards = new List<Sprite>();
    private List<List<Sprite>> aiHoleCards = new List<List<Sprite>>();
    private List<bool> aiFolded = new List<bool>();
    private List<GameObject> aiCardObjects = new List<GameObject>();

    private List<Sprite> community = new List<Sprite>();

    private bool playerFolded = false;
    private bool isBetActive = false;
    private int totalPlayers;
    private int currentBetAmount = 0;

    private enum BettingPhase { PreFlop, Flop, Turn, River, Showdown }
    private BettingPhase currentPhase;

    void Start()
    {
        StartGame();
    }

    void StartGame()
    {
        ClearCards();
        deck = new List<Sprite>(cardSprites);
        playerHoleCards.Clear();
        community.Clear();
        aiHoleCards.Clear();
        aiFolded.Clear();
        aiCardObjects.Clear();
        playerFolded = false;
        isBetActive = false;
        currentBetAmount = 0;
        currentPhase = BettingPhase.PreFlop;

        // Setup AI hands
        foreach (var spot in aiSpots)
        {
            aiHoleCards.Add(new List<Sprite>());
            aiFolded.Add(false);

            for (int i = 0; i < 2; i++)
                DealCardToAI(aiHoleCards.Count - 1, i, true);
        }

        // Setup player hand
        for (int i = 0; i < 2; i++)
            DealUICardToPlayer(i);

        totalPlayers = aiSpots.Count + 1;

        pokerUI.BeginPlayerTurn(isBetActive);
    }

    void DealUICardToPlayer(int cardIndex)
    {
        if (deck.Count == 0) return;

        int index = Random.Range(0, deck.Count);
        Sprite chosen = deck[index];
        deck.RemoveAt(index);
        playerHoleCards.Add(chosen);

        GameObject uiCard = Instantiate(uiCardPrefab, playerHandCanvas);
        Image img = uiCard.GetComponent<Image>();
        img.sprite = chosen;

        RectTransform rt = uiCard.GetComponent<RectTransform>();
        float spacing = 120f;
        float yOffset = -300f;
        rt.anchoredPosition = new Vector2(-spacing / 2 + cardIndex * spacing, yOffset);
        rt.localRotation = Quaternion.identity;
        rt.localScale = Vector3.one;
    }

    void DealCardToAI(int aiIndex, int cardNumber, bool faceDown)
    {
        if (deck.Count == 0) return;

        int index = Random.Range(0, deck.Count);
        Sprite chosen = deck[index];
        deck.RemoveAt(index);

        aiHoleCards[aiIndex].Add(chosen);

        GameObject card = Instantiate(cardPrefab, aiSpots[aiIndex]);
        card.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        card.transform.localPosition = new Vector3(cardNumber * 0.3f, 0, 0);
        card.transform.localRotation = Quaternion.Euler(-90, 0, 0);

        card.GetComponent<SpriteRenderer>().sprite = faceDown ? cardBackSprite : chosen;
        aiCardObjects.Add(card);
    }

    void DealCardToTable()
    {
        if (deck.Count == 0) return;

        int index = Random.Range(0, deck.Count);
        Sprite chosen = deck[index];
        deck.RemoveAt(index);
        community.Add(chosen);

        GameObject card = Instantiate(cardPrefab, communityCards);
        card.GetComponent<SpriteRenderer>().sprite = chosen;
        card.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);

        float spacing = 0.5f;
        float cardHeight = 1.0f;
        Vector3 worldPos = table.position + new Vector3(spacing * communityCards.childCount, cardHeight + 3.25f, 0);
        card.transform.position = worldPos;
        card.transform.rotation = Quaternion.Euler(-90, 0, 0);
    }

    void ClearCards()
    {
        foreach (Transform child in playerHandCanvas)
            Destroy(child.gameObject);

        foreach (Transform child in communityCards)
            Destroy(child.gameObject);

        foreach (Transform spot in aiSpots)
        {
            foreach (Transform child in spot)
                Destroy(child.gameObject);
        }

        playerHoleCards.Clear();
        community.Clear();
        aiCardObjects.Clear();
    }

    public bool IsBetActive() => isBetActive;

    public void PlayerRaises(int raiseAmount)
    {
        Debug.Log("Player raises by " + raiseAmount);
        currentBetAmount += raiseAmount;
        isBetActive = true;
        StartCoroutine(HandleAIActions());
    }

    public void PlayerCalls()
    {
        Debug.Log("Player calls current bet: " + currentBetAmount);
        StartCoroutine(HandleAIActions());
    }

    public void PlayerChecks()
    {
        Debug.Log("Player checks.");
        StartCoroutine(HandleAIActions());
    }

    public void PlayerBets(int amount)
    {
        Debug.Log("Player bets " + amount);
        isBetActive = true;
        currentBetAmount = amount;
        StartCoroutine(HandleAIActions());
    }

    public void PlayerFolds()
    {
        Debug.Log("Player folds.");
        playerFolded = true;
        StartCoroutine(HandleAIActions());
    }

    public bool IsPlayerFolded()
    {
        return playerFolded;
    }


    IEnumerator HandleAIActions()
    {
        for (int i = 0; i < aiHoleCards.Count; i++)
        {
            if (aiFolded[i]) continue;

            yield return new WaitForSeconds(1f);

            int decision = Random.Range(0, 100);

            if (!isBetActive)
            {
                // No active bet: AI can either Bet, Check, or Fold
                if (decision < 10)
                {
                    Debug.Log($"AI {i + 1} folds (no bet yet).");
                    aiFolded[i] = true;
                    ClearAICards(i);
                }
                else if (decision < 70)
                {
                    Debug.Log($"AI {i + 1} checks.");
                }
                else
                {
                    Debug.Log($"AI {i + 1} bets 50.");
                    isBetActive = true;
                    currentBetAmount = 50;
                }
            }
            else
            {
                // There IS a bet: AI can Call, Raise, or Fold
                if (decision < 20)
                {
                    Debug.Log($"AI {i + 1} folds.");
                    aiFolded[i] = true;
                    ClearAICards(i);
                }
                else if (decision < 70)
                {
                    Debug.Log($"AI {i + 1} calls {currentBetAmount}.");
                }
                else
                {
                    Debug.Log($"AI {i + 1} raises by 50.");
                    isBetActive = true;
                    currentBetAmount += 50;
                }
            }
        }

        yield return new WaitForSeconds(1f);

        AdvanceGamePhase();
    }

    void ClearAICards(int aiIndex)
    {
        Transform spot = aiSpots[aiIndex];
        foreach (Transform child in spot)
            Destroy(child.gameObject);
    }

    public void AdvanceGamePhase()
    {
        isBetActive = false;

        switch (currentPhase)
        {
            case BettingPhase.PreFlop:
                DealFlop();
                currentPhase = BettingPhase.Flop;
                pokerUI.BeginPlayerTurn(isBetActive);
                break;

            case BettingPhase.Flop:
                DealTurn();
                currentPhase = BettingPhase.Turn;
                pokerUI.BeginPlayerTurn(isBetActive);
                break;

            case BettingPhase.Turn:
                DealRiver();
                currentPhase = BettingPhase.River;
                pokerUI.BeginPlayerTurn(isBetActive);
                break;

            case BettingPhase.River:
                RevealAICards();
                currentPhase = BettingPhase.Showdown;
                StartCoroutine(EndHandAfterDelay());
                break;

            default:
                Debug.LogWarning("Unexpected phase reached.");
                break;
        }
    }

    void DealFlop()
    {
        for (int i = 0; i < 3; i++)
            DealCardToTable();
    }

    void DealTurn()
    {
        DealCardToTable();
    }

    void DealRiver()
    {
        DealCardToTable();
    }

    void RevealAICards()
    {
        for (int i = 0; i < aiSpots.Count; i++)
        {
            if (aiFolded[i]) continue;

            int cardNum = 0;
            foreach (Transform card in aiSpots[i])
            {
                if (cardNum < aiHoleCards[i].Count)
                {
                    card.GetComponent<SpriteRenderer>().sprite = aiHoleCards[i][cardNum];
                    cardNum++;
                }
            }
        }

        // After revealing, immediately evaluate winner
        EvaluateWinner();
    }


    IEnumerator EndHandAfterDelay()
    {
        Debug.Log("🏁 Hand Over.");
        if (pokerUI != null && pokerUI.resultText != null)
        {
            pokerUI.resultText.text = "Starting next hand...";
        }

        yield return new WaitForSeconds(2f);

        pokerBank.ResetPot();
        StartGame();
    }

    // 🧠 New AI Hand Strength Evaluation
    int EvaluateHandStrength(List<Sprite> holeCards)
    {
        int score = 0;

        if (holeCards.Count < 2) return 0;

        string name1 = holeCards[0].name.ToLower();
        string name2 = holeCards[1].name.ToLower();

        if (GetRank(name1) == GetRank(name2))
            score += 5; // Pair bonus

        if (IsHighCard(name1)) score += 2;
        if (IsHighCard(name2)) score += 2;

        if (GetSuit(name1) == GetSuit(name2))
            score += 1; // Suited bonus

        return score;
    }

    string GetRank(string cardName)
    {
        if (cardName.Contains("01")) return "A";
        if (cardName.Contains("11")) return "J";
        if (cardName.Contains("12")) return "Q";
        if (cardName.Contains("13")) return "K";
        if (cardName.Contains("10")) return "10";
        if (cardName.Contains("09")) return "9";
        if (cardName.Contains("08")) return "8";
        if (cardName.Contains("07")) return "7";
        if (cardName.Contains("06")) return "6";
        if (cardName.Contains("05")) return "5";
        if (cardName.Contains("04")) return "4";
        if (cardName.Contains("03")) return "3";
        if (cardName.Contains("02")) return "2";
        return "";
    }

    bool IsHighCard(string cardName)
    {
        return cardName.Contains("01") || cardName.Contains("11") ||
               cardName.Contains("12") || cardName.Contains("13") ||
               cardName.Contains("10");
    }

    string GetSuit(string cardName)
    {
        cardName = cardName.ToLower();

        if (cardName.Contains("heart"))
            return "heart";
        if (cardName.Contains("diamond"))
            return "diamond";
        if (cardName.Contains("club"))
            return "club";
        if (cardName.Contains("spade"))
            return "spade";

        return "";
    }

    void EvaluateWinner()
    {
        Debug.Log("Evaluating winner...");

        List<(string name, List<Sprite> fullHand, bool folded)> players = new List<(string, List<Sprite>, bool)>();

        // Add player (only if not folded)
        if (!playerFolded)
        {
            List<Sprite> playerFullHand = new List<Sprite>(playerHoleCards);
            playerFullHand.AddRange(community);
            players.Add(("Player", playerFullHand, false));
        }

        // Add AIs
        for (int i = 0; i < aiHoleCards.Count; i++)
        {
            if (aiFolded[i]) continue; // Skip folded AIs
            List<Sprite> aiFullHand = new List<Sprite>(aiHoleCards[i]);
            aiFullHand.AddRange(community);
            players.Add(($"AI {i + 1}", aiFullHand, false));
        }

        if (players.Count == 0)
        {
            Debug.Log("No players left in hand!");
            return;
        }
        else if (players.Count == 1)
        {
            Debug.Log(players[0].name + " wins by default!");
            return;
        }

        string bestPlayer = "";
        int bestScore = -1;
        string bestHandName = "";

        foreach (var player in players)
        {
            int score = EvaluateHand(player.fullHand, out string handName);

            Debug.Log($"{player.name} has {handName}");

            if (score > bestScore)
            {
                bestScore = score;
                bestPlayer = player.name;
                bestHandName = handName;
            }
        }

        Debug.Log($"🏆 {bestPlayer} wins with {bestHandName}!");
    }

    int EvaluateHand(List<Sprite> cards, out string handName)
    {
        List<int> ranks = new List<int>();
        Dictionary<string, int> suits = new Dictionary<string, int>();

        foreach (var card in cards)
        {
            string name = card.name.ToLower();
            if (name.Contains("01")) ranks.Add(14); // Ace = 14
            else if (name.Contains("02")) ranks.Add(2);
            else if (name.Contains("03")) ranks.Add(3);
            else if (name.Contains("04")) ranks.Add(4);
            else if (name.Contains("05")) ranks.Add(5);
            else if (name.Contains("06")) ranks.Add(6);
            else if (name.Contains("07")) ranks.Add(7);
            else if (name.Contains("08")) ranks.Add(8);
            else if (name.Contains("09")) ranks.Add(9);
            else if (name.Contains("10")) ranks.Add(10);
            else if (name.Contains("11")) ranks.Add(11);
            else if (name.Contains("12")) ranks.Add(12);
            else if (name.Contains("13")) ranks.Add(13);

            string suit = GetSuit(card.name);
            if (!suits.ContainsKey(suit))
                suits[suit] = 0;
            suits[suit]++;
        }

        ranks.Sort();
        ranks.Reverse(); // high to low

        bool isFlush = false;
        foreach (var suit in suits)
        {
            if (suit.Value >= 5)
            {
                isFlush = true;
                break;
            }
        }

        bool isStraight = false;
        int straightHigh = 0;
        for (int i = 0; i < ranks.Count - 4; i++)
        {
            if (ranks[i] - 1 == ranks[i + 1] &&
                ranks[i + 1] - 1 == ranks[i + 2] &&
                ranks[i + 2] - 1 == ranks[i + 3] &&
                ranks[i + 3] - 1 == ranks[i + 4])
            {
                isStraight = true;
                straightHigh = ranks[i];
                break;
            }
        }
        // Ace-low straight (5-4-3-2-A)
        if (!isStraight && ranks.Contains(14) &&
            ranks.Contains(2) && ranks.Contains(3) &&
            ranks.Contains(4) && ranks.Contains(5))
        {
            isStraight = true;
            straightHigh = 5;
        }

        // Count duplicates
        Dictionary<int, int> counts = new Dictionary<int, int>();
        foreach (var r in ranks)
        {
            if (!counts.ContainsKey(r))
                counts[r] = 0;
            counts[r]++;
        }

        bool four = counts.ContainsValue(4);
        bool three = counts.ContainsValue(3);
        int pairs = 0;
        foreach (var c in counts.Values)
        {
            if (c == 2) pairs++;
        }

        // Check for rankings
        if (isStraight && isFlush)
        {
            if (straightHigh == 14)
            {
                handName = "Royal Flush";
                return 10;
            }
            handName = "Straight Flush";
            return 9;
        }
        if (four)
        {
            handName = "Four of a Kind";
            return 8;
        }
        if (three && pairs > 0)
        {
            handName = "Full House";
            return 7;
        }
        if (isFlush)
        {
            handName = "Flush";
            return 6;
        }
        if (isStraight)
        {
            handName = "Straight";
            return 5;
        }
        if (three)
        {
            handName = "Three of a Kind";
            return 4;
        }
        if (pairs >= 2)
        {
            handName = "Two Pair";
            return 3;
        }
        if (pairs == 1)
        {
            handName = "One Pair";
            return 2;
        }

        handName = "High Card";
        return 1;
    }



}
