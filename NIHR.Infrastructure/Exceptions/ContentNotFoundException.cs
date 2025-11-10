using System;

public class ContentNotFoundException : Exception
{
    public ContentNotFoundException() : base($"Content Id cannot be null or empty.")
    {
    }
}