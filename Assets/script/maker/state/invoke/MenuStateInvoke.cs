public class MenuStateInvoke : StateInvoke<MenuState> {
  protected override MakerStateType StateType {
    get {
      return MakerStateType.Menu;
    }
  }
}