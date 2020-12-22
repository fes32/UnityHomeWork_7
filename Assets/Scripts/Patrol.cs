using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrol : MonoBehaviour
{
    [SerializeField] private Enemy _templay;
    [SerializeField] private Transform _path;
    [SerializeField] private Transform[] _points;

    private Enemy _enemy;
    private int _currentPoint=0;

    private void OnEnable()
    {
        _points = new Transform[_path.childCount];

        for(int i=0; i<_path.childCount; i++)
        {
            _points[i] = _path.GetChild(i);
        }

       _enemy =  Instantiate(_templay, _points[0].position, Quaternion.identity);
    }

    void Update()
    {
        Transform target = _points[_currentPoint];

        _enemy.transform.position = Vector3.MoveTowards(_enemy.transform.position, target.position, _enemy.Speed * Time.deltaTime);

        if(_enemy.transform.position == target.position)
        {
            _currentPoint++;

            if (_currentPoint >= _points.Length)
            {
                _currentPoint = 0;
            }
        }
    }

    private void OnDisable()
    {
        Destroy(_enemy);
    }
}
