using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace CommonHelper
{
    public class ObjectDeepCopy
    {
        public static T CloneOf<T>(T serializableObject) where T:class
        {
            if (serializableObject == null)
            {
                return null;
            }

            object objCopy = null;

            MemoryStream stream = new MemoryStream();
            BinaryFormatter binFormatter = new BinaryFormatter();
            binFormatter.Serialize(stream, serializableObject);
            stream.Position = 0;
            objCopy = (T)binFormatter.Deserialize(stream);
            stream.Close();
            return (T)objCopy;
        }
    }
}
