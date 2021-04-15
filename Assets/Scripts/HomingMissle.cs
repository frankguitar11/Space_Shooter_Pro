using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMissle : MonoBehaviour
{

    [SerializeField] float _speed = 4.5f;

    [SerializeField] GameObject[] _activeEnemies;
    private GameObject _target = null;
    private float minDistance;
    private Vector3 currentPosition;

    private float _missleYBounds = 6.3f;
    private float _missleXBounds = 9.2f;

    // Start is called before the first frame update
    void Start()
    {
        _target = CalculateClosestEnemy();      
    }

    // Update is called once per frame
    void Update()
    {
        MoveToTarget();
        BoundsCheck();
    }

    private GameObject CalculateClosestEnemy()
    {
        _activeEnemies = GameObject.FindGameObjectsWithTag("Enemy");

        minDistance = Mathf.Infinity;
        currentPosition = transform.position;

        foreach (GameObject enemy in _activeEnemies)
        {
            float distance = Vector3.Distance(enemy.transform.position, currentPosition);
            if (distance < minDistance)
            {
                _target = enemy;
                minDistance = distance;
            }
        }

        return _target;
    }

    private void MoveToTarget()
    {
        if(_target != null)
        {
            if(Vector3.Distance(transform.position, _target.transform.position) != 0)
            {
                //Move Towards
                transform.position = Vector2.MoveTowards(transform.position, _target.transform.position,
                                        _speed * Time.deltaTime);

                //Look at Target
                Vector2 direction = (transform.position - _target.transform.position).normalized;
                var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                var offset = 90f;

                transform.rotation = Quaternion.Euler(Vector3.forward * (angle + offset));
            }
        }

        if (_target == null)
        {
            _target = CalculateClosestEnemy();
        }
    }

    private void BoundsCheck()
    {
        if(transform.position.x < -_missleXBounds)
        {
            Destroy(this.gameObject);
        }
        else if(transform.position.x > _missleXBounds)
        {
            Destroy(this.gameObject);
        }

        if(transform.position.y > _missleYBounds)
        {
            Destroy(this.gameObject);
        }
        else if(transform.position.y < - _missleYBounds)
        {
            Destroy(this.gameObject);
        }
    }
}
