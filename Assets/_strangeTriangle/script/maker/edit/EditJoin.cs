using System.Collections.Generic;
using Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent (typeof (EditManager))]
public class EditJoin : MonoBehaviour {

  public Toggle joinBlocksToggle;

  [HideInInspector]
  public bool isActive;

  Transform world;
  EditManager editManager;
  MakerActionsManager actionsManager;
  TouchComponent touchComponent;

  List<SelectStyleMnger> potentialBlocks = new List<SelectStyleMnger> ();

  MakerBlockType CubeMaker;
  MakerBlockType HalfCubeMaker;
  Color joinBlockColor = new Color (
    255.0f / 255.0f,
    215.0f / 255.0f,
    0 / 255.0f
  );

  void Start () {

    world = GameObject.Find ("World").transform;

    actionsManager = GameObject.FindObjectOfType<MakerActionsManager> ();
    editManager = GetComponent<EditManager> ();
    editManager.OnTargetChange.AddListener (OnTargetChange);

    touchComponent = GetComponent<TouchComponent> ();
    touchComponent.onTouchEnd.AddListener (OnTouchEnd);

    joinBlocksToggle.onValueChanged.AddListener (value => {

      if (!value) return;

      OnJoinBlocksClick ();

    });

    CubeMaker = actionsManager.GetMakerBlockType (MakerBlockType.Names.CubeMaker);
    HalfCubeMaker = actionsManager.GetMakerBlockType (MakerBlockType.Names.HalfCubeMaker);

  }

  void OnDisable () {

    Clear ();

  }

  void OnTargetChange (bool value) {

    Clear ();

    if (editManager.selected.Count != 1) {

      joinBlocksToggle.gameObject.SetActive (false);
      return;

    }

    var target = editManager.selected[0].GetTarget ();
    var targetData = target.GetComponent<EditableBlock> ().data;

    if (targetData.blockType == MakerBlockType.Names.RotateHandleMaker) {

      joinBlocksToggle.gameObject.SetActive (false);
      return;

    }

    joinBlocksToggle.gameObject.SetActive (true);

  }

  float CalculateYMargin (Vector3 targetPos, Vector2 targetScreenPos) {

    Vector2 marginPos = Camera.main.WorldToScreenPoint (targetPos + Vector3.up);

    return Mathf.Abs (marginPos.y - targetScreenPos.y) * .5f;

  }

  float CalculateRightYAngle (Vector3 targetPos, Vector2 targetScreenPos) {

    Vector2 rightScreenPos = Camera.main.WorldToScreenPoint (targetPos + Vector3.right);

    return Vector2.Angle (targetScreenPos - rightScreenPos, Vector2.up);

  }

  float CalculateForwardYAngle (Vector3 targetPos, Vector2 targetScreenPos) {

    Vector2 rightScreenPos = Camera.main.WorldToScreenPoint (targetPos - Vector3.forward);

    return Vector2.Angle (targetScreenPos - rightScreenPos, Vector2.up);

  }

  void OnJoinBlocksClick () {

    var target = editManager.selected[0].GetTarget ();
    Vector2 targetScreenPos = Camera.main.WorldToScreenPoint (target.position);

    var yMargin = CalculateYMargin (target.position, targetScreenPos);

    var yRightAngle = CalculateRightYAngle (target.position, targetScreenPos);
    var yForwardAngle = CalculateForwardYAngle (target.position, targetScreenPos);

    foreach (var block in actionsManager.blocksInScene) {

      if (block == target) continue;

      Vector2 currScreenPos = Camera.main.WorldToScreenPoint (block.position);
      var diff = targetScreenPos - currScreenPos;
      var currAngle = Vector2.Angle (targetScreenPos - currScreenPos, Vector2.up);

      if (
        (
          currAngle > yRightAngle + .001f ||
          currAngle < yRightAngle - .001f
        ) &&
        (
          currAngle > yForwardAngle + .001f ||
          currAngle < yForwardAngle - .001f
        ) &&
        Mathf.Abs (diff.x) > yMargin
      ) continue;

      var select = new SelectStyleMnger ();
      select.SetColor (joinBlockColor);
      select.Select (block);
      potentialBlocks.Add (select);

    }

    isActive = potentialBlocks.Count > 0;

  }

  void Clear () {

    foreach (var item in potentialBlocks) {

      item.Deselect ();

    }

    potentialBlocks.Clear ();
    isActive = false;

    joinBlocksToggle.isOn = false;

  }

