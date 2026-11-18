using System;
using PurrNet;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestNetwork : NetworkIdentity
{
    public InputAction playerMovement;

    public InputAction changeColor;

    public InputAction changeHealth;

    [SerializeField] private TMP_Text _healthText;

    [SerializeField] private Color _color;

    [SerializeField] private Renderer _renderer;

    [SerializeField] private NetworkIdentity _networkIdentity;

    [SerializeField] private SyncVar<int> _health = new(100);

    [SerializeField] private int _localHealth = 100;

    private void Awake()
    {
        _health.onChanged += OnChanged;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        _health.onChanged -= OnChanged;
    }

    private void OnChanged(int newValue)
    {
        _healthText.text = newValue.ToString();
    }

    private void Start()
    {
        playerMovement.Enable();
        changeColor.Enable();
        changeHealth.Enable();
    }

    private void Update()
    {
        if (changeColor.triggered)
            SetColor(_color);

        //if (changeHealth.triggered)
        //TakeDamage(10);

        if (changeHealth.triggered)
            SetHealth(_localHealth - 10);

    }


    //An RPC is a Command or instrucition for somoene to run something(i.e.: Client instructing Server to run some piece of Code)
    [ObserversRpc(bufferLast:true)] //Helpful when player join and calls the latest change in the SetColor

    private void SetColor(Color color)
    {
        _renderer.material.color = color;
    }

    private void SetHealth(int health)
    {
        _localHealth = health;
    }


    protected override void OnSpawned()
    {
        base.OnSpawned();

        if (!isServer)
            return;

        Instantiate(_networkIdentity, Vector3.zero, Quaternion.identity);


    }

    [ServerRpc]
    private void TakeDamage(int damage)
    {
        _health.value -= damage;
    }


}

//53:00