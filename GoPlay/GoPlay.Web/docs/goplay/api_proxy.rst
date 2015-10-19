API PROXY
=========

API Proxy was implemented to provide support to deprecated v0 API.

When I took over GToken system in 2014, I inherited a website and a set of API.
* The website was later re-skinned and re-programmed in python.
* The set of API had some security holes in it and was lack of consistency between API functions. Eventually the API was redesigned, implemented, and pushed to production on Oct 2014 and has been referred to the online document at http://dev2.gtoken.com/docs/hld/_build/html/api.html

But the thing with API is that it is a life-time contract, to publish an API to public is the equivalent of committing its maintenance as long as the service is functioning. By the time new API was out, there had already been 5 games integrated with the old API: EndGods (SJ), Basketball (LeChao), Fishing Hero (UMGame), Susy and Mine Mania (PF). Two biggest reasons that prevented these games to adopt new API were
* In SJ case, no one knew what the live server was doing. They remain unknown until now, and choose to develop a new server instead of fixing the old one.
* In other cases, the games were not generating enough revenue to justify the extra development cost
To cope with that situation, I developed a proxy server, which has been referring to as the api-proxy

The specification of api-proxy was translated to English by me (the original was Chinese). Request access to the doc at https://docs.google.com/document/d/1g2E0-PbZGJDE5C_hmfnFEF_qUDMp6IXcOPZrWf6f7RQ/edit

api-proxy code base is hosted on github https://github.com/gtoken/api-proxy
Basically api-proxy is extremely lean, it doesn't do much other than reads request, transforms it into the new format, sends to gtoken.com/api, takes the response, transforms response back to old format and sends back to client. A pretty standard proxy.

Production api-proxy is hosted on gtoken-prod-2 EC2 and is available at http://www.gtoken.com/index.aspx, its nginx config is at /etc/nginx/sites-available/api-proxy

Dev api-proxy is hosted on my personal AWS, and is available at http://test.gtoken.com/index.aspx
