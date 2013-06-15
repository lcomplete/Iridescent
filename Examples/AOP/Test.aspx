<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Test.aspx.cs" Inherits="Examples.AOP.Test" %>
<%@ Import Namespace="Examples.IService" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <%=Iridescent.DependencyResolution.ContainerFactory.Singleton.Resolve<ITestService>().GetCurrentDate() %>
    </div>
    </form>
</body>
</html>
