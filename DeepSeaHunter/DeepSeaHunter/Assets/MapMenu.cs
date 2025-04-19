using UnityEngine;

public class MapMenu : MonoBehaviour
{
    public GameObject mapPanel;

    void Start()
    {
        mapPanel.SetActive(false);
    }

    public void OpenMap()
    {
        mapPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void CloseMap()
    {
        mapPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}