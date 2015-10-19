Application Stack
=================

Messenger Service
-----------------

The service is built in Java_, using netty_ NIO framework.

Synchronization (sync) Service
------------------------------

The service is built in Java_, using spark_ web application framework, embedded jetty_ as web server and container.

Notification Service
--------------------

Service

Worker

Database
--------

The system is a combination of MySQL for permanent storage, Redis and SSDB for caching, accessibility and scalability.

Requirement
-----------

Server must be installed:

* Proxy Server lastest version nginx_
* JDK7 or lastest version java_
* MySQL_ lastest version
* Redis Server lastest version redis_
* SSDB Server lastest version ssdb_

Deployment
----------

We will get binary builds of messenger service, sync service, notification service, and notification worker into the server.

For each binary:

* Enter **conf** folder.
  * Change file to **production.config.ini**, **production.log4j.ini**.
  * Update to new host, port.

* Edit **javaConsole**:

  * JAVA=...
  * BASENAME=...
  * BASEDIR=...
  * APPENV=production
  * XMS=... #heap memory size

* Run **sh javaConsole start|restart|stop**

With nginx_:

* Config proxy past websocket to Messenger Service.

.. _nginx: http://nginx.org
.. _Java: http://www.oracle.com/technetwork/java/javase/downloads/jdk8-downloads-2133151.html
.. _netty: http://netty.io/
.. _jetty: http://eclipse.org/jetty/
.. _spark: http://sparkjava.com/
.. _ssdb: http://ssdb.io/
.. _MySQL: http://dev.mysql.com/downloads/mysql/
.. _commons-pool: http://commons.apache.org/proper/commons-pool/
.. _redis: http://redis.io
