using System.Collections.Generic;
using Generic;
using UnityEngine;

enum AddRotationHandlerStep {
  SelectBlocks,
  ChooseCenter,
  AddHandler
}

public class AddRotationHandler : MonoBehaviour {

  List<Transform> blocks = new List<Transform> ();
  List<Color> blocksColor = new List<Color> ();
  AddRotationHandlerStep state;

  void Start () {

    var touchComponent = GetComponent<TouchComponent> ();

    touchComponent.onTouchEnd.AddListener (OnTouchEnd);

  }

  void OnEnable () {

    state = AddRotationHandlerStep.SelectBlocks;

  }

  void Select (Vector2 touchPos) {

    var block = Utility.GetBlocksOnTapPos (touchPos);

    if (!block) return;

    else if (blocks.Exists (b => b == block)) {

      var i = blocks.IndexOf(block);
      var color = blocksColor[i];
      var renderer = block.GetComponent<Renderer> ();

      renderer.material.color = color;

      blocks.RemoveAt(i);
      blocksColor.RemoveAt(i);

    } else {

      blocks.Add (block);
      var renderer = block.GetComponent<Renderer> ();
      blocksColor.Add (renderer.material.color);
      renderer.material.color = Color.grey;

    }

  }

  void OnTouchEnd (Vector2 touchPos) {

    Select (touchPos);

  }

}