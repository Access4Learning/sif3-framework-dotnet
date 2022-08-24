using Sif.Framework.Models.DataModels;
using Sif.Specification.DataModel.Au;
using System.Xml.Serialization;

namespace Sif.Framework.Training.TestProvider.Models;

[XmlRoot("StudentPersonal", Namespace = "http://www.sifassociation.org/datamodel/au/3.4", IsNullable = false)]
[XmlType(Namespace = "http://www.sifassociation.org/datamodel/au/3.4")]
public class StudentPersonal : StudentPersonalType, IDataModel
{
}