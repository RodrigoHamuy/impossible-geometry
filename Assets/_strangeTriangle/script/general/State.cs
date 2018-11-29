public class State {

  public UnityEventBool OnChange = new UnityEventBool ();

  private bool enable;

  private string id;
  private string type;

  public State (string id, string type) {
    this.id = id;
  }

  public string Type {
    get {
      return type;
    }
  }

  public bool Enable {
    get {
      return enable;
    }
  }

  public string Id {
    get {
      return id;
    }
  }

  public void Enter () {
    Change (true);
  }

  public void Exit () {
    Change (false);
  }

  void Change (bool value) {
    enable = value;
    OnChange.Invoke (enable);
  }

}