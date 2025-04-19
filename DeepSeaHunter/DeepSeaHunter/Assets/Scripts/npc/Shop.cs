using UnityEngine;
using TMPro; // For UI
using System.Collections.Generic;

public class Shop : MonoBehaviour
{
    [System.Serializable]
    public class TradeItem
    {
        public string requiredItem;  
        public int requiredAmount;  
        public string rewardItem;    
        public int rewardAmount = 1; 
    }

    public List<TradeItem> tradeOptions = new List<TradeItem>();
    public GameObject shopUI;
    public TMP_Text shopText;

    private void Start()
    {
        UpdateShopUI();
        shopUI.SetActive(false);
    }

    public void OpenShop()
    {
        shopUI.SetActive(true);
        UpdateShopUI();
    }

    public void CloseShop()
    {
        shopUI.SetActive(false);
    }

    public void AttemptTrade(int index)
    {
        if (index >= 0 && index < tradeOptions.Count)
        {
            TradeItem trade = tradeOptions[index];
            if (PlayerInventory.Instance.HasItem(trade.requiredItem, trade.requiredAmount))
            {
                PlayerInventory.Instance.RemoveItem(trade.requiredItem, trade.requiredAmount);
                PlayerInventory.Instance.AddItem(trade.rewardItem, trade.rewardAmount);
                Debug.Log($"✅ Traded {trade.requiredAmount} {trade.requiredItem} for {trade.rewardAmount} {trade.rewardItem}.");
            }
            else
            {
                Debug.Log("❌ Not enough items to trade.");
            }
        }
    }

    void UpdateShopUI()
    {
        shopText.text = "";
        for (int i = 0; i < tradeOptions.Count; i++)
        {
            TradeItem trade = tradeOptions[i];
            shopText.text += $"{i + 1}. Trade {trade.requiredAmount} {trade.requiredItem} for {trade.rewardAmount} {trade.rewardItem}\n";
        }
    }
}
