using System;
public class BrokerExceptionsException : System.Exception
{
    public BrokerExceptionsException() { }
    public BrokerExceptionsException(string message) : base(message) { }
    public BrokerExceptionsException(string message, System.Exception inner) : base(message, inner) { }
    protected BrokerExceptionsException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
};