<%@ Import Namespace="Viewer" %>

<%@ Page Language="C#" AutoEventWireup="true" Inherits="Viewer._Default" %>

<!DOCTYPE html>
<html>
<head id="Head1" runat="server">
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    <meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1, user-scalable=no">

    <title>Axon VUE</title>
    <link rel="icon" href="favicon.ico" type="image/x-icon" />

    <link rel="stylesheet" href="viewer-assets/css/normalize.min.css">
    <link rel="stylesheet" href="viewer-assets/css/viewercontrol.css">
    <link rel="stylesheet" href="viewer-assets/css/viewer.css">

    <script src="//ajax.googleapis.com/ajax/libs/jquery/1.10.2/jquery.min.js"></script>
    <script>window.jQuery || document.write('<script src="viewer-assets/js/jquery-1.10.2.min.js"><\/script>');</script>
    <script src="//cdnjs.cloudflare.com/ajax/libs/underscore.js/1.8.3/underscore-min.js"></script>
    <script>window._ || document.write('<script src="viewer-assets/js/underscore.min.js"><\/script>');</script>
    <script src="viewer-assets/js/jquery.hotkeys.min.js"></script>

    <!--[if lt IE 9]>
        <link rel="stylesheet" href="viewer-assets/css/legacy.css">
        <script src="viewer-assets/js/html5shiv.js"></script>
    <![endif]-->

    <script src="viewer-assets/js/viewercontrol.js"></script>
    <script src="viewer-assets/js/viewer.js"></script>
    <!-- Configuration information used for this sample. -->
    <script src="sample-config.js"></script>
