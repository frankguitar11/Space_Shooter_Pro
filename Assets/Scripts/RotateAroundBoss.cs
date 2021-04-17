using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAroundBoss : MonoBehaviour
{
    [SerializeField] GameObject _bossEnemy;

    [SerializeField] float _rotationSpeed = 20f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(_bossEnemy.transform.position, Vector3.forward, _rotationSpeed * Time.deltaTime);
    }
}
