using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{

    [SerializeField] private float _shake = 0;
    [SerializeField] private float _shakeAmount = 0.7f;
    [SerializeField] private float _decreaseFactor = 1;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (_shake > 0)
        {
            this.transform.position = transform.position + Random.insideUnitSphere * _shakeAmount;
            _shake -= Time.deltaTime * _decreaseFactor;
        }
        else
        {
            _shake = 0;
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(0, 1, -10), 10f * Time.deltaTime);
        }
    }

    public void MainCameraShake()
    {
        _shake = 0.8f;
    }
}
