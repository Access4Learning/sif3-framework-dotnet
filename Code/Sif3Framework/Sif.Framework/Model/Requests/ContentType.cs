using System.ComponentModel;

namespace Sif.Framework.Model.Requests
{
    /// <summary>
    /// Enumeration of content types.
    /// </summary>
    public enum ContentType
    {
        [Description("application/xml")]
        XML,

        [Description("application/json")]
        JSON
    }
}