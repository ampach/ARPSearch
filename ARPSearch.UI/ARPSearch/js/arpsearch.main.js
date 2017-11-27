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
            console.log(data);
            renderSearchResults(data);
        }, function (error) {
            console.log(error);
        });
    }

    function renderSearchResults(data) {
        renderFacets(data.Facets);
        renderResultBody(data.Results);
    }

    function renderFacets(facets) {
        for (var i = 0; i < facets.length; i++) {
            var facetDefEl = document.getElementById(facets[i].ViewType);
            if (facetDefEl) {
                var facettemplateId = facetDefEl.value;
                var teplateHtml = document.getElementById(facettemplateId);
                if (teplateHtml) {
                    var template = uscore.template(teplateHtml.innerHTML);

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

        var template = uscore.template(
            document.getElementById('result-template').innerHTML
        );

        var templateData = {
            listTitle: "Olympic Volleyball Players",
            listItems: [
                {
                    name: "Misty May-Treanor",
                    hasOlympicGold: true
                },
                {
                    name: "Kerri Walsh Jennings",
                    hasOlympicGold: true
                },
                {
                    name: "Jennifer Kessy",
                    hasOlympicGold: false
                },
                {
                    name: "April Ross",
                    hasOlympicGold: false
                }
            ]
        };

        var r = template(templateData);

        document.getElementsByClassName("arps-content")[0].innerHTML = r;
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