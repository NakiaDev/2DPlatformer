using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

/*Allows the camera to shake when the player punches, gets hurt, etc. Put any other custom camera effects in this script!*/

public class CameraEffects : MonoBehaviour
{
    [SerializeField] Vector3 _cameraWorldSize;
    [SerializeField] CinemachineFramingTransposer _cinemachineFramingTransposer;
    CinemachineBasicMultiChannelPerlin _multiChannelPerlin;
    [SerializeField] float _screenYDefault;
    [SerializeField] float _screenYTalking;
    [Range(0, 10)]
    float _shakeLength = 10;
    CinemachineVirtualCamera _virtualCamera;

    private void Awake()
    {
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
    }

    void Start()
    {      
        //Ensures we can shake the camera using Cinemachine. Don't really worry too much about this weird stuff. It's just Cinemachine's variables.
        _cinemachineFramingTransposer = _virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        _screenYDefault = _cinemachineFramingTransposer.m_ScreenX;
        
        //Inform the player what CameraEffect it should be controlling, no matter what scene we are on.
        if (NewPlayer.Instance != null)
            NewPlayer.Instance.CameraEffects = this;
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
        _multiChannelPerlin = _virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        //Tells the virtualCamera what to follow
        _virtualCamera.Follow = NewPlayer.Instance?.transform;
    }

    void Update()
    {
        _multiChannelPerlin.m_FrequencyGain += (0 - _multiChannelPerlin.m_FrequencyGain) * Time.deltaTime * (10 - _shakeLength);
    }

    public void Shake(float shake, float length)
    {
        _shakeLength = length;
        _multiChannelPerlin.m_FrequencyGain = shake;
    }
}
