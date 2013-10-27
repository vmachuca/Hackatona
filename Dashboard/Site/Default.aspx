<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01//EN" "http://www.w3.org/TR/html4/strict.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head>

    <title>Eu Ando de Ônibus</title>
    <meta http-equiv="Content-Type" content="text/html; charset=iso-8859-1" />
    <meta http-equiv="X-UA-Compatible" content="IE=7,IE=9" />
    <meta name="viewport" content="initial-scale=1, maximum-scale=1,user-scalable=no" />

    <!-- JS -->
    
    <!--Frameworks-->
    
    <script type="text/javascript" src="resources/js/framework/jquery-1.8.3.js" ></script>
    <script type="text/javascript" src="resources/js/framework/jquery-ui-1.9.1.js" ></script>
    <script type="text/javascript" src="resources/js/framework/jquery-ui.combobox.js"></script>
    <script type="text/javascript" src="resources/js/framework/jquery.ui.touch-punch.js"  ></script>
    <script type="text/javascript" src="resources/js/framework/jquery.zdialog.js" ></script>
    <script type="text/javascript" src="resources/js/framework/jquery.chosen.js" ></script>
    <script type="text/javascript" src="resources/js/framework/jquery.mousewheel.js"></script>
    <script type="text/javascript" src="resources/js/framework/jquery.timespinner.js"></script>
    <script type="text/javascript" src="resources/js/framework/jquery.timer.js"></script>
    <script type="text/javascript" src="resources/js/framework/globalize.js"></script>
    <script type="text/javascript" src="resources/js/framework/globalize.culture.pt-BR.js"></script>
    
    <script type="text/javascript" charset="iso-8859-1" src="resources/js/Framework/heatmap.js"></script>
    <script type="text/javascript" charset="iso-8859-1" src="resources/js/Framework/heatmap-arcgis.js"></script>

    <script src="http://js.arcgis.com/3.7/"></script>
    <script type="text/javascript">
        var dojoConfig = { isDebug: true, parseOnLoad: true };
    </script>

    <script src="resources/js/highcharts/highcharts.js" type="text/javascript" ></script>
    <script src="Resources/js/highcharts/modules/exporting.js" type="text/javascript" ></script>

    <script type="text/javascript" charset="iso-8859-1" src="resources/js/layout/layout.utils.js" ></script>
    <script type="text/javascript" charset="iso-8859-1" src="resources/js/layout/layout.windows.js" ></script>

    <script type="text/javascript" charset="iso-8859-1" src="resources/js/business/core.bl.avaliacao.js"  ></script>
    <script type="text/javascript" charset="iso-8859-1" src="resources/js/business/core.bl.avl.js"  ></script>

    <script type="text/javascript" charset="iso-8859-1" src="resources/js/core.web.system.js"  ></script>
    <script type="text/javascript" charset="iso-8859-1" src="resources/js/core.utils.js" ></script>

    <script type="text/javascript" charset="iso-8859-1" src="resources/js/core.web.utils.js"></script>
    <script type="text/javascript" charset="iso-8859-1" src="resources/js/core.main.js"></script>

    <!-- End JS -->

    <!-- CSS -->

    <!--Frameworks-->

    <link rel="stylesheet" type="text/css" href="resources/css/esri.css" />
    <link rel="stylesheet" type="text/css" href="resources/css/application.css" />
    <link rel="stylesheet" type="text/css" href="resources/css/application-icons.css" />

    <link rel="stylesheet" type="text/css" href="resources/css/dijit.css" />
    <link rel="stylesheet" type="text/css" href="resources/css/social/twitter.css" />
    <link rel="stylesheet" type="text/css" href="resources/css/social/youtube.css" />
    <link rel="stylesheet" type="text/css" href="resources/css/social/flickr.css" />
    <link rel="stylesheet" type="text/css" href="resources/css/social/panoramio.css" />
    <link rel="stylesheet" type="text/css" href="resources/css/social/ushahidi.css" />

    <link rel="stylesheet" type="text/css" href="http://serverapi.arcgisonline.com/jsapi/arcgis/3.5/js/dojo/dijit/themes/claro/claro.css" />

    <link rel="stylesheet" type="text/css" href="resources/css/jquery-ui.css">
    <link rel="stylesheet" type="text/css" href="resources/css/jquery-ui-smoothness.css">
    <link rel="stylesheet" type="text/css" href="resources/css/jquery-chosen.css">
    <link rel="stylesheet" type="text/css" href="resources/css/windows.css" />

    <link rel="Shortcut Icon" href="resources/images/header/logo.ico" />
    <!--[if gte IE 9]>
    <link rel="stylesheet" type="text/css" href="css/ie9.css">
    <![endif]-->
    <!-- End CSS -->
</head>
<body class="modernGrey">
    <div id="map"></div>
	<div id="mapcon">
		
		<div id="zoomSlider"></div>
		<div id="containerHeatMap"></div>
		<div id="topMenuBar" style="display: block;">
            
			<div id="topMenuCon">
                <div id="topMenuLeftImage">
                    <img src="resources/images/header/spTransLogo.png" />
                </div>
				<div id="topMenuLeft">
					<h1 id="mapTitle" title="Eu Ando de Ônibus">Eu Ando de Ônibus</h1>
				</div>
                
				<div id="topMenuRight">
					<div id="menuList">
                        <span class="headerElement" ><div id="opacitySlider" style="width:100px"></div></span>
                        <span class="headerIconButton" onclick="avlBL.Window.AverageTime.Show()"><img src="resources/images/header/buttons/clock_24x24.png" /></span>
                        <span class="headerIconButton" onclick="avalBL.Window.Chart.Show()"><img src="resources/images/header/buttons/chart_24x24.png" /></span>
                        <span class="headerIconButton" onclick="utils.Window.HeatMap.Show()"><img src="resources/images/header/buttons/fire_24x24.png" /></span>
                        <span class="headerIconButton" onclick="utils.Window.About.Show()"><img src="resources/images/header/buttons/about_24x24.png" /></span>
                    </div>
					<div class="clear"></div>
				</div>
			</div>
		</div>
		<div id="settingsDialog"></div>
		<div id="alertDialog"></div>
		<div id="aboutDialog"></div>
        
	</div>
    <!-- End JavaScript -->
</body>
</html>