</head>
<body>
    <input type="text" id="txtGUID" style="display: none" value="<% =GuidVal %>" />
    <div id="viewer1"></div>
    <div id="attachments" style="display: none;">
        <b>Attachments:</b>
        <p id="attachmentList">
        </p>
        <asp:HiddenField ID="hdn" />
        <label id="lblDocName"></label>
    </div>

    <script type="text/javascript">        
        var docID;
        var OrderNo =<%=OrderNo%>;
        var PartNo =<%=PartNo%>;
        var FileDiskName ="<%=FileDiskName%>";
        var FileName ="<%=FileName%>";
        if(OrderNo == "")
        {
            alert('File Not Found');            
        }


        jQuery(function($) {
            var viewingSessionId = '';
            var query = (function parseQuery() {
                var query = {};
                var temp = window.location.search.substring(1).split('&');
                for (var i = temp.length; i--;) {
                    var q = temp[i].split('=');
                    query[q.shift()] = decodeURIComponent(q.join('='));
                }
                console.log(query);
                return query;
            })();

            // setup -- you probably already have this, so you can ignore it
            function createSession(data) {
                data.serverCaching = "none";
                console.log(data);
                if (data.viewingSessionId)
                {
                    return data.viewingSessionId;
                }
                 //alert(sampleConfig.imageHandlerUrl + '/ViewingSession');
                return $.ajax({
                    url: sampleConfig.imageHandlerUrl + '/ViewingSession',
                    method: 'POST',
                    contentType: 'application/json',
                    data: JSON.stringify(data)
                }).then(function (response) {
                    viewingSessionId = response.viewingSessionId || response.documentID;
                    $("#hdn").text(viewingSessionId);
                    return viewingSessionId;
                });
            }

            function getTemplate(templateName) {
                return $.ajax({url: templateName})
                    .then(function (response) {
                        return response;
                    });
            }

            function getJson(fileName) {

                return $.ajax({ url: fileName })
                    .then(function (response) {
                        // IIS Express will not use the correct MIME type for json, so we may need to parse it as a string
                        if (typeof response === 'string') {
                            return JSON.parse(response);
                        }
                        return response;
                    });
            }

            function getResourcesAndEmbedViewer(demoConfig) {
                var sessionData = {};
                  
                if (query.viewingSessionId) {
                    sessionData.viewingSessionId = query.viewingSessionId;
                } else {
                    sessionData.source = getDocumentSource(FileDiskName || '');
                }

                $.when(
                    sessionData.document, // args[0]
                    createSession(sessionData), // args[1]
                    <%=htmlTemplates%>, // args[2]
                    getJson(sampleConfig.viewerAssetsPath + '/languages/' + sampleConfig.languageFile), // args[3]
                    getJson('redactionReason.json'), // args[4]
                    getJson('predefinedSearch.json'), // args[5]
                    demoConfig.options || {}) // args[6]
                    .done(buildViewerOptions);
            }

            function getDocumentSource(documentId) {
                var source = {};
                if (documentId.search(/^https?:\/\//) === 0) {
                    source.type = 'url';
                    source.url = documentId;
                } else {
                    source.type = 'document';
                    // source.fileName = documentId || 'PdfDemoSample.pdf';
                    source.fileName = OrderNo + "\\" + PartNo + "\\" + documentId || 'PdfDemoSample.pdf';
                    // source.fileName = 'PdfDemoSample.pdf';
                }
                docID = documentId;
                console.log(source);
                return source;
            }

            function buildViewerOptions() {
                var args = [].slice.call(arguments);

                var optionsOverride = args.pop(); // always last arg

                var options = {
                    annotationsMode: "LayeredAnnotations",
                    documentID: encodeURIComponent(args[1]),
                    language: args[3],
                    predefinedSearch: args[5],
                    template: args[2],
                    signatureCategories: 'Signature,Initials,Title',
                    immediateActionMenuMode: 'hover',
                    redactionReasons: args[4],
                    documentDisplayName: args[0],
                    uiElements: {
                        download: true,
                        fullScreenOnInit: true,
                        advancedSearch: true,
                        attachments: true,
                        redactionReasons: true
                    },
                    discardOutOfViewText: true
                };

                var combinedOptions = _.extend(optionsOverride, options);

                embedViewer(combinedOptions);
            }

            function embedViewer(options) {
                var viewer = $('#viewer1').pccViewer(options);
                $("#SignatureBox").click();
            }

            getResourcesAndEmbedViewer({
                options: {
                    imageHandlerUrl: sampleConfig.imageHandlerUrl,
                    resourcePath: sampleConfig.viewerAssetsPath + '/img'
                }
            });
        });

        $(document).on('click', '.MakeSignaturea', function () {
            if ($("g[fill-opacity]").html() != undefined) {
                var Svgtxt = $("g[fill-opacity]").html();
                $.ajax({
                    async: true,
                    method: "POST",
                    contentType: "application/json",
                    url: "/viewer/viewer-webtier/pcc.ashx/v2/viewingSessions/" + $("#hdn").text() + "/searchTasks",
                    data: "{\n\t\"input\": {\n\t\t\"searchTerms\": [\n\t\t\t{\n\t\t\t\t\"type\": \"simple\",\n\t\t\t\t\"pattern\": \"Signature\"\n\t\t\t}\n\t\t]\n\t}\n}",
                    success: function (data) {
                        debugger;
                        var processID = data.processId;
                        $.ajax({
                            async: true,
                            method: "GET",
                            contentType: "application/json",
                            url: "/viewer/viewer-webtier/pcc.ashx/v2/searchTasks/" + processID + "/results",
                            success: function (data) {
                                if (data.results.length > 0) {
                                    //  var result = _.groupBy(data.results, 'pageIndex');
                                    // var result = data.results.length;
                                    for (k = 0; k < data.results.length; k++) {
                                        //var NewpageIndex = k + 1;
                                        var pages = 0;
                                        var Width = 612;
                                        var Height = 792;
                                        var x = data.results[k].lineRectangles[0].x;
                                        var y = data.results[k].lineRectangles[0].y;
                                        var Anno = $.parseXML(Svgtxt).getElementsByTagName("svg")[0];
                                        Anno.setAttribute("x", x);
                                        Anno.setAttribute("y", y);
                                        Anno.setAttribute("data-pcc-mark", "mark-25");
                                        //$("g[fill-opacity]").html("")

                                        $("g[fill-opacity]").append(xmlToString(Anno));


                                        //for (i = 0; i < result[k].length; i++) {
                                        //    Width = result[k][i].pageData.width;
                                        //    Height = result[k][i].pageData.height;
                                        //    var x = result[k][i].lineRectangles[0].x;
                                        //    var y = result[k][i].lineRectangles[0].y;
                                        //    var Annotation = $.parseXML(Data.VF).getElementsByTagName("annotation")[0];
                                        //    Annotation.setAttribute("x", x);
                                        //    Annotation.setAttribute("y", y);
                                        //    var x_percent = x / Width;
                                        //    var y_percent = y / Height;
                                        //    Annotation.setAttribute("x_percent", x_percent);
                                        //    Annotation.setAttribute("y_percent", y_percent);
                                        //    NewXmlData += Annotation;
                                        //}

                                    }
                                }
                            },
                            error: function (xhr, textStatus) {
                                console.log(xhr.status + " - " + xhr.responseText)
                            }
                        }).done(function (data) { });
                    },
                    error: function (xhr, textStatus) {
                        console.log(xhr.status + " - " + xhr.responseText)
                    }
                }).done(function (data) { });
            }
        });


        function xmlToString(xmlData) {

            var xmlString;
            //IE
            if (window.ActiveXObject) {
                xmlString = xmlData.xml;
            }
            // code for Mozilla, Firefox, Opera, etc.
            else {
                xmlString = (new XMLSerializer()).serializeToString(xmlData);
            }
            return xmlString;
        }


        $(document).on('click', '.DownloadSave', function () {
            var viewerPlugin = $('#viewer1').pccViewer();
            var viewerControl = viewerPlugin.viewerControl;
            var Data = viewerControl.burnMarkup();

            var XmlData = Data.VF;
            // var SigData = viewerControl.serverSearch("signature").fc;


            //if(SigData.length != 0)               
            //{
            //    NewXmlData = "<?xml version='1.0'?><documentAnnotations><pages><page id='1' pageWidth='"+Width+"' pageHeight='"+Height+"'>";
            //    for(i = 0; i < SigData.length; i++)
            //    {
            //        var x=SigData[i].nV.x;
            //        var y=SigData[i].nV.y;
            //        var Width=result[i].gT.width;
            //        var Height=result[i].gT.height;
            //        var Annotation= $.parseXML(Data.VF).getElementsByTagName("annotation")[0];
            //        Annotation.setAttribute("x",x);
            //        Annotation.setAttribute("y",y);
            //        var x_percent = x / Width;
            //        var y_percent = y / Height;
            //        Annotation.setAttribute("x_percent",x_percent);
            //        Annotation.setAttribute("y_percent",y_percent);
            //        NewXmlData += Annotation;
            //    }
            //    NewXmlData += "</page></pages><highlights /></documentAnnotations>";
            //}
            //else
            //{
            //    NewXmlData=XmlData;
            //}
            $.ajax({
                method: "POST",
                contentType: "application/json",
                data: XmlData,
                url: "/viewer/viewer-webtier/pcc.ashx/ViewingSession/u" + $("#hdn").text() + "/MarkupBurner",
                success: function (data) {
                    if (confirm("Are you sure you want to save this Document..??")) {
                        var ProcessID = data.processId;
                        $.ajax({
                            method: "GET",
                            contentType: "application/json",
                            url: "/viewer/viewer-webtier/pcc.ashx/ViewingSession/u" + $("#hdn").text() + "/MarkupBurner/" + ProcessID + "",
                            success: function (ProcessResponse) {
                                if (ProcessResponse.state == "complete") {
                                    var ProcessID = ProcessResponse.processId;
                                    var GUID = '';
                                    GUID = $('#txtGUID').val();
                                    //  var urlParams = window.location.search;
                                    //   var Extension = urlParams.toString().split('&')[3].split('=')[1].split('.')[1];
                                    var Extension = FileName.toString().split('.')[1];
                                    GUID = GUID + '.' + Extension;
                                    $.ajax({
                                        method: "GET",
                                        contentType: "application/json",
                                        url: "/viewer/viewer-webtier/pcc.ashx/ViewingSession/u" + $("#hdn").text() + "/MarkupBurner/" + ProcessID + "/Document?",
                                        data: { GUID: GUID },
                                        success: function (data) {
                                            var o = new Object();
                                            // o.BatchId = '58823316-c0aa-4770-9f2d-98a3a3f1a702.pdf';
                                            o.BatchId = GUID;
                                            //o.OrderNo = urlParams.toString().split('&')[0].split('=')[1];
                                            //o.PartNo = urlParams.toString().split('&')[1].split('=')[1];
                                            //o.FileNamewithSigned = urlParams.toString().split('&')[3].split('=')[1].split('.')[0] + '_Signed.' + Extension;
                                            //o.FileName=urlParams.toString().split('&')[3].split('=')[1];
                                            o.OrderNo = OrderNo;
                                            o.PartNo = PartNo;
                                            o.FileNamewithSigned = FileName.toString().split('.')[0] + '_Signed.' + Extension;
                                            o.FileName = FileName;
                                            saveUploadsAsync(o);
                                        },
                                        error: function (xhr, textStatus) {
                                            console.log(xhr.status + " - " + xhr.responseText)
                                        }
                                    }).done(function (data) { });
                                }
                                else {
                                    alert("Some Error Occurs..!!")
                                }
                            },
                            error: function (xhr, textStatus) {
                                console.log(xhr.status + " - " + xhr.responseText)
                            }
                        }).done(function (data) { });
                    }
                },
                error: function (xhr, textStatus) {
                    console.log(xhr.status + " - " + xhr.responseText)
                }

            }).done(function (data) { });
            //var MarkupCall = {              
            //    "crossDomain": true,
            //    "url": "/viewer-webtier/pcc.ashx/ViewingSession/u" + $("#hdn").text() + "/MarkupBurner",
            //    "method": "POST",              
            //    "headers": {
            //        "content-type": "text/xml",
            //        "cache-control": "no-cache"
            //    },
            //    "data": ""+XmlData+""
            //}

            //$.ajax(MarkupCall).done(function (response) {
            //    alert("test");
            //    var ProcessID = response.processId;
            //    var ProcessCall = {                  
            //        "crossDomain": true,
            //        "contentType": "application/json; charset=utf-8",
            //        "url": "/viewer-webtier/pcc.ashx/ViewingSession/u" + $("#hdn").text() + "/MarkupBurner/" + ProcessID+"",
            //        "method": "GET",
            //        "headers": {
            //            "cache-control": "no-cache",
            //            "Accept": "*/*"                       
            //        }
            //    }
            //    $.ajax(ProcessCall).done(function (ProcessResponse) {

            //        if(ProcessResponse.state == "complete")
            //        {
            //            debugger;
            //            var ProcessID = ProcessResponse.processId;                      
            //            $.ajax({
            //                method: "GET",
            //                contentType: "application/json",
            //                url: "viewer-webtier/pcc.ashx/ViewingSession/u" + $("#hdn").text() + "/MarkupBurner/" + ProcessID + "/Document?",
            //                success: function(data) {                               
            //                    alert("File Save SuccessFully");                              
            //                },
            //                error: function(xhr, textStatus) {
            //                    console.log(xhr.status + " - " + xhr.responseText)
            //                }
            //            }).done(function(data){});
            //        }
            //        else
            //        {
            //            alert("Some Error Occurs..!!")
            //        }
            //    });
            //});
        });

        function saveUploadsAsync(data) {

            $.ajax({
                async: false,
                type: "POST",
                url: "/Viewer/Default.aspx/AddFileToPartNew",
                //  url: "/Clients/Secured/Data/GetMRBillingData.aspx/AddFileToPartNew",               
                data: JSON.stringify({
                    BatchId: data.BatchId,
                    OrderNo: data.OrderNo,
                    PartNo: data.PartNo,
                    FileName: decodeURI(data.FileName),
                    FileNamewithSigned: decodeURI(data.FileNamewithSigned)
                }),
                contentType: "application/json; charset=utf-8",
                //  data: JSON.stringify(data),
                dataType: "json",
                success: function (result) {
                    alert("File Save SuccessFully..");
                },
                error: function () {
                },
                always: function () {
                }
            });
        }

        $(document).ready(function () {
            //// $(".MakeSignature").click(PlaceSign);
            //$(".MakeSignature").on('click', "PlaceSign"); 

            // PlaceSign();
        });





        // "data": "<?xml version=\"1.0\"?><documentAnnotations><pages><page id=\"1\" pageWidth=\"612\" pageHeight=\"792\"><annotation nodeId=\"3DCE0A17-1F10-F2A0-E84F-31E014428D97\" lineColor=\"0\" interactionMode=\"0\" stampSize=\"122\" opacity=\"255\" highlightGroupID=\"92_1465420601728_0542\" drawType=\"text_selection_redaction\" startIndex=\"281\" selectedText=\"Xx [XXXX XX XXXXXX] Xxxxxxxx Xxxxxxx Xxxxxxxx Xxxxxxx (“XXX”) xxxxxxxxxxx xxxxxx xxx \r\n[XXXX XX XXXXXXXXXXXX XXXXXX]&#x20;\" textLength=\"121\" lineWidth=\"1\" dragDirection=\"br\" meta=\"\" label=\"\" saveDate=\"Wed Jun 8 2016\" saveTime=\"21:16:56 GMT+0000\" uuid=\"\" formUser=\"\" created=\"Wed Jun 8 21:16:41 GMT+0000 2016\" modified=\"Wed Jun 8 21:16:41 GMT+0000 2016\"><![CDATA[]]></annotation></page></pages><highlights /></documentAnnotations>"
        // "data":"<?xml version=\"1.0\"?><documentAnnotations><pages><page id=\"1\" pageWidth=\"612\" pageHeight=\"792\"><annotation nodeId=\"EBD6F076-096F-708E-A719-5A6CBBAE4AF4\" lineColor=\"\" interactionMode=\"0\" stampSize=\"58\" opacity=\"0\" fillColor=\"\" x=\"234.5453373286502\" y=\"7.019408280963191\" x_percent=\"0.3832440152428925\" y_percent=\"0.008862889243640393\" height_percent=\"0.09559512652296158\" width_percent=\"0.2\" height=\"75.71134020618557\" width=\"122.4\" drawType=\"signature_text\" fontWidth=\"24.90625\" fontHeight=\"14.4375\" lineWidth=\"0\" dragDirection=\"br\" meta=\"\" label=\"\" saveDate=\"Mon Jul 2 2018\" saveTime=\"9:57:12 GMT+0000\" uuid=\"\" formUser=\"\" created=\"Mon Jul 2 9:57:8 GMT+0000 2018\" modified=\"Mon Jul 2 9:57:8 GMT+0000 2018\"><![CDATA[<TEXTFORMAT LEADING=\"2\"><P ALIGN=\"center\" ><FONT FACE=\"Dancing Script\" SIZE= \"58\" COLOR=\"#000000\"  LETTERSPACING=\"0\" KERNING=\"0\" >Sejal</FONT></P></TEXTFORMAT>]]></annotation></page><page id=\"2\" pageWidth=\"612\" pageHeight=\"792\"></page><page id=\"3\" pageWidth=\"612\" pageHeight=\"792\"></page><page id=\"4\" pageWidth=\"612\" pageHeight=\"792\"></page><page id=\"5\" pageWidth=\"0\" pageHeight=\"0\"></page></pages><highlights /></documentAnnotations>"

    </script>

</body>
</html>

