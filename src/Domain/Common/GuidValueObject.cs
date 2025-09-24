namespace Domain.Common;

public abstract class GuidValueObject : ValueObject
{
    public Guid Value { get; }

    protected GuidValueObject(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("Id cannot be empty.", nameof(value));

        Value = value;
    }

    public override string ToString() => Value.ToString();

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}