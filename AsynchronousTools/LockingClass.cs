namespace AsynchronousTools
{
    using System.Diagnostics.CodeAnalysis;

    public class LockingClass
    {
        // Basiccally a lock used for console access that can be used by multiple classes
        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Reviewed. Suppression is OK here.")]
        public static object ConsoleLock = new object();
    }
}