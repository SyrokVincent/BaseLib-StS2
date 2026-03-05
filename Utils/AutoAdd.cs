using MegaCrit.Sts2.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BaseLib.Patches;
using BaseLib.Patches.Content;

namespace BaseLib.Utils;

/// <summary>
/// Used to automatically register models to be added to pools (or to find types in certain namespaces for other purposes).
/// If models utilize the CustomModel classes, AutoAdd is not necessary.
/// </summary>
/// <typeparam name="AutoAddType"></typeparam>
public class AutoAdd<AutoAddType>
{
    public readonly IEnumerable<Type> FoundTypes;

    public AutoAdd(Type typeInTargetNamespace, Predicate<Type> filter = null)
    {
        string targetNamespace = typeInTargetNamespace.Namespace;

        FoundTypes = from type in Assembly.GetAssembly(typeInTargetNamespace).GetTypes()
                    where type.IsClass && type.Namespace != null && type.Namespace.StartsWith(targetNamespace) && type.IsAssignableTo(typeof(AutoAddType)) && (filter == null || filter(type))
                    select type;
    }

    public void RegisterCards()
    {
        if (!typeof(AutoAddType).IsAssignableTo(typeof(CardModel))) throw new Exception($"Cannot register cards with non-CardModel type {typeof(AutoAddType).FullName}");

        foreach (var type in FoundTypes)
        {
            if (type.IsAbstract) continue;
            CustomContentDictionary.AddModel(type);
        }
    }
}
