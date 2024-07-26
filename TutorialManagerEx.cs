using UnityEngine;

public class TutorialManagerEx : MonoBehaviour
{
    public TutorialManager tutorialManager; // TutorialManager에 대한 참조

    private void Start()
    {
        GameObject root = GameObject.Find("@Tutorial");
        if (root == null)
        {
            root = new GameObject { name = "@Tutorial" };
            Object.DontDestroyOnLoad(root);
        }
        if (tutorialManager != null)
        {
            // TutorialManager의 Action 이벤트에 구독
            tutorialManager.OnReachOven += OnReachOven;
            tutorialManager.OnReachBasket += OnReachBasket;
            tutorialManager.OnReachCounter += OnReachCounter;
            tutorialManager.OnReachCashPoint += OnReachCashPoint;
            tutorialManager.OnReachTrashPoint += OnReachTrashPoint;
            tutorialManager.OnReachUnlockPoint += OnReachUnlockPoint;
        }
    }

    private void OnDestroy()
    {
        if (tutorialManager != null)
        {
            // TutorialManager의 Action 이벤트에서 구독 해제
            tutorialManager.OnReachOven -= OnReachOven;
            tutorialManager.OnReachBasket -= OnReachBasket;
            tutorialManager.OnReachCounter -= OnReachCounter;
            tutorialManager.OnReachCashPoint -= OnReachCashPoint;
            tutorialManager.OnReachTrashPoint -= OnReachTrashPoint;
            tutorialManager.OnReachUnlockPoint -= OnReachUnlockPoint;
        }
    }

    // 각 이벤트 핸들러에서 다음 목표로 넘어가는 로직 구현
    private void OnReachOven()
    {
        UpdateGuideArrow();
    }

    private void OnReachBasket()
    {
        UpdateGuideArrow();
    }

    private void OnReachCounter()
    {
        UpdateGuideArrow();
    }

    private void OnReachCashPoint()
    {
        UpdateGuideArrow();
    }

    private void OnReachTrashPoint()
    {
        UpdateGuideArrow();
    }

    private void OnReachUnlockPoint()
    {
        UpdateGuideArrow();
    }

    // GuideArrow의 위치와 회전을 업데이트하는 메서드
    private void UpdateGuideArrow()
    {
        if (tutorialManager.GuideArrow != null)
        {
            Transform guideArrow = tutorialManager.GuideArrow;
            // GuideArrow를 목표를 향해 회전시킵니다.
            tutorialManager.FootArrow.LookAt(guideArrow);
            Debug.Log(guideArrow.name);
            Debug.Log(guideArrow.position);
        }
    }
}
