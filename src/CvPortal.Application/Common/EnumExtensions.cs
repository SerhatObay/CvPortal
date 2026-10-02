using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace CvPortal.Application.Common;

public static class EnumExtensions
{
    /// <summary>
    /// Enum değerinin [Display(Name = ...)] açıklamasını döner.
    /// </summary>
    public static string GetDisplayName(this Enum value)
    {
        var member = value.GetType().GetMember(value.ToString()).FirstOrDefault();
        return member?.GetCustomAttribute<DisplayAttribute>()?.GetName() ?? value.ToString();
    }
}
