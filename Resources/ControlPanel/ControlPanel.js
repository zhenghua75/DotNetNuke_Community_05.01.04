jQuery(document).ready(function() {
    jQuery("select[id$='cboPanes']").change(function() {
        if (jQuery("input[value='LAYOUT'][checked]").length == 0) {
    
            var paneId = jQuery("select[id$='cboPanes'] option:selected").text();
            var pane = jQuery("td[id$='" + paneId + "'],div[id$='" + paneId + "']");
            var isHidden = (pane.height() < 2);

            if (isHidden) {
                //var hiddenpanes = jQuery(".DNNEmptyPane").toggleClass("DNNEmptyPane").append("<div class='_DNNRemove' style='height:25px' />");
                pane.removeClass("DNNEmptyPane").append("<div class='_DNNRemove' style='height:0px' />");
                jQuery("._DNNRemove").animate({ height: 29 }, "slow");
            }

            var size = { width: (pane.width() < 150 ? 150 : pane.width()), height: (pane.height() < 25 ? 25 : pane.height()) };
            var offset = pane.offset();

            var highlight = jQuery('#dnnPaneHighlight');
            if (highlight.length < 1) {
                highlight = jQuery("<div id='dnnPaneHighlight' style='position:absolute;border:2px dashed #333;text-align:center;font-weight:bold;font-size:1.2em;display:none;' />")
                    .appendTo("#Body");
            }

            highlight.css("top", offset.top)
                    .css("left", offset.left)
                    .width(size.width)
                    .height(size.height)
                    .text(paneId)
                    .fadeIn(100)
                    .fadeOut(100);

            if (isHidden) {
                //            jQuery("._DNNRemove").animate({ opacity: 1.0 }, 2000, function() { $(this).remove() });
                //            hiddenpanes.toggleClass("DNNEmptyPane");
                jQuery("._DNNRemove").animate({ opacity: 1.0 }, 100).animate({height: 0}, "slow", function() { jQuery(this).remove() });
                pane.addClass("DNNEmptyPane");

            }
        }
    });
});
