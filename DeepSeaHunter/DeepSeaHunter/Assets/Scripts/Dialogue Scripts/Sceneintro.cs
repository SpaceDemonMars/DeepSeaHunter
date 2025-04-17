using UnityEngine;

public class SceneIntro : MonoBehaviour
{
    public string introThought = "I’m not sure what I was expecting when I got here, but this place is… rundown.\nWhat could possibly be worth living in a shack like that?";

    void Start()
    {
        PlayerThoughts.Instance.ShowThought(introThought);
    }
}
