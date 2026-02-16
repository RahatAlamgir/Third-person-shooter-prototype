using DG.Tweening;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class ChestAnimator : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] private Transform chestTop;
    [SerializeField][Range(-360, 360)] private float angle = -120;

    [Header("Adjust")]
    [SerializeField] private bool doAdjust = false;
    [SerializeField][Range(-180, 180)] private float offsetAngle = 0;
    [SerializeField][Range(-180, 180)] private float MaxOpenAngle = 0;
    [SerializeField][Range(-180,180)] private float MaxCloseAngle = 0;

    [Header("Effect")]

    [SerializeField] private Ease openEffect = Ease.OutBack;
    [SerializeField] private Ease closeEffect = Ease.Linear;


    private enum _axis { X, Y ,Z};
    [SerializeField] private _axis rotationAxis = _axis.X;
    private Vector3 axisVector = new Vector3 (1, 0, 0);
    [SerializeField] private float duration = 1f;
    private bool isOpen = false;
    private bool isAnimating = false;


    private void Start()
    {

        axisVector = new Vector3(rotationAxis == _axis.X ? 1 : 0,
                                 rotationAxis == _axis.Y ? 1 : 0,
                                 rotationAxis == _axis.Z ? 1 : 0);
        if (doAdjust) DoAdjustSetting();
        else SetDefault();
        
    }

    private void SetOffset()
    {
        chestTop.DOKill();
        if (offsetAngle != 0)
        {
            chestTop.DOLocalRotate(axisVector*offsetAngle, 0.01f);
        }
    }
    private void SetDefault()
    {
        MaxOpenAngle = angle;
        MaxCloseAngle = -angle;
    }
    private void DoAdjustSetting()
    {
        SetOffset();

    }
    public void ChestOpen()
    {
        chestTop.DOKill();
        if (!isOpen)
        {
            chestTop.DOLocalRotate(axisVector* MaxOpenAngle, duration).SetEase(openEffect);
            isOpen = true;
            StartCoroutine(StayAnimating(duration));
        }
        
    }
    public void ChestClose()
    {
        chestTop.DOKill();
        if (isOpen)
        {
            chestTop.DOLocalRotate(axisVector*MaxCloseAngle, duration).SetEase(closeEffect);
            isOpen = false;
            StartCoroutine(StayAnimating(duration));
        }
        
    }
    public void ChestOpenClose(float delay = 0.5f)
    {
        if(delay< duration) delay = duration;
        StartCoroutine(FullAnimation(delay));
        
    }

    private IEnumerator FullAnimation(float delay)
    {
        ChestOpen();
        yield return new WaitForSeconds(delay);
        ChestClose();
    }
    private IEnumerator StayAnimating(float delay)
    {
        isAnimating = true;
        yield return new WaitForSeconds(delay);
        isAnimating = false;
    }
    public void SetAngle(float angle)
    {
        this.angle = angle;
    }
    public bool IsOpen() => isOpen;
    public bool IsAnimating() => isAnimating;

}
