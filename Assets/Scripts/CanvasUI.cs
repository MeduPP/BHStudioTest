using UnityEngine;

public class CanvasUI : MonoBehaviour
{
    public RectTransform playersPanel;

    static CanvasUI instance;

    void Awake()
    {
        instance = this;
    }

    public static RectTransform GetPlayersPanel() => instance.playersPanel;
}
