using UnityEngine;

public class SpinObject : MonoBehaviour
{
    float speed = 500f;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, speed * Time.deltaTime);
    }
}
