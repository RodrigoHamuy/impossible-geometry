public static class LevelMakerConfig {

  public static LevelMakerSetting Data = null;

}

public class LevelMakerSetting {

  public LevelMakerSetting (string name, bool load, bool save, string levelData) {

    LevelName = name;
    CanLoad = load;
    CanSave = save;
    AvailableOffline = true;
    LevelData = levelData;

  }

  public LevelMakerSetting (string name, bool load, bool save) {

    LevelName = name;
    CanLoad = load;
    CanSave = save;
    AvailableOffline = false;

  }

  public bool AvailableOffline {
    get;
    private set;
  }

  public string LevelData {
    get;
    private set;
  }

  public bool CanLoad {
    get;
    private set;
  }

  public bool CanSave {
    get;
    private set;
  }

  public string LevelName {
    get;
    private set;
  }

}