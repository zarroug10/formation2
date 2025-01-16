using System;

namespace API.Errors;// name space that makes all the file containing it visible to each other

public class Exceptions(int statusCode ,string message,string? details ) // class with 3 params details message and syayusCode
{
    public int StatusCode { get; set; } = statusCode;//prop for Status code 
    public string Message { get; set; } = message; // prop for Message
    public string? Details { get; set; } = details ;// prop for declaration
}
