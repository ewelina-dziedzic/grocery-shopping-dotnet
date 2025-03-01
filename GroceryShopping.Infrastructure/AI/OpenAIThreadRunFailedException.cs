using OpenAI.Assistants;

namespace GroceryShopping.Infrastructure.AI;

#pragma warning disable OPENAI001
public class OpenAIThreadRunFailedException(RunStatus threadRunStatus)
    : Exception($"OpenAI thread run failed with status: {threadRunStatus}");
#pragma warning restore OPENAI001