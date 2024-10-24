using UnityEngine;
using System.Collections;

public class ParticleManager : MonoBehaviour
{

    public void Play(string path, Transform parent, Vector3 position, Define.Particle type = Define.Particle.Effect)
    {
        if (path.Contains("Particle/") == false)
            path = $"Particle/{path}";


        GameObject particleInstance = GameManager.Instance.Resource.Instantiate(path, parent);

        if (particleInstance != null)
        {
            if (position != null)
            {
                particleInstance.transform.SetParent(parent);  // 부모로 설정
                particleInstance.transform.localPosition = position;  // 부모에 상대적인 위치 설정
            }

            ParticleSystem particleSystem = particleInstance.GetOrAddComponent<ParticleSystem>();

            if (type == Define.Particle.Loop)
            {
                particleSystem.Play();
            }
            else if (type == Define.Particle.Effect)
            {
                particleSystem.Play();
                float totalDuration = particleSystem.main.duration + particleSystem.main.startLifetime.constantMax;
                StartCoroutine(CleanupParticleAfterDelay(particleInstance, totalDuration));
            }
        }
    }

    private IEnumerator CleanupParticleAfterDelay(GameObject particleInstance, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (particleInstance != null)
        {
            GameManager.Instance.Resource.Destroy(particleInstance);
        }
    }

    public void Clear(GameObject particleInstance)
    {
        if (particleInstance != null)
        {
            ParticleSystem particleSystem = particleInstance.GetOrAddComponent<ParticleSystem>();
            if (particleSystem != null)
            {
                particleSystem.Clear();
            }
           
        }
    }
}
