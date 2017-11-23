using System.Runtime.Serialization;

namespace ARPSearch.Enums
{
    [DataContract]
    public enum ResultTypes
    {
        Success,
        Faild,
        Undefined,
        Redirect,
        Exception
    }
}