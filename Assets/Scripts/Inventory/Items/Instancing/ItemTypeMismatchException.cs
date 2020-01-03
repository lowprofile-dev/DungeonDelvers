using System;

public class ItemTypeMismatchException : System.Exception
{
    public ItemTypeMismatchException() { }
    public ItemTypeMismatchException(string message) : base(message) { }
    public ItemTypeMismatchException(string message, Exception inner) : base(message, inner) { }
}