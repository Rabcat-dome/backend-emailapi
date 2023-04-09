namespace PTTDigital.Email.Api.Provider.Service;

internal record AssemblyServiceType
{
    internal Type? ServiceType
    {
        get
        {
            var count = ServiceTypes?.Count() ?? 0;
            if (HasBaseClass && count > 1)
            {
                return ServiceTypes?.Skip(1).FirstOrDefault();
            }
            else
            {
                return ServiceTypes?.FirstOrDefault();
            }
        }
    }

    internal IEnumerable<Type>? ServiceTypes { get; init; }
    internal Type? ImplementationType { get; init; }
    internal string? AssemblyName { get; init; }
    internal bool HasBaseClass { get; init; }

    public override string ToString()
    {
        if (ServiceType is not null && ImplementationType is not null)
        {
            return $"{AssemblyName}: {ServiceType.Name}, {ImplementationType.Name}";
        }
        else
        {
            return $"{AssemblyName}: Invalid Types";
        }
    }
}
