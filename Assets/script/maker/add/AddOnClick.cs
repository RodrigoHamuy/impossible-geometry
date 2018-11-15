﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maker {

  public class AddOnClick : MonoBehaviour {

    public Transform blockPrefab;
    public Transform blockPreviewPrefab;

    public SpawnerMode mode = SpawnerMode.Block;

    Renderer blockPreview;

    void Start () {

      blockPreview = Instantiate (blockPreviewPrefab, Vector3.zero, Quaternion.identity).GetComponent<Renderer> ();

      blockPreview.enabled = false;

    }

    public void SetMode (SpawnerMode mode) {

      this.mode = mode;

    }

    public void StartBlockPreview (Vector2 screenPos) {

      blockPreview.transform.position = TouchUtility.MakerHitPosition (screenPos, gameObject);
      blockPreview.enabled = true;

    }

    public void MoveBlockPreview (Vector2 screenPos) {

      blockPreview.transform.position = TouchUtility.MakerHitPosition (screenPos, gameObject);

    }

    public void AddBlock (Vector2 screenPos) {

      blockPreview.enabled = false;

    }

  }

}