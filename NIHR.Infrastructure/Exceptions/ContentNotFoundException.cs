using System;

public class ContentNotFoundException : Exception
{
    public ContentNotFoundException() : base($"Content value cannot be null or empty.")
    {
    }
}