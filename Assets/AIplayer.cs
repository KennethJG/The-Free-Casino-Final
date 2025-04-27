using UnityEngine;
using System.Collections.Generic;

public class AIPlayer
{
    public Transform spot;                      // World location on table
    public List<Sprite> handValues = new List<Sprite>(); // Actual card faces
    public List<GameObject> cards = new List<GameObject>(); // GameObjects for flip/reveal
}
