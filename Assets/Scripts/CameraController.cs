using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public bool _isF;
    public bool _isMainUI;

    public static bool orbitStateCamera = false;

    private bool rotate = true;



    [Header("旋转中心目标物体")][SerializeField] private GameObject _target;
    [Header("拖动灵敏度")][SerializeField] private float _sensitivity = 2.0f;
    [Header("移动速度")][SerializeField] private float _speed = 0.1f;

    private Vector3 movement;

    private void Start()
    {
        _isF = _isMainUI;
        orbitStateCamera = false;
    }


    private void Update()
    {
        movement = Vector3.zero;

        W_A_S_D();

        Vector3 newPosition = transform.position + movement;

        Collider[] colliders = Physics.OverlapSphere(newPosition, 1.5f);

        if (!orbitStateCamera)
        {
            foreach (Collider hit in colliders)
            {
                if (hit.CompareTag("Wall") | hit.CompareTag("Floor"))
                {
                    newPosition = transform.position;
                    break;
                }
            }
        }

        if (newPosition != transform.position)
            
        {
              transform.position = newPosition;
         }


        if (orbitStateCamera & !_isMainUI)
        {
            StartCoroutine(RotateCamera());
        }



        if (Input.GetKeyUp(KeyCode.F))

        {
            _isF = !_isF;
        }

        if (!_isMainUI)
        {
            if (orbitStateCamera)
            {
                _isF = true;

            }
            else
            {
                _isF = false;
            }
        }

        if (!_isF)
        {
            if (Input.GetMouseButton(0))
            {
                Around();
            }
        }
        else
        {
            if (Input.GetMouseButton(0))
            {
                LookAround();
            }
            else
            {
                if (rotate & !_isMainUI ) { Rotate(); }
            }
        }
    }

    private void W_A_S_D()
    {
        float boost = 0.9f;
        if (Input.GetKey(KeyCode.LeftShift))
        { boost = 6; }

        if (Input.GetKey(KeyCode.W))
        {
            movement = transform.forward * _speed * boost;
        }

        if (Input.GetKey(KeyCode.S))
        {
            movement = -transform.forward * _speed * boost;
        }

        if (Input.GetKey(KeyCode.A))
        {
            movement = -transform.right * _speed * boost;
        }

        if (Input.GetKey(KeyCode.D))
        {
            movement = transform.right * _speed * boost;
        }
    }

    private void LookAround()
    {
        float mouseX = Input.GetAxis("Mouse X") * _sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * _sensitivity;
        if (_target != null)
        {
            transform.RotateAround(_target.transform.position, Vector3.up, mouseX);
            transform.RotateAround(_target.transform.position, transform.right, -mouseY);
            transform.LookAt(_target.transform);
        }
    }

    private void Around()
    {
        float rotateX = 0;
        float rotateY = 0;
        rotateX = transform.localEulerAngles.x - Input.GetAxis("Mouse Y") * _sensitivity;
        rotateY = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * _sensitivity;

        transform.localEulerAngles = new Vector3(rotateX, rotateY, 0);
    }

    private void Rotate()
    {
        float mouseX = 0.3f;
        if (_target != null)
        {
            transform.RotateAround(_target.transform.position, Vector3.up, mouseX);
            transform.LookAt(_target.transform);
        }
    }
    IEnumerator RotateCamera()
    {
        if (Input.GetAxis("Mouse X") == 0 & Input.GetAxis("Mouse Y") == 0)
        {
            yield return new WaitForSeconds(1.5f);
            rotate = true;
        }
    }
}
