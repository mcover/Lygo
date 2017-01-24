using UnityEngine;

public class LayerOrderSorter : MonoBehaviour
{
    public string SortingLayerName;
    public int SortingOrder;

    private void Start()
    {
        GetComponent<Renderer>().sortingLayerName = SortingLayerName;
        GetComponent<Renderer>().sortingOrder = SortingOrder;
    }
}
