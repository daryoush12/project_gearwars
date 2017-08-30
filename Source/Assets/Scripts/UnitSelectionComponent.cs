using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace rtsprj {

public class UnitSelectionComponent : MonoBehaviour
{
        public GameObject selectionCirclePrefab;
        RaycastHit hit;
        bool isSelecting = false;
        Vector3 mousePosition;
        Vector3 newPosition;
        List<SelectableUnitComponent> SelectedUnits = new List<SelectableUnitComponent>();
        public bool IsWithinSelectionBounds(GameObject gameObject)
        {
            if (!isSelecting)
                return false;
            var camera = Camera.main;
            var viewportBounds = Utils.GetViewportBounds(camera, mousePosition, Input.mousePosition);
            return viewportBounds.Contains(camera.WorldToViewportPoint(gameObject.transform.position));
        }
        void OnGUI()
        {
            if (isSelecting)
            {
                // Create a rect from both mouse positions
                var rect = Utils.GetScreenRect(mousePosition, Input.mousePosition);
                Utils.DrawScreenRect(rect, new Color(0.8f, 0.8f, 0.95f, 0.25f));
                Utils.DrawScreenRectBorder(rect, 2, new Color(0.8f, 0.8f, 0.95f));
            }
        }
        void Update()
        {
			newPosition = transform.position;
        // If we press the left mouse button, begin selection and remember the location of the mouse
        if( Input.GetMouseButtonDown(0))
        {
            isSelecting = true;
            mousePosition = Input.mousePosition;

            foreach( var selectableObject in FindObjectsOfType<SelectableUnitComponent>() )
            {
                if( selectableObject.selectionCircle != null )
                {
                    Destroy( selectableObject.selectionCircle.gameObject );
                    selectableObject.selectionCircle = null;
                }
            }
        }
        // If we let go of the left mouse button, end selection
        if( Input.GetMouseButtonUp(0))
        {
            var selectedObjects = new List<SelectableUnitComponent>();
            SelectedUnits = selectedObjects;
            foreach( var selectableObject in FindObjectsOfType<SelectableUnitComponent>() )
            {
                if( IsWithinSelectionBounds( selectableObject.gameObject ) )
                {
                    selectedObjects.Add( selectableObject );
                }
            }
            var sb = new StringBuilder();
            sb.AppendLine( string.Format( "Selecting [{0}] Units", selectedObjects.Count ) );
            foreach( var selectedObject in selectedObjects )
                sb.AppendLine( "-> " + selectedObject.gameObject.name );
            Debug.Log( sb.ToString() );
            isSelecting = false;
        }
        // Highlight all objects within the selection box
        if( isSelecting )
        {
            foreach( var selectableObject in FindObjectsOfType<SelectableUnitComponent>() )
            {
                if( IsWithinSelectionBounds( selectableObject.gameObject ) )
                {
                    if( selectableObject.selectionCircle == null )
                    {
                        selectableObject.selectionCircle = Instantiate( selectionCirclePrefab );
                        selectableObject.selectionCircle.transform.SetParent( selectableObject.transform, false );
                        selectableObject.selectionCircle.transform.eulerAngles = new Vector3( 90, 0, 0 );
                    }
                }
                else
                {
                    if( selectableObject.selectionCircle != null )
                    {
                        Destroy( selectableObject.selectionCircle.gameObject );
                        selectableObject.selectionCircle = null;
                    }
                }
            }
        }

            //Get mouse location:
            //Loop for every unit to move into said location:
            //At the same time execute animation from ControllerScript
            if (Input.GetMouseButtonDown (1)&& SelectedUnits.Count > 0) {
				RaycastHit hit;
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				if(Physics.Raycast(ray,out hit)){
					newPosition = hit.point;
					Debug.Log ("Commanded units to move. Location: "+newPosition.ToString());
					var selectedObjects = SelectedUnits;
					SelectedUnits = selectedObjects;
					foreach( var selectedObject in selectedObjects ) {
                        Instantiate(Resources.Load("Sphere"), newPosition, Quaternion.identity);
						selectedObject.MoveUnit(newPosition);
						//SelectedUnits.Clear ();
                    }		
				}
			}
    }
}
}