using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AutoScroll : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float mScrollSpeed = 0.1f;  // 스크롤 속도

    [SerializeField] private float mScrollDelay = 2f;  // 자동 스크롤 시작 딜레이

    [SerializeField] private bool mIsVerticalScroll;

    [SerializeField] private RectTransform mScrollRectTransform;

    private ScrollRect mScrollRect;  // 스크롤 영역
    private Coroutine? mCoAutoScroll; // 자동 스크롤 코루틴
    private bool mIsPointerEnter = false; // 현재 포인터가 영역에 들어왔는가?

    private void Awake()
    {
        mScrollRect = GetComponent<ScrollRect>();
    }

    private void OnEnable()
    {
        ToggleAutoScroll(true);
    }


    private void OnDisable()
    {
        ToggleAutoScroll(false);
    }

    private void ToggleAutoScroll(bool isEnable)
    {
        mScrollRect.velocity = Vector2.zero;

        if (mCoAutoScroll is not null)
            StopCoroutine(mCoAutoScroll);

        if (isEnable)
        {
            mCoAutoScroll = StartCoroutine(CoAutoScroll());
            mIsPointerEnter = false;
        }
    }

    private IEnumerator CoAutoScroll()
    {
        yield return new WaitForSecondsRealtime(mScrollDelay);

        while (true)
        {
            // 스크롤 방향에 따라 계산
            if (mIsVerticalScroll)
                mScrollRect.verticalNormalizedPosition -= mScrollSpeed * Time.deltaTime / mScrollRectTransform.sizeDelta.y;
            else
                mScrollRect.horizontalNormalizedPosition -= mScrollSpeed * Time.deltaTime / mScrollRectTransform.sizeDelta.x;

            // 스크롤의 끝 영역에 도달했다면 방향을 반전
            if ((mIsVerticalScroll ? mScrollRect.verticalNormalizedPosition : mScrollRect.horizontalNormalizedPosition) <= 0f || (mIsVerticalScroll ? mScrollRect.verticalNormalizedPosition : mScrollRect.horizontalNormalizedPosition) >= 1f)
                mScrollSpeed = -mScrollSpeed;

            // 영역에 포인터가 진입한 상태에서 클릭 또는 드래그를 했다면?
            if (mIsPointerEnter && (Input.GetMouseButton(0) || Input.GetAxis("Mouse ScrollWheel") > 0))
                ToggleAutoScroll(false);

            yield return null;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mIsPointerEnter = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mIsPointerEnter = false;
        ToggleAutoScroll(true);
    }
}