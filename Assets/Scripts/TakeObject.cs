using UnityEngine;
using System.Collections.Generic;

public class TakeObject : MonoBehaviour
{
    public GameObject handpoint;

    [Header("Tags de objetos agarrables")]
    public List<string> pickableTags = new List<string> { "Prueba", "Prueba2", "Prueba3" }; 

    private GameObject pickedObject = null;

    void Update()
    {
        if (pickedObject != null)
        {

            if (Input.GetKeyDown("r"))
            {
                DropObject();
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (IsPickable(other.gameObject) && Input.GetKeyDown("e") && pickedObject == null)
        {
            PickUpObject(other.gameObject);
        }

        if (pickedObject != null && Input.GetKeyDown("f"))
        {
            CapsuleSlot slot = other.gameObject.GetComponent<CapsuleSlot>();
            if (slot != null)
            {
                bool placed = slot.TryPlaceObject(pickedObject);
                if (placed)
                {
                    pickedObject = null;
                }
                else
                {
                    Debug.Log("Esta cápsula no va aquí, o el slot ya está ocupado.");
                }
            }
        }
    }

    private bool IsPickable(GameObject obj)
    {
        foreach (string tag in pickableTags)
        {
            if (obj.CompareTag(tag))
                return true;
        }
        return false;
    }

    private void PickUpObject(GameObject obj)
    {
        obj.GetComponent<Rigidbody>().useGravity = false;
        obj.GetComponent<Rigidbody>().isKinematic = true;
        obj.transform.position = handpoint.transform.position;
        obj.transform.SetParent(handpoint.transform);
        pickedObject = obj;
    }

    private void DropObject()
    {
        pickedObject.GetComponent<Rigidbody>().useGravity = true;
        pickedObject.GetComponent<Rigidbody>().isKinematic = false;
        pickedObject.transform.SetParent(null);
        pickedObject = null;
    }
}