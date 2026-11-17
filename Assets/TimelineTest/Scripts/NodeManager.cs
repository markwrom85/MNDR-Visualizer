using UnityEngine;
using TMPro;

public class NodeManager : MonoBehaviour
{
    [SerializeField] private GameObject[] objectsToShow;
    [SerializeField] private GameObject[] objectsToHide;

    public void ShowDetails()
    {
        SetObjectsActive(objectsToShow, true);
        SetObjectsActive(objectsToHide, false);
    }

    public void HideDetails()
    {
        SetObjectsActive(objectsToShow, false);
        SetObjectsActive(objectsToHide, true);
    }

    private static void SetObjectsActive(GameObject[] objects, bool active)
    {
        if (objects == null)
        {
            return;
        }

        foreach (GameObject target in objects)
        {
            if (target != null)
            {
                target.SetActive(active);
            }
        }
    }
}
