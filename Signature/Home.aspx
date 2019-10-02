<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Home.aspx.cs" Inherits="Home" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8"/>
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no" />

    
    <link rel="icon" href="favicon.ico" type="image/x-icon" />

    <link rel="stylesheet" href="viewer-assets/css/normalize.min.css"/>
    <link rel="stylesheet" href="viewer-assets/css/viewercontrol.css"/>
    <link rel="stylesheet" href="viewer-assets/css/viewer.css"/>

    <script src="//ajax.googleapis.com/ajax/libs/jquery/1.10.2/jquery.min.js"></script>
    <script src="//cdnjs.cloudflare.com/ajax/libs/underscore.js/1.8.3/underscore-min.js"></script>
    <script src="viewer-assets/js/jquery.hotkeys.min.js"></script>

    <!--[if lt IE 9]>
        <link rel="stylesheet" href="viewer-assets/css/legacy.css">
        <script src="viewer-assets/js/html5shiv.js"></script>
    <![endif]-->

    <script src="viewer-assets/js/viewercontrol.js"></script>
    <script src="viewer-assets/js/viewer.js"></script>
    <!-- Configuration information used for this sample. -->
    <script src="sample-config.js"></script>

    <title></title>
    <script type="text/javascript" >
        $(document).ready(function () {
            var pluginOptions = {
                documentID: "1234abcd",
                encryption: false,
                viewMode: "EqualWidthPages"
            };
            var api = $("#myDiv").pccViewer(pluginOptions).viewerControl;
            api.on("PageCountReady", function () {
                api.changeToLastPage();
            });
        });
        
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div id="myDiv">
        
    </div>
    </form>
</body>
</html>
