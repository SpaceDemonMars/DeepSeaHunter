using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;

    public List<Quest> activeQuests = new List<Quest>();
    public List<Quest> completedQuests = new List<Quest>();
    [SerializeField] public List<Quest> allQuests;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void StartQuest(Quest newQuest)
    {
        if (newQuest.requiredQuest != QuestID.None && !IsQuestCompleted(newQuest.requiredQuest))
        {
            Debug.Log($"Cannot start {newQuest.questName} yet. Complete {newQuest.requiredQuest} first.");
            return;
        }

        if (!activeQuests.Contains(newQuest))
        {
            Debug.Log($"Quest started: {newQuest.questName}");
            activeQuests.Add(newQuest);
            ActivateFirstObjective(newQuest);
        }
    }

    private void ActivateFirstObjective(Quest quest)
    {
        if (quest.objectives.Count > 0)
            quest.objectives[0].status = ObjectiveStatus.Active;
    }

    public bool IsQuestCompleted(QuestID questID)
    {
        return completedQuests.Exists(q => q.questID == questID);
    }

    public void CompleteObjective(QuestID questID, int objectiveIndex)
    {
        Quest quest = activeQuests.Find(q => q.questID == questID);
        if (quest != null && objectiveIndex < quest.objectives.Count)
        {
            quest.objectives[objectiveIndex].status = ObjectiveStatus.Completed;
            Debug.Log($"Objective {objectiveIndex} completed: {quest.objectives[objectiveIndex].description}");

            if (objectiveIndex + 1 < quest.objectives.Count)
            {
                quest.objectives[objectiveIndex + 1].status = ObjectiveStatus.Active;
            }
            else
            {
                CompleteQuest(quest);
            }
        }
    }

    private void CompleteQuest(Quest quest)
    {
        Debug.Log($"Quest Completed: {quest.questName}!");
        quest.isCompleted = true;
        activeQuests.Remove(quest);
        completedQuests.Add(quest);
        GiveRewards(quest);
    }

    private void GiveRewards(Quest quest)
    {
        foreach (QuestReward reward in quest.rewards)
        {
            Item rewardItem = new Item
            {
                itemName = reward.itemName,
                quantity = reward.amount,
                itemId = GenerateItemID(reward.itemName) // Simple placeholder
            };
            GameManager.instance.playerScript.inven.addItem(rewardItem);
            Debug.Log($"Reward: {reward.amount}x {reward.itemName}");
        }
    }

    private int GenerateItemID(string itemName)
    {
        // ID generator placeholde for now replace with real database later
        return itemName.GetHashCode();
    }
}
