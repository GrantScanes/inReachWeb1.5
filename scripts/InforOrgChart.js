$.widget("custom.inforOrgChart",
    {
        options: {
            id: "",
            data: {},
            allowMultiSelected: true,
            nodeClick: "",
            nodeTitleField: "",
            nodeTitleImage: "",
            nodeLine1Field: "",
            nodeLine2Field: "",
            nodeLine3Field: "",
            nodeLine1Title: "",
            nodeLine2Title: "",
            nodeLine3Title: "",
            renderNode: "",
            idField: "",
            parentIdField: "",
            parentId2Field: "",
            ShowZoomButtons: true,
            contextNodeid: null,
            vertical: false,
            allowSelect: true,
            allowRemoveNode: false
        },
        _el: null,
        _wrap: "",
        _outer: "",
        _content: "",
        _svgContainer: "",
        _tileWidth: 300,
        _tileHeight: 150,
        _spacebetweentiles: 100,
        _tilecss: "tile-wide",
        _curWidth: 0,
        _curHeight: 0,
        _MIN_SCALE: 0, // 1=scaling when first loaded
        _MAX_SCALE: 64,
        _elWidth: 0,
        _elHeight: 0,
        _viewportWidth: null,
        _viewportHeight: null,
        _scale: null,
        _lastScale: null,
        _x: 0,
        _lastX: 0,
        _y: 0,
        _lastY: 0,
        _pinchCenter: null,
        _restrictScale: function(scale) {
            if (scale < this._MIN_SCALE) {
                scale = this._MIN_SCALE;
            } else if (scale > this._MAX_SCALE) {
                scale = this._MAX_SCALE;
            }
            return scale;
        },
        _absolutePosition: function(el) {
            var x = 0,
                y = 0;
            while (el !== null) {
                x += el.offsetLeft;
                y += el.offsetTop;
                el = el.offsetParent;
            }
            return { x: x, y: y };
        },
        _updateLastPos: function(deltaX, deltaY) {
            this._lastX = this._x;
            this._lastY = this._y;
        },
        _translate: function(deltaX, deltaY) {
            // We restrict to the min of the viewport width/height or current width/height as the
            // current width/height may be smaller than the viewport width/height
            var newX = this._lastX + deltaX;
            //var newX = this._restrictRawPos(this._lastX + deltaX / this._scale,
            //    Math.min(this._viewportWidth, this._curWidth), this._elWidth);
            this._x = newX;
            //$(this._content)[0].style.top = Math.ceil(newX ) + "px";
            $(this._content)[0].style.marginLeft = Math.ceil(newX) + "px";
            var newY = this._lastY + deltaY;
            //var newY =var newY = this._restrictRawPos(this._lastY + deltaY / this._scale,
            //   Math.min(this._viewportHeight, this._curHeight), this._elHeight);
            this._y = newY;
            $(this._content)[0].style.marginTop = Math.ceil(newY) + "px";
            //$(this._content)[0].style.left = Math.ceil(newY ) + "px";
            //$(this._content)[0].style.position = "absolute";
            //this._content.css({ top: top, left: left, position: "absolute" });
        },
        _restrictRawPos: function(pos, viewportDim, imgDim) {
            if (pos < viewportDim / this._scale - imgDim) { // too far left/up?
                pos = viewportDim / this._scale - imgDim;
            } else if (pos > 0) { // too far right/down?
                pos = 0;
            }
            return pos;
        },
        _zoom: function(scaleBy) {
            var orgchart = this.element, self = this, o = this.options;
            self._scale = self._restrictScale(self._lastScale * scaleBy);
            self._curWidth = self._elWidth * self._scale;
            self._curHeight = self._elHeight * self._scale;
            //this._el.style.width = Math.ceil(this._curWidth) + "px";
            //this._el.style.height = Math.ceil(this._curHeight) + "px";
            $(self._content).css({ '-ms-transform': "scale(" + self._scale + ")" });
            $(self._content).css({ '-moz-transform': "scale(" + self._scale + ")" });
            $(self._content).css({ '-webkit-transform': "scale(" + self._scale + ")" });
            $(self._content).css({ 'transform': "scale(" + self._scale + ")" });
            $(this._outer).css({ width: this._viewportWidth, height: this._viewportHeight });
            //$(this._wrap).css({ width: this._viewportWidth * self._scale, height: this._viewportHeight * self._scale   });
            //$(self._svgContainer).css({ width: self._viewportWidth * self._scale, height: self._viewportHeight * self._scale });
            d3.select("#" + self._svgContainer.attr("id")).selectAll("svg").remove();
            d3.select("#" + self._svgContainer.attr("id")).append("svg").attr("id", self.options.id + "svg1").style("width", self._viewportWidth).style("height", self._viewportHeight);
            // Adjust margins to make sure that we aren't out of bounds
            self._translate(0, 0);
        },
        _updateLastScale: function() {
            this._lastScale = this._scale;
        },
        _zoomAround: function(scaleBy, rawZoomX, rawZoomY, doNotUpdateLast) {
            var orgchart = this.element, self = this, o = this.options;
            // Zoom
            self._zoom(scaleBy);
            // New raw center of viewport
            var rawCenterX = -self._x + Math.min(self._viewportWidth, self._curWidth) / 2 / self._scale;
            var rawCenterY = -self._y + Math.min(self._viewportHeight, self._curHeight) / 2 / self._scale;
            // Delta
            var deltaX = (rawCenterX - rawZoomX) * self._scale + self._tileWidth;
            var deltaY = (rawCenterY - rawZoomY) * self._scale + self._tileHeight;
            // Translate back to zoom center
            self._translate(deltaX, deltaY);
            if (!doNotUpdateLast) {
                self._updateLastScale();
                self._updateLastPos();
            }
        },
        _zoomCenter: function(scaleBy) {
            // Center of viewport
            var zoomX = -this._x + Math.min(this._viewportWidth, this._curWidth) / 2 / this._scale;
            var zoomY = -this._y + Math.min(this._viewportHeight, this._curHeight) / 2 / this._scale;
            this._zoomAround(scaleBy, zoomX, zoomY);
        },
        _zoomIn: function() {
            var orgchart = this.element, self = this, o = this.options;
            self._scale = 1 + self._lastScale;
            self._zoom(self._scale);
            self._updateLastScale();
            self._updateLastPos();
            self._drawlines();
        },
        _zoomOut: function() {
            var orgchart = this.element, self = this, o = this.options;
            self._scale = (1 / 2);
            this._zoom(self._scale);
            self._updateLastScale();
            self._updateLastPos();
            self._drawlines();
        },
        _toggleView: function() {
            var orgchart = this.element, self = this, o = this.options;
            o.vertical = !o.vertical;
            var btnToggleViewSpan = $("#toggleButtonSpan");
            if (o.vertical) {
                btnToggleViewSpan.removeClass("mif-flow-cascade");
                btnToggleViewSpan.addClass("mif-flow-tree");
            } else {
                btnToggleViewSpan.removeClass("mif-flow-tree");
                btnToggleViewSpan.addClass("mif-flow-cascade");
            }
            $(self._content).html("");
            self._drawTree(0, 0);
            self._resizeChart();
            self._drawlines();
        },
        _rawCenter: function(e) {
            var pos = this._absolutePosition(this._el);
            // We need to account for the scroll position
            var scrollLeft = window.pageXOffset ? window.pageXOffset : document.body.scrollLeft;
            var scrollTop = window.pageYOffset ? window.pageYOffset : document.body.scrollTop;
            var zoomX = -this._x + (e.center.x - pos.x + scrollLeft) / this._scale;
            var zoomY = -this._y + (e.center.y - pos.y + scrollTop) / this._scale;
            return { x: zoomX, y: zoomY };
        },
        _prepareData: function() {
            var orgchart = this.element, self = this, o = this.options;
            if (o.data.length === 0 || o.data.length === undefined)
                o.data = new Array();
            _.each(o.data,
                function(i) {
                    if (!i.hasOwnProperty("collapsed")) {
                        i.collapsed = false;
                    }
                    if (!i.hasOwnProperty("hidden")) {
                        i.hidden = false;
                    }
                    if (!i.hasOwnProperty("selected")) {
                        i.selected = false;
                    }
                });
        },
        _create: function() {
            var orgchart = this.element, self = this, o = this.options;
            var element = self.element;
            self._prepareData();
            self._wrap = $("#" + self.options.id);
            self._wrap.addClass("orgchartwrap");
            self._outer = $("<div/>").addClass("orgchartouter ").attr("id", self.options.id + "orgchartouter");
            $(self._outer)[0].style.marginTop = "50px";
            self._outer.appendTo(self._wrap);
            $(self._outer)[0].style.position = "absolute";
            self._content = $("<div/>").addClass("content").attr("id", self.options.id + "orgchart");
            $(self._content)[0].style.position = "absolute";
            self._content.appendTo(self._outer);
            $(self._content)[0].style.marginLeft = 0 + "px";
            $(self._content)[0].style.marginTop = 0 + "px";
            var btnContainer = $("<div/>");
            $(btnContainer)[0].style.float = "right";
            $(btnContainer)[0].style.opacity = "0.5";
            btnContainer.appendTo(self._wrap);
            var btnToggleView = $("<button/>").addClass("cycle-button").attr("id", "toggleButton");
            btnToggleView.appendTo(btnContainer);
            $(btnToggleView).click(function() { self._toggleView(); });
            var btnToggleViewSpan = $("<span/>").attr("id", "toggleButtonSpan");
            if (o.vertical) {
                btnToggleViewSpan.addClass("mif-flow-tree");
            } else {
                btnToggleViewSpan.addClass("mif-flow-cascade");
            }
            btnToggleViewSpan.appendTo(btnToggleView);
            if (o.ShowZoomButtons) {
                var zoomInbtn = $("<button/>").addClass("cycle-button");
                zoomInbtn.appendTo(btnContainer);
                $(zoomInbtn).click(function() { self._zoomIn(); });
                var zoomInbtnSpan = $("<span/>");
                zoomInbtnSpan.addClass("mif-zoom-in");
                zoomInbtnSpan.appendTo(zoomInbtn);
                var zoomOutbtn = $("<button/>").addClass("cycle-button");
                zoomOutbtn.appendTo(btnContainer);
                $(zoomOutbtn).click(function() { self._zoomOut(); });
                var zoomOutbtnSpan = $("<span/>");
                zoomOutbtnSpan.addClass("mif-zoom-out");
                zoomOutbtnSpan.appendTo(zoomOutbtn);
                var resetbtn = $("<button/>").addClass("cycle-button");
                resetbtn.appendTo(btnContainer);
                $(resetbtn).click(function() { self.reset(); });
                var resetbtnSpan = $("<span/>");
                resetbtnSpan.addClass("mif-shrink");
                resetbtnSpan.appendTo(resetbtn);
                if (o.contextNodeid !== undefined && o.contextNodeid !== null) {
                    var locationbtn = $("<button/>").addClass("cycle-button");
                    locationbtn.appendTo(btnContainer);
                    $(locationbtn).click(function() { self.zoomToNode(); });
                    var locationbtnSpan = $("<span/>");
                    locationbtnSpan.addClass("mif-location");
                    locationbtnSpan.appendTo(locationbtn);
                }
            }
            self._svgContainer = $("<div/>").addClass("svg-container ").attr("id", self.options.id + "svgContainer");
            self._svgContainer.prependTo(self._outer);
            var level = 0;
            self._drawTree(0, level);
            element.data("inforOrgChart", self);
            self._el = $(self._outer)[0];
            self._resizeChart();
            var hammer = new Hammer(self._el,
                {
                    domEvents: true,
                    prevent_default: true
                });
            hammer.add(new Hammer.Pinch({ threshold: 0, pointers: 0 }));
            hammer.get("pinch").set({ enable: true });
            hammer.on("pan",
                function(e) {
                    self._translate(e.deltaX, e.deltaY);
                    self._drawlines();
                });
            hammer.on("panend",
                function(e) {
                    self._updateLastPos();
                    //self._drawlines();
                });
            hammer.on("pinch",
                function(e) {
                    // We only calculate the pinch center on the first pinch event as we want the center to
                    // stay consistent during the entire pinch
                    if (self._pinchCenter === null) {
                        self._pinchCenter = self._rawCenter(e);
                        var offsetX = self._pinchCenter.x * self._scale - (-self._x * self._scale + Math.min(self._viewportWidth, self._curWidth) / 2);
                        var offsetY = self._pinchCenter.y * self._scale - (-self._y * self._scale + Math.min(self._viewportHeight, self._curHeight) / 2);
                        self._pinchCenterOffset = { x: offsetX, y: offsetY };
                    }
                    // When the user pinch zooms, she/he expects the pinch center to remain in the same
                    // relative location of the screen. To achieve self, the raw zoom center is calculated by
                    // first storing the pinch center and the scaled offset to the current center of the
                    // image. The new scale is then used to calculate the zoom center. self has the effect of
                    // actually translating the zoom center on each pinch zoom event.
                    var newScale = self._restrictScale(self._scale * e.scale);
                    var zoomX = self._pinchCenter.x * newScale - self._pinchCenterOffset.x;
                    var zoomY = self._pinchCenter.y * newScale - self._pinchCenterOffset.y;
                    var zoomCenter = { x: zoomX / newScale, y: zoomY / newScale };
                    self._zoom(e.scale);
                    self._drawlines();
                });
            hammer.on("pinchend",
                function(e) {
                    self._updateLastScale();
                    self._updateLastPos();
                    self._pinchCenter = null;
                    self._drawlines();
                });
            hammer.on("doubletap",
                function(e) {
                    var c = self._rawCenter(e);
                    self._zoomAround(2, c.x, c.y);
                    self._drawlines();
                });
        },
        _repositionSiblings: function(id, fromLeft, diff, level) {
            var orgchart = this.element, self = this, o = this.options;
            var siblings = $(self._content).find("[data-level=" + level + "]");
            _.each(siblings,
                function(s) {
                    if (o.vertical) {
                        var siblingTop = parseInt($(s).attr("data-top"));
                        if (siblingTop >= fromLeft && id !== $(s).attr("id")) {
                            var snewtop = siblingTop + diff;
                            $(s).css({ top: snewtop });
                            $(s).attr("data-top", snewtop);
                        }
                    } else {
                        var siblingLeft = parseInt($(s).attr("data-left"));
                        if (siblingLeft >= fromLeft && id !== $(s).attr("id")) {
                            var snewleft = siblingLeft + diff;
                            $(s).css({ left: snewleft });
                            $(s).attr("data-left", snewleft);
                        }
                    }
                });
        },
        _repositionParents: function(item) {
            var orgchart = this.element, self = this, o = this.options;
            var it = item;
            var min = 0;
            var max = 0;
            var parentInData = _.find(self.options.data,
                function(i) {
                    return i[self.options.idField] === item[self.options.parentIdField];
                });
            while (parentInData !== undefined) {
                var parentTile = $("#" + self.options.id + parentInData[self.options.idField]);
                min = 0;
                max = 0;
                var children = $(self._content).find("[data-parentid=" + parentInData[self.options.idField] + "]");
                children = _.filter(children, function(itm) { return $(itm).attr("data-collapsed") === "false"; });
                var childrenlength = children.length;
                _.each(children,
                    function(s, ind) {
                        var siblingtile = $(s);
                        if (o.vertical) {
                            if (ind === 0) {
                                min = parseInt(siblingtile.attr("data-top"));
                            }
                            if (siblingtile.attr("data-top") < min) {
                                min = parseInt(siblingtile.attr("data-top"));
                            }
                            if (siblingtile.attr("data-top") > max) {
                                max = parseInt(siblingtile.attr("data-top"));
                            }
                        } else {
                            if (ind === 0) {
                                min = parseInt(siblingtile.attr("data-left"));
                            }
                            if (siblingtile.attr("data-left") < min) {
                                min = parseInt(siblingtile.attr("data-left"));
                            }
                            if (siblingtile.attr("data-left") > max) {
                                max = parseInt(siblingtile.attr("data-left"));
                            }
                        }
                    });
                var newtop;
                var oldparenttop;
                var newleft;
                var oldparentleft;
                var dif;
                if (childrenlength > 1) {
                    if (o.vertical) {
                        newtop = (min);
                        oldparenttop = parseInt(parentTile.attr("data-top"));
                        dif = newtop - oldparenttop;
                        //parentTile.css({ top: newtop });
                        //parentTile.attr("data-top", newtop);
                        self._repositionSiblings(parentTile.attr("id"), newtop, dif, parseInt(parentTile.attr("data-level")));
                    } else {
                        newleft = (min + this._tileWidth) + ((max - (min + self._tileWidth)) / 2) - (self._tileWidth / 2);
                        oldparentleft = parseInt(parentTile.attr("data-left"));
                        dif = newleft - oldparentleft;
                        parentTile.css({ left: newleft });
                        parentTile.attr("data-left", newleft);
                        self._repositionSiblings(parentTile.attr("id"), newleft, dif, parseInt(parentTile.attr("data-level")));
                    }
                } else {
                    var tile = $("#" + self.options.id + it[this.options.idField]);
                    var levelsibs = _.filter($(self._content).find("[data-parentid=" + it[self.options.parentIdField] + "]"), function(itm) { return $(itm).attr("data-collapsed") === "false"; }).length;
                    if (levelsibs === 1) {
                        if (o.vertical) {
                            if (parseInt(tile.attr("data-top")) !== parseInt(parentTile.attr("data-top"))) {
                                newtop = parseInt(parentTile.attr("data-top")) + (self._tileWidth / 2);
                                oldparenttop = parseInt(parentTile.attr("data-top"));
                                dif = newtop - oldparenttop;
                                //parentTile.css({ top: newtop});
                                //parentTile.attr("data-top", newtop);
                                self._repositionSiblings(parentTile.attr("id"), newtop, dif, parseInt(parentTile.attr("data-level")));
                            }
                        } else {
                            if (parseInt(tile.attr("data-left")) !== parseInt(parentTile.attr("data-left"))) {
                                newleft = parseInt(tile.attr("data-left"));
                                oldparentleft = parseInt(parentTile.attr("data-left"));
                                dif = newleft - oldparentleft;
                                parentTile.css({ left: newleft });
                                parentTile.attr("data-left", newleft);
                                self._repositionSiblings(parentTile.attr("id"), newleft, dif, parseInt(parentTile.attr("data-level")));
                            }
                        }
                    }
                }
                it = parentInData;
                parentInData = _.find(self.options.data,
                    function(i) {
                        return i[self.options.idField] === parseInt(parentTile.attr("data-parentid"));
                    });
            }
        },
        _removeNode: function(id, parentId) {
            var orgchart = this.element, self = this, o = this.options;
            
            var parent = _.find(o.data, function (i) { return parseInt(i[self.options.idField]) === parseInt(parentId) });
            var children = _.filter(o.data, function (i) { return parseInt(i[self.options.parentIdField]) === parseInt(id) });
            _.each(children,
                function (item) {
                    item[self.options.parentIdField] = parent[self.options.idField];
                });
            o.data = _.reject(o.data, function (i) { return parseInt(i[self.options.idField]) === parseInt(id) });
            $(self._content).html("");
            self._drawTree(0, 0);
            self._resizeChart();
            $(this._content)[0].style.marginLeft = 0 + "px";
            $(this._content)[0].style.marginTop = 0 + "px";
            self._drawlines();
        },
        _toggleChildren: function(parentid, collapsed) {
            var orgchart = this.element, self = this, o = this.options;
            var parent = _.find(o.data, function(i) { return parseInt(i[self.options.idField]) === parentid });
            var children = _.filter(o.data, function(i) { return parseInt(i[self.options.parentIdField]) === parentid });
            _.each(children,
                function(item) {
                    item.hidden = collapsed;
                    if (parent.collapsed) {
                        item.hidden = true;
                    }
                    self._toggleChildren(item[self.options.idField], collapsed);
                });
        },
        _toggleExpandCollapse: function(nodeid) {
            var orgchart = this.element, self = this, o = this.options;
            var icimage = $("#ecimage" + nodeid);
            var collapsed = false;
            if ($(icimage).hasClass("mif-minus") === true) {
                collapsed = true;
            }
            var dataitem = _.find(o.data, function(itm) { return parseInt(itm[self.options.idField]) === parseInt(nodeid); });
            dataitem.collapsed = collapsed;
            self._toggleChildren(nodeid, collapsed);
            $(self._content).html("");
            self._drawTree(0, 0);
            self._resizeChart();
            $(this._content)[0].style.marginLeft = 0 + "px";
            $(this._content)[0].style.marginTop = 0 + "px";
            self._drawlines();
        },
        _deSelectNodes: function() {
            var orgchart = this.element, self = this, o = this.options;
        },
        _nodeClicked: function(item) {
            var orgchart = this.element, self = this, o = this.options;
            var tile = $("#" + self.options.id + item[self.options.idField]);
            if (o.allowSelect) {
                if (tile.hasClass("element-selected")) {
                    tile.removeClass("element-selected");
                } else {
                    if (!o.allowMultiSelected) {
                        self._content.find("." + self._tilecss).removeClass("element-selected");
                        _.each(o.data, function(sel) { sel.selected = false; });
                    }
                    tile.addClass("element-selected");
                    _.each(o.data, function(sel) { if (sel[self.options.idField] === item[self.options.idField]) sel.selected = true; });
                }
            }
            var result = 0;
            if (typeof o.nodeClick === "function") {
                result = o.nodeClick(item);
            } else {
                if (typeof window[o.nodeClick] === "function") {
                    result = window[o.nodeClick](item);
                } else {
                    var f = eval("(function(){" + o.nodeClick + "})");
                    result = f.call(item);
                }
            }
            return result;
        },
        _renderNode: function(tile, data) {
            var orgchart = this.element, self = this, o = this.options;
            var result = 0;
            if (typeof o.renderNode === "function") {
                result = o.renderNode(tile, data);
            } else {
                if (typeof window[o.renderNode] === "function") {
                    result = window[o.renderNode](tile, data);
                } else {
                    var f = eval("(function(){" + o.renderNode + "})");
                    result = f.call(tile, data);
                }
            }
            return result;
        },
        _drawTile: function(top, left, item, level) {
            var orgchart = this.element, self = this, o = this.options;
            if (o.vertical) {
            }
            if (left < 0) left = 0;
            var tile = $("<div/>").addClass(this._tilecss).attr("id", self.options.id + item[self.options.idField]);
            if (item.selected) {
                tile.addClass("element-selected");
            }
            tile.css({ top: top, left: left, position: "absolute" });
            tile.attr("data-left", left);
            tile.attr("data-top", top);
            tile.attr("data-parentid", item[self.options.parentIdField]);
            tile.attr("data-level", level);
            tile.attr("data-collapsed", false);
            tile.attr("data-dataid", item[self.options.idField]);
            tile.appendTo(this._content);
            tile.click(function() { self._nodeClicked(item); });
            self._renderNode(tile, item);
            var tilecontent = $("<div/>");
            tilecontent.addClass("tile-content");
            tilecontent.appendTo(tile);
            if (o.nodeTitleImage !== undefined && o.nodeTitleImage.length > 0) {
                var img = $("<img/>");
                img.attr("src", item[o.nodeTitleImage]);
                img.css({ width: "27px", height: "27px" });
                img.appendTo(tilecontent);
            }
            var span = $("<span/>");
            span.html("<b>" + item[o.nodeTitleField] + "</b> </br></br>");
            span.addClass("orgchartTileSpans");
            span.appendTo(tilecontent);
            if (item[o.nodeLine1Field] !== undefined) {
                var span1 = $("<span/>");
                span1.html("<b>" + o.nodeLine1Title + "</b> " + item[o.nodeLine1Field].trunc(30, false) + "  </br></br>");
                span1.addClass("orgchartTileSpans");
                span1.appendTo(tilecontent);
            }
            if (item[o.nodeLine2Field] !== undefined) {
                var span2 = $("<span/>");
                span2.html("<b>" + o.nodeLine2Title + "</b> " + item[o.nodeLine2Field].trunc(30, false)  + "  </br></br>");
                span2.addClass("orgchartTileSpans");
                span2.appendTo(tilecontent);
            }
            if (item[o.nodeLine3Field] !== undefined) {
                var span3 = $("<span/>");
                span3.html("<b>" + o.nodeLine3Title + "</b> " + item[o.nodeLine3Field].trunc(30, false)  + "  </br></br>");
                span3.addClass("orgchartTileSpans");
                span3.appendTo(tilecontent);
            }
            if (self.options.allowRemoveNode) {
                var removeButton = $("<button/>").addClass("cycle-button");
                removeButton.appendTo(tilecontent);
                $(removeButton).click(function () { self._removeNode(item[self.options.idField], item[self.options.parentIdField]); });
                removeButton.css({ top:  (self._tileHeight) - 39, left: (self._tileWidth ) - 40, position: "absolute" });
                var removeButtonSpan = $("<span/>");
                removeButtonSpan.addClass("mif-cross").attr("id", "removeimage" + item[self.options.idField]);
                removeButtonSpan.appendTo(removeButton);
            }
            var children = _.find(o.data, function(c) { return parseInt(c[self.options.parentIdField]) === parseInt(item[self.options.idField]) });
            if (children !== undefined) {
                var expandCollapseToggle = $("<button/>").addClass("cycle-button");
                expandCollapseToggle.appendTo(tilecontent);
                $(expandCollapseToggle).click(function() { self._toggleExpandCollapse(item[self.options.idField]); });
                expandCollapseToggle.css({ top: (self._tileHeight) - 39, left: (self._tileWidth / 2) - 12, position: "absolute" });
                var expandCollapseSpanToggle = $("<span/>");
                if (item.collapsed) {
                    expandCollapseSpanToggle.addClass("mif-plus").attr("id", "ecimage" + item[self.options.idField]);
                    expandCollapseSpanToggle.appendTo(expandCollapseToggle);
                    //expandCollapseSpanToggle.css({ width: "2.125rem" });
                    //expandCollapseSpanToggle.css({ height: "2.125rem" });
                } else {
                    expandCollapseSpanToggle.addClass("mif-minus").attr("id", "ecimage" + item[self.options.idField]);
                    expandCollapseSpanToggle.appendTo(expandCollapseToggle);
                    //expandCollapseSpanToggle.css({ width: "2.125rem" });
                    //expandCollapseSpanToggle.css({ height: "2.125rem" });
                }
            }
            //var dropTop = $("<div/>");
            //dropTop.addClass("dropTop");
            //dropTop.attr("id", self.options.id + item[self.options.idField] + "dropTop");
            ////dropTop.css("visibility", "hidden");
            //dropTop.css("position", "absolute");
            //dropTop.css("z-index", "1000000");
            //dropTop.attr("data-dataid", item[self.options.idField]);
            //dropTop.css({ top: 0, left: 0, height: self._tileHeight / 2, width: "100%"});
            //dropTop.appendTo(tilecontent);
            //var dropBottom = $("<div/>");
            //dropBottom.addClass("dropBottom");
            //dropBottom.attr("id", self.options.id + item[self.options.idField] + "dropBottom");
            ////dropBottom.css("visibility", "hidden");
            //dropBottom.css("position", "absolute");
            //dropBottom.css("z-index", "1000000");
            //dropBottom.attr("data-dataid", item[self.options.idField]);
            //dropBottom.css({ top: self._tileHeight / 2, left: "0", height: self._tileHeight / 2, width: "100%" });
            //dropBottom.appendTo(tilecontent);
        },
        _drawlines: function() {
            var orgchart = this.element, self = this, o = this.options;
            var svg = d3.select("#" + self.options.id + "svg1");
            svg.selectAll("path").remove();
            _.each(o.data,
                function(i) {
                    var tile = $("#" + self.options.id + i[o.idField]);
                    if ($(tile).attr("data-collapsed") === "false") {
                        var path = document.createElementNS("http://www.w3.org/2000/svg", "path"),
                            $path = $(path);
                        $path.attr("id", self.options.id + "path" + i[o.idField])
                            .attr("d", "M0 0")
                            .attr("stroke", "#686868")
                            .attr("fill", "none")
                            .attr("stroke-width", "2px");
                        $("#" + self.options.id + "svg1").append($path);
                        if (i[o.parentIdField] > 0) {
                            connectElements($("#" + self.options.id + "svgContainer"), $("#" + self.options.id + "svg1"), $("#" + self.options.id + "path" + i[o.idField]), $("#" + self.options.id + i[o.idField]), $("#" + self.options.id + i[o.parentIdField]), self.options.vertical);
                        }
                    }
                });
            _.each(o.data,
                function(i) {
                    var tile = $("#" + self.options.id + i[o.idField]);
                    if ($(tile).attr("data-collapsed") === "false") {
                        var path = document.createElementNS("http://www.w3.org/2000/svg", "path"),
                            $path = $(path);
                        $path.attr("id", self.options.id + "pathsecond" + i[o.idField])
                            .attr("d", "M0 0")
                            .attr("stroke", "#FF3553")
                            .attr("fill", "none")
                            .attr("stroke-width", "2px");
                        $("#" + self.options.id + "svg1").append($path);
                        if (i.hasOwnProperty(o.parentId2Field)) {
                            if (i[o.parentId2Field].length > 0) {
                                _.each(i[o.parentId2Field],
                                    function(c) {
                                        if ($("#" + self.options.id + c).length > 0) {
                                            connectElements($("#" + self.options.id + "svgContainer"), $("#" + self.options.id + "svg1"), $("#" + self.options.id + "pathsecond" + i[o.idField]), $("#" + self.options.id + i[o.idField]), $("#" + self.options.id + c), self.options.vertical);
                                        }
                                    });
                            }
                        }
                    }
                });
        },
        _resizeChart: function() {
            var orgchart = this.element, self = this, o = this.options;
            var viewWidth = $(self._wrap).width();
            var viewHeight = $(self._wrap).height();
            self._viewportWidth = $(self._wrap).width();
            self._viewportHeight = $(self._wrap).height() - 50;
            var maxwidth = 0;
            var maxHeight = 0;
            _.each($("#" + o.id).find("." + self._tilecss),
                function(i) {
                    if ($(i).attr("data-collapsed") === "false") {
                        if ($(i).attr("data-left") > maxwidth) maxwidth = parseFloat($(i).attr("data-left"));
                        if ($(i).attr("data-top") > maxHeight) maxHeight = parseFloat($(i).attr("data-top"));
                    }
                });
            self._curwidth = maxwidth + (self._tileWidth) + self._spacebetweentiles;
            self._curheight = maxHeight + (self._tileHeight) + self._spacebetweentiles;
            self._scale = Math.min(self._viewportWidth / self._curwidth, self._viewportHeight / self._curheight);
            self._lastScale = 1;
            self._elWidth = self._elWidth + (self._tileWidth) + self._spacebetweentiles;
            self._elHeight = self._elHeight + (self._tileHeight) + self._spacebetweentiles;
            $(this._content)[0].style.marginLeft = 0 + "px";
            $(this._content)[0].style.marginTop = 0 + "px";
            self._zoom(self._scale);
            self._updateLastScale();
            self._updateLastPos();
            self._drawlines();
        },
        _drawTree: function(level, parentid) {
            var orgchart = this.element, self = this, o = this.options;
            var levelobjs = [];
            if (level === 0 && parentid === 0 && _.size(levelobjs) === 0) {
                _.each(o.data,
                    function(r) {
                        if (_.find(o.data,
                            function(i) {
                                return parseInt(i[self.options.idField]) === parseInt(r[self.options.parentIdField]);
                            }) === undefined)
                            levelobjs.push(r);
                    });
            } else {
                levelobjs = _.filter(o.data,
                    function(i) {
                        if (parseInt(i[self.options.parentIdField]) === null || parseInt(i[self.options.parentIdField]) === undefined)
                            i[self.options.parentIdField] = 0;
                        return parseInt(i[self.options.parentIdField]) === parentid;
                    });
            }
            var siblings = 1;
            levelobjs = _.sortBy(levelobjs, function(p) { return p[self.options.parentIdField]; });
            levelobjs = _.filter(levelobjs, function(p) { return p.hidden === false || p.hidden === undefined; });
            var previousTileLeft = 0;
            _.each(levelobjs,
                function(i, index) {
                    var parentTile = $("#" + self.options.id + parentid);
                    if (parentTile === undefined || parentTile === null || parentTile.length === 0) {
                        if (o.vertical) {
                            self._drawTile(siblings * self._spacebetweentiles + (index * self._tileHeight) - (self._tileHeight / 2), (level * self._tileWidth) + (level * self._spacebetweentiles), i, level);
                        } else {
                            self._drawTile((level * self._tileHeight) + (level * self._spacebetweentiles), siblings * self._spacebetweentiles + (index * self._tileWidth) - (self._tileWidth / 2), i, level);
                        }
                    } else {
                        if (o.vertical) {
                            var top = parentTile.data("top");
                            //if (siblings > 1) {
                            top = (top + self._tileHeight) + (self._spacebetweentiles);
                            //}
                            var maxsibling = 0;
                            _.each($(self._content).find("[data-level=" + level + "]"),
                                function(s, ind) {
                                    var siblingtile = $(s);
                                    if (siblingtile.attr("data-top") > maxsibling) {
                                        maxsibling = parseInt(siblingtile.attr("data-top"));
                                    }
                                });
                            if (maxsibling > 0 && top <= maxsibling + self._tileHeight) {
                                top = maxsibling + self._spacebetweentiles + self._tileHeight;
                            }
                            if (top <= 0) top = self._spacebetweentiles;
                            self._drawTile(top, (level * self._tileWidth) + (level * (self._spacebetweentiles / 2)), i, level);
                            self._repositionParents(i, $("#" + i[self.options.idField]));
                        } else {
                            var left = parentTile.data("left");
                            if (siblings > 1) {
                                left = (left + self._tileWidth) + (self._spacebetweentiles);
                            }
                            var maxsibling = 0;
                            _.each($(self._content).find("[data-level=" + level + "]"),
                                function(s, ind) {
                                    var siblingtile = $(s);
                                    if (siblingtile.attr("data-left") > maxsibling) {
                                        maxsibling = parseInt(siblingtile.attr("data-left"));
                                    }
                                });
                            if (maxsibling > 0 && left <= maxsibling + self._tileWidth) {
                                left = maxsibling + self._spacebetweentiles + self._tileWidth;
                            }
                            if (left <= 0) left = self._spacebetweentiles;
                            self._drawTile((level * self._tileHeight) + (level * (self._spacebetweentiles / 2)), left, i, level);
                            self._repositionParents(i, $("#" + i[self.options.idField]));
                        }
                    }
                    siblings = siblings + 1;
                });
            _.each(levelobjs,
                function(i) {
                    self._drawTree(level + 1, i[self.options.idField]);
                });
            if (level === 0) {
                var item = _.find(o.data, function(i) { return i[self.options.idField] === o.contextNodeid; });
                if (item !== undefined) {
                    self._nodeClicked(item);
                    var tile = $("#" + self.options.id + item[self.options.idField]);
                    tile.addClass("element-selected");
                }
            }
        },
        _getLargestId: function() {
            var orgchart = this.element, self = this, o = this.options;
            var max = _.max(o.data, function(i) { return i[self.options.idField]; });
            if (max === -Infinity)
                return 0;
            return max[self.options.idField];
        },
        resizeChart: function() {
            this._resizeChart();
            this._drawlines();
        },
        reset: function() {
            var orgchart = this.element, self = this, o = this.options;
            $(self._content).html("");
            self._drawTree(0, 0);
            self._resizeChart();
            $(this._content)[0].style.marginLeft = 0 + "px";
            $(this._content)[0].style.marginTop = 0 + "px";
            self._x = 0;
            self._y = 0;
            self._drawlines();
            self._updateLastScale();
            self._updateLastPos();
        },
        zoomToNode: function() {
            var orgchart = this.element, self = this, o = this.options;
            var item = _.find(o.data, function(i) { return i[self.options.idField] === o.contextNodeid; });
            if (item !== undefined) {
                self.reset();
                $(this._content)[0].style.marginLeft = 0 + "px";
                $(this._content)[0].style.marginTop = 0 + "px";
                var top = $("#" + self.options.id + item[self.options.idField]).attr("data-top");
                var left = $("#" + self.options.id + item[self.options.idField]).attr("data-left");
                self._x = left;
                self._y = top;
                self._scale = 1;
                self._updateLastScale();
                self._zoomAround(self._scale, left, top);
                self._drawlines();
                self._updateLastScale();
                self._updateLastPos();
                var tile = $("#" + self.options.id + item[self.options.idField]);
                tile.addClass("element-selected");
            }
        },
        resetOptions: function(key, value) {
            var orgchart = this.element, self = this, o = this.options;
            o[key] = value;
            $(self._content).html("");
            self._drawTree(0, 0);
            self._resizeChart();
            self._drawlines();
        },
        addNodeAbove: function(data, nodeid) {
            var orgchart = this.element, self = this, o = this.options;
            var child = _.find(o.data, function(i) { return i[self.options.idField] === parseInt(nodeid); });
            var id = self._getLargestId();
            data[self.options.idField] = id + 1;
            data[self.options.parentIdField] = null;
            if (child !== undefined) {
                data[self.options.parentIdField] = child[self.options.parentIdField];
                child[self.options.parentIdField] = data[self.options.idField];
            }
            if (o.data.length === 0 || o.data.length === undefined) {
                o.data = new Array();;
            }
            o.data.push(data);
            self._prepareData();
            $(self._content).html("");
            self._drawTree(0, 0);
            self._resizeChart();
            self._drawlines();
            var tile = $("#" + self.options.id + data[self.options.idField]);
            self._renderNode(tile, data);
        },
        addNode: function(data) {
            var orgchart = this.element, self = this, o = this.options;
            var id = self._getLargestId();
            data[self.options.idField] = id + 1;
            o.data.push(data);
            self._prepareData();
            $(self._content).html("");
            self._drawTree(0, 0);
            self._resizeChart();
            self._drawlines();
            var tile = $("#" + self.options.id + data[self.options.idField]);
            self._renderNode(tile, data);
            return "#" + self.options.id + data[self.options.idField];
        },
        addNodeBelow: function(data, nodeid) {
            var orgchart = this.element, self = this, o = this.options;
            data[self.options.parentIdField] = nodeid;
            var id = self._getLargestId();
            data[self.options.idField] = id + 1;
            o.data.push(data);
            self._prepareData();
            $(self._content).html("");
            self._drawTree(0, 0);
            self._resizeChart();
            self._drawlines();
            var tile = $("#" + self.options.id + data[self.options.idField]);
            self._renderNode(tile, data);
        },
        getSelected: function() {
            var orgchart = this.element, self = this, o = this.options;
            return _.filter(o.data, function(i) { return i.selected; });
        },
        getData: function() {
            var orgchart = this.element, self = this, o = this.options;
            return o.data;
        },
        getScale: function() {
            var orgchart = this.element, self = this, o = this.options;
            return self._scale;
        }
    });