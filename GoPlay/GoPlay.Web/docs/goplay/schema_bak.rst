Database Schema
===============

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
``email``                     varchar(64)    Unique
``gender``                    varchar(8)     ``male``, ``female``, ``other``
``avatar_filename``           text
``cover_filename``            text
``vip``                       varchar(16)    ``standard``, ``gold``, or ``NULL``
``goplay_token``              decimal(16, 2) Play Token balance
``free_goplay_token``         decimal(16, 2) Free Play Token balance
``inviter_id``                int            References ``customer_account(id)``
``referral_code``             varchar(64)    Unique
``country_code``              varchar
``country_name``              varchar
``created_at``                timestamp
``updated_at``                timestamp
``last_login_at``             timestamp
``password_credential_id``    int            References ``customer_login_password(id)``
``bio``                       text
``game_id``                   int            References ``game(id)``. Refer the the source where user is from
``account_manager``           varchar
``account_manager_note``      text
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

``partner_account``
```````````````````

======================= ============ ====================================
Column                  Type         Notes
======================= ============ ====================================
``id``                  serial
``name``                varchar(32)
``client_id``           varchar(40)  OAuth2 Client ID
``client_secret``       varchar(55)  OAuth2 Client Secret
``client_type``         varchar      default ``public``
``_redirect_uris``      text         OAuth2 redirect URLs
``_default_scopes``     text         OAuth2 default scopes
======================= ============ ====================================


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

Authentication
--------------

``customer_login_password``
```````````````````````````

======================= ============ ====================================
Column                  Type         Notes
======================= ============ ====================================
``id``                  serial
``customer_account_id`` int          References ``customer_account(id)``. Unique (only one password login per account)
``username``            varchar(32)  Unique
``email``               varchar(64)  Unique
``password``            varchar(40)  BCrypt hash of password
======================= ============ ====================================


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


``studio_admin_assignment``
```````````````````````````

======================= ============ ====================================
Column                  Type         Notes
======================= ============ ====================================
``studio_id``           int          References ``studio(id)``
``game_admin_id``       int          References ``customer_account(id)``
======================= ============ ====================================

**Constraint**: Unique on (``studio_id``, ``customer_account_id``).


``game_access_token``
`````````````````````
======================= ============ ====================================
Column                  Type         Notes                               
======================= ============ ====================================
``id``                  serial
``game_id``             int          References ``game(id)``
``customer_account_id`` int          References ``customer_account(id)``
``token``               varchar      SHA1 access token, generated from game.uid, customer_account.password and secret key
``data``                text
``meta_data``           text
``saved_at``            timestamp
``stats``               text
======================= ============ ====================================

**Constraint**: Unique on (``game_id``, ``customer_account_id``)

Games
-----

``studio``
``````````

======================= ============ ====================================
Column                  Type         Notes
======================= ============ ====================================
``id``                  serial
``name``                varchar(32)
``created_at``          timestamp
``updated_at``          timestamp
``is_archived``         boolean
======================= ============ ====================================

``game``
````````
======================== ============ ====================================
Column                   Type         Notes
======================== ============ ====================================
``id``                   serial
``studio_id``            integer      References ``studio(id)``
``guid``                 uuid         Used as game_id in APIs. Secret. Also known as ``BasicKey`` in API v0
``name``                 varchar(128)
``description``          text
``created_at``           timestamp
``updated_at``           timestamp
``is_active``            boolean      Default False
``is_archived``          boolean      Default False
``icon_filename``        varchar      The same icon that is used in the mobile app
``banner_filename``      varchar
``download_links``       JSON         {'google', 'apple', 'apk', 'pc'}
``slider_images``        JSON         The list of slider images in game detail: { 'images': [ {'filename', 'index'}, ... ] }
``thumb_filename``       varchar      The thumbnail used in games listing page
``genre``                varchar
``short_description``    varchar      The game's description in games listing page
``current_version``      varchar
``current_changelog``    text
``file_size``            varchar      '69MB', '16KB', etc.
``content_rating``       varchar
``endpoint``             varchar      Endpoint for exchange request. Optional
``gtoken_client_id``     varchar      Vendor sets this. To apply additional security mechanism if needed
``gtoken_client_secret`` varchar      Vendor sets this. To apply additional security mechanism if needed
``released_at``          timestamp
======================== ============ ====================================


``credit_type``
```````````````

======================= ============ ====================================
Column                  Type         Notes
======================= ============ ====================================
``id``                  serial       Primary Key
``game_id``             integer      References ``game(id)``
``name``                varchar(32)
``exchange_rate``       integer      Exchange rate from 01 Play Token to credits. Null value indicates no direct exchange available
``free_exchange_rate``  integer      Exchange rate from 01 Free Play Token to credits. Null value indicates no direct exchange available
``icon_filename``       text
``old_db_id``           int          ``old_db_id`` is not an actual id, or else it would have gone to ``id``. ``old_db_id`` in the old design is unique *within* a game only.
``string_identifier``   varchar      Vendor-specified unique string to identify the package/credit      
``is_active``           boolean
``is_archived``         boolean
``created_at``          timestamp
``updated_at``          timestamp
======================= ============ ====================================


``package``
```````````

======================= ============= ====================================
Column                  Type          Notes
======================= ============= ====================================
``id``                  serial        Primary Key
``game_id``             integer       References ``game(id)``
``name``                varchar(32)
``goplay_token_value``  decimal(16,2)               
``icon_filename``       text
``old_db_id``           int           ``old_db_id`` is not an actual id, or else it would have gone to ``id``. ``old_db_id`` in the old design is unique *within* a game only.
``string_identifier``   varchar       Vendor-specified unique string to identify the package/credit      
``is_active``           boolean
``is_archived``         boolean
``created_at``          timestamp
``updated_at``          timestamp
``limited_time_offer``  int           Allow us to put a limit on how many purchase can be made
======================= ============= ====================================


