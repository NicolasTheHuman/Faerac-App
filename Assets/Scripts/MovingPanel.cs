using System.Collections;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class MovingPanel : MonoBehaviour
{
    [SerializeField] private float _transitionTime = 0.2f;
    private RectTransform _rect;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
    }

    public void MovePanelX(float xPos)
    {
        StartCoroutine(MovePanel(true, xPos));
    }

    public void MovePanelY(float yPos)
    {
        StartCoroutine(MovePanel(false, yPos));
    }

    IEnumerator MovePanel(bool moveX, float target)
    {
        float time = 0f;
        Vector2 startPos = _rect.anchoredPosition;
        Vector2 targetPos = startPos;

        if (moveX)
            targetPos.x = target;
        else
            targetPos.y = target;

        while (time < _transitionTime)
        {
            time += Time.deltaTime;
            float t = time / _transitionTime;
            _rect.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }

        _rect.anchoredPosition = targetPos;
    }
}