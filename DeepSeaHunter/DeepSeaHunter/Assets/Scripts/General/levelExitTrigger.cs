
using UnityEngine.SceneManagement;
using UnityEngine;

public class levelExitTrigger : MonoBehaviour
{
    [SerializeField] Scene goToScene;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //when player enters, load next scene
            //SceneManager.LoadScene(goToScene.name);
            GameManager.instance.youLose(); //temporary
        }
    }
}
