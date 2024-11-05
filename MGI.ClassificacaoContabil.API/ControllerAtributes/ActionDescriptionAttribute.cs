namespace MGI.ClassificacaoContabil.API.ControllerAtributes
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class ActionDescriptionAttribute : Attribute
    {
        public string Description { get; }

        public ActionDescriptionAttribute(string description)
        {
            Description = description;
        }
    }
}
