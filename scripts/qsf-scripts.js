;(function($, $J) {

    var INTERACTION_LEVEL = 1;
    var TRANSITION_DURATION = 300;
    var ALL_CONTROLS = "All Controls";
    var ANIMATION_TYPE = "easeOutQuad";

    var $aboutDemoWrapper;
    var $codeViewerWrapper;
    var cachedSelectedNode;
    var controlName;
    var $nav;
    var $switchButton;
    var $activator;
    var $sidebar;
    var $animationContainer;
    var $skinChooser;

    $(function() {
        $skinChooser = $("#skin-chooser");

        $nav = $("#nav");
        $switchButton = $("#nav-switch > span");

        $sidebar = $("#sidebar");

        $activator = $skinChooser.find(".sc-activator");
        $animationContainer = $skinChooser.find("div.animation-container");

        refreshTimer();
        colorizeBackground();
        $("#nav-switch").click(animateNavigationPanel);
        $("#isolate-demo").click(showInstructionsInExternalWindow);
        $("#sidebar-toggler").click(toggleSideBar);
        $activator.click(toggleSkinChooser);
    });

    function animateNavigationPanel(e) {
        var transitionProperty = {
            "margin-left": "0px"
        };

        if ($nav.hasClass("root-nav-active")) {
            transitionProperty["margin-left"] = "-100%";
            $switchButton.text(ALL_CONTROLS);
        } else {
            $switchButton.text(controlName);
        }

        $switchButton.toggleClass( "back-nav forward-nav" );

        $nav.find(".nav-wrap")
            .transition(transitionProperty, TRANSITION_DURATION, ANIMATION_TYPE, function() {
                $nav.toggleClass("root-nav-active");
            });
    }

    function showInstructionsInExternalWindow(e) {
        var winSize = ",width=" + (screen.availWidth - 300).toString() + ",height=" + (screen.availHeight - 300).toString();
        window.open(qsfInstructionsPage, "codeViewer", "status=0,toolbar=0,scrollbars=1,resizable=1" + winSize);
    }

    function colorizeBackground(skinName) {
        $("#example")
            .removeClass()
            .addClass("background-" + $("#skin-chooser .sc-current").text().toLowerCase());
    }

    function toggleSkinChooser(skinName) {
        var transitionProperty = {
            "height": 0
        };

        $animationContainer
           .stopTransition();

        if (!$activator.hasClass("active")) {
            transitionProperty.height = $animationContainer
                .css("height", "auto").outerHeight();

            $animationContainer.css("height", "0");
            $skinChooser.css("z-index", "10000");
        }

        $animationContainer
            .transition(transitionProperty, TRANSITION_DURATION, ANIMATION_TYPE, function() {
                if ($activator.hasClass("active")) {
                    $skinChooser.css("z-index", "");
                }
                $activator.toggleClass("active");
            });

    }

    function toggleSideBar() {
        var transitionProperty = {
            "margin-left": 0
        };

        if ($sidebar.data("expanded")) {
            transitionProperty["margin-left"] = -300;
        }

        $sidebar.transition(transitionProperty, TRANSITION_DURATION, ANIMATION_TYPE, function() {
            $sidebar.data("expanded") ? $sidebar.removeData("expanded") : $sidebar.data("expanded", true);
        });

    }

    //#region DBReset Section

    function refreshTimer() {
        var resetNotice = $get('qsfDbResetNotice');

        if (!resetNotice) {
            return;
        }

        var timeout = resetNotice.getElementsByTagName("strong")[0];
        var remainingTime = {
            h: /(\d+) hour/gi,
            m: /(\d+) minute/gi,
            s: /(\d+) second/gi
        };

        var initialValue = timeout.firstChild.nodeValue;
        remainingTime.h = remainingTime.h.exec(initialValue);
        remainingTime.m = remainingTime.m.exec(initialValue);
        remainingTime.s = remainingTime.s.exec(initialValue);

        for (var i in remainingTime) { 
            remainingTime[i] = remainingTime[i] ? remainingTime[i][1] : 0; 
        }

        var tickInterval = null;
        var tick = function() {

            var timeFormatter = [];

            --remainingTime.s;

            if (remainingTime.s < 0) {
                --remainingTime.m;

                if (remainingTime.m < 0) {
                    --remainingTime.h;

                    if (remainingTime.h < 0) {
                        clearInterval(tickInterval);
                        window.location.href = window.location.href;
                        return;
                    }

                    remainingTime.m = 59;
                }

                remainingTime.s = 59;
            }

            if (remainingTime.h > 0) {
                timeFormatter[timeFormatter.length] = remainingTime.h;
                timeFormatter[timeFormatter.length] = remainingTime.h > 1 ? " hours, " : " hour, ";
            }

            if (remainingTime.m > 0) {
                timeFormatter[timeFormatter.length] = remainingTime.m;
                timeFormatter[timeFormatter.length] = remainingTime.m > 1 ? " minutes, " : " minute, ";
            }

            timeFormatter[timeFormatter.length] = remainingTime.s;
            timeFormatter[timeFormatter.length] = remainingTime.s > 1 ? " seconds" : " second";

            timeout.innerHTML = timeFormatter.join("");
        };

        tickInterval = setInterval(tick, 1000);
    }

    //#endregion

    //#region CodeViewer Section

    window.toolBarDemoSource_onClientButtonClicking = function(sender, args) {
        var fileName = args.get_item().get_text();
        var filePath = args.get_item().get_value();
        var codeContainerName = fileName.replace(/\./g, "-");
        var item = args.get_item();
        var parent = item.get_parent();
        var $codeListings = $codeViewerWrapper.find("#code-listing");
        var $contentPage = $codeListings.find("." + codeContainerName);
        var elementToSelect = item.get_element();

        if (!(parent instanceof Telerik.Web.UI.RadToolBar)) {
            parent.set_text(item.get_text());
            elementToSelect = parent.get_element();
        }

        $(sender.get_element())
            .find(".rtbItemSelected")
            .removeClass("rtbItemSelected")
            .end()
            .find(elementToSelect)
            .addClass("rtbItemSelected");

        if ($contentPage.length > 0) {
            $codeListings.children().hide();
            $contentPage.show();
        }
        else {
            $.ajax({
                type: "POST",
                url: qsfViewSourceService + "/GetFileContent",
                data: $J.serialize({
                    path: filePath,
                    fileName: fileName
                }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function(result) {
                    result = result.d || result;
                    $codeListings.children().hide();
                    $("<div></div>")
                        .html(result)
                        .addClass(codeContainerName)
                        .appendTo($codeListings);
                    SyntaxHighlighter.highlight();
                }
            });
        }
    };

    window.actionButtons_OnClientLoad = function(sender) {
        var selectedIndex = sender.get_selectedIndex();

        $aboutDemoWrapper = $("#about-demo-wrapper");
        $codeViewerWrapper = $("#code-viewer-wrapper");

        if (selectedIndex > 0) {
            $aboutDemoWrapper.hide();
        } else {
            $codeViewerWrapper.hide();
        }
    };

    window.actionButtons_OnClientTabSelected = function(sender, args) {
        var tab = args.get_tab();
        var $toolbar = $(".RadToolBar");
        var toolbar = $find($toolbar[0].id);

        if (tab.get_navigateUrl()) {
            return;
        }

        $aboutDemoWrapper.toggle();
        $codeViewerWrapper.toggle(); 

        toolbar.repaint();
    };

    //#endregion

    //#region LeftSideNavigation Section

    window.controlDemos_OnClientLoad = function(sender) {
        controlName = sender.get_nodes().getNode(0).get_text();
        $(sender.get_element()).find(".rtMinus .rtPlus").on("touchend", function(e) {
            e.preventDefault();
        });

    };

    window.controlDemos_OnClientNodeClicked = function(sender, args) {
        var node = args.get_node();
        var $element = $(node.get_element());
        var $selectedNodeElement;
     
        if (node.get_nodes().get_count() > 0 && cachedSelectedNode) {
            $element.find('div.rtSelected').removeClass("rtSelected");
            $selectedNodeElement = $(cachedSelectedNode.get_element());
            $selectedNodeElement.find('div').addClass("rtSelected");
        }
    };

    window.controlDemos_OnClientNodeClicking = function (sender, args) {
        var node = args.get_node();
      
        if (node.get_nodes().get_count() > 0 && node.get_level() === INTERACTION_LEVEL) {
            node.toggle();
            args.set_cancel(true);
        }
        
        if (!cachedSelectedNode) {
            cachedSelectedNode = sender.get_selectedNode();
        }
    };

    window.controlDemos_OnClientNodeCollapsing = function(sender, args) {
        if (args.get_node().get_level() !== INTERACTION_LEVEL) {
            args.set_cancel(true);
        }
    };

    //#endregion

    //#region SyntaxHighlighter

    window.SyntaxHighlighter.defaults['toolbar'] = false;
    window.SyntaxHighlighter.defaults['quick-code'] = false;
    window.SyntaxHighlighter.defaults['gutter'] = false;
    window.SyntaxHighlighter.all();

    //#endregion

    //#region Browser sniffing

    $(function() {

        var BrowserInfo = Telerik.Web.Browser;

        $.each( ["chrome", "ff", "ie", "opera", "safari"], function() {
            if ( BrowserInfo[this] ) {
                $("html").addClass( String.format( "t-{0} t-{0}{1}", this, BrowserInfo.version ) );
            }
        })

    });

    //#endregion

    //#region Retain iframe styles the same as in QSF
    $(function () {
        var mergeValues = [];
        // If additional styles need to be merged, populate the CSS property in this array.
        var fontProperties = ["font-family", "font-size"];

        var dBody = $(document).find("body");

        $.each(fontProperties, function (index, item) {
            mergeValues.push({
                style: item,
                value: dBody.css(item)
            })
        });

        $(".demo-container iframe").each(function (i, item) {
            $(item).load(function (ev) {
                $.each(mergeValues, function (index, styleRepository) {
                    try {
                        $(item.contentDocument).find("body")
                            .css(styleRepository.style, styleRepository.value);
                    } catch (e) {

                    }
                })
            })          
        });
    });

    //#endregion

})($telerik.$, Sys.Serialization.JavaScriptSerializer);