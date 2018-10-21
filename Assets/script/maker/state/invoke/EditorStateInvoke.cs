public class EditorStateInvoke : StateInvoke<EditorState> {
  protected override MakerStateType StateType {
    get {
      return MakerStateType.EditMode;
    }
  }
}