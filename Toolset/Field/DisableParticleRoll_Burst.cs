using UnityEngine;
using Unity.Burst;
using UnityEngine.ParticleSystemJobs;
using Unity.Mathematics;

/*
 * Unity component to disable particle billboard rolling in VR
 */

[RequireComponent (typeof (ParticleSystem))]
public class DisableParticleRoll : MonoBehaviour
{
    private ParticleSystem m_ParticleSystem;
    private Job m_Job = new Job();

    private float m_LastRotation;

    private void Start()
    {
        m_ParticleSystem = GetComponent<ParticleSystem>();
    }

    void Update()
    {
        var camera = Camera.main;
        if (!camera)
            return;

        float currentRotation = camera.transform.rotation.eulerAngles.z;
        m_Job.m_RotationDifference = (currentRotation - m_LastRotation);
        m_LastRotation = currentRotation;
    }

    void OnParticleUpdateJobScheduled()
    {
        if (m_Job.m_RotationDifference != 0.0f)
            m_Job.ScheduleBatch(m_ParticleSystem, 4096);
    }

    [BurstCompile]
    struct Job : IJobParticleSystemParallelForBatch
    {
        public float m_RotationDifference;

        public void Execute(ParticleSystemJobData particles, int startIndex, int count)
        {
            m_RotationDifference = m_RotationDifference / 180 * math.PI;
            var rotations = particles.rotations.z;

            for (int i = startIndex; i < startIndex + count; i++)
                rotations[i] += m_RotationDifference;
        }
    }
}
