using UnityEngine;

public class PokerBank : MonoBehaviour
{
    public int playerChips = 1000;  // Starting chips for the player
    public int pot = 0;             // Current pot total

    // Call this when the player places a bet
    public void PlayerBet(int amount)
    {
        if (playerChips >= amount)
        {
            playerChips -= amount;
            pot += amount;
            Debug.Log($"💰 Player bets {amount}. Player now has {playerChips}. Pot is now {pot}.");
        }
        else
        {
            Debug.Log("❌ Not enough chips to bet.");
        }
    }

    // Call this to reset the pot (e.g. after someone wins)
    public void ResetPot()
    {
        pot = 0;
    }

    // Get current pot amount
    public int GetPot()
    {
        return pot;
    }

    // Get player's current chip balance
    public int GetPlayerChips()
    {
        return playerChips;
    }

    // Award the pot to the player (e.g. if they win)
    public void AwardPotToPlayer()
    {
        playerChips += pot;
        Debug.Log($"🏆 Player wins the pot! New balance: {playerChips}");
        pot = 0;
    }
}
