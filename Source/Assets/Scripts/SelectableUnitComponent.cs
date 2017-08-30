using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
namespace rtsprj
{
    public class SelectableUnitComponent : MonoBehaviour
    {
        public GameObject selectionCircle;
		//Unit Attributes:
		GameObject unit;
		[SerializeField]
		UnitType Type;
		[SerializeField]
		Tier UnitTier;
		[SerializeField]
		StateMachine UnitStates;
		//NavigationMesh
		NavMeshAgent agent;

        void Start()
        {
			agent = this.gameObject.GetComponent<NavMeshAgent>();
        }

        void Update()
        {
			if (!agent.hasPath)
				UnitStates = StateMachine.Waiting;
        }

        public void MoveUnit(Vector3 position)
        {
			agent.destination = position;           
			Debug.Log(this.gameObject.tag + " Was commanded"+ "ID: "+this.gameObject.GetInstanceID());
			UnitStates = StateMachine.Moving;
        }
    }
}

