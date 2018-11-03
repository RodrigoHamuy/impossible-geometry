using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MakerBtn : MonoBehaviour {

	public MakerBtnDataType style;

	public void InitBtn () {

		var toggle = GetComponent<Toggle> ();

		var imgs = new List<Image> ();

		imgs.Add (
			transform.Find ("Deselected/DeselectedImg").GetComponent<Image> ()
		);

		imgs.Add (
			transform.Find ("Selected/SelectedImg").GetComponent<Image> ()
		);

		foreach (var img in imgs) {

			img.sprite = style.sprite;

		}

	}

}