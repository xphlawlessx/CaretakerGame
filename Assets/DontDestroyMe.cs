﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyMe : MonoBehaviour
{
	private void Awake()
	{
		DontDestroyOnLoad(this.gameObject);
		PlayerPrefs.DeleteAll();
	}
}
