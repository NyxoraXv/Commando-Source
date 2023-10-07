using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace sunny.mask{
	[ExecuteInEditMode]
public class RemoteUIAdvancedMask : UIAdvancedMaskBase {

	private Material uiMaterial;

	public GameObject[] targets;
	public bool addToChildren;
	private List<Material> uiMaterials = new List<Material>();
	private List<Material> worldMaterials = new List<Material>();


	private Matrix4x4 uiMatrix;
	private Matrix4x4 worldMatrix;
	// Use this for initialization
	void OnEnable () {

		updateMaskedObjects ();
		updateMaskMaterials();
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if(Application.isPlaying){
			if (updateEveryFrame){
				updateMaskedObjects ();
				updateMaskMaterials();
			}
		}else{
			updateMaskedObjects ();
			updateMaskMaterials();
		}
	
	}

	public void updateMaskedObjects(){
		if (uiMaterial == null) {
				uiMaterial = new Material (Shader.Find ("Advanced Masked/UI"));
		}
		uiMaterials.Clear();
		uiMaterials.Add (uiMaterial);
		if(targets != null&& targets.Length>0)
			foreach (GameObject gObj in targets) {
				if (gObj == null) {
				} else if (addToChildren) {
					foreach (Graphic g in gObj.GetComponentsInChildren<Graphic>(true)){
						Material ma = setMaterialToUI (g, uiMaterial);
						if(ma )uiMaterials.Add (ma);
					}
						foreach(Renderer t in gObj.GetComponentsInChildren<Renderer>()){

						worldMaterials.AddRange(t.sharedMaterials);
					}
				} else {
					if( GetComponent<Graphic>())
						setMaterialToUI (GetComponent<Graphic>(), uiMaterial);
					if( GetComponent<Renderer>())
						worldMaterials.AddRange(GetComponent<Renderer>().sharedMaterials);
						print (gObj.name+": "+GetComponent<Renderer>());
				}

			}
	}
	public void updateMaskMaterials(){
		Canvas canvas = GetComponentInParent<Canvas> ();
		RectTransform rT = GetComponent<RectTransform> ();
			if (canvas) {
				uiMatrix = UIAdvancedMaskBase.getMatrixOnCanvas (rT, canvas);
				insertDataToMaterial (uiMaterials, texture, uiMatrix);
			} else {
				worldMatrix = UIAdvancedMaskBase.getMatrixOnWorld (rT);
				insertDataToMaterial (worldMaterials, texture, worldMatrix);
			}
			worldMaterials.Clear ();
	}
		void OnDrawGizmos(){
			RectTransform rT = GetComponent<RectTransform> ();
			if(rT){
				Vector3[] corners = new Vector3[4];
				rT.GetWorldCorners (corners);
				Gizmos.DrawLine (corners[0], corners[1]);
				Gizmos.DrawLine (corners[1], corners[2]);
				Gizmos.DrawLine (corners[2], corners[3]);
				Gizmos.DrawLine (corners[3], corners[0]);
			}
	}
	void OnDisable () {
		disabledDataToMaterial(uiMaterials);
		disabledDataToMaterial(worldMaterials);
	}
}
}
