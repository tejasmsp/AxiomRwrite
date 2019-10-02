<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Viewer.Login" %>

<!DOCTYPE html>

<link href="viewer-assets/css/CommonCombobox.css" rel="stylesheet" />
<link href="viewer-assets/css/Modal.css" rel="stylesheet" />
<link href="viewer-assets/css/Input.css" rel="stylesheet" />
<link href="viewer-assets/css/jquery.ui.css" rel="stylesheet" />
<link href="viewer-assets/css/Master.css" rel="stylesheet" />
<link href="viewer-assets/css/Login.css" rel="stylesheet" />
<link href="viewer-assets/css/Colors.css" rel="stylesheet" />


<%--<script src="http://ajax.googleapis.com/ajax/libs/jquery/1.7.1/jquery.min.js" type="text/javascript"></script>
<script src="//code.jquery.com/ui/1.11.4/jquery-ui.js"></script>--%>
<script src="http://ajax.googleapis.com/ajax/libs/jquery/1.9.0/jquery.min.js"></script>
<script src="//code.jquery.com/ui/1.11.4/jquery-ui.js"></script>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">
        $(document).ready(function ()
        {            
            $("#DOB").datepicker({
                changeMonth: true,
                changeYear: true,
                yearRange: '1900:2100',
                numberOfMonths: 1,
                onSelect: function (selected)
                {
                    var dt = new Date(selected);                  
                    dt.setDate(dt.getDate());
                    $("#DOB").datepicker("option", "maxDate", dt);
                }
            });
        });
    </script>
</head>
<body class="loginBody" style="font-size: 11pt;">
    <form id="form1" runat="server">
        <div class="loginSpace"></div>
        <div class="loginPanel">
            <div class="logoPanel">
                <img src="viewer-assets/img/logo-axiom.png" />
            </div>
            <div class="loginWrapper bgTan3">
                <div id="loginBox">
                    <div class="innerLoginWrapper textWhite">
                        <div class="loginUL" style="padding-top: 50px;">
                            <div>
                                <asp:TextBox ID="DOB" runat="server" placeholder="Date Of Birth" autocomplete="off"></asp:TextBox>
                                    <asp:RequiredFieldValidator Display="Dynamic" ID="DOBRequired" runat="server" Font-Size="X-Small"
                            ControlToValidate="DOB" ErrorMessage="Date Of Birth is required." 
                            ToolTip="DOB is required." ValidationGroup="LoginGroup">Please Enter DOB in MM/dd/yyyy Format</asp:RequiredFieldValidator>
                           <asp:RegularExpressionValidator ID="ExpName" runat="server" Font-Size="X-Small" 
                                ErrorMessage="Please Enter Correct DOB"   ValidationGroup="LoginGroup"
                                ControlToValidate="DOB"     
                                ValidationExpression="^(0[1-9]|1[012])[- /.](0[1-9]|[12][0-9]|3[01])[- /.](19|20)\d\d$" />
                            </div>
                            <div>
                                <asp:Button ID="Submit" CssClass="bgOrange" runat="server" Text="SUBMIT" ValidationGroup="LoginGroup" OnClick="btnSubmit_Click" />
                            </div>
                            <div>
                                <asp:Label ID="lblErrormsg" runat="server"  Font-Names="Verdana" ForeColor="Red"></asp:Label>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="subLogo">
                <img src="viewer-assets/img/axiomGroupLogoBottom.png" />
            </div>
        </div>
    </form>
</body>
</html>
