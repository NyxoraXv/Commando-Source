using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace sunny.mask{
	[ExecuteInEditMode]
public class UIAdvancedMask : UIAdvancedMaskBase,ICanvasRaycastFilter {
	private Material uiMaterial;

	[Tooltip("the performance will be faster if it's a readable Texture2D")]
	public bool maskOnRaycast = true;

	private Matrix4x4 uiMatrix;
	private Matrix4x4 worldMatrix;
	private List<Material> uiMaterials = new List<Material>();
	private List<Material> worldMaterials = new List<Material>();

	void OnEnable () {
		updateMaskedObjects ();
		updateMaskMaterials();
	}
	public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera){
		if (!maskOnRaycast)
			return true;;
		var rectTransform = (RectTransform)transform;
		Vector2 localPositionPivotRelative;
		RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform) transform, sp, eventCamera, out localPositionPivotRelative);

		float x = 0;
		float y = 0;

		//get uv
		x = localPositionPivotRelative.x / rectTransform.rect.width;
		y = localPositionPivotRelative.y / rectTransform.rect.height;

		x += rectTransform.pivot.x;
		y += rectTransform.pivot.y;

		if (forceClampX)
			x = Mathf.Clamp01 (x);
		if (forceClampY)
			y = Mathf.Clamp01 (y);
		
		Color result =getColorFromPixel(texture,x,y);
		float a = UseRedAsAlpha ? result.r : result.a;
		if (flippedAlpha)
			a = 1 - a;
		a*=alpha;
		return a > .5f;
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

	void OnTransformChildrenChanged(){
		updateMaskedObjects ();

	}
	public void updateMaskedObjects(){
		if (uiMaterial == null) {
				uiMaterial = new Material (Shader.Find ("Advanced Masked/UI"));
		}
		{
			uiMaterials.Clear();
			uiMaterials.Add (uiMaterial);

			foreach (Graphic g in GetComponentsInChildren<Graphic>()){
				Material ma = setMaterialToUI (g, uiMaterial);
				if(ma )
					uiMaterials.Add (ma);
			}


		}
		{
			worldMaterials.Clear();
			foreach(Renderer t in GetComponentsInChildren<Renderer>()){
				worldMaterials.AddRange(t.sharedMaterials);
			}
					

		}
		//
	}
	public void updateMaskMaterials(){
		RectTransform rT = GetComponent<RectTransform> ();	
		if (uiMaterials.Count>0) {
			Canvas canvas = GetComponentInParent<Canvas> ();
			uiMatrix = UIAdvancedMaskBase.getMatrixOnCanvas (rT, canvas);
			insertDataToMaterial (uiMaterials, texture, uiMatrix);
		} 
		if (worldMaterials.Count>0) {
			worldMatrix = UIAdvancedMaskBase.getMatrixOnWorld (rT);
				insertDataToMaterial (worldMaterials, texture, worldMatrix);
				worldMaterials.Clear ();
		}

	}

	void OnDisable () {
		disabledDataToMaterial(uiMaterials);
		disabledDataToMaterial(worldMaterials);
	}


	void OnDrawGizmos(){

		RectTransform rT = GetComponent<RectTransform>();
		Vector3[] corners = new Vector3[4];
		rT.GetWorldCorners(corners);
		Gizmos.DrawLine(corners[0],corners[1]);
		Gizmos.DrawLine(corners[1],corners[2]);
		Gizmos.DrawLine(corners[2],corners[3]);
		Gizmos.DrawLine(corners[3],corners[0]);


	}
}

}