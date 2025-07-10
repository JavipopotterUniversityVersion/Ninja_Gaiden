using System.Collections;
using UnityEngine;

public class JumpState : IState
{
    [SerializeField] SpriteRenderer _sr;
    [SerializeField] float _jumpHeight = 2.0f;
    [SerializeField] float _jumpDuration = 0.7f;
    [SerializeField] float _jumpSpeedMultiplier = 1.65f;
    [SerializeField] AnimationCurve _jumpCurve;
    [SerializeField] Rigidbody2D _rb;

    public override void Enter()
    {
        _brain.PlayAnimation("PLAYER_JUMP_0");
        StartCoroutine(JumpCoroutine());
        _rb.linearVelocity = new Vector2(_rb.linearVelocityX * _jumpSpeedMultiplier, _rb.linearVelocityY);
    }

    IEnumerator JumpCoroutine()
    {
        for (float t = 0; t < _jumpDuration; t += Time.deltaTime)
        {
            float normalizedTime = t / _jumpDuration;
            float height = _jumpCurve.Evaluate(normalizedTime) * _jumpHeight;
            _sr.transform.localPosition = new Vector3(_sr.transform.localPosition.x, height, _sr.transform.localPosition.z);
            yield return new WaitForEndOfFrame();
        }

        _sr.transform.localPosition = new Vector3(_sr.transform.localPosition.x, 0, _sr.transform.localPosition.z);

        _brain.ChangeState(StateNames.PLAYER_IDLE);
    }
}
