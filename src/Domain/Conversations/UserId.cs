using Domain.Common;

namespace Domain.Conversations;

public class UserId : GuidValueObject
{
    private UserId(Guid value) : base(value) { }

    public static UserId New() => new(Guid.NewGuid());
    public static UserId FromGuid(Guid value) => new(value);
}