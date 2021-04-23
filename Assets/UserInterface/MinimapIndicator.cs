using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapIndicator : MonoBehaviour
{
	private Transform _target;
	[SerializeField] private Material pMAt;
	[SerializeField] private Material kMAt;
	private MeshRenderer rend;
	private Canvas _canvas;
	private Vector3 baseScale = new Vector3(0.01f,0.01f,0.01f);
	private Vector3 maxScale =  new Vector3(0.2f,0.2f,0.2f);
	public Transform Target
	{
		get => _target;
		set
		{
			_target = value;
			rend.material = _target.GetComponent<FirstPersonController>() != null ? pMAt : kMAt;
		}
	}

	private void Awake()
	{
		rend = GetComponentInChildren<MeshRenderer>();
		_canvas = GetComponentInChildren<Canvas>();
	}

	Vector3 _offset  = new Vector3(0,20,0);
	
	private void LateUpdate()
	{
		if(Target==null) return;
		transform.position = Target.position + _offset;
	}

	public void RadarPing()
	{
		StartCoroutine(AnimateRadar());
	}

	private IEnumerator AnimateRadar()
	{
		while (_canvas.transform.localScale != maxScale)
		{
			_canvas.transform.localScale += new Vector3(0.01f,0.01f,0.01f);
			yield return null;
		}

		_canvas.transform.localScale = baseScale;
	}
}
