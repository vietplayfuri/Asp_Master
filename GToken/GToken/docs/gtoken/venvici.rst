Venvici and GToken Integration
==============================

This document explains current drawbacks of Venvici-GToken integration and the approach we are taking in the new system.

.. contents::

Current drawbacks
-----------------

The current system doesn't not draw a clear line on where data should be stored and the scope of responsibilities belonging to each server.

The commission logic is spreaded between both Venvici (where the commission credit and BV are kept) and GToken (where the calculation logic is kept). This causes every promotion a joined effort of the two sides and requires deployment sync even though it doesn't have to be.

In the same manner, user credential is passed between the two servers in plain text and once the data was passed, there is no sync mechanism to keep the credential synced. We would like GToken to keep a master copy of the critical data and enable Venvici, GoPlay La, etc to access via APIs.

Scope of responsibility
-----------------------

Moving ahead, GToken is positioning itself as a loyalty system, using the currency GToken. GToken will cut loose from game-related activities and push them to GoPlay La, a new website that inherit GToken current design, code and data.

Venvici, as it has always been doing, houses the logic and data needed for its MLM network to maintain, namely, BV, Commission Credit, Placement Trees, and Commission Logic.

Data Migration
--------------

To separate data between the two systems, it was decided that GToken would keep *user credential and GToken Balance*. Venvici would keep *BV, Commission Credit, and Placement Trees*.

Actionable items from Venvici on this activity are:

* Remove user credential in Venvici database
* Use GToken API for logging in
* Allow GToken engineer to access Venvici database to retrieve user's GToken balance and transaction history related to this currency

API restructure
---------------

The separation of responsibility means GToken can focus on keeping track of user's payment on different affiliate system (which GoPlay La is one of them) and Venvici can focus on maintain MLM structure and calculate commission.

The set of APIs that GToken makes available to all partner, including Venvici, is documented at api_.

.. _api: api.html

In turn, GToken's need for Venvic's API reduce to only 2. 


``submitTransaction``
`````````````````````

The API updates Venvici with every purchase user has made and therefore might be qualified for commission. Notice that GToken is no longer responsible for all commission calculation logic, as opposed to current flow. So given the detail of the transaction (price, charge amount, discount percentage, etc), it is entirely up to Venvici site to calculate commission.

**Endpoint**: to be defined by Venvici engineer

**Request**:

=============================== ============== ==============================
Parameter                       Type           Notes
=============================== ============== ==============================
``username``                    string         GToken's username
``transaction``                 JSON           See ``transaction``'s definition_
=============================== ============== ==============================

.. _definition: api.html#transaction

**Response (JSON)**

================= ====== ==========================================
Key               Types  Notes
================= ====== ==========================================
``success``       bool
``message``       string Human-readable error message
================= ====== ==========================================

``addMember``
`````````````

As user's credential is be kept on GToken site, the field ``md5Password`` and ``password`` are be removed from the API params.

**Endpoint**: ``/web/addMember.jsp``

**Request**:

=============================== ============== ==============================
Parameter                       Type           Notes
=============================== ============== ==============================
``username``                    string         GToken's username
``email``                       string
``introducerId``                string         GToken's username of the introducer
``country``                     string         Country code by ISO 3166  
=============================== ============== ==============================

**Response (JSON)**

================= ====== ==========================================
Key               Types  Notes
================= ====== ==========================================
``success``       bool
``message``       string Human-readable error message
================= ====== ==========================================
