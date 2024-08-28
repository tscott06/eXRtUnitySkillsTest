using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Billboard : MonoBehaviour
{
    public bool flip180Degrees;
    private Transform _mainCameraTransform => Camera.main.transform;


    void FixedUpdate()
    {
        if (_mainCameraTransform == null)
        {
            return;
        }

        transform.LookAt(_mainCameraTransform.position, Vector3.up);


        if (flip180Degrees)
        {
            transform.Rotate(0, 180, 0);
        }
    }
}
