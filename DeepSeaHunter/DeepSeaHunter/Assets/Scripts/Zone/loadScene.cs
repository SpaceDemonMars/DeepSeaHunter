using UnityEngine;
using UnityEngine.SceneManagement;

public class loadScene : MonoBehaviour
{
    [SerializeField] string sceneName;
    private void OnTriggerEnter(Collider other)
    {
        playerController player = other.GetComponent<playerController>();
        if (player != null)
        {
            GameManager.instance.Save(); //create backup incase smth goes wrong
            GameManager.instance.SaveOnSceneChange();
            SceneManager.LoadScene(sceneName);
        }
    }
}
