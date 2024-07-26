using UnityEngine;

public class TutorialManagerEx : MonoBehaviour
{
    public TutorialManager tutorialManager; // TutorialManager�� ���� ����

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
            // TutorialManager�� Action �̺�Ʈ�� ����
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
            // TutorialManager�� Action �̺�Ʈ���� ���� ����
            tutorialManager.OnReachOven -= OnReachOven;
            tutorialManager.OnReachBasket -= OnReachBasket;
            tutorialManager.OnReachCounter -= OnReachCounter;
            tutorialManager.OnReachCashPoint -= OnReachCashPoint;
            tutorialManager.OnReachTrashPoint -= OnReachTrashPoint;
            tutorialManager.OnReachUnlockPoint -= OnReachUnlockPoint;
        }
    }

    // �� �̺�Ʈ �ڵ鷯���� ���� ��ǥ�� �Ѿ�� ���� ����
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

    // GuideArrow�� ��ġ�� ȸ���� ������Ʈ�ϴ� �޼���
    private void UpdateGuideArrow()
    {
        if (tutorialManager.GuideArrow != null)
        {
            Transform guideArrow = tutorialManager.GuideArrow;
            // GuideArrow�� ��ǥ�� ���� ȸ����ŵ�ϴ�.
            tutorialManager.FootArrow.LookAt(guideArrow);
            Debug.Log(guideArrow.name);
            Debug.Log(guideArrow.position);
        }
    }
}
