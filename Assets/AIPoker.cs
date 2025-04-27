using UnityEngine;
using System.Collections.Generic;

public class PokerAI : MonoBehaviour
{
    public enum Action { Fold, Check, Call, Raise }

    public Action DecideAction(List<Sprite> holeCards, List<Sprite> communityCards, bool isBetActive)
    {
        // Simple random-based AI decision (you can make smarter later)
        float decision = Random.value; // 0.0 to 1.0

        if (!isBetActive)
        {
            if (decision < 0.6f)
            {
                return Action.Check;
            }
            else
            {
                return Action.Raise;
            }
        }
        else
        {
            if (decision < 0.4f)
            {
                return Action.Call;
            }
            else if (decision < 0.7f)
            {
                return Action.Fold;
            }
            else
            {
                return Action.Raise;
            }
        }
    }
}
