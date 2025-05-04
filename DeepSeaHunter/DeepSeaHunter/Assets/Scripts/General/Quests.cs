using System;
using System.Collections.Generic;
using UnityEngine;

// QuestID 
public enum QuestID
{
    None,
    AskAboutNathan,
    CheckOutNathansHut,
    AskAboutNathansDiveSpots,
    DeliverCrabsToJim,
    AskAlanToTakeYouTo1stDiveSpot,
    GetGearFromJewel,
    LeaveWithAlan,
    FindClues,
    HeadTo2ndDiveSpot,
    FindNathan,
    DefeatTheBoss
   
}

// Quest Status
public enum ObjectiveStatus
{
    Inactive,
    Active,
    Completed
}

// Objectives
[Serializable]
public class Objective
{
    public string description;
    public ObjectiveStatus status = ObjectiveStatus.Inactive;
}

// Rewards
[Serializable]
public class QuestReward
{
    public string itemName;
    public int amount;
}

// Quests
[Serializable]
public class Quest
{
    public QuestID questID;
    public string questName;
    public string questDescription;
    public List<Objective> objectives = new List<Objective>();
    public List<QuestReward> rewards = new List<QuestReward>();
    public QuestID requiredQuest = QuestID.None;
    public bool isCompleted = false;
}
