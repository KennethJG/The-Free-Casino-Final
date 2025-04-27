using UnityEngine;
using TMPro;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager instance;

    public TextMeshProUGUI balanceText;
    private int balance = 0;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Start()
    {
        LoadBalance();
        UpdateUI();
    }

    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Backslash)) // '\' key
        {
            Debug.Log("💰 Dev cheat activated: +$100");
            AddMoney(100);
        }
#endif
    }

    public void AddMoney(int amount)
    {
        balance += amount;
        UpdateUI();
        SaveBalance();
    }

    public void SubtractMoney(int amount)
    {
        balance -= amount;
        if (balance < 0) balance = 0;
        UpdateUI();
        SaveBalance();
    }

    public void UpdateUI()
    {
        balanceText.text = "Balance: $" + balance;
    }

    private void SaveBalance()
    {
        PlayerPrefs.SetInt("PlayerBalance", balance);
    }

    private void LoadBalance()
    {
        balance = PlayerPrefs.GetInt("PlayerBalance", 1000);
    }

    public bool CanAfford(int amount)
    {
        return balance >= amount;
    }

    public int GetBalance()
    {
        return balance;
    }
}