  void OnTouchEnd (Vector2 touchPos) {

    if (!isActive) return;

    var block = Utility.MakerGetBlockOnTapPos (touchPos);

    if (!block) return;

    var target = editManager.selected[0].GetTarget ();
    Vector2 targetScreenPos = Camera.main.WorldToScreenPoint (target.position);
    var yMargin = CalculateYMargin (target.position, targetScreenPos);
    var yRightAngle = CalculateRightYAngle (target.position, targetScreenPos);
    var yForwardAngle = CalculateForwardYAngle (target.position, targetScreenPos);

    Vector2 currScreenPos = Camera.main.WorldToScreenPoint (block.position);
    var diff = targetScreenPos - currScreenPos;
    var currAngle = Vector2.Angle (targetScreenPos - currScreenPos, Vector2.up);

    if (Mathf.Abs (diff.x) < yMargin) {

      JoinBlocksVertically (target.position, block.position);

    } else if (
      (
        currAngle < yRightAngle + .001f &&
        currAngle > yRightAngle - .001f
      ) ||
      (
        currAngle < yForwardAngle + .001f &&
        currAngle > yForwardAngle - .001f
      )
    ) {

      JoinBlocksHorizontally (target.position, block.position);

    }
  }

  void JoinBlocksHorizontally (Vector3 start, Vector3 end) {

    var startInEndPlane = projectPosAtPlane (end, Vector3.up, start);

    var endAtStartPlane = projectPosAtPlane (start, Vector3.up, end);

    var startToEnd = end - startInEndPlane;
    var startToEndDir = startToEnd.normalized;
    var distance = startToEnd.magnitude;

    var middle = Mathf.Round (distance * .5f);

    for (int i = 0; i < distance; i++) {

      var isMiddle = i == middle;

      if (isMiddle) {

        var startRotation = Quaternion.LookRotation (startToEnd, Vector3.up);
        var endRotation = Quaternion.LookRotation (-startToEnd, Vector3.up);
        var nextPosStartPlane = start + startToEndDir * (i + 1);
        var nextPosEndPlane = startInEndPlane + startToEndDir * (i + 1);

        actionsManager.AddBlock (
          new MakerAction (
            MakerActionType.Add,
            null,
            HalfCubeMaker,
            nextPosStartPlane,
            Vector3.one,
            startRotation,
            world
          ),
          false
        );

        actionsManager.AddBlock (
          new MakerAction (
            MakerActionType.Add,
            null,
            HalfCubeMaker,
            nextPosEndPlane,
            Vector3.one,
            endRotation,
            world
          ),
          false
        );

      } else {

        Vector3 nextPos;

        if (i < middle) {

          nextPos = start + startToEndDir * (i + 1);

        } else {

          nextPos = startInEndPlane + startToEndDir * (i + 1);

        }

        actionsManager.AddBlock (
          new MakerAction (
            MakerActionType.Add,
            null,
            CubeMaker,
            nextPos,
            Vector3.one,
            Quaternion.identity,
            world
          ),
          false
        );

      }

    }

  }

  static Vector3 projectPosAtPlane (Vector3 origin, Vector3 normal, Vector3 posWorld) {

    var posScreen = Camera.main.WorldToScreenPoint (posWorld);

    var posRay = Camera.main.ScreenPointToRay (posScreen);

    var plane = new Plane (normal, origin);

    float posDist;

    plane.Raycast (posRay, out posDist);

    return posRay.origin + posRay.direction * posDist;

  }

  void JoinBlocksVertically (Vector3 from, Vector3 to) {

    if (from.y > to.y) {

      var temp = to;
      to = from;
      from = to;

    }

    var fromScreen = Camera.main.WorldToScreenPoint (from);
    var toScreen = Camera.main.WorldToScreenPoint (to);

    var middScreen = fromScreen + (toScreen - fromScreen) * .5f;

    AddBlock (from, Vector3.up, middScreen);
    AddBlock (to, -Vector3.up, middScreen);

  }

  void AddBlock (Vector3 pos, Vector3 dir, Vector3 middScreen) {

    var middWorld = Utility.GetPlaneHitFromScreen (middScreen, pos, Vector3.right);
    middWorld = Utility.Round (middWorld, 1.0f);

    var canAdd = true;

    while (canAdd) {

      pos += dir;

      var blockType = pos == middWorld ?
        HalfCubeMaker :
        CubeMaker;

      var rotation = pos == middWorld ?
        Quaternion.LookRotation (dir, Vector3.right) :
        Quaternion.identity;

      actionsManager.AddBlock (
        new MakerAction (
          MakerActionType.Add,
          null,
          blockType,
          pos,
          Vector3.one,
          rotation,
          world
        ),
        false
      );

      canAdd = pos != middWorld;

    }

  }

}