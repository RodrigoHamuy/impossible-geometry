using System.Collections.Generic;
using UnityEngine;

public class Saver{

  List<Transform> blocks = new List<Transform>();

  public void OnBlockAdded(Transform block) {
  
    blocks.Add(block);
  
  }
  
  public void OnBlockRemoved(Transform block) {
  
    blocks.Remove(block);
  
  }
  
  public void Save(){
  
    throw new UnityException("not yet implemented");
  
  }

}
