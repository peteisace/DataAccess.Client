using System;
using System.Data;

namespace Peteisace.DataAccess.Client
{
    public class SimpleDataReader : IDataReader
    {
        private IDataReader _source;
        public SimpleDataReader(IDataReader source)
        {
            this._source = source;
        }

        public object this[int i] => this._source[i];

        public object this[string name] => this._source[name];

        public int Depth => this._source.Depth;

        public bool IsClosed => this._source.IsClosed;

        public int RecordsAffected => this._source.RecordsAffected;

        public int FieldCount => this._source.FieldCount;

        public void Close()
        {
            this._source.Close();
        }

        public void Dispose()
        {
            this._source.Dispose();
        }

        public bool? GetNullableBoolean(string name)
        {
            return this.GetValue<bool?>(name);
        }

        public bool GetBoolean(string name)
        {
            return this.GetValue<bool>(name, false);
        }

        public bool GetBoolean(int i)
        {
            return this._source.GetBoolean(i);
        }

        public byte? GetNullableByte(string name)
        {
            return this.GetValue<byte?>(name, true);
        }

        public byte GetByte(string name)
        {
            return this.GetValue<byte>(name);
        }

        public byte GetByte(int i)
        {
            return this._source.GetByte(i);
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            return this._source.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
        }

        public char? GetNullableChar(string name)
        {
            return this.GetValue<char?>(name, true);
        }

        public char GetChar(string name)
        {
            return this.GetValue<char>(name);
        }

        public char GetChar(int i)
        {
            return this._source.GetChar(i);
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            return this._source.GetChars(i, fieldoffset, buffer, bufferoffset, length);
        }

        public IDataReader GetData(int i)
        {
            return this._source.GetData(i);
        }

        public string GetDataTypeName(int i)
        {
            return this._source.GetDataTypeName(i);
        }

        public DateTime? GetNullableDateTime(string name)
        {
            return this.GetValue<DateTime?>(name, true);
        }

        public DateTime GetDateTime(string name)
        {
            return this.GetValue<DateTime>(name);
        }

        public DateTime GetDateTime(int i)
        {
            return this._source.GetDateTime(i);
        }

        public decimal? GetNullableDecimal(string name)
        {
            return this.GetValue<decimal?>(name, true);
        }

        public decimal GetDecimal(string name)
        {
            return this.GetValue<decimal>(name);
        }

        public decimal GetDecimal(int i)
        {
            return this._source.GetDecimal(i);
        }

        public double? GetNullableDouble(string name)
        {
            return this.GetValue<double?>(name, true);
        }

        public double GetDouble(string name)
        {
            return this.GetValue<double>(name);
        }

        public double GetDouble(int i)
        {
            return this._source.GetDouble(i);
        }

        public Type GetFieldType(int i)
        {
            return this._source.GetFieldType(i);
        }

        public float? GetNullableFloat(string name)
        {
            return this.GetValue<float?>(name, true);
        }

        public float GetFloat(string name)
        {
            return this.GetValue<float>(name);
        }

        public float GetFloat(int i)
        {
            return this._source.GetFloat(i);
        }

        public Guid? GetNullableGuid(string name)
        {
            return this.GetValue<Guid?>(name, true);
        }

        public Guid GetGuid(string name)
        {
            return this.GetValue<Guid>(name);
        }

        public Guid GetGuid(int i)
        {
            return this._source.GetGuid(i);
        }

        public short? GetNullableInt16(string name)
        {
            return this.GetValue<short?>(name, true);
        }

        public short GetInt16(string name)
        {
            return this.GetValue<short>(name);
        }

        public short GetInt16(int i)
        {
            return this._source.GetInt16(i);
        }

        public int? GetNullableInt32(string name)
        {
            return this.GetValue<int?>(name, true);
        }

        public int GetInt32(string name)
        {
            return this.GetValue<int>(name);
        }
        public int GetInt32(int i)
        {
            return this._source.GetInt32(i);
        }

        public long? GetNullableInt64(string name)
        {
            return this.GetValue<long?>(name, true);
        }

        public long GetInt64(string name)
        {
            return this.GetValue<long>(name);
        }

        public long GetInt64(int i)
        {
            return this._source.GetInt64(i);
        }

        public string GetName(int i)
        {
            return this._source.GetName(i);
        }

        public int GetOrdinal(string name)
        {
            // TODO: Resultset::Ordinal cache.
            return this._source.GetOrdinal(name);
        }

        public DataTable GetSchemaTable()
        {
            return this._source.GetSchemaTable();
        }

        public string GetNullableString(string name)
        {
            return  this.GetValue<string>(name, true);
        }

        public string GetString(string name)
        {
            return this.GetValue<string>(name);
        }

        public string GetString(int i)
        {
            return this._source.GetString(i);
        }

        public object GetValue(int i)
        {
            return this._source.GetValue(i);
        }

        public object GetValue(string name)
        {
            return this.GetValue(name, false);
        }

        public int GetValues(object[] values)
        {
            return this._source.GetValues(values);
        }

        public bool IsDBNull(string name)
        {
            var cIndex = this._source.GetOrdinal(name);
            if(cIndex < 0)
            {
                throw new ArgumentException($"There is no column {name}.", nameof(name));
            }

            return this._source.IsDBNull(cIndex);
        }

        public bool IsDBNull(int i)
        {
            return this._source.IsDBNull(i);
        }

        public bool NextResult()
        {
            return this._source.NextResult();
        }

        public bool Read()
        {
            return this._source.Read();
        }

        private T GetValue<T>(string name, bool allowNull)
        {
            var value = this.GetValue(name, allowNull);
            return (T)value;
        }

        private T GetValue<T>(string name)
        {
            return this.GetValue<T>(name, false);
        }


        private object GetValue(string name, bool allowNull)
        {
            var index = this.GetOrdinal(name);
            if(index < 0)
            {
                throw new ArgumentException($"Could not find column {name}.", nameof(name));
            }

            var value = this.GetValue(index);
            
            if(!allowNull && value == DBNull.Value)
            {
                throw new InvalidCastException($"Value stored at ${name} is DBNull. Use appropriate method to retrieve.");
            }

            return value == DBNull.Value ? null : value;
        }
    }
}