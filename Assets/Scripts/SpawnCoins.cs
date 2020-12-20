using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCoins : MonoBehaviour
{
    [SerializeField] private Transform _path;
    [SerializeField] private Coin _template;

    private Transform[] _points;
   
    void Start()
    {
        _points = new Transform[_path.childCount];

        for(int i=0; i<_path.childCount; i++)
        {
            _points[i] = _path.GetChild(i);
        }

        for (int i = 0; i < _points.Length; i++)
        {
            Instantiate(_template, _points[i].position, Quaternion.identity);
        }
    }
}