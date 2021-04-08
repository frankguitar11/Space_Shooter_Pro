using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySineMovement : MonoBehaviour
{

    [SerializeField] float CurveSpeed = 5;
    [SerializeField] float MoveSpeed = 1;
    [SerializeField] private float _curveRadius = 2f;

    private float fTime = 0;
    private Vector3 vLastPos = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        vLastPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        SineMovement();
    }

    public void SineMovement()
    {
        vLastPos = transform.position;

        fTime += Time.deltaTime * CurveSpeed;

        Vector3 vSin = new Vector3(Mathf.Sin(fTime) * _curveRadius, 0, 0);
        Vector3 vLin = new Vector3(MoveSpeed, MoveSpeed, 0);

        transform.position += (vSin + vLin) * Time.deltaTime;

        //Line visualization
        //Debug.DrawLine(vLastPos, transform.position, Color.green, 100);
    }
}
