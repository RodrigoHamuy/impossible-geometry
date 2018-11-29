class MakerStateObserver : StateObserver<MakerState> {
  protected override MakerStateType StateType {
    get {
      return MakerStateType.General;
    }
  }
}