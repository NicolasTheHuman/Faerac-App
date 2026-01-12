using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BannersController : MonoBehaviour
{
    [SerializeField] private Image _logo;
    [SerializeField] private Banner _topBanner;
    [SerializeField] private Banner _downBanner;
    [SerializeField] private float _transitionTime = 0.2f;
    
    
    public void MoveTopBanner(float y)
    {
        StartCoroutine(MoveBannerCoroutine(_topBanner, new Vector2(0, y)));
    }
    
    public void MoveDownBanner(float y)
    {
        StartCoroutine(MoveBannerCoroutine(_downBanner, new Vector2(0, y)));
    }

    IEnumerator MoveBannerCoroutine(Banner banner, Vector2 targetPos)
    {
        var elapsedTime = 0f;
        var startPos = banner.bannerBack.rectTransform.anchoredPosition;
        
        while (elapsedTime <= _transitionTime)
        {
            elapsedTime += Time.deltaTime;
            banner.bannerBack.rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, elapsedTime / _transitionTime);
            banner.bannerFront.rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, elapsedTime / _transitionTime);
            yield return null;
        }

        banner.bannerBack.rectTransform.anchoredPosition = targetPos;
        banner.bannerFront.rectTransform.anchoredPosition = targetPos;
    }

    public void MoveLogoYAxis(float y)
    {
        StartCoroutine(MoveLogoCoroutine(y));
    }

    IEnumerator MoveLogoCoroutine(float y)
    {
        var time = 0f;
        var startPos = _logo.rectTransform.anchoredPosition;
        var targetPos = startPos;
        targetPos.y = y;

        while (time <= _transitionTime)
        {
            time += Time.deltaTime;
            _logo.rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, time / _transitionTime);
            yield return null;
        }

        _logo.rectTransform.anchoredPosition = targetPos;
    }

    public void ScaleLogo(float scale = 1f)
    {
        StartCoroutine(ScaleLogoCoroutine(scale));
    }

    IEnumerator ScaleLogoCoroutine(float scale)
    {
        var elapsedTime = 0f;
        var startScale = _logo.rectTransform.sizeDelta;
        var targetScale = Vector2.one * scale;

        while (elapsedTime <= _transitionTime)
        {
            elapsedTime += Time.deltaTime;
            _logo.rectTransform.sizeDelta = Vector2.Lerp(startScale, targetScale, elapsedTime / _transitionTime);
            yield return null;
        }

        _logo.rectTransform.sizeDelta = targetScale;
    }
}

[Serializable]
public struct Banner
{
    public Image bannerBack;
    public Image bannerFront;

    public Banner(Image back, Image front)
    {
        bannerBack = back;
        bannerFront = front;
    }
}
