﻿using System;
using System.Collections.Generic;
using System.Linq;
using Actors;
using Environment;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private GameObject kidPrefab;

    [SerializeField] private GameObject careTakerPrefab;
    [SerializeField] private GameObject miniMapPrefab;
    [SerializeField] private Transform playerSpawn;
    private readonly List<Vector3> _enterExits = new List<Vector3>();
    private readonly int _groupsMax = 4;
    private readonly float _tMax = 10f;
    private int _groups;
    private Vector3 _kidPos;
    private int _kidsCount;
    private int _kidsEscaped;
    private int _kidsIdentified;
    private float _levelTime = 10f; // in minutes , converted to seconds in Start
    private List<RoomObjective> _rooms;
    private float _t;
    private int _totalDamageValue;
    private PlayerClass player;

    private void Awake()
    {
        player = Instantiate(careTakerPrefab, playerSpawn.position, Quaternion.identity).GetComponent<PlayerClass>();
        var mini = Instantiate(miniMapPrefab, playerSpawn.position, Quaternion.identity)
            .GetComponent<MinimapIndicator>();
        mini.Target = player.transform;
        player.minimapIndicatior = mini.gameObject;
    }

    private void Start()
    {
        _t = _tMax;
        _levelTime *= 60;
        var exits = FindObjectsOfType<LevelEnterExit>();
        foreach (var e in exits) _enterExits.Add(e.transform.position);
        _rooms = FindObjectsOfType<RoomObjective>().ToList();
        foreach (var room in _rooms) room.OnRoomDestroyed += RemoveRoom;

        SpawnKids();
        var kids = FindObjectsOfType<Kid>();
        _kidsCount = kids.Length;
        foreach (var kid in kids) OnAllRoomsCleared += kid.LeaveLevel;
    }

    private void Update()
    {
        _levelTime -= Time.deltaTime;
        if (_levelTime < 0) EndLevel();
        if (_groups > _groupsMax) return;
        _t -= Time.deltaTime;
        if (_t <= 0)
        {
            _t = _tMax;
            SpawnKids();
        }
    }

    private void EndLevel()
    {
        PlayerPrefs.SetFloat("Time", Mathf.Clamp(_levelTime, 0, 999));
        PlayerPrefs.SetInt("Escaped", _kidsEscaped);
        PlayerPrefs.SetInt("Identified", _kidsIdentified);
        PlayerPrefs.SetInt("Damage", _totalDamageValue);
        SceneManager.LoadScene("Score");
    }

    private event Action OnAllRoomsCleared;


    private void SpawnKids()
    {
        _groups++;

        var groupSize = Random.Range(2, 5);
        var pos = _enterExits[Random.Range(0, _enterExits.Count)];
        var kids = new List<Kid>();
        var group = new KidGroup();
        for (var i = 0; i < groupSize; i++)
        {
            var k = Instantiate(kidPrefab, pos, Quaternion.identity).GetComponent<Kid>();
            var mini = Instantiate(miniMapPrefab, pos, Quaternion.identity).GetComponent<MinimapIndicator>();
            mini.Target = k.transform;
            k.minimapIndicator = mini.gameObject;
            k.minimapIndicator.SetActive(false);
            k.transform.position = pos;
            kids.Add(k);
            k.Group = group;
            k.playerHasGoodMemory = player.SelectedClass == PlayerClass.CaretakerClass.Detective;
        }

        group.Kids = kids;
    }

    public void DestroyKid(Kid kid)
    {
        if (kid.isIdentified)
            _kidsIdentified++;
        else
            _kidsEscaped++;

        _kidsCount--;
        kid.Group.Kids.Remove(kid);
        Destroy(kid.gameObject);
    }

    public RoomObjective GetRoom(Vector3 pos)
    {
        if (_rooms.Count == 0)
        {
            OnAllRoomsCleared?.Invoke();
            return null;
        }

        _kidPos = pos;
        _rooms.Sort(SortByDistanceFromKids);
        var slice = _rooms.Count / 2;
        var objective = Random.Range(0f, 1f) > 0.5f //Todo soemthing better
            ? _rooms[Random.Range(0, slice)]
            : _rooms[Random.Range(_rooms.Count - slice, _rooms.Count - 1)];

        return objective;
    }

    public void AddDamageCashValue(int cashValue)
    {
        _totalDamageValue += cashValue;
    }


    private void RemoveRoom(RoomObjective room)
    {
        _rooms.Remove(room);
    }

    public Vector3 GetExitPos()
    {
        return _enterExits[Random.Range(0, _enterExits.Count)];
    }

    private int SortByDistanceFromKids(RoomObjective roomA, RoomObjective roomB)
    {
        var distA = (roomA.DoorPos - _kidPos).sqrMagnitude;
        var distB = (roomB.DoorPos - _kidPos).sqrMagnitude;
        return distA.CompareTo(distB);
    }
}