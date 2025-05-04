using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    [System.Serializable]
    public class TradeItem
    {
        public Item requiredItem;
        public int requiredAmount;
        public Item rewardItem;
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
        if (index < 0 || index >= tradeOptions.Count) return;

        TradeItem trade = tradeOptions[index];
        if (trade.requiredItem == null || trade.rewardItem == null) return;

        string requiredName = trade.requiredItem.itemName.ToLower();

        // Handle currency trades
        if (requiredName == "fish")
        {
            if (playerInven.Instance.getFish() >= trade.requiredAmount)
            {
                playerInven.Instance.setFish(playerInven.Instance.getFish() - trade.requiredAmount);
                playerInven.Instance.addItem(trade.rewardItem);
                PlayTradeSFX();
            }
        }
        else if (requiredName == "scrap")
        {
            if (playerInven.Instance.getScrap() >= trade.requiredAmount)
            {
                playerInven.Instance.setScrap(playerInven.Instance.getScrap() - trade.requiredAmount);
                playerInven.Instance.addItem(trade.rewardItem);
                PlayTradeSFX();
            }
        }
        else
        {
            // Regular item trade
            if (playerInven.Instance.HasItem(trade.requiredItem.itemName, trade.requiredAmount))
            {
                playerInven.Instance.RemoveItem(trade.requiredItem.itemName, trade.requiredAmount);

                Item rewardCopy = ScriptableObject.Instantiate(trade.rewardItem);
                rewardCopy.quantity = trade.rewardAmount;
                playerInven.Instance.addItem(rewardCopy);
                PlayTradeSFX();
            }
        }
    }

    private void PopulateShopUI()
    {
        ClearShopUI();

        for (int i = 0; i < tradeOptions.Count; i++)
        {
            TradeItem trade = tradeOptions[i];
            GameObject buttonObj = Instantiate(tradeButtonPrefab, tradeButtonParent);

            TMP_Text buttonText = buttonObj.GetComponentInChildren<TMP_Text>();
            if (buttonText != null)
            {
                string req = trade.requiredItem != null ? trade.requiredItem.itemName : "???";
                string rew = trade.rewardItem != null ? trade.rewardItem.itemName : "???";
                buttonText.text = $"Trade {trade.requiredAmount} {req} → {trade.rewardAmount} {rew}";
            }

            Button button = buttonObj.GetComponent<Button>();
            if (button != null)
            {
                int capturedIndex = i;
                button.onClick.AddListener(() => AttemptTrade(capturedIndex));
            }
        }
    }

    private void ClearShopUI()
    {
        foreach (Transform child in tradeButtonParent)
        {
            Destroy(child.gameObject);
        }
    }

    private void PlayTradeSFX()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX();
    }
}
