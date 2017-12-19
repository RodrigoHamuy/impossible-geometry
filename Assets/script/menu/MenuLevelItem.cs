using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuLevelItem : MonoBehaviour, IPointerClickHandler
{

	public string level;
	public Animator fadeAnim;

	bool loadingLevel = false;

    public void OnPointerClick(PointerEventData eventData){

		// if (! Utility.canPlayerMove) return;

		if( loadingLevel ) return;

		if ( ! isFacing() ) return;

		StartLoadLevel();

	}

	bool isFacing(){
		var cam = Camera.main;

		return Vector3.Dot( cam.transform.forward, transform.forward ) > 0.1f;
	}

	public void StartLoadLevel(){
		loadingLevel = true;

		fadeAnim.SetTrigger("onLevelComplete");

		Invoke( "LoadLevel", 1 );

	}

	void LoadLevel(){
		SceneManager.LoadScene(level);
		Utility.canPlayerMove = true;
	}
}
