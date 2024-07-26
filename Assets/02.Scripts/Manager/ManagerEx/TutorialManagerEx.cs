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
            // GuideArrow의 위치로 바라보게 함
            Vector3 targetPosition = _tutorialManager.GuideArrow.position;

            // 현재 FootArrow의 위치
            Vector3 currentPosition = _tutorialManager.FootArrow.transform.position;

            // 방향을 계산
            Vector3 direction = targetPosition - currentPosition;
            direction.y = 0; // y축 회전을 무시하여 수평 방향만 계산

            if (direction != Vector3.zero)
            {
                // LookAt을 사용하여 방향 설정
                _tutorialManager.FootArrow.transform.LookAt(new Vector3(targetPosition.x, currentPosition.y, targetPosition.z));

                // x축 회전을 고정
                Vector3 fixedRotation = _tutorialManager.FootArrow.transform.rotation.eulerAngles;
                fixedRotation.x = 90;
                _tutorialManager.FootArrow.transform.rotation = Quaternion.Euler(fixedRotation);
            }
        }
    }
}
