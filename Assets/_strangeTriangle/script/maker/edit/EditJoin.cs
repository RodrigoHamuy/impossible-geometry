using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof (EditManager))]
public class EditJoin : MonoBehaviour {

  public GameObject joinBlocksBtn;

  EditManager editManager;
  MakerActionsManager actionsManager;

  Color joinBlockColor = new Color (
    255.0f / 255.0f,
    215.0f / 255.0f,
    0 / 255.0f
  );

  void Start () {

    actionsManager = GameObject.FindObjectOfType<MakerActionsManager> ();
    editManager = GetComponent<EditManager> ();
    editManager.OnTargetChange.AddListener (OnTargetChange);

    var toggle = joinBlocksBtn.GetComponent<Toggle> ();
    toggle.onValueChanged.AddListener (value => {

      if (!value) return;

      OnJoinBlocksClick ();

    });

  }

  void OnTargetChange (bool value) {

    joinBlocksBtn.SetActive (
      (!editManager.selectMultiple && editManager.selected.Count == 1)
    );

  }

  void OnJoinBlocksClick () {

    var target = editManager.selected[0].GetTarget ();
    var targetScreenPos = Camera.main.WorldToScreenPoint (target.position);
    var marginPos = Camera.main.WorldToScreenPoint (target.position + Vector3.up);

    var margin = Mathf.Abs (marginPos.y - targetScreenPos.y) * .5f;

    var potentialBlocks = new List<SelectStyleMnger> ();

    foreach (var block in actionsManager.blocksInScene) {

      var screenCoords = Camera.main.WorldToScreenPoint (block.position);
      var diff = targetScreenPos - screenCoords;

      if (Mathf.Abs (diff.x) > margin) continue;

      var select = new SelectStyleMnger ();
      select.SetColor (joinBlockColor);
      select.Select (block);
      potentialBlocks.Add (select);

    }

  }

}