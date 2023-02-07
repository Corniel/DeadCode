﻿namespace DeadCode.Syntax;

public partial class SystemType
{
    public static readonly SystemType System_Void = New(typeof(void), SpecialType.System_Void);

    public static readonly SystemType System_Attribute = typeof(System.Attribute);
    public static readonly SystemType System_Exception = typeof(System.Exception);
    public static readonly SystemType System_ObsoleteAttribute = typeof(System.ObsoleteAttribute);

    public static readonly SystemType Microsoft_AspNetCore_Mvc_Routing_HttpMethodAttribute = new("Microsoft.AspNetCore.Mvc.Routing.HttpMethodAttribute");
}
