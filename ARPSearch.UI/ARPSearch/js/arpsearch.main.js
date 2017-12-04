; (function () {
    function ARPsearch() { }

    ARPsearch.latestChangedFilter = [];

    function search() {
        var root = document.getElementById("arpsearch-results");

        var data = {
            confID: root.getAttribute("data-confid"),
            searchService: root.getAttribute("data-searchservice"),
            searchResult: root.getAttribute("data-searchresult"),

            LastChangedFilterName: ARPsearch.latestChangedFilter[ARPsearch.latestChangedFilter.length-1],
            Filters: getFilters(),
            //SearchBoxQuery: "",
            CurrentUrl: window.location.href,
            Page: document.getElementById("arpsearch-current-page").value
        }

        ajax(root.getAttribute("data-requesturl"), data).then(function (data) {
            var obj = JSON.parse(data);
            console.log(obj);
            renderSearchResults(obj);
        }, function (error) {
            console.log(error);
        });
    }

    function getFilters() {
        var result = [];
        var dropdounFilters = document.querySelectorAll("select.arp-filter");
        for (var i = 0; i < dropdounFilters.length; i++) {
            var category = dropdounFilters[i].getAttribute("data-category");
            var val = dropdounFilters[i].value;
            if (val && val !== "-1") {
                result.push({
                    FieldName: category,
                    FieldValue: val
                });
            }
        }

        var checkboxFilters = document.querySelectorAll("input.arp-filter[type='checkbox']");
        for (var i = 0; i < checkboxFilters.length; i++) {
            if (checkboxFilters[i].checked) {
                var category = checkboxFilters[i].getAttribute("data-category");

                var val = checkboxFilters[i].value;
                if (val && val !== "-1") {
                    result.push({
                        FieldName: category,
                        FieldValue: val
                    });
                }
            }
        }
        return result;
    }

    function initevents() {
        var showMoreButtoms = document.querySelectorAll(".arp-showmore-button");
        for (var i = 0; i < showMoreButtoms.length; i++) {
            showMoreButtoms[i].addEventListener('click', showMore, false);
        }

        document.addEventListener("filterChange", filterChangeHandler, false);

        document.querySelector('body').addEventListener('change', function (event) {

            if (event.target.tagName.toLowerCase() === 'select' && hasClass(event.target, 'arp-filter')) {
                triggerSelectFilterChanngedEvent(event.target);
            }
            if (event.target.tagName.toLowerCase() === 'input' && event.target.type.toLowerCase() === 'checkbox' && hasClass(event.target, 'arp-filter')) {
                triggerSelectFilterChanngedEvent(event.target);
            }

        });
    }

    function hasClass(element, cls) {
        return (' ' + element.className + ' ').indexOf(' ' + cls + ' ') > -1;
    }

    function triggerSelectFilterChanngedEvent(e) {
        if (window.CustomEvent) {

            var event = new CustomEvent("filterChange", {
                detail: {},
                bubbles: true,
                cancelable: true
            });
            if (e.value !== "-1") {
                if (hasClass(e, 'single') && e.tagName.toLowerCase() === 'input' && e.type.toLowerCase() === 'checkbox') {
                    if (e.checked) {
                        ARPsearch.latestChangedFilter.push(e.getAttribute("data-category"));
                    } else {
                        if (ARPsearch.latestChangedFilter[ARPsearch.latestChangedFilter.length - 1] == e.getAttribute("data-category")) {
                            ARPsearch.latestChangedFilter.pop();
                        }
                    }
                } else {
                    ARPsearch.latestChangedFilter.push(e.getAttribute("data-category"));
                }
                
            }
            document.querySelector(".arp-search-result").innerHTML = "";
            document.getElementById("arpsearch-current-page").value = 1;
            e.dispatchEvent(event);
        }
    }

    

    // newMessage event handler
    function filterChangeHandler(e) {
        console.log(e);
        search();
    }

    function showMore() {
        var page = document.getElementById("arpsearch-current-page");
        if (page) {
            var newValue = parseInt(page.value) + 1;
            page.value = newValue;
            search();

            var rootPagesizeAttr = document.getElementById("arpsearch-results").getAttribute("data-pagesize");
            var pageSize = parseInt(rootPagesizeAttr);
            var totalResultCount = parseInt(document.getElementById("arpsearch-total-count").value);

            if ((newValue * pageSize) >= totalResultCount) {
                var showMoreButtoms = document.querySelectorAll(".arp-showmore-button");
                for (var i = 0; i < showMoreButtoms.length; i++) {
                    showMoreButtoms[i].remove();
                }
            }
        }
    }

    function renderSearchResults(data) {
        renderFacets(data.Facets);
        renderResultBody(data.Results);
        var totalResultCount = document.getElementById("arpsearch-total-count");
        if (totalResultCount) {
            totalResultCount.value = data.TotalResult;
        }
    }

    function renderFacets(facets) {
        var facetsContainer = document.querySelector(".arp-facets-container");
        if (facetsContainer) {
            facetsContainer.innerHTML = "";

            for (var i = 0; i < facets.length; i++) {
                if (facets[i].Enabled && facets[i].Values.length > 0) {
                    var facetDefEl = document.getElementById("fd_" + facets[i].ViewType);
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
        var resultsContainer = document.querySelector(".arp-search-result");
        if (resultsContainer) {
            for (var i = 0; i < results.length; i++) {
                if (results[i]) {
                    var resultsMap = document.getElementById("sr_" + results[i].TemplateId);
                    var templateId = undefined;
                    if (resultsMap) {
                        templateId = resultsMap.value;
                    } else {
                        templateId = "default-search-result";
                    }

                    if (templateId) {
                        var teplateHtml = document.getElementById(templateId);
                        if (teplateHtml) {
                            var template = uscore.template(teplateHtml.innerHTML);
                            resultsContainer.innerHTML = resultsContainer.innerHTML + template(results[i]);
                        }
                    }
                    
                }
            }
        }
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
        initevents();
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