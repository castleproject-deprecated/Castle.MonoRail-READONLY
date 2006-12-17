using System;

namespace Castle.Igloo.Attributes
{
    /// <summary>
    /// Specifies that a controller m�thod must not declench a navigation to another view.
    /// </summary>
    [AttributeUsageAttribute( AttributeTargets.Class | AttributeTargets.Method )]
    public class NoNavigationAttribute : Attribute 
    {
    }
}
