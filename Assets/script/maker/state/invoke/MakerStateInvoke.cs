public class MakerStateInvoke : StateInvoke<MakerState> {
  protected override MakerStateType StateType {
    get {
      return MakerStateType.General;
    }
  }
}