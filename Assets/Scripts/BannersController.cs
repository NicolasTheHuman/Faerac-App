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

    #region Vector moving banners

    private float _topBannerXPosTarget;
    private float _topBannerYPosTarget;
    private float _downBannerXPosTarget;
    private float _downBannerYPosTarget;

    public void SetTopBannerXPosTarget(float x) => _topBannerXPosTarget = x;
    public void SetTopBannerYPosTarget(float x) => _topBannerYPosTarget = x;
    
    public void SetDownBannerXPosTarget(float x) => _downBannerXPosTarget = x;
    public void SetDownBannerYPosTarget(float x) => _downBannerYPosTarget = x;

    public void MoveTopBannerToTarget()
    {
        StartCoroutine(MoveBannerCoroutine(_topBanner, new Vector2(_topBannerXPosTarget, _topBannerYPosTarget)));
    }
    
    public void MoveDownBannerToTarget()
    {
        StartCoroutine(MoveBannerCoroutine(_downBanner, new Vector2(_downBannerXPosTarget, _downBannerYPosTarget)));
    }
    
    #endregion
    
    public void MoveTopBannerX(float x)
    {
        Debug.Log($"top banner x with {_topBanner.bannerFront.rectTransform.anchoredPosition.y} y");
        StartCoroutine(MoveBannerCoroutine(_topBanner, new Vector2(x, _topBanner.bannerFront.rectTransform.anchoredPosition.y)));
    }
    
    public void MoveTopBannerY(float y)
    {
        Debug.Log($"top banner y with {_topBanner.bannerFront.rectTransform.anchoredPosition.x} x");
        StartCoroutine(MoveBannerCoroutine(_topBanner, new Vector2(_topBanner.bannerFront.rectTransform.anchoredPosition.x, y)));
    }
    
    public void MoveDownBannerX(float x)
    {
        StartCoroutine(MoveBannerCoroutine(_downBanner, new Vector2(x, _downBanner.bannerFront.rectTransform.anchoredPosition.y)));
    }
    
    public void MoveDownBannerY(float y)
    {
        StartCoroutine(MoveBannerCoroutine(_downBanner, new Vector2(_downBanner.bannerFront.rectTransform.anchoredPosition.x, y)));
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

    public void RotateTopBannerX(float x)
    {
        var rotation = _topBanner.bannerFront.rectTransform.rotation;
        StartCoroutine(RotateBannerCoroutine(_topBanner, new Vector3(x, rotation.y, rotation.z)));
    }
    
    public void RotateTopBannerY(float y)
    {
        var rotation = _topBanner.bannerFront.rectTransform.rotation;
        StartCoroutine(RotateBannerCoroutine(_topBanner, new Vector3(rotation.x, y, rotation.z)));
    }
    
    public void RotateTopBannerZ(float z)
    {
        var rotation = _topBanner.bannerFront.rectTransform.rotation;
        StartCoroutine(RotateBannerCoroutine(_topBanner, new Vector3(rotation.x, rotation.y, z)));
    }
    
    public void RotateDownBannerX(float x)
    {
        var rotation = _downBanner.bannerFront.rectTransform.rotation;
        StartCoroutine(RotateBannerCoroutine(_downBanner, new Vector3(x, rotation.y, rotation.z)));
    }
    
    public void RotateDownBannerY(float y)
    {
        var rotation = _downBanner.bannerFront.rectTransform.rotation;
        StartCoroutine(RotateBannerCoroutine(_downBanner, new Vector3(rotation.x, y, rotation.z)));
    }
    
    public void RotateDownBannerZ(float z)
    {
        var rotation = _downBanner.bannerFront.rectTransform.rotation;
        StartCoroutine(RotateBannerCoroutine(_downBanner, new Vector3(rotation.x, rotation.y, z)));
    }

    IEnumerator RotateBannerCoroutine(Banner banner, Vector3 rotation)
    {
        var elapsedTime = 0f;

        var startPos = banner.bannerBack.rectTransform.rotation;

        while (elapsedTime <= _transitionTime)
        {
            elapsedTime += Time.deltaTime;
            banner.bannerBack.rectTransform.rotation = Quaternion.Lerp(startPos, Quaternion.Euler(rotation), elapsedTime / _transitionTime);
            banner.bannerFront.rectTransform.rotation = Quaternion.Lerp(startPos, Quaternion.Euler(rotation), elapsedTime / _transitionTime);
            yield return null;
        }

        banner.bannerBack.rectTransform.rotation = Quaternion.Euler(rotation);
        banner.bannerFront.rectTransform.rotation = Quaternion.Euler(rotation);
    }


    private float _targetLogoXPos;
    private float _targetLogoYPos;

    public void SetLogoXTargetPos(float x) => _targetLogoXPos = x;
    public void SetLogoYTargetPos(float y) => _targetLogoYPos = y;

    public void MoveLogoToTargetPos() => StartCoroutine(MoveLogoCoroutine(new Vector2(_targetLogoXPos, _targetLogoYPos)));
    
    public void MoveLogoXAxis(float x)
    {
        StartCoroutine(MoveLogoCoroutine(new Vector2(x, _logo.rectTransform.anchoredPosition.y)));
    }
    
    public void MoveLogoYAxis(float y)
    {
        StartCoroutine(MoveLogoCoroutine(new Vector2(_logo.rectTransform.anchoredPosition.x, y)));
    }

    IEnumerator MoveLogoCoroutine(Vector2 targetPos)
    {
        var time = 0f;
        var startPos = _logo.rectTransform.anchoredPosition;

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
        var startScale = _logo.rectTransform.localScale;
        var targetScale = Vector3.one * scale;

        while (elapsedTime <= _transitionTime)
        {
            elapsedTime += Time.deltaTime;
            _logo.rectTransform.localScale = Vector3.Lerp(startScale, targetScale, elapsedTime / _transitionTime);
            yield return null;
        }

        _logo.rectTransform.localScale = targetScale;
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
