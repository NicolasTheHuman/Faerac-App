using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BannersController : MonoBehaviour
{
    public static BannersController Instance { get; private set; }

    [SerializeField] private Image _logo;
    [SerializeField] private Banner _topBanner;
    [SerializeField] private Banner _downBanner;
    [SerializeField] private float _transitionTime = 0.2f;
    [SerializeField] private TextMeshProUGUI _sectionLabel;

    private struct BannerState
    {
        public Vector2 posBack, posFront;
        public Quaternion rotBack, rotFront;
    }

    private struct LogoState
    {
        public Vector2 pos;
        public Vector3 scale;
    }

    private BannerState _topInitial, _downInitial;
    private LogoState _logoInitial;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        if (_sectionLabel != null) _sectionLabel.gameObject.SetActive(false);
    }

    private void Start()
    {
        _topInitial = new BannerState
        {
            posBack  = _topBanner.bannerBack.rectTransform.anchoredPosition,
            posFront = _topBanner.bannerFront.rectTransform.anchoredPosition,
            rotBack  = _topBanner.bannerBack.rectTransform.rotation,
            rotFront = _topBanner.bannerFront.rectTransform.rotation,
        };
        _downInitial = new BannerState
        {
            posBack  = _downBanner.bannerBack.rectTransform.anchoredPosition,
            posFront = _downBanner.bannerFront.rectTransform.anchoredPosition,
            rotBack  = _downBanner.bannerBack.rectTransform.rotation,
            rotFront = _downBanner.bannerFront.rectTransform.rotation,
        };
        _logoInitial = new LogoState
        {
            pos   = _logo.rectTransform.anchoredPosition,
            scale = _logo.rectTransform.localScale,
        };
    }

    public void ResetToInitial()
    {
        StopAllCoroutines();
        StartCoroutine(ResetCoroutine());
        _sectionLabel.gameObject.SetActive(false );
    }

    private IEnumerator ResetCoroutine()
    {
        var elapsed = 0f;

        var topBackStart   = _topBanner.bannerBack.rectTransform.anchoredPosition;
        var topFrontStart  = _topBanner.bannerFront.rectTransform.anchoredPosition;
        var topBackRotStart  = _topBanner.bannerBack.rectTransform.rotation;
        var topFrontRotStart = _topBanner.bannerFront.rectTransform.rotation;

        var downBackStart  = _downBanner.bannerBack.rectTransform.anchoredPosition;
        var downFrontStart = _downBanner.bannerFront.rectTransform.anchoredPosition;
        var downBackRotStart  = _downBanner.bannerBack.rectTransform.rotation;
        var downFrontRotStart = _downBanner.bannerFront.rectTransform.rotation;

        var logoPosStart   = _logo.rectTransform.anchoredPosition;
        var logoScaleStart = _logo.rectTransform.localScale;

        while (elapsed < _transitionTime)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / _transitionTime;

            _topBanner.bannerBack.rectTransform.anchoredPosition  = Vector2.Lerp(topBackStart,  _topInitial.posBack,  t);
            _topBanner.bannerFront.rectTransform.anchoredPosition = Vector2.Lerp(topFrontStart, _topInitial.posFront, t);
            _topBanner.bannerBack.rectTransform.rotation  = Quaternion.Lerp(topBackRotStart,  _topInitial.rotBack,  t);
            _topBanner.bannerFront.rectTransform.rotation = Quaternion.Lerp(topFrontRotStart, _topInitial.rotFront, t);

            _downBanner.bannerBack.rectTransform.anchoredPosition  = Vector2.Lerp(downBackStart,  _downInitial.posBack,  t);
            _downBanner.bannerFront.rectTransform.anchoredPosition = Vector2.Lerp(downFrontStart, _downInitial.posFront, t);
            _downBanner.bannerBack.rectTransform.rotation  = Quaternion.Lerp(downBackRotStart,  _downInitial.rotBack,  t);
            _downBanner.bannerFront.rectTransform.rotation = Quaternion.Lerp(downFrontRotStart, _downInitial.rotFront, t);

            _logo.rectTransform.anchoredPosition = Vector2.Lerp(logoPosStart, _logoInitial.pos, t);
            _logo.rectTransform.localScale       = Vector3.Lerp(logoScaleStart, _logoInitial.scale, t);

            yield return null;
        }

        _topBanner.bannerBack.rectTransform.anchoredPosition  = _topInitial.posBack;
        _topBanner.bannerFront.rectTransform.anchoredPosition = _topInitial.posFront;
        _topBanner.bannerBack.rectTransform.rotation  = _topInitial.rotBack;
        _topBanner.bannerFront.rectTransform.rotation = _topInitial.rotFront;

        _downBanner.bannerBack.rectTransform.anchoredPosition  = _downInitial.posBack;
        _downBanner.bannerFront.rectTransform.anchoredPosition = _downInitial.posFront;
        _downBanner.bannerBack.rectTransform.rotation  = _downInitial.rotBack;
        _downBanner.bannerFront.rectTransform.rotation = _downInitial.rotFront;

        _logo.rectTransform.anchoredPosition = _logoInitial.pos;
        _logo.rectTransform.localScale       = _logoInitial.scale;
    }

    public void SetSection(string sectionName)
    {
        if (_sectionLabel == null) return;
        _sectionLabel.text = sectionName;
        _sectionLabel.gameObject.SetActive(true);
    }

    public void HideSection()
    {
        if (_sectionLabel == null) return;
        _sectionLabel.gameObject.SetActive(false);
    }

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
