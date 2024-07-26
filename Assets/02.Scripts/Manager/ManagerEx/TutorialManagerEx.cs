using UnityEngine;

public class TutorialManagerEx : MonoBehaviour
{
    private TutorialManager _tutorialManager;

    void Start()
    {
        _tutorialManager = GameManager.Instance.Tutorial;
        _tutorialManager.Init();
    }

    void Update()
    {
        UpdateFootArrowDirection();
    }

    private void UpdateFootArrowDirection()
    {
        if (_tutorialManager.GuideArrow != null && _tutorialManager.FootArrow != null)
        {
            // GuideArrow�� ��ġ�� �ٶ󺸰� ��
            Vector3 targetPosition = _tutorialManager.GuideArrow.position;

            // ���� FootArrow�� ��ġ
            Vector3 currentPosition = _tutorialManager.FootArrow.transform.position;

            // ������ ���
            Vector3 direction = targetPosition - currentPosition;
            direction.y = 0; // y�� ȸ���� �����Ͽ� ���� ���⸸ ���

            if (direction != Vector3.zero)
            {
                // LookAt�� ����Ͽ� ���� ����
                _tutorialManager.FootArrow.transform.LookAt(new Vector3(targetPosition.x, currentPosition.y, targetPosition.z));

                // x�� ȸ���� ����
                Vector3 fixedRotation = _tutorialManager.FootArrow.transform.rotation.eulerAngles;
                fixedRotation.x = 90;
                _tutorialManager.FootArrow.transform.rotation = Quaternion.Euler(fixedRotation);
            }
        }
    }
}
