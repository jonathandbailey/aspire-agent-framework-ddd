using Domain.Common;

namespace Domain.Conversations;

public class ExchangeId : GuidValueObject
{
    private ExchangeId(Guid value) : base(value) { }

    public static ExchangeId New() => new(Guid.NewGuid());
    public static ExchangeId FromGuid(Guid value) => new(value);
}