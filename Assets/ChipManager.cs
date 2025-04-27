using System.Collections.Generic;
using UnityEngine;

public class ChipManager : MonoBehaviour
{
    public Transform chipSource;             // The parent holding chip10, chip100, etc.
    public Transform chipDisplayParent;      // Where the chips will appear on the table (spawn location)

    private Dictionary<int, GameObject> chipPrefabs = new Dictionary<int, GameObject>();
    private List<GameObject> spawnedChips = new List<GameObject>();

    void Awake()
    {
        // Load all chip prefabs from the chipSource children
        foreach (Transform child in chipSource)
        {
            string name = child.name.ToLower();
            if (name.Contains("10"))
                chipPrefabs[10] = child.gameObject;
            else if (name.Contains("100"))
                chipPrefabs[100] = child.gameObject;
        }
    }

    public void DisplayBetAsChips(int amount)
    {
        ClearChips();

        int[] chipValues = new int[] { 100, 10 };
        float xOffset = 0;

        foreach (int chipValue in chipValues)
        {
            while (amount >= chipValue)
            {
                amount -= chipValue;
                GameObject chipPrefab = chipPrefabs[chipValue];
                GameObject chip = Instantiate(chipPrefab, chipDisplayParent);
                chip.transform.localPosition += new Vector3(xOffset, 0, 0);
                spawnedChips.Add(chip);
                xOffset += 0.3f; // space between chips
            }
        }
    }

    public void ClearChips()
    {
        foreach (GameObject chip in spawnedChips)
        {
            Destroy(chip);
        }
        spawnedChips.Clear();
    }
}
