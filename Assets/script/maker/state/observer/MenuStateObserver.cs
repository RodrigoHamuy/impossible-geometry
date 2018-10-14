class MenuStateObserver : StateObserver<MenuState> {
  protected override MakerStateType StateType {
    get {
      return MakerStateType.Menu;
    }
  }
}