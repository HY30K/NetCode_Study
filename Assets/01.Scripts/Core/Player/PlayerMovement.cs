using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [Header("참조데이터")]
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private Transform _bodyTransform;
    private Rigidbody2D _rigidbody;

    [Header("Settings")]
    [SerializeField] private float _movementSpeed = 4f; //이동속도
    [SerializeField] private float _turningRate = 30f; //회전속도

    private Vector2 _prevMovementInput;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return; //오너일 경우만 처리
        _inputReader.MovementEvent += HandleMovement;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;
        _inputReader.MovementEvent -= HandleMovement;
    }

    private void HandleMovement(Vector2 movementInput)
    {
        _prevMovementInput = movementInput;
    }

    private void Update()
    {
        //여기는 과제로 제시
        if (!IsOwner) return;

        float zRotation = _prevMovementInput.x * -_turningRate * Time.deltaTime;
        _bodyTransform.Rotate(0, 0, zRotation);
    }

    private void FixedUpdate()
    {
        //여기는 과제로 제시
        if (!IsOwner) return; //오너가 아니면 리턴

        _rigidbody.velocity = _bodyTransform.up * (_prevMovementInput.y * _movementSpeed);
    }
}

