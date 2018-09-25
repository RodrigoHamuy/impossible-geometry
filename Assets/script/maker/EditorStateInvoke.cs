using UnityEngine;

public class EditorStateInvoke : MonoBehaviour {

  public EditorState stateToInvoke;

  public UnityEventEditorState OnStateInvoke;

  public void Invoke(){
    OnStateInvoke.Invoke(stateToInvoke);
  }

}