Credits
-------

``credit_balance (unused)``
```````````````````````````
======================= ============ ====================================
Column                  Type         Notes
======================= ============ ====================================
``id``                  serial
``credit_type_id``      integer      References ``credit_type(id)``
``customer_account_id`` integer      References ``customer_account(id)``
``balance``             integer      Balance in game credits
======================= ============ ====================================

**Constraint**: Unique on (``credit_type_id``, ``customer_account_id``)


``gtoken_package``
``````````````````
======================= ============= ====================================
Column                  Type          Notes
======================= ============= ====================================
``id``                  serial
``name``                varchar
``price``               decimal(16,3)
``currency``            varchar       Default to ``USD``
``icon_filename``       text  
``goplay_token_amount`` decimal(16,3)
``sku``                 varchar
``icon_animation_html`` text
======================= ============= ====================================

``topup_card``
``````````````
======================= ============= ====================================
Column                  Type          Notes
======================= ============= ====================================
``id``                  serial
``customer_account_id`` integer       References ``customer_account(id)``. Null till used.
``card_number``         varchar(12)
``card_password``       varchar(12)
``amount``              integer       The value of the card
``validity_date``       timestamp
``status``              varchar       ``used``, ``unused``
``is_free``             boolean       is_free card adds to free_goplay_token
``used_at``             timestamp
``created_at``          timestamp
``is_bv``               boolean
``currency``            varchar       Default ``USD``
``price``               decimal(16,3) 
======================= ============= ====================================


Transactions
------------

``coin_transaction``
````````````````````

Records all play-token-related transactions, with associated metadata. A user's Play Token balance can be completely reconstructed by a ``SUM(amount)`` query over this table.

======================== ============= ====================================
Column                   Type          Notes
======================== ============= ====================================
``id``                   serial
``order_id``             varchar       uuid
``customer_account_id``  integer       References ``customer_account(id)``
``receiver_account_id``  integer       References ``customer_account(id)``. Nullable
``sender_account_id``    integer       References ``customer_account(id)``. Nullable
``amount``               decimal(16,3) Change in Play Token balance
``price``                decimal(16,3) The monetary value
``partner_account_id``   integer       References ``partner_account(id)``. Nullable
``game_id``              integer       References ``game(id)``. Nullable
``payment_method``       varchar       ``PayPal``, ``Top Up Card``, ``eNETS``
``topup_card_id``        integer       References ``topup_card(id)``. Nullable
``credit_type_id``       integer       References ``credit_type(id)``. Nullable
``package_id``           integer       References ``package(id)``. Nullable
``gtoken_package_id``    integer       References ``gtoken_package(id)``, Nullable
``paypal_redirect_urls`` text          JSON format, hosted approval and execute urls for paypal transactions. Nullable
``paypal_payment_id``    varchar
``created_at``           timestamp
``status``               varchar       ``success``, ``cancelled` and ``failure`` for all transactions. ``payment_pending``, ``payment_created``, ``payment_approved`` and ``payment_executed`` for ``PayPal``
``description``          text          Extra human-readable information
``telkom_order_id``      varchar       Telkom Unique Transaction ID
``ip_address``           varchar
``country_code``         varchar
``use_gtoken``           boolean
======================== ============= ====================================

``free_coin_transaction``
`````````````````````````

Records all *free*-play-token-related transactions, with associated metadata. A user's *free* Play Token balance can be completely reconstructed by a ``SUM(amount)`` query over this table.

======================= ============= ====================================
Column                  Type          Notes
======================= ============= ====================================
``id``                  serial
``order_id``            varchar       uuid
``customer_account_id`` integer       References ``customer_account(id)``
``amount``              decimal(16,2) Change in free Play Token balance
``price``               decimal(16,2) The monetary value
``game_id``             integer       References ``game(id)``. Nullable
``credit_type_id``      integer       References ``credit_type(id)``. Nullable
``package_id``          integer       References ``package(id)``. Nullable
``created_at``          timestamp
``status``              varchar       Whatever state a transaction can be ``success``, ``failure``, ``pending``, etc
``description``         text          Extra human-readable information
``ip_address``          varchar
``country_code``        varchar
``payment_method``      varchar
``topup_card_id``       integer       References ``topup_card(id)``. Nullable
======================= ============= ====================================


``credit_transaction``
``````````````````````
Records all credit-related transactions, with associated metadata. A user's credit balance for any game can be completely reconstructed by a ``SUM(amount)`` query over this table.

Note that a credit purchase will have a corresponding entry in ``coin_transaction`` or ``free_coin_transaction``.

============================= ============= ====================================
Column                        Type          Notes
============================= ============= ====================================
``id``                        serial
``customer_account_id``       integer       References ``customer_account(id)``
``coin_transaction_id``       integer       References ``coin_transaction(id)``. Nullable.
``free_coin_transaction_id``  integer       References ``free_coin_transaction(id)``. Nullable.
``amount``                    decimal(16,2) Change in credit balance
``game_id``                   integer       References ``game(id)``
``credit_type_id``            integer       References ``credit_type(id)``     
``package_id``                integer       References ``package(id)``
``created_at``                timestamp
``status``                    varchar       Whatever state a transaction can be ``success``, ``failure``, ``pending``, etc
``description``               text          Extra human-readable information
============================= ============= ====================================



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
``ip_adress``           varchar
``game_id``             integer
``user_agent``          varchar
``game_id``             int          References ``game(id)``
``status``              varchar      ``success``, ``fail``
``message``             text         Extra human-readable information
``data``                json         ``data``
``created_at``          timestamp
``country_code``        varchar
``customer_account_id`` integer
======================= ============ ====================================
