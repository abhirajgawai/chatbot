namespace Chatbot.Infrastructure.WebEssentials;

public class ProblemException : Exception
{
    public int Code { get; private set; }

    public ProblemException(string message, int problemCode)
        : base(message)
    {
        Code = problemCode;
    }
}

