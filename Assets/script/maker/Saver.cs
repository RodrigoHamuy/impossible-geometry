class Saver{

  List<Transform> blocks = new List<Transform>();

  public void OnBlockAdded(Transform block) {
  
    blocks.push(block);
  
  }
  
  public void OnBlockRemoved(Transform block) {
  
    blocks.remove(block);
  
  }
  
  public void Save(){
  
    throw "not yet implemented";
  
  }

}
