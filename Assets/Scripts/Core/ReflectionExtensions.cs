using System;
using System.Reflection;

public static class ReflectionExtensions
{
    public static bool HasAttribute<T>(this MemberInfo member, bool inherit = false)
        where T : Attribute
    {
        return member.GetCustomAttribute<T>(inherit) != null;
    }

    public static Type GetFieldOrPropertyType(this MemberInfo member)
    {
        switch (member)
        {
            case FieldInfo fieldInfo:
                return fieldInfo.FieldType;
            case PropertyInfo propertyInfo:
                return propertyInfo.PropertyType;
            default:
                throw new InvalidOperationException($"{member} is not a field or property");
        }
    }
    
    public static T GetAttributeOrInherited<T>(this Type type)
        where T : Attribute
    {
        while (type != null)
        {
            var attribute = type.GetCustomAttribute<T>(inherit: false);
            if (attribute != null)
                return attribute;
            type = type.BaseType;
        }
        return null;
    }
}