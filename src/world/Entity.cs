using System;
using System.Collections.Generic;
using System.Text;
using Axiom.MathLib;
using OpenSim.types;

namespace OpenSim.world
{
    public class Entity
    {
        protected libsecondlife.LLUUID uuid;
        protected Vector3 position;
        protected Vector3 velocity;
        protected Quaternion rotation;
        protected string name;
        protected List<Entity> children;
	public bool needupdate;

        public Entity()
        {
            uuid = new libsecondlife.LLUUID();
            position = new Vector3();
            velocity = new Vector3();
            rotation = new Quaternion();
            name = "(basic entity)";
            children = new List<Entity>();
        }

        public virtual void update() {
            // Do any per-frame updates needed that are applicable to every type of entity
            foreach (Entity child in children)
            {
            	if(child.needupdate) 
			child.update();
            }
        }

        public virtual string getName()
        {
            return name;
        }

        public virtual Mesh getMesh()
        {
            Mesh mesh = new Mesh();

            foreach (Entity child in children)
            {
                mesh += child.getMesh();
            }

            return mesh;
        }
    }
}
