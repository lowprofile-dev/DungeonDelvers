using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sirenix.Utilities;

public class DeserializationFailureException : Exception
{
    public Type DeserializedType { get; private set; }

    public DeserializationFailureException(Type type)
    {
        DeserializedType = type;
    }
}
