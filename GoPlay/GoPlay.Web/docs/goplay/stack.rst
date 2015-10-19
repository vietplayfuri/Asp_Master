Application Stack
=================

Web Framework
-------------

The application will be built in Python_, using the Flask_ web framework. We will use Blueprints_ to divide the application into four sub-applications (the mobile API, public website, admin web app, and partner API), but all four will be served side-by-side from each application instance.

Where templating is needed, we will use Jinja2_.

For form rendering and validation, we use `flask-wtforms`_, a Flask extension of wtforms_. Given an HTTP post, it is up to the developer and the circumstance to either render a template or encode to JSON (AJAX).

Database
--------

The sub-applications given above should not contain their own database models. All models will be contained in a separate sub-package of the application. We will use the Storm_ ORM, communicating with RDS via psycopg2_, and manage the database schema via Schemup_. 


Deployment
----------

We will deploy the Flask application via Gunicorn_ behind nginx_. 

Gunicorn should be configured to use gevent_ workers so that long-running backend actions (e.g. outbound queries to OAuth providers such as Facebook) will not block the worker, or require complicated workarounds.

Static files should be served directly from nginx, using the `try-files`_ directive.

.. _Python: http://www.python.org/
.. _Flask: http://flask.pocoo.org/
.. _Jinja2: http://jinja.pocoo.org/
.. _flask-wtforms: https://flask-wtf.readthedocs.org/en/latest/
.. _wtforms: https://wtforms.readthedocs.org/en/latest/
.. _Storm: https://storm.canonical.com/
.. _psycopg2: http://initd.org/psycopg/
.. _Schemup: https://github.com/brendonh/schemup
.. _Blueprints: http://flask.pocoo.org/docs/blueprints/
.. _Gunicorn: http://gunicorn.org/
.. _nginx: http://nginx.org/
.. _gevent: http://www.gevent.org/
.. _try-files: http://nginx.org/en/docs/http/ngx_http_core_module.html#try_files

