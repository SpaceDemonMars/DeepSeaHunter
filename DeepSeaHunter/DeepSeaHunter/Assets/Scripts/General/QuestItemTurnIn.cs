using UnityEngine;

public class QuestItemTurnIn : MonoBehaviour
{
    [Header("Turn-In Settings")]
    public QuestID questToCheck = QuestID.DeliverCrabsToJim;
    public int objectiveIndex = 0;

    public Item requiredItem; 
    public int requiredAmount = 3;

    public void TryTurnIn()
    {
        if (!QuestManager.instance.activeQuests.Exists(q => q.questID == questToCheck))
            return;

        if (!playerInven.Instance.HasItem(requiredItem.itemName, requiredAmount))
            return;

        playerInven.Instance.RemoveItem(requiredItem.itemName, requiredAmount);
        QuestManager.instance.CompleteObjective(questToCheck, objectiveIndex);
        GameManager.instance.ShowQuestPopup($"Objective Complete: Delivered {requiredAmount} {requiredItem.itemName} to Jim!");
    }
}
