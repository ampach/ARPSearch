; (function () {
    function ARPsearch() { }

    

    function search() {
        var root = document.getElementById("arpsearch-results");

        var data = {
            confID: root.getAttribute("data-confid"),
            searchService: root.getAttribute("data-searchservice"),
            searchResult: root.getAttribute("data-searchresult"),

            //LastChangedFilterName: "",
            //Filters: [{ FieldName: "", FieldValue: ""}],
            //SearchBoxQuery: "",
            CurrentUrl: window.location.href,
            //Page: ""
        }
        ajax(root.getAttribute("data-requesturl"), data).then(function (data) {
            var obj = JSON.parse(data);
            console.log(obj);
            renderSearchResults(obj);
        }, function (error) {
            console.log(error);
        });
    }

    function renderSearchResults(data) {
        renderFacets(data.Facets);
        renderResultBody(data.Results);
    }

    function renderFacets(facets) {
        var facetsContainer = document.querySelector(".arps-sidebar .facets-container");
        if (facetsContainer) {
            for (var i = 0; i < facets.length; i++) {
                if (facets[i].Enabled && facets[i].Values.length > 0) {
                    var facetDefEl = document.getElementById(facets[i].ViewType);
                    if (facetDefEl) {
                        var facettemplateId = facetDefEl.value;
                        var teplateHtml = document.getElementById(facettemplateId);
                        if (teplateHtml) {
                            var template = uscore.template(teplateHtml.innerHTML);
                            facetsContainer.innerHTML = facetsContainer.innerHTML + template(facets[i]);
                        }

                    }
                }
            }
        }
        
    }

    function renderResultBody(results) {
        
    }

    ARPsearch.search = search;

    var ready = function (fn) {

        // Sanity check
        if (typeof fn !== 'function') return;

        // If document is already loaded, run method
        if (document.readyState === 'complete') {
            return fn();
        }

        // Otherwise, wait until document is loaded
        document.addEventListener('DOMContentLoaded', fn, false);

    };

    ready(function () {
        initUnderscore();
        search();

        //var template = uscore.template(
        //    document.getElementById('result-template').innerHTML
        //);

        //var templateData = {
        //    listTitle: "Olympic Volleyball Players",
        //    listItems: [
        //        {
        //            name: "Misty May-Treanor",
        //            hasOlympicGold: true
        //        },
        //        {
        //            name: "Kerri Walsh Jennings",
        //            hasOlympicGold: true
        //        },
        //        {
        //            name: "Jennifer Kessy",
        //            hasOlympicGold: false
        //        },
        //        {
        //            name: "April Ross",
        //            hasOlympicGold: false
        //        }
        //    ]
        //};

        //var r = template(templateData);

        //document.getElementsByClassName("arps-content")[0].innerHTML = r;
    });

    function initUnderscore() {
        window.uscore = _.noConflict();
        uscore.templateSettings.variable = "rc";
    }

    function ajax(url, requestuestBody) {
        return new Promise(function (succeed, fail) {

            var request = new XMLHttpRequest();
            request.open("POST", url, true);
            request.setRequestHeader('Content-Type', 'application/json');

            request.addEventListener("load", function () {
                if (request.status < 400)
                    succeed(request.responseText);
                else
                    fail(new Error("Request failed: " + request.statusText));
            });

            request.addEventListener("error", function () {
                fail(new Error("Network error"));
            });

            request.send(JSON.stringify(requestuestBody));
        });
    }

    window.arpsearch = ARPsearch;

}());