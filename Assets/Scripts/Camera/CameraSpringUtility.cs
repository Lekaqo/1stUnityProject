using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSpringUtility 
{
    public Vector3 Values;

    private float frequence;
    private float damp;
    private Vector3 dampValues;


    public CameraSpringUtility(float _frequence, float _damp)
    {
        frequence = _frequence;
        damp = _damp;
    }
    public void UpdateSpring(float _deltalTime, Vector3 _target)
    {
        Values -= _deltalTime * frequence * dampValues;
        //假设Values的初始值是（10，10，10），_deltalTime=0，frequence频率=0，dampValues=（0，0，0）， Values并未进行衰减
        dampValues = Vector3.Lerp(a: dampValues, b:Values - _target, t:damp * _deltalTime);
        // dampValues初始为（0，0，0），_target初始为（0，0，0）原点，Values减去（0，0，0）仍为（10，10，10）
        //乘上过度时间，从0过渡到10的差值过程，dampValues值会逐渐增加，Values逐渐衰减为0
    }

}
