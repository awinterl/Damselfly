﻿window.ScrollMonitor =
{
    Init: function (scrollAreaID, DotNetRef, initialScrollPos) {
        var scrollArea = document.getElementById(scrollAreaID);
        scrollArea.scrollTop = initialScrollPos;

        var markerVisibleState = null;

        function onScroll() {
            DotNetRef.invokeMethodAsync("HandleScroll", scrollArea.scrollTop);
        }

        window.addEventListener('resize', onScroll);
        scrollArea.addEventListener('scroll', onScroll);
    }
}