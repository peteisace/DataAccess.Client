using System;
namespace Peteisace.DataAccess.Client
{
    public interface IParameterInfo
    {
        string Name
        {
            get;
        }

        object Value
        {
            get;
        }
    }
}
