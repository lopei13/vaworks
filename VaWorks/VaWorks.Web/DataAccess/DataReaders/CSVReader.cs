using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using VaWorks.Web.DataAccess.Entities;

namespace VaWorks.Web.DataAccess.DataReaders
{
    public class CSVReader : System.Data.IDataReader
    {
        private Stream _stream;
        private StreamReader _reader;
        private bool _eof;
        private bool _hasHeaders;

        Dictionary<string, object> _map = new Dictionary<string, object>();
        List<object> _list = new List<object>();

        public CSVReader(Stream stream, bool hasHeaders, char delimitter)
        {
            _stream = stream;
            _reader = new StreamReader(stream);
            _hasHeaders = hasHeaders;

            if (hasHeaders) {
                _stream.Seek(0, SeekOrigin.Begin);

                string[] headers = _reader.ReadLine().Split(delimitter);
                foreach(var h in headers) {
                    _map.Add(h, null);
                }
            } 
        }

        public object this[string name]
        {
            get
            {
                if (_hasHeaders) {
                    if (_map.Count == 0) {
                        throw new Exception("Call read before accessing data.");
                    }

                    if (_map.ContainsKey(name)) {
                        return _map[name];
                    } else {
                        throw new Exception($"Reader does not contain the column {name}.");
                    }
                } else {
                    throw new Exception("Reader does not contain any headers.  Get values by index.");
                }
            }
        }

        public object this[int i]
        {
            get
            {
                if (_hasHeaders) {

                    if (_map.Count == 0) {
                        throw new Exception("Call read before accessing data.");
                    }

                    if (_map.Count > i) {
                        return _map.ElementAt(i).Value;
                    } else {
                        throw new Exception("Index is outside the length of the reader.");
                    }
                } else {
                    if(_list.Count == 0) {
                        throw new Exception("Call read before accessing data.");
                    }

                    if(_list.Count > i) {
                        return _list[i];
                    } else {
                        throw new Exception("Index is outside the length of the reader.");
                    }
                }
            }
        }

        public int Depth
        {
            get
            {
                return 0;
            }
        }

        public int FieldCount
        {
            get
            {
                return _map.Count;
            }
        }

        public bool IsClosed
        {
            get
            {
                return _eof;
            }
        }

        public int RecordsAffected
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void Close()
        {
            _list.Clear();
            _map.Clear();
            _reader.Close();
            _reader.Dispose();
        }

        public void Dispose()
        {
            Close();
        }

        public bool GetBoolean(int i)
        {
            return bool.Parse(this[i].ToString());
        }

        public byte GetByte(int i)
        {
            throw new NotImplementedException();
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public char GetChar(int i)
        {
            throw new NotImplementedException();
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public IDataReader GetData(int i)
        {
            throw new NotImplementedException();
        }

        public string GetDataTypeName(int i)
        {
            throw new NotImplementedException();
        }

        public DateTime GetDateTime(int i)
        {
            throw new NotImplementedException();
        }

        public decimal GetDecimal(int i)
        {
            throw new NotImplementedException();
        }

        public double GetDouble(int i)
        {
            return double.Parse(this[i].ToString());
        }

        public Type GetFieldType(int i)
        {
            throw new NotImplementedException();
        }

        public float GetFloat(int i)
        {
            throw new NotImplementedException();
        }

        public Guid GetGuid(int i)
        {
            throw new NotImplementedException();
        }

        public short GetInt16(int i)
        {
            throw new NotImplementedException();
        }

        public int GetInt32(int i)
        {
            return int.Parse(this[i].ToString());
        }

        public long GetInt64(int i)
        {
            throw new NotImplementedException();
        }

        public string GetName(int i)
        {
            throw new NotImplementedException();
        }

        public int GetOrdinal(string name)
        {
            throw new NotImplementedException();
        }

        public DataTable GetSchemaTable()
        {
            throw new NotImplementedException();
        }

        public string GetString(int i)
        {
            return this[i].ToString();
        }

        public object GetValue(int i)
        {
            return this[i];
        }

        public int GetValues(object[] values)
        {
            throw new NotImplementedException();
        }

        public bool IsDBNull(int i)
        {
            throw new NotImplementedException();
        }

        public bool NextResult()
        {
            return false;
        }

        public bool Read()
        {
            foreach(var k in _map.Keys) {
                _map[k] = null;
            }

            _list.Clear();

            string line = _reader.ReadLine();

            _eof = line == null;

            if (!_eof) {
                string[] columns = line.Split('\t');

                for (int i = 0; i < columns.Length; i++) {
                    if (_hasHeaders) {
                        string key = _map.ElementAt(i).Key;
                        _map[key] = columns[i];
                    } else {
                        _list.Add(columns[i]);
                    }
                }

            }

            return !_eof;
        }
     
    }
}