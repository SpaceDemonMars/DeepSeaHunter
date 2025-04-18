using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MinimapController : MonoBehaviour
{
    [Header("References")]
    public Transform player;                      // The player to track
    public RectTransform minimapRect;              // UI Panel for minimap
    public RawImage minimapImage;                  // Minimap base texture
    public GameObject markerPrefab;                // Prefab for quest markers
    public RectTransform exploredMask;             // Mask that grows as player explores

    [Header("Settings")]
    public float mapWorldSize = 500f;               // Size of world area covered
    public float minimapSize = 200f;                // Size of minimap UI (px)
    public float revealRadius = 10f;                // How much area is revealed around player

    private List<Transform> questMarkers = new List<Transform>();
    private Texture2D explorationTexture;
    private Color32[] clearPixels;

    void Start()
    {
        InitializeExplorationMask();
        FindQuestMarkers();
    }

    void Update()
    {
        UpdatePlayerLocation();
        RevealExploredArea();
    }

    void InitializeExplorationMask()
    {
        explorationTexture = new Texture2D(256, 256);
        clearPixels = new Color32[explorationTexture.width * explorationTexture.height];

        for (int i = 0; i < clearPixels.Length; i++)
            clearPixels[i] = new Color32(0, 0, 0, 0); // Start fully transparent (hidden)

        explorationTexture.SetPixels32(clearPixels);
        explorationTexture.Apply();

        exploredMask.GetComponent<RawImage>().texture = explorationTexture;
    }

    void FindQuestMarkers()
    {
        foreach (GameObject marker in GameObject.FindGameObjectsWithTag("QuestMarker"))
        {
            Transform markerUI = Instantiate(markerPrefab, minimapRect).transform;
            questMarkers.Add(marker.transform);
        }
    }

    void UpdatePlayerLocation()
    {
        Vector2 playerPos = new Vector2(player.position.x, player.position.z);
        Vector2 minimapPos = (playerPos / mapWorldSize) * minimapSize;

        minimapRect.localPosition = -minimapPos;

        // Update quest markers on minimap
        for (int i = 0; i < questMarkers.Count; i++)
        {
            Vector2 questPos = new Vector2(questMarkers[i].position.x, questMarkers[i].position.z);
            Vector2 questMinimapPos = (questPos / mapWorldSize) * minimapSize;
            minimapRect.GetChild(i).localPosition = questMinimapPos;
        }
    }

    void RevealExploredArea()
    {
        Vector2 playerMapPos = new Vector2(
            (player.position.x / mapWorldSize) * explorationTexture.width,
            (player.position.z / mapWorldSize) * explorationTexture.height
        );

        int radius = Mathf.RoundToInt(revealRadius * (explorationTexture.width / mapWorldSize));

        for (int y = -radius; y <= radius; y++)
        {
            for (int x = -radius; x <= radius; x++)
            {
                int px = Mathf.RoundToInt(playerMapPos.x + x);
                int py = Mathf.RoundToInt(playerMapPos.y + y);

                if (px >= 0 && px < explorationTexture.width && py >= 0 && py < explorationTexture.height)
                {
                    float dist = Mathf.Sqrt(x * x + y * y);
                    if (dist <= radius)
                    {
                        explorationTexture.SetPixel(px, py, Color.white);
                    }
                }
            }
        }

        explorationTexture.Apply();
    }
}