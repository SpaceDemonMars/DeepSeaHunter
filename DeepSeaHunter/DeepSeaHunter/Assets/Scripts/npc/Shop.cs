using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

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
    public GameObject tradeButtonPrefab; 
    public Transform tradeButtonParent; 

    private void Start()
    {
        shopUI.SetActive(false);
    }

    public void OpenShop()
    {
        shopUI.SetActive(true);
        PopulateShopUI();
    }

    public void CloseShop()
    {
        shopUI.SetActive(false);
        ClearShopUI();
    }
        public void AttemptTrade(int index)
    {
        if (index >= 0 && index < tradeOptions.Count)
        {
            TradeItem trade = tradeOptions[index];
            string requiredItemLower = trade.requiredItem.ToLower();

            if (requiredItemLower == "fish")
            {
                if (playerInven.Instance.getFish() >= trade.requiredAmount)
                {
                    playerInven.Instance.setFish(playerInven.Instance.getFish() - trade.requiredAmount);
                    playerInven.Instance.AddItem(trade.rewardItem, trade.rewardAmount);

            //        Debug.Log($"Traded {trade.requiredAmount} fish for {trade.rewardAmount} {trade.rewardItem}");
                    AudioManager.Instance.PlaySFX();
                }
                else
                {
             //       Debug.Log("Not enough fish to trade.");
                }
            }
            else if (requiredItemLower == "scrap")
            {
                if (playerInven.Instance.getScrap() >= trade.requiredAmount)
                {
                    playerInven.Instance.setScrap(playerInven.Instance.getScrap() - trade.requiredAmount);
                    playerInven.Instance.AddItem(trade.rewardItem, trade.rewardAmount);

            //        Debug.Log($"Traded {trade.requiredAmount} scrap for {trade.rewardAmount} {trade.rewardItem}");
                    AudioManager.Instance.PlaySFX();
                }
                else
                {
          //          Debug.Log("Not enough scrap to trade.");
                }
            }
            else
            {
                if (playerInven.Instance.HasItem(trade.requiredItem, trade.requiredAmount))
                {
                    playerInven.Instance.RemoveItem(trade.requiredItem, trade.requiredAmount);
                    playerInven.Instance.AddItem(trade.rewardItem, trade.rewardAmount);

          //          Debug.Log($"Traded {trade.requiredAmount} {trade.requiredItem} for {trade.rewardAmount} {trade.rewardItem}");
                    AudioManager.Instance.PlaySFX();
                }
                else
                {
           //         Debug.Log("Not enough items to trade.");
                }
            }
        }
    }

    void PopulateShopUI()
    {
        ClearShopUI();

        for (int i = 0; i < tradeOptions.Count; i++)
        {
            TradeItem trade = tradeOptions[i];
            GameObject buttonObj = Instantiate(tradeButtonPrefab, tradeButtonParent);

            TMP_Text buttonText = buttonObj.GetComponentInChildren<TMP_Text>();
            if (buttonText != null)
            {
                buttonText.text = $"Trade {trade.requiredAmount} {trade.requiredItem} → {trade.rewardAmount} {trade.rewardItem}";
            }

            Button button = buttonObj.GetComponent<Button>();
            if (button != null)
            {
                int capturedIndex = i;
                button.onClick.AddListener(() => AttemptTrade(capturedIndex));
            }
        }
    }

    void ClearShopUI()
    {
        foreach (Transform child in tradeButtonParent)
        {
            Destroy(child.gameObject);
        }
    }
}
