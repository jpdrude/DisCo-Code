/*
Project DisCo (Discrete Choreography) (GPL) initiated by Jan Philipp Drude

This file is part of Project DisCo.

Copyright (c) 2021, Jan Philipp Drude <jpdrude@gmail.com>

A full build of Project DisCo is available at <http://www.project-disco.com>

Project DisCo's underlaying Source Code is free to use; you can 
redistribute it and/or modify it under the terms of the GNU 
General Public License as published by the Free Software Foundation; 
either version 3 of the License, or (at your option) any later version. 

The Project DisCo source code is distributed in the hope that it will 
be useful, but WITHOUT ANY WARRANTY; without even the implied warranty 
of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
with the DisCo-Classes repository; 
If not, see <http://www.gnu.org/licenses/>.

@license GPL-3.0 <https://www.gnu.org/licenses/gpl.html>

The Project DisCo base classes build on Wasp developed by Andrea Rossi.
You can find Wasp at: <https://github.com/ar0551/Wasp>

Significant parts of Project DisCo have been developed by Jan Philipp Drude
as part of research on virtual reality, digital materials and 
discrete design at: 
dMA - digital Methods in Architecture - Prof. Mirco Becker
Leibniz University Hannover
*/

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/*
 * Toolitem: Draw Field
 * 
 * Drawns particles in particle system
 * Distance to particles sets field values
 */

public class TIDrawField : ToolItem
{
    GameObject target = null;

    Vector3 targetBasePos;

    [SerializeField]
    GameObject targetPointer;

    [SerializeField]
    Mesh sphereMesh;

    GameObject positionHighlight;

    ControllerTarget hightlightTarget;

    ParticleSystem particles;

    [SerializeField]
    float radius = 1;

    [SerializeField]
    GameObject targetOverride;

    float Radius
    {
        get
        {
            float _radius = radius;
            Transform go = transform.parent;

            while (go != null)
            {
                _radius *= go.localScale.x;

                go = go.transform.parent;
            }

            return _radius;
        }
    }

    float part_angularVelocity = 0;
    float part_lifetime = float.MaxValue;
    float part_rotation = 0;
    float part_size = 0.5f;
    float part_startLifetime = float.MaxValue;
    Vector3 part_velocity = Vector3.zero;

    bool click = false;

    float partsPerUnit = 10f;

    Region gameArea;

    List<Vector3> particleContainer = new List<Vector3>();
    public List<Vector3> ParticleContainer 
    { 
        get { return particleContainer; } 
        set { particleContainer = value; }
    }

    float Volume
    {
        get
        {
            return (float)((4.0 / 3.0) * System.Math.PI * System.Math.Pow(Radius, 3));
        }
    }



    public override void Initialize(Toolset _toolSet)
    {
        if (target != null)
            Destroy(target);

        base.Initialize(_toolSet);

        if (targetOverride == null)
        {
            target = Instantiate(ToolSet.ControllerTarget, ToolSet.ControllerTarget.transform.position, ToolSet.ControllerTarget.transform.rotation,
                         ToolSet.ControllerTarget.transform.parent);
            target.name = "DrawFieldTarget";
        }
        else
            target = targetOverride;

        hightlightTarget = target.GetComponent<ControllerTarget>();
        hightlightTarget.SetMesh(sphereMesh);

        positionHighlight = target.transform.GetChild(0).gameObject;

        targetBasePos = target.transform.localPosition - Vector3.forward * radius;

        GameObject tp = Instantiate(targetPointer, target.transform);
        tp.transform.localPosition = Vector3.zero;
        tp.transform.localRotation = Quaternion.identity;
        tp.GetComponent<MeshRenderer>().material = MaterialHolder.DeleteSphereMat;

        target.SetActive(false);

        particles = ParticleField.System;

        gameArea = InitializeGameArea.GameArea;
    }

    public override void ActivateItem()
    {
        PlacementReferences.PlacementType = PlacementReferences.PlaceChoreo.DrawField;
        target.SetActive(true);
        ActivateField();
        enabled = true;
    }

    public override void DeactivateItem()
    {
        target.SetActive(false);
        particles.gameObject.SetActive(false);
        enabled = false;
    }

    public override void Click()
    {
        click = true;
    }

    public override void Unclick()
    {
        click = false;
    }

    public override void ScrollUp()
    {
        ScaleRange(Time.deltaTime);
        positionHighlight.transform.localScale = new Vector3(radius * 2, radius * 2, radius * 2);
        target.transform.localPosition = targetBasePos + Vector3.forward * radius;
        hightlightTarget.Highlight();

    }

    public override void ScrollDown()
    {
        ScaleRange(-Time.deltaTime);
        positionHighlight.transform.localScale = new Vector3(radius * 2, radius * 2, radius * 2);
        target.transform.localPosition = targetBasePos + Vector3.forward * radius;
        hightlightTarget.Highlight();
    }

    private void Update()
    {
        if (click)
            SpawnParticles(target.transform.position);
    }


    void LoadCloud()
    {
        ParticleSystem.Particle[] load_particles = new ParticleSystem.Particle[particleContainer.Count];

        for (int i = 0; i < particleContainer.Count; ++i)
        {
            load_particles[i] = makeParticle(particleContainer[i]);
        }

        particles.SetParticles(load_particles, load_particles.Length);
    }

    public void ActivateField()
    {
        particles.gameObject.SetActive(true);
        LoadCloud();
    }

    void ScaleRange(float offset)
    {
        radius += offset;

        if (Volume * partsPerUnit < 1 && offset < 0)
        {
            radius -= offset;
        }
    }

    void SpawnParticles(Vector3 pos)
    {
        int spawnNumber = (int)(Volume * partsPerUnit);
        Vector3 spawnPos;
        List<ParticleSystem.Particle> newParticles = new List<ParticleSystem.Particle>();

        for (int i = 0; i < spawnNumber; i++)
        {
            spawnPos = pos + Random.insideUnitSphere * Radius / 2;

            if (gameArea.Contains(spawnPos))
            {
                particleContainer.Add(spawnPos);
                newParticles.Add(makeParticle(spawnPos));
            }
        }

        //Get the particles already in the system
        ParticleSystem.Particle[] particles_original = new ParticleSystem.Particle[particles.particleCount];
        particles.GetParticles(particles_original);

        ParticleSystem.Particle[] particles_new = newParticles.ToArray();

        //Combine the currently existing particles with the new ones you just created
        ParticleSystem.Particle[] particles_final = new ParticleSystem.Particle[particles_original.Length + particles_new.Length];
        particles_original.CopyTo(particles_final, 0);
        particles_new.CopyTo(particles_final, particles_original.Length);

        //And put them into the particle system
        particles.SetParticles(particles_final, particles_final.Length);
    }

    //This initialises all of the ParticleSystem.Particle attributes whenever you make one.
    //Each attribute is assigned a variable that I store as class variables. I didn't write them in because it would end up too messy looking.
    ParticleSystem.Particle makeParticle(Vector3 position)
    {
        ParticleSystem.Particle r = new ParticleSystem.Particle();
        r.angularVelocity = part_angularVelocity;
        r.startColor = MaterialHolder.AffectedColor;
        r.remainingLifetime = part_lifetime;
        r.position = position;
        r.rotation = part_rotation;
        r.startSize = part_size;
        r.startLifetime = part_startLifetime;
        r.velocity = part_velocity;

        return r;
    }
}
