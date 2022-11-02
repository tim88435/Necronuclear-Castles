using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interpolator : MonoBehaviour
{
    [SerializeField] private float _timeElapsed = 0f;//time since the last transform position update
    [SerializeField] private float _timeToReachTarget = 0.05f;//time to used to lerp to the next position per tick
    [SerializeField] private float _movementThreshhold = 0.05f;//linear distance to check if one should exterpelate instead of inerpolate (depending if you're moving still)
    private readonly List<TransformUpdate> _futureTransformUpdates = new List<TransformUpdate>();//list of all transforms with their ticks in order of which to apply the transforms
    private float _squareMovementThreshold;//distance to check if one should exterpelate instead of inerpolate (depending if you're moving still)
    private TransformUpdate _to;//the transform update that the player is moving towards
    private TransformUpdate _from;//current player position to use to interpolate
    private TransformUpdate _previous;//last transform update to use for interpolation
    private void Start()
    {
        _squareMovementThreshold = _movementThreshhold * _movementThreshhold;//set the movement threshhold to work on an angle
        _to = new TransformUpdate(NetworkManager.Singleton.ServerTick, transform.position);
        _from = new TransformUpdate(NetworkManager.Singleton.InterpolationTick, transform.position);
        _previous = new TransformUpdate(NetworkManager.Singleton.InterpolationTick, transform.position);
    }
    private void Update()
    {
        for (int i = 0; i < _futureTransformUpdates.Count; i++)
        {
            if (NetworkManager.Singleton.ServerTick >= _futureTransformUpdates[i].Tick)
            {
                _previous = _to;
                _to = _futureTransformUpdates[i];
                _from = new TransformUpdate(NetworkManager.Singleton.InterpolationTick, transform.position);
                _futureTransformUpdates.RemoveAt(i);
                i--;
                _timeElapsed = 0f;
                _timeToReachTarget = (_to.Tick - _from.Tick) * Time.fixedDeltaTime;
            }
        }
        _timeElapsed += Time.deltaTime;
        InterpolatePosition(_timeElapsed / _timeToReachTarget);
    }
    private void InterpolatePosition(float lerpAmount)
    {
        if ((_to.Position - _previous.Position).sqrMagnitude < _squareMovementThreshold)//if 
        {
            if (_to.Position != _from.Position)
            {
                transform.position = Vector3.Lerp(_from.Position, _to.Position, lerpAmount);
            }
            return;
        }
        transform.position = Vector3.LerpUnclamped(_from.Position, _to.Position, lerpAmount);
    }
    public void NewUpdate(ushort tick, Vector3 position)
    {
        if (tick <= NetworkManager.Singleton.InterpolationTick)
        {
            return;
        }
        for (int i = 0; i < _futureTransformUpdates.Count; i++)
        {
            if (tick < _futureTransformUpdates[i].Tick)
            {
                _futureTransformUpdates.Insert(i, new TransformUpdate(tick, position));
                return;
            }
        }
        _futureTransformUpdates.Add(new TransformUpdate(tick, position));
    }
}
