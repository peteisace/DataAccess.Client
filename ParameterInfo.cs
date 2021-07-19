using System;

namespace Peteisace.DataAccess.Client
{
    public class ParameterInfo : IParameterInfo
    {
        private string _name;
        private object _value;

        public ParameterInfo(string name, object value)
        {
            this._name = name;
            this._value = value;
        }

        public string Name => this._name;

        public object Value => this._value;
    }
}
