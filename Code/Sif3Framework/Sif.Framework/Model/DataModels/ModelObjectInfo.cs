using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sif.Framework.Model
{
    public class ModelObjectInfo
    {
        private String objectName = null;
        private Type objectType = null;

        public ModelObjectInfo(String objectName, Type objectType) : base()
        {
            SetObjectName(objectName);
            SetObjectType(objectType);
        }

        public String GetObjectName()
        {
            return this.objectName;
        }

        public void SetObjectName(String objectName)
        {
            this.objectName = objectName;
        }

        public Type GetObjectType()
        {
            return this.objectType;
        }

        public void SetObjectType(Type objectType)
        {
            this.objectType = objectType;
        }

        public override String ToString()
        {
            return "ModelObjectInfo [objectName=" + this.objectName + ", objectType=" + this.objectType.FullName + "]";
        }

        public override int GetHashCode()
        {
            return 31 + ((this.objectName == null) ? 0 : this.objectName.GetHashCode());
        }

        public override Boolean Equals(Object obj)
        {
            if (this == obj)
            {
                return true;
            }
            if (obj == null)
            {
                return false;
            }
            if (!GetType().Equals(obj.GetType()))
            {
                return false;
            }
            ModelObjectInfo other = (ModelObjectInfo)obj;
            if (this.objectName == null)
            {
                if (other.objectName != null)
                {
                    return false;
                }
            }
            else if (!this.objectName.Equals(other.objectName))
            {
                return false;
            }
            return true;
        }

    }

}
