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
using TMPro;
using UnityEngine;

/*
 * Deletes Particles within sphere
 */

public class ParticleSphere : TriggerSphere
{
    ParticleSystem particles;

    TIDrawField drawField;
    public TIDrawField DrawField { get { return drawField; } set { drawField = value; } }

    bool justClicked = false;

    public override void Initialize(float _scale, Material mat, Vector3 _sphereBottom, Vector3 _sphereDirection)
    {
        base.Initialize(_scale, mat, _sphereBottom, _sphereDirection);

        particles = ParticleField.System;
    }

    private void Update()
    {
        List<ParticleSystem.Particle> toKeep = new List<ParticleSystem.Particle>();
        if (Clicked)
        {
            justClicked = true;
            ParticleSystem.Particle[] particles_original = new ParticleSystem.Particle[particles.particleCount];
            particles.GetParticles(particles_original);
            float scale = Scale / 2;

            foreach (ParticleSystem.Particle particle in particles_original)
            {

                float dist = Vector3.Distance(transform.position, particle.position);
                if (dist > scale)
                {
                    toKeep.Add(particle);
                }
            }

            KeepParticles(toKeep.ToArray());
        }
        else if (!Clicked && justClicked)
        {
            drawField.ParticleContainer = SaveChanges();
            justClicked = false;
        }
    }

    public List<Vector3> SaveChanges()
    {
        List<Vector3> changedParticles = new List<Vector3>();

        ParticleSystem.Particle[] particles_original = new ParticleSystem.Particle[particles.particleCount];
        particles.GetParticles(particles_original);

        foreach (ParticleSystem.Particle particle in particles_original)
        {
            changedParticles.Add(particle.position);
        }

        return changedParticles;
    }

    void KeepParticles(ParticleSystem.Particle[] keepParticles)
    {
        particles.SetParticles(keepParticles, keepParticles.Length);
    }
}

