class EditorStateObserver : StateObserver<EditorState> {
  protected override MakerStateType StateType {
    get {
      return MakerStateType.EditMode;
    }
  }
}