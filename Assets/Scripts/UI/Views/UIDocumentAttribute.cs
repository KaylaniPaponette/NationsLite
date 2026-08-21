using System;

[AttributeUsage(AttributeTargets.Class)]
public class UIDocumentAttribute : Attribute
{
    public string uiDocName { get; }
    public string stylesheetName { get; }
    
    public UIDocumentAttribute(string uiDocName, string stylesheetName = null)
    {
        this.uiDocName = uiDocName;
        this.stylesheetName = stylesheetName;
    }
}
