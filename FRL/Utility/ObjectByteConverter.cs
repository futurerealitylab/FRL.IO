using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace FRL.Utility {
  class ObjectByteConverter {
    public static byte[] ObjectToByteArray(object obj) {
      BinaryFormatter bf = new BinaryFormatter();
      using (var ms = new MemoryStream()) {
        bf.Serialize(ms, obj);
        return ms.ToArray();
      }
    }

    public static T FromByteArray<T>(byte[] data) {
      if (data == null) return default(T);
      BinaryFormatter bf = new BinaryFormatter();
      using (MemoryStream ms = new MemoryStream(data)) {
        object obj = bf.Deserialize(ms);
        return (T)obj;
      }
    }
  }
}