using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    [Header("����������")]
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private Transform _bodyTransform;
    private Rigidbody2D _rigidbody;

    [Header("Settings")]
    [SerializeField] private float _movementSpeed = 4f; //�̵��ӵ�
    [SerializeField] private float _turningRate = 30f; //ȸ���ӵ�

    private Vector2 _prevMovementInput;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return; //������ ��츸 ó��
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
        //����� ������ ����
        if (!IsOwner) return;

        float zRotation = _prevMovementInput.x * -_turningRate * Time.deltaTime;
        _bodyTransform.Rotate(0, 0, zRotation);
    }

    private void FixedUpdate()
    {
        //����� ������ ����
        if (!IsOwner) return; //���ʰ� �ƴϸ� ����

        _rigidbody.velocity = _bodyTransform.up * (_prevMovementInput.y * _movementSpeed);
    }
}

