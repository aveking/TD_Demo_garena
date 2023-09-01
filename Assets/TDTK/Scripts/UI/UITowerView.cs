using UnityEngine;
using UnityEngine.UI;

using System.Collections;
using System.Collections.Generic;

using TDTK;

namespace TDTK {

	public class UITowerView : MonoBehaviour {

		private Transform thisT;
		private GameObject thisObj;
		private CanvasGroup canvasGroup;
		private static UITowerView instance;
		public static UITowerView GetInstance(){ return instance; }
		
		
		private UnitTower currentTower;
		
		public RectTransform towerPanelRectT;
		private float towerPanelPosX;
		
		public Text lbTowerName;
		public Text lbTowerLevel;
		public Text lbTowerDesp1;
		public Text lbTowerDesp2;
		
		public GameObject butUpgradeObj1;
		public GameObject butUpgradeObj2;
		public GameObject butSellObj;
		public GameObject butFPSObj;
		
		private CanvasGroup butUpgrade1Canvas;
		private CanvasGroup butUpgrade2Canvas;
		
		public GameObject directionControlObj;
		public Slider sliderDrection;
		
		public GameObject rscPanelObj;
		public List<UIObject> rscItemList=new List<UIObject>();
		
		
		//public RectTransform buttonParent;
		//private Vector3 buttonParentDefaultPos;
		
		void Awake(){
			instance=this;
			thisT=transform;
			thisObj=gameObject;
			canvasGroup=thisObj.GetComponent<CanvasGroup>();
			if(canvasGroup==null) canvasGroup=thisObj.AddComponent<CanvasGroup>();
			
			canvasGroup.alpha=0;
			thisT.localPosition=new Vector3(0, 9999, 0);
			
			butUpgrade1Canvas=butUpgradeObj1.GetComponent<CanvasGroup>();
			butUpgrade2Canvas=butUpgradeObj2.GetComponent<CanvasGroup>();
		}
		
		
		void Start(){
			List<Rsc> rscList=ResourceManager.GetResourceList();
			for(int i=0; i<rscList.Count; i++){
				if(i==0) rscItemList[0].Init();
				else rscItemList.Add(UIObject.Clone(rscItemList[0].rootObj, "Rsc"+(i+1)));
				
				rscItemList[i].imgRoot.sprite=rscList[i].icon;
				rscItemList[i].label.text=rscList[i].value.ToString();
			}
			
			rscPanelObj.SetActive(false);
			
			//buttonParentDefaultPos=buttonParent.localPosition;
			
			towerPanelPosX=towerPanelRectT.localPosition.x;
			
			//thisObj.SetActive(false);
		}
		
		
		void OnEnable(){
			TDTK.onTowerUpgradingE += Show;
			TDTK.onTowerDestroyedE += OnRemoveTower;
			TDTK.onTowerSoldE += OnRemoveTower;
		}
		void OnDisable(){
			TDTK.onTowerUpgradingE -= Show;
			TDTK.onTowerDestroyedE -= OnRemoveTower;
			TDTK.onTowerSoldE -= OnRemoveTower;
		}
		
		
		void Update(){
			if(currentTower==null) return;
			
			Vector3 screenPos=Camera.main.WorldToScreenPoint(currentTower.thisT.position);
			
			float x=screenPos.x>Screen.width/2 ? -towerPanelPosX : towerPanelPosX ;
			
			towerPanelRectT.localPosition=new Vector3(x, towerPanelRectT.localPosition.y, 0);
			
			if(Input.GetKeyDown(KeyCode.Escape)) Hide();
		}
		
		
		void OnRemoveTower(UnitTower tower){
			if(tower==currentTower) Hide();
		}
		
		
		
		public void OnSellButton(){
			currentTower.Sell();
			Hide();
		}
		public void OnUpgradeButton(int index){
			string exception=currentTower.Upgrade(index);
			if(exception!="") UIMessage.DisplayMessage(exception);
		}
		
		public void OnHoverSellButton(){
			if(currentTower==null) return;
			UpdateResourcePanel(currentTower.GetValue());
			rscPanelObj.SetActive(true);
		}
		public void OnExitSellButton(){
			rscPanelObj.SetActive(false);
		}
		
		public void OnHoverUpgradeButton(int index){
			if(currentTower==null) return;
			if(index==0 && butUpgrade1Canvas.alpha<1) return;
			if(index==1 && butUpgrade2Canvas.alpha<1) return;
			
			UpdateResourcePanel(currentTower.GetCost(index));
			rscPanelObj.SetActive(true);
		}
		public void OnExitUpgradeButton(){
			rscPanelObj.SetActive(false);
		}
		
		
		public void UpdateResourcePanel(List<int> costList){
			for(int i=0; i<rscItemList.Count; i++){
				rscItemList[i].label.text=costList[i].ToString();
			}
		}
		
		
		public void OnFPSButton(){
			FPSControl.Show(currentTower);
			_Hide();
		}
		
		
		public void OnDirectionSlider(){
			if(currentTower==null) return;
			currentTower.ChangeScanAngle((int)sliderDrection.value);
		}
		
		
		public void UpdateDisplay(){
			if(currentTower==null) return;
			
			lbTowerName.text=currentTower.unitName;
			lbTowerLevel.text="lvl"+currentTower.GetLevel();
			lbTowerDesp1.text="damage: "+currentTower.GetDamageMin()+"-"+currentTower.GetDamageMax();
			lbTowerDesp1.text=currentTower.GetDespStats();//"damage: "+currentTower.GetDamageMin()+"-"+currentTower.GetDamageMax();
			lbTowerDesp2.text=currentTower.GetDespGeneral();
			
			
			sliderDrection.value=currentTower.dirScanAngle;
			directionControlObj.SetActive(currentTower.directionalTargeting);
			
			
			int upgradeOption=currentTower.ReadyToBeUpgrade();
			butUpgrade1Canvas.alpha=upgradeOption>=1 ? 1 : 0 ;
			butUpgrade1Canvas.interactable=upgradeOption>=1 ? true : false ;
			butUpgrade2Canvas.alpha=upgradeOption>=2 ? 1 : 0 ;
			butUpgrade2Canvas.interactable=upgradeOption>=2 ? true : false ;
			//butUpgradeObj1.SetActive(upgradeOption>=1 ? true : false);
			//butUpgradeObj2.SetActive(upgradeOption>=2 ? true : false);
			
			butSellObj.SetActive(currentTower.canBeSold);
			
			bool enableFPS=FPSControl.ActiveInScene();
			if(enableFPS && FPSControl.UseTowerWeapon() && currentTower.FPSWeaponID==-1) enableFPS=false;
			if(enableFPS && !FPSControl.IsIDAvailable(currentTower.FPSWeaponID)) enableFPS=false;
			butFPSObj.SetActive(enableFPS);
		}
		
		
		
		
		public static bool IsOn(){ return instance==null ? false : instance.currentTower!=null; }
		
		public static void Show(UnitTower tower){ instance._Show(tower); }
		public void _Show(UnitTower tower){
			bool fadeIn=currentTower==null;
			
			currentTower=tower;
			UpdateDisplay();
			
			//buttonParent.localPosition=buttonParentDefaultPos;
			
			thisT.localPosition=Vector3.zero;
			if(fadeIn) UIMainControl.FadeIn(canvasGroup, 0.25f);
		}
		public static void Hide(){ instance._Hide(); }
		public void _Hide(){
			if(!thisObj.activeInHierarchy) return;
			
			currentTower=null;
			GameControl.SelectTower(null);
			
			UIMainControl.FadeOut(canvasGroup, 0.25f);
			StartCoroutine(DelayHide());
		}
		
		IEnumerator DelayHide(){
			yield return new WaitForSeconds(0.25f);
			thisT.localPosition=new Vector3(0, 9999, 0);
			//thisObj.SetActive(false);
		}
		
	}

}