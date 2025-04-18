using UnityEngine;

public class moveOverTime : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float rotSpeed;
    [SerializeField] Transform startPos;
    [SerializeField] Transform endPos;
    // Update is called once per frame

    [SerializeField] bool moveAngle;
    [SerializeField] bool moveAngleFast;
    Transform target;

    private void Start()
    {
        target = endPos;
    }
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, endPos.position, moveSpeed * Time.deltaTime);
        if (transform.position == endPos.position)
            transform.position = startPos.position;

        if (moveAngle)
        {
            if (moveAngleFast)
                transform.rotation = Quaternion.RotateTowards(transform.rotation, target.rotation, rotSpeed * Time.deltaTime);
            else
                transform.rotation = Quaternion.Lerp(transform.rotation, target.rotation, rotSpeed * Time.deltaTime);
            if (transform.rotation == target.rotation)
            {
                if (target == endPos)
                    target = startPos;
                else
                    target = endPos;
            }
        }
    }
}
