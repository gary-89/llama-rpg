using System.Reflection;
using Xunit.Sdk;

namespace LlamaRpg.Tests;

public sealed class RepeatAttribute : DataAttribute
{
    private readonly int _count;
    private readonly int _param1;
    private readonly int _param2;
    private readonly int _param3;

    public RepeatAttribute(int count, int param1, int param2, int param3)
    {
        if (count < 1)
        {
            throw new ArgumentOutOfRangeException(paramName: nameof(count), message: "Repeat count must be greater than 0.");
        }

        _count = count;
        _param1 = param1;
        _param2 = param2;
        _param3 = param3;
    }

    public override IEnumerable<object[]> GetData(MethodInfo testMethod)
    {
        return Enumerable.Range(start: 1, count: _count).Select(_ => new object[] { _param1, _param2, _param3 });
    }
}
