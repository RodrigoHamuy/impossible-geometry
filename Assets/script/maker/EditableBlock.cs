using System.Collections.Generic;
using UnityEngine;

public class EditableBlock : MonoBehaviour {

  public EditableBlockData data = new EditableBlockData ();

  public void SyncData () {

    data.position = transform.localPosition;
    data.rotation = transform.localRotation;
    data.scale = transform.localScale;

    var parentData = transform.parent.GetComponent<EditableBlock> ();

    if (parentData) {

      data.parent = parentData.data.id;

    } else {

      data.parent = -1;

    }

  }

  public void SyncTransform (List<EditableBlock> allBlocks) {

    if (data.parent != -1) {

      var parent = allBlocks.Find (b => b.data.id == data.parent);
      transform.parent = parent.transform;

    }

    if (data.rotateControllerId != -1) {

      var rotateController = allBlocks.Find (
          b => b.data.id == data.rotateControllerId
        )
        .GetComponent<RotateController> ();

      rotateController.AddRotateTouchEmitter (
        GetComponentInChildren<RotateTouchEmitter> ()
      );

    }

    transform.localPosition = data.position;
    transform.localRotation = data.rotation;
    transform.localScale = data.scale;

  }

}