using UnityEngine;
using System.Collections;
namespace rtsprj{
	public class Unit : MonoBehaviour {
		float speed = 5.0f;
		GameObject unit;
		[SerializeField]
		UnitType Type;
		[SerializeField]
		Tier UnitTier;
		[SerializeField]
		StateMachine UnitStates;


		void Start(){
           // unit = this.gameObject;
		}
	}	
}

