using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlackjackManager : MonoBehaviour
{
    public GameObject cardPrefab;
    public Sprite[] cardSprites;
    public Sprite cardBack;
    public Transform playerHand, dealerHand;
    public Transform table;
    public BlackjackUI blackjackUI;

    private List<Sprite> deck;
    private List<int> playerValues = new List<int>();
    private List<int> dealerValues = new List<int>();
    private List<Sprite> dealerCards = new List<Sprite>();
    private bool dealerHiddenCardRevealed = false;
    private int currentBet = 0;

    void Start()
    {
        blackjackUI.ShowBetPrompt();
    }

    public void StartRound(int bet)
    {
        if (!MoneyManager.instance.CanAfford(bet))
        {
            blackjackUI.ShowResult("❌ Not enough money!");
            return;
        }

        // Full reset
        currentBet = bet;
        MoneyManager.instance.SubtractMoney(bet);

        deck = new List<Sprite>(cardSprites);
        playerValues.Clear();
        dealerValues.Clear();
        dealerCards.Clear();
        dealerHiddenCardRevealed = false;

        ClearHand(playerHand);
        ClearHand(dealerHand);

        // Deal fresh hand
        DealCard(playerHand, playerValues, true);
        DealCard(playerHand, playerValues, true);

        DealCard(dealerHand, dealerValues, true);
        DealCard(dealerHand, dealerValues, false);

        blackjackUI.ShowPlayerOptions();
    }

    void ClearHand(Transform hand)
    {
        foreach (Transform child in hand)
        {
            DestroyImmediate(child.gameObject);
        }
    }

    public void PlayerHit()
    {
        DealCard(playerHand, playerValues, true);

        var (total, isSoft) = CalculateTotalWithSoftCheck(playerValues);
        if (total > 21)
        {
            blackjackUI.ShowResult("Bust! You lose.", total);
        }
        else
        {
            blackjackUI.UpdateActionPrompt();
        }
    }

    public void PlayerStand()
    {
        StartCoroutine(DealerPlay());
    }

    IEnumerator DealerPlay()
    {
        // 🔄 Reveal dealer's hidden card
        if (dealerCards.Count > 0 && dealerHand.childCount >= 2)
        {
            Transform hiddenCard = dealerHand.GetChild(1);
            hiddenCard.GetComponent<SpriteRenderer>().sprite = dealerCards[0];
            dealerValues[1] = GetCardValue(dealerCards[0]);
        }

        yield return new WaitForSeconds(1f); // Wait a second for reveal animation

        var (dealerTotal, isSoft) = CalculateTotalWithSoftCheck(dealerValues);

        // 🃏 Dealer must hit until 17 or more
        while (dealerTotal < 17 || (dealerTotal == 17 && isSoft))
        {
            DealCard(dealerHand, dealerValues, true);
            yield return new WaitForSeconds(1f); // Wait after dealing each card
            (dealerTotal, isSoft) = CalculateTotalWithSoftCheck(dealerValues);
        }

        var (playerTotal, _) = CalculateTotalWithSoftCheck(playerValues);

        // 🎯 Decide outcome
        if (dealerTotal > 21 || playerTotal > dealerTotal)
        {
            MoneyManager.instance.AddMoney(currentBet * 2);
            blackjackUI.ShowResult("You win!", playerTotal, dealerTotal);
        }
        else if (dealerTotal == playerTotal)
        {
            MoneyManager.instance.AddMoney(currentBet);
            blackjackUI.ShowResult("Push. No One Wins.", playerTotal, dealerTotal);
        }
        else
        {
            blackjackUI.ShowResult("Dealer wins.", playerTotal, dealerTotal);
        }

        // ✅ STOP HERE — do NOT auto-reset
        // Wait for player to press E (play again) or Q (return to casino)
    }



    (int total, bool isSoft) CalculateTotalWithSoftCheck(List<int> hand)
    {
        int total = 0;
        int aces = 0;

        foreach (int val in hand)
        {
            total += val;
            if (val == 11) aces++;
        }

        while (total > 21 && aces > 0)
        {
            total -= 10;
            aces--;
        }

        return (total, aces > 0);
    }

    // (Your requested DealCard version, unchanged)
    void DealCard(Transform hand, List<int> values, bool faceUp)
    {
        if (deck.Count == 0) return;

        int index = Random.Range(0, deck.Count);
        Sprite selected = deck[index];
        deck.RemoveAt(index);

        GameObject card = Instantiate(cardPrefab, hand);
        card.transform.localScale = new Vector3(0.84f, 1.87f, 1f);
        card.transform.rotation = Quaternion.Euler(-90, 0, 0);

        int cardIndex = hand.childCount - 1;
        float spacing = 0;
        spacing = 0.5f * cardIndex;
        card.transform.position = new Vector3(table.position.x + spacing , 4.4f, hand.position.z);

        card.GetComponent<SpriteRenderer>().sprite = faceUp ? selected : cardBack;

        if (hand == dealerHand && !faceUp)
            dealerCards.Add(selected);

        int cardValue = GetCardValue(selected);
        values.Add(faceUp || hand != dealerHand ? cardValue : 0);
    }

    int GetCardValue(Sprite sprite)
    {
        string name = sprite.name.ToLower();
        string numberPart = name.Substring(name.Length - 2);

        if (int.TryParse(numberPart, out int number))
        {
            if (number == 1) return 11; // Ace
            if (number >= 11 && number <= 13) return 10; // Face cards
            return number;
        }

        Debug.LogWarning($"⚠️ Unrecognized card name: {name}");
        return 0;
    }
}
