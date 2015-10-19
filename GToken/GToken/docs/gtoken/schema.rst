GToken Database Schema
======================

The schema listing given here is not intended to be comprehensive. It is intended to give a general structure and address some specific decisions, but other tables may be added and those given here may be expanded.

Tables used purely by the OAuth2 provider (e.g. ``oauth2_grant``, ``oauth2_token``) are also omitted. See the documentation for `OAuth2 Servers`_.

.. _`OAuth2 Servers`: https://flask-oauthlib.readthedocs.org/en/latest/oauth2.html

.. contents::

Accounts
--------

``customer_account``
````````````````````

============================= ============== ====================================
Column                        Type           Notes
============================= ============== ====================================
``id``                        serial         Primary Key
``nickname``                  varchar(40)    Display name
``email``                     varchar(64)
``username``                  varchar(32)    Unique
``password``                  varchar(40)    BCrypt hash of password
``gender``                    varchar(8)     ``male``, ``female``, ``other``
``avatar_filename``           varchar
``cover_filename``            varchar
``vip``                       varchar(16)    ``standard``, ``gold``, or ``NULL``
``gtoken``                    decimal(16, 3) GToken balance
``inviter_id``                int            References ``customer_account(id)``
``country_code``              varchar
``country_name``              varchar
``created_at``                timestamp
``updated_at``                timestamp
``last_login_at``             timestamp
``bio``                       text
``partner_id``                int            References ``partner(id)``. Refer the the source where user is from
``is_archived``               boolean        Default False
``locale``                    varchar        Locale setting of the user. Default to ``en``
============================= ============== ====================================


``friend``
``````````

======================= ============ ====================================
Column                  Type         Notes
======================= ============ ====================================
``friend1_id``          integer      References ``customer_account(id)``
``friend2_id``          integer      References ``customer_account(id)``
``sent_at``             timestamp    The time the friend request was sent
``status``              varchar      ``pending``, ``rejected`` and ``accepted``. Once the status is ``accepted``, clone the entity and switch friend1_id and friend2_id
======================= ============ ====================================

**Constraint**: Unique on (``friend1_id``, ``friend2_id``)

``partner``
```````````

======================= ============ ====================================
Column                  Type         Notes
======================= ============ ====================================
``id``                  serial
``name``                varchar(32)
``uid``                 uuid         Used as ``partner_uid`` in APIs. Secret.
``identifier``          varchar      Unique. Natural Key.
``endpoint``            varchar
``client_id``           varchar(40)  OAuth2 Client ID
``client_secret``       varchar(55)  OAuth2 Client Secret
``_redirect_uris``      text         OAuth2 redirect URLs
``_default_scopes``     text         OAuth2 default scopes
======================= ============ ====================================

Authentication
--------------

``customer_login_oauth``
````````````````````````

======================= ============ ====================================
Column                  Type         Notes
======================= ============ ====================================
``id``                  serial
``customer_account_id`` int          References ``customer_account(id)``
``service``             varchar(32)  Third-party OAuth service used
``identity``            text         Identity in service
======================= ============ ====================================

**Constraint**: Unique on (``customer_account_id``, ``service``).

``access_token``
`````````````````````
======================= ============ ====================================
Column                  Type         Notes                               
======================= ============ ====================================
``id``                  serial
``partner_id``          int          References ``partner(id)``
``customer_account_id`` int          References ``customer_account(id)``
``token``               varchar      SHA1 access token, generated from game.uid, customer_account.password and secret key
======================= ============ ====================================

**Constraint**: Unique on (``game_id``, ``customer_account_id``)

``verification_token``
``````````````````````

======================= ============ ====================================
Column                  Type         Notes
======================= ============ ====================================
``code``                uuid         Primary key
``customer_account_id`` int          References ``customer_account(id)``
``validation_time``     timestamp    One hour from created time
``is_valid``            bool         Default true
======================= ============ ====================================

Transactions
------------

``transaction``
```````````````

Records all gtoken-related transactions, with associated metadata. A user's GToken balance can be completely reconstructed by a ``SUM(amount)`` query over this table.

======================== ============= ====================================
Column                   Type          Notes
======================== ============= ====================================
``id``                   serial
``order_id``             varchar       uuid
``customer_account_id``  integer       References ``customer_account(id)``
``partner_id``           integer       References ``partner(id)``. Nullable
``price``                decimal(16,3) The monetary value
``status``               varchar       ``pending``, ``failure``, ``cancelled``, ``success``
``description``          text          Extra human-readable information
``ip_address``           varchar
``country_code``         varchar
``created_at``           timestamp
``updated_at``           timestamp
======================== ============= ====================================

System Log
----------

``api_log``
```````````
Records all api calls

======================= ============ ====================================
Column                  Type         Notes
======================= ============ ====================================
``id``                  serial
``action``              varchar
``version``             varchar
``customer_account_id`` integer      References ``customer_account(id)``
``partner_id``          integer      References ``partner(id)``
``user_agent``          varchar
``status``              varchar      ``success``, ``fail``
``message``             text         Extra human-readable information
``data``                json         Request content
``created_at``          timestamp
``ip_adress``           varchar
``country_code``        varchar
======================= ============ ====================================
