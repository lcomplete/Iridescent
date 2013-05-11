<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Examples._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <input type="button" onclick="requestTest()" value="requestTest" /><br />
        <input type="button" onclick="syncInvokePageMethod()" value="invokePageMethod" /><br />
        <input type="button" onclick="asyncInvokePageMethod()" value="asyncInvokePageMethod" /><br />
        <input type="button" onclick="invokeClassMethod()" value="invokeClassMethod" /><br />
        <input type="button" onclick="getSessionValue()" value="getSessionValue" />
    </div>
    </form>
    <script type="text/javascript">
        function getSessionValue() {
            Examples._Default.GetSessionValue(function (result) {
                alert(result);
            });
        }
        function requestTest() {
            Iridescent.Ajax.post("RequestTest.aspx", "A Method Name", { abc: "parameterstuff" }, function (result) {
                alert(result);
            });
        }
        function syncInvokePageMethod() {
            alert(Examples._Default.GetServerTime("localtime: "+new Date()));
        }
        function asyncInvokePageMethod() {
            Examples._Default.GetServerTime("localtime: " + new Date(), function (result) {
                alert(result);
            })
        }
        function invokeClassMethod() {
            Examples.Ajax.Add(3, 4, function (result) {
                alert(result);
            })
        }
    </script>
</body>
</html>
