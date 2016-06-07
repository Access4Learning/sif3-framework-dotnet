using Sif.Framework.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sif.Framework.Service
{
    public interface ModelLink
    {
        /// <summary>
        /// Returns the information for the 'single object'. The returned object holds the name of a 'single object' (i.e StudentPersonal) and the physical class this maps to for the data model supported for this implementation.
        /// </summary>
        /// <returns>See Desc.</returns>
        ModelObjectInfo GetSingleObjectClassInfo();

        /// <summary>
        /// Returns the information for the 'collection-style object'. The returned object holds the name of a 'collection-style object' (i.e StudentPersonal) and the physical class this maps to for the data model supported for this implementation.
        /// </summary>
        /// <returns>See Desc.</returns>
        ModelObjectInfo GetMultiObjectClassInfo();
    }
}