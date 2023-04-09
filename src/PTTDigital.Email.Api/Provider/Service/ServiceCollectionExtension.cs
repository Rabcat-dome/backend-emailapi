using System.Reflection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace PTTDigital.Email.Api.Provider.Service;

/// <remarks />
internal static partial class ServiceCollectionExtension
{
    /// <summary>
    /// Adds a scope service of the type specified in Assemblies to the
    /// specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="configureOptions">The configuration being bound.</param>
    /// <param name="contextLifetime">The lifetime with which to register the AssemblyType service in the container.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    /// <seealso cref="ServiceLifetime.Scoped"/>
    internal static IServiceCollection AddAssemblyTypes(this IServiceCollection services
        , Action<IAssemblyTypeOptions> configureOptions
        , ServiceLifetime contextLifetime = ServiceLifetime.Scoped)
    {
        IAssemblyTypeOptions options = new AssemblyTypeOptions();
        configureOptions(options);

        var assemblies = GetReferencedAssemblies(options.AssemblyStrings);
        var serviceTypes = GetAssemblyTypeInfos(assemblies);

        if (!serviceTypes.Any())
        {
            return services;
        }

        Action<Type, Type> tryAddService = contextLifetime switch
        {
            ServiceLifetime.Singleton => services.TryAddSingleton,
            ServiceLifetime.Transient => services.TryAddTransient,
            _ => services.TryAddScoped,
        };

        foreach (var item in serviceTypes)
        {
            if (item.ServiceType is not null && item.ImplementationType is not null)
            {
                tryAddService(item.ServiceType, item.ImplementationType);
            }
        }

        return services;
    }

    private static IEnumerable<Assembly> GetReferencedAssemblies(IEnumerable<string> assemblyStrings)
    {
        if (assemblyStrings is null)
        {
            return Enumerable.Empty<Assembly>();
        }

        var assemblies = assemblyStrings.Select(assembly => Assembly.Load(assembly)).ToList();

        return assemblies ?? Enumerable.Empty<Assembly>();
    }

    private static IReadOnlyList<AssemblyServiceType> GetAssemblyTypeInfos(IEnumerable<Assembly> assemblies)
    {
        var result = assemblies.SelectMany(assembly =>
        {
            var assemblyName = assembly.GetName().Name;
            return assembly.DefinedTypes.SelectMany(typeInfo =>
            {
                var items = new List<AssemblyServiceType>();
                var infInfo = IsImplementInterfaceType(typeInfo);
                if (typeInfo.IsPublic && infInfo.IsImplementInterface)
                {
                    items.Add(new AssemblyServiceType
                    {
                        AssemblyName = assemblyName,
                        ServiceTypes = typeInfo.ImplementedInterfaces,
                        ImplementationType = typeInfo.AsType(),
                        HasBaseClass = infInfo.HasBaseClass,
                    });
                }

                var nestedTypes = typeInfo.DeclaredNestedTypes.Where(nestedType => nestedType.IsNestedPublic)
                    .Select(nestedType =>
                    {
                        var infInfo = IsImplementInterfaceType(nestedType);
                        if (!infInfo.IsImplementInterface)
                        {
                            return null;
                        }

                        return new AssemblyServiceType
                        {
                            AssemblyName = assemblyName,
                            ServiceTypes = nestedType.ImplementedInterfaces,
                            ImplementationType = nestedType.AsType(),
                            HasBaseClass = infInfo.HasBaseClass,
                        };
                    }).Where(c => c is not null).ToList();

                if (nestedTypes.Any())
                {
                    items.AddRange(nestedTypes);
                }

                return items;
            }).ToArray();
        }).OrderBy(c => c.ToString()).ToList();

        return result;
    }

    private static InterfaceInfo IsImplementInterfaceType(TypeInfo typeInfo)
    {
        var isBaseObject = typeInfo.BaseType is not null && typeInfo.BaseType.Equals(typeof(object));
        var hasBaseClass = typeInfo.BaseType is not null && !string.IsNullOrEmpty(typeInfo.Namespace) &&
                           typeInfo.Namespace.Equals(typeInfo.BaseType.Namespace);

        var isObsolete = typeInfo.CustomAttributes is not null &&
                         typeInfo.CustomAttributes.Any(c => c.AttributeType.Equals(typeof(ObsoleteAttribute)));
        var hasInterface = typeInfo.ImplementedInterfaces.Any();

        var result = (isBaseObject || hasBaseClass) && typeInfo.IsClass && hasInterface && !isObsolete &&
                     !typeInfo.IsGenericType;
        return new InterfaceInfo(result, hasBaseClass);
    }
}