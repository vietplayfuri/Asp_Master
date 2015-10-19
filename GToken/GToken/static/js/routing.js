// These functions are hack for get route parameter
// We shouldn't use this with Angular app. However, I can't config angular route
// Please remove these functions
(function () {
    var router = {},
        routes = [];
    var makeRegEx = function (pattern) {
        var regExpStr = '^' + pattern
            .replace(/\//g, "\\/")
            .replace(/\?/g, "\\?")
            .replace(/:([0-9a-zA-Z-_]*)/g,"([0-9a-zA-Z-_]*)") + '$';
        return new RegExp(regExpStr);
    };

    router.getRouteParams = function getRouteParams (routePattern, path) {
        var path = path || decodeURIComponent(location.pathname + location.hash),
            pattern = makeRegEx(routePattern);
        if (pattern.test(path)) {
            var context = {};
            var params = path.match(pattern);
            //remove the first match, it is a redundant matched item contain the whole url
            params.shift();
            routePattern.match(/:(\w*)/g)
                .map(function(arg){ return arg.replace(':', ''); })
                .forEach(function(key, index) {
                    // map the param to context variable
                    // e.g context.section = 'homePage';
                    // e.g context.pageIndex = '12';
                    context[key] = params[index];
                });
            return context;
        }
        return null;
    }

    function process (href) {
        var path = typeof href === 'string' && href || (location.pathname + location.hash);
        var route = routes.filter(function (r) {
            return r.pattern.test(path);
        }).pop();
        if (route) {
            route.callback(router.getRouteParams(route.originalPattern, path));
        }
    }

    router.navigate = function (path) {
        if (history.pushState) {
            history.pushState(null, null, path);
        }
        process.call(path);
        return this;
    }

    var processRouting;
    router.register = function (pattern, callback) {
        routes.push({
            originalPattern: pattern,
            pattern: makeRegEx(pattern),
            callback: callback
        });
        if (processRouting) clearTimeout(processRouting);
        processRouting = setTimeout(function () {
            process();
        });
    };

    window.addEventListener('popstate', process);
    window.router = router;
})();