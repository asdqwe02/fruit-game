using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Int64 _userId = -1;
    [SerializeField] private PlayerSide _playerSide;

    public PlayerSide playerSide
    {
        get { return _playerSide; }
    }


    public Int64 UserID
    {
        get { return _userId; }
        set
        {
            _userId = value;
            OnUserIDUpdate();
        }
    }

    [SerializeField] private List<Blade> _blades;

    private void Awake()
    {
        _blades = GetComponentsInChildren<Blade>().ToList();
    }


    public void UpdateBladeUserID(Int64 userID)
    {
        foreach (var blade in _blades)
        {
            blade.userID = userID;
        }
    }

    void OnUserIDUpdate()
    {
        UpdateBladeUserID(_userId);
    }

    public void DisableBlades()
    {
        foreach (var blade in _blades)
        {
            blade.enabled = false;
        }
    }

    public void EnableBlades()
    {
        foreach (var blade in _blades)
        {
            blade.enabled = true;
        }
    }

    public enum PlayerSide
    {
        LEFT,
        RIGHT
    }
}