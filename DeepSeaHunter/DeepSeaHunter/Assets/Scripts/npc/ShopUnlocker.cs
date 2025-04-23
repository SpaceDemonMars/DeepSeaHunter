using UnityEngine;

public class ShopUnlocker : MonoBehaviour
{
    public QuestID requiredQuestToUnlock;
    private npcAI npc;

    void Start()
    {
        npc = GetComponent<npcAI>();
    }

    void Update()
    {
        if (npc != null && QuestManager.instance.IsQuestCompleted(requiredQuestToUnlock))
        {
            npc.shopUnlocked = true;
            Destroy(this); 
        }
    }
}
