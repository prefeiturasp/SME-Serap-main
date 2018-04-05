/**
 * @function Preview de fórmulas MathJax
 * @namespace Controller
 * @author https://github.com/christianp/writemaths
 * @author Julio Cesar da Silva - 03/03/2015
 */
(function ($) {

	function saveSelection(containerEl) {
		var charIndex = 0, start = 0, end = 0, foundStart = false, stop = {};
		var sel = rangy.getSelection()

		function traverseTextNodes(node, range) {
			if (node.nodeType == 3) {
				if (!foundStart && node == range.startContainer) {
					start = charIndex + range.startOffset;
					foundStart = true;
				}
				if (foundStart && node == range.endContainer) {
					end = charIndex + range.endOffset;
					throw stop;
				}
				charIndex += node.length;
			} else {
				for (var i = 0, len = node.childNodes.length; i < len; ++i) {
					traverseTextNodes(node.childNodes[i], range);
				}
			}
		}

		if (sel.rangeCount) {
			try {
				traverseTextNodes(containerEl, sel.getRangeAt(0));
			} catch (ex) {
				if (ex != stop) {
					throw ex;
				}
			}
		}

		return {
			start: start,
			end: end
		};
	};

    function restoreSelection(containerEl, savedSel) {
    	var charIndex = 0, range = rangy.createRange(), foundStart = false, stop = {};
    	range.collapseToPoint(containerEl, 0);

    	function traverseTextNodes(node) {
    		if (node.nodeType == 3) {
    			var nextCharIndex = charIndex + node.length;
    			if (!foundStart && savedSel.start >= charIndex && savedSel.start <= nextCharIndex) {
    				range.setStart(node, savedSel.start - charIndex);
    				foundStart = true;
    			}
    			if (foundStart && savedSel.end >= charIndex && savedSel.end <= nextCharIndex) {
    				range.setEnd(node, savedSel.end - charIndex);
    				throw stop;
    			}
    			charIndex = nextCharIndex;
    		} else {
    			for (var i = 0, len = node.childNodes.length; i < len; ++i) {
    				traverseTextNodes(node.childNodes[i]);
    			}
    		}
    	}

    	try {
    		traverseTextNodes(containerEl);
    	} catch (ex) {
    		if (ex == stop) {
    			rangy.getSelection().setSingleRange(range);
    		} else {
    			throw ex;
    		}
    	}
    };

    var endDelimiters = {
        '$': /[^\\]\$/,
        '\\(': /[^\\]\\\)/,
        '$$': /[^\\]\$\$/,
        '\\[': /[^\\]\\\]/
    }
    var re_startMaths = /(?:^|[^\\])\$\$|(?:^|[^\\])\$|\\\(|\\\[|\\begin\{(\w+)\}/;

    function findMaths(txt, target) {
    	var i = 0;
    	var m;
    	var startDelimiter, endDelimiter;
    	var start, end;
    	var startChop, endChop;
    	var re_end;

    	while (txt.length) {
    		m = re_startMaths.exec(txt);

    		if (!m)     // if no maths delimiters, target is not in a maths section
    			return null;

    		startDelimiter = m[0];
    		start = m.index;

    		if (i + start >= target)    // if target was before the starting delimiter, it's not in a maths section
    			return null;

    		startChop = start + startDelimiter.length;
    		txt = txt.slice(startChop);

    		if (startDelimiter.match(/^\\begin/)) {    //if this is an environment, construct a regexp to find the corresponding \end{} command.
    			var environment = m[1];
    			re_end = new RegExp('[^\\\\]\\\\end\\{' + environment + '\\}');    // don't ask if this copes with nested environments
    		}
    		else if (startDelimiter.match(/.\$/)) {
    			re_end = endDelimiters[startDelimiter.slice(1)];
    		} else {
    			re_end = endDelimiters[startDelimiter];    // get the corresponding end delimiter for the matched start delimiter
    		}

    		m = re_end.exec(txt);

    		if (!m) {    // if no ending delimiter, target is in a maths section
    			return {
    				start: i + start,
    				end: i + startChop + txt.length,
    				math: txt,
    				startDelimiter: startDelimiter,
    				endDelimiter: endDelimiter
    			};
    		}

    		endDelimiter = m[0];
    		end = m.index + 1;    // the end delimiter regexp has a "not a backslash" character at the start because JS regexps don't do negative lookbehind
    		endChop = end + endDelimiter.length - 1;
    		if (i + startChop + end >= target) {    // if target is before the end delimiter, it's in a maths section
    			return {
    				start: i + start,
    				end: i + startChop + endChop,
    				math: txt.slice(0, end),
    				startDelimiter: startDelimiter,
    				endDelimiter: endDelimiter.slice(1)
    			};
    		}
    		else {
    			txt = txt.slice(endChop);
    			i += startChop + endChop;
    		}
    	}
    };

    $.fn.writemaths = function (custom_options) {
        $(this).each(function () {
            var options = $.extend({
                cleanMaths: function (m) { return m; },
                callback: function () { },
                iFrame: false,
                position: 'left top',
                previewPosition: 'left top'
            }, custom_options);
            var textarea = $(this).is('textarea,input');
            var root = this;
            var el;
            var iframe;
            if (options.of == 'this')
                options.of = root;

            if (options.iFrame) {
                iframe = $(this).find('iframe')[0];
                el = $(iframe).contents().find('body');
            }
            else {
                el = $(this);
            }

            el.addClass('writemaths tex2jax_ignore');

            var previewElement = $('<div class="wm_preview"/>');

            $('body').append(previewElement);

            try {
                var queue = MathJax.Callback.Queue(MathJax.Hub.Register.StartupHook("End", {}));
            } catch (e) { }
            var txt, sel, range;


            /**
             * @function Atualizar a posição do preview
             * @namespace writemaths
             * @autor: unknown
             */
            function positionPreview() {
                var of = options.of ? options.of : options.iFrame ? iframe : textarea ? root : document;
                previewElement.position({ my: options.previewPosition, at: options.position, of: of, collision: 'fit' })
            };

            /**
             * @function Atualizar fórmula no preview
             * @namespace writemaths
             * @autor: unknown
             */
            function updatePreview(e) {

                previewElement.hide();

                if (textarea) {
                    sel = $(this).getSelection();
                    range = { startOffset: sel.start, endOffset: sel.end };
                    txt = $(this).val();
                }
                else {
                    sel = options.iFrame ? rangy.getIframeSelection(iframe) : rangy.getSelection();
                    var anchor = sel.anchorNode;
                    range = sel.getRangeAt(0);
                    if (anchor.nodeType == anchor.TEXT_NODE) {
                        while (anchor.previousSibling) {
                            anchor = anchor.previousSibling;
                            range.startOffset += anchor.textContent.length;
                            range.endOffset += anchor.textContent.length;
                        }
                        anchor = anchor.parentNode;
                    }
                    if ($(anchor).add($(anchor).parents()).filter('code,pre,.wm_ignore').length)
                        return;
                    txt = $(anchor).text();
                }
                //only do this if the selection has zero width
                //so when you're selecting blocks of text, distracting previews don't pop up
                if (range.startOffset != range.endOffset)
                    return;
                var target = range.startOffset;
                var q = findMaths(txt, target);
                if (!q)
                    return;
                var math;
                if (q.startDelimiter.match(/^\\begin/))
                    math = q.startDelimiter + q.math + (q.endDelimiter ? q.endDelimiter : '');
                else
                    math = q.math;
                if (!math.length)
                    return;

                previewElement.show();

                if (math != $(this).data('writemaths-lastMath')) {
                    var script = document.createElement('script');
                    script.setAttribute('type', 'math/tex');
                    script.textContent = options.cleanMaths(math);
                    previewElement.html(script);
                    $(this).data('writemaths-lastMath', math);
                    queue.Push(['Typeset', MathJax.Hub, previewElement[0]]);
                    queue.Push(positionPreview);
                    queue.Push(options.callback);
                }

                positionPreview();
                updateElement();
            };


            //core
            try { updatePreview = $.throttle(100, updatePreview); } catch (e) {  }

            // periodically check the iFrame still exists 
            if (options.iFrame) {
                function still_there() {
                    if (!$(iframe).parents('html').length) {
                        previewElement.remove();
                        clearInterval(still_there_interval);
                        el.off();
                    }
                };
                var still_there_interval = setInterval(still_there, 100);
            }

            el.on('blur', function (e) {
                previewElement.hide();
            }).on('keyup click', updatePreview);

            if (options.iFrame) {
                $(el[0].ownerDocument).on('scroll', updatePreview);
            }
            else {
                el.on('scroll', updatePreview);
            }

            // perda focus do redactor
            $('body').on('click', function (e) {
                var elem, evt = e ? e : event;
                if (evt.srcElement) elem = evt.srcElement;
                else if (evt.target) elem = evt.target;
                var _parentElem = $(elem).parent().parent()[0];
                if (_parentElem == undefined) {
                    previewElement.hide();
                }
                else if (_parentElem.className != "ng-isolate-scope writemaths tex2jax_ignore" && _parentElem.className != "redactor-box") {
                    previewElement.hide();
                }
            });


            /**
             * @function Plugins para preview de mathjax
             * @namespace writemaths
             * @autor: Julio Cesar da Silva: 23/07/2015
             */
            function updateElement() {
                previewElement.children().first().children().each(function (index, elem) {
                    if (index > 0) {
                        $(elem).remove();
                    }
                });
            };

            if (!window.watch_writemaths) {
                window.watch_writemaths = true;
                setInterval(updateElement, 100);
            }
        });
        return this;
    };

    (function (b, c) {
        var $ = b.$ || b.Cowboy || (b.Cowboy = {}), a; $.throttle = a = function (e, f, j, i) { var h, d = 0; if (typeof f !== "boolean") { i = j; j = f; f = c } function g() { var o = this, m = +new Date() - d, n = arguments; function l() { d = +new Date(); j.apply(o, n) } function k() { h = c } if (i && !h) { l() } h && clearTimeout(h); if (i === c && m > e) { l() } else { if (f !== true) { h = setTimeout(i ? k : l, i === c ? e - m : e) } } } if ($.guid) { g.guid = j.guid = j.guid || $.guid++ } return g }; $.debounce = function (d, e, f) { return f === c ? a(d, e, false) : a(d, f, e !== false) }
    })($);

})(jQuery);