using UnityEngine;

public class moveOverTime : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] Transform startPos;
    [SerializeField] Transform endPos;
    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, endPos.position, moveSpeed * Time.deltaTime);
        if (transform.position == endPos.position)
            transform.position = startPos.position;
    }
}
