using UnityEngine;

public class CompassElement : MonoBehaviour
{
    [Tooltip("The marker on the compass for this element指南针上这个元素的标记")]
    public CompassMarker compassMarkerPrefab;
    [Tooltip("Text override for the marker, if it's a direction如果是方向，则标记的文本覆盖")]
    public string textDirection;

    Compass m_Compass;

    void Awake()
    {
        m_Compass = FindObjectOfType<Compass>();
        DebugUtility.HandleErrorIfNullFindObject<Compass, CompassElement>(m_Compass, this);

        var markerInstance = Instantiate(compassMarkerPrefab);

        markerInstance.Initialize(this, textDirection);
        m_Compass.RegisterCompassElement(transform, markerInstance);
    }

    void OnDestroy()
    {
        m_Compass.UnregisterCompassElement(transform);
    }
}
