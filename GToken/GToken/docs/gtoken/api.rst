Partner API
===========

The API listing given here is not intended to be comprehensive. It is intended to give a general structure and address some specific decisions, but other API calls may be added and those given here may be expanded.

.. contents::



Versioning
----------

Every API will include a version number in its URL, e.g.:

* ``/api/1/login``
* ``/api/2/login``

These multiple versions will have separate implementations in the application, and old versions will be kept available for as long as possible. 

An API call can be *extended* without increasing its version number. This usually means adding new optional fields, and modifying the API implementation to use them only if available. The call must still work as expected if the new fields are not present.

If an API call must be changed in a way that breaks existing clients, it should be given a new version.

Each API call may have its own version numbers. The "current" API document for app developers should use the latest version of each call, and these must always be compatible with each other. So, if a new version of one API breaks existing versions of other APIs, those APIs should be given new, compatible versions at the same time (and this should be noted in the documentation).


Common Data Structures
----------------------

Profile
```````

A JSON object representing a customer's account information, with the following keys:

============================= ======= =====================================
Key                           Type    Notes
============================= ======= =====================================
``uid``                       int
``account``                   string  Unique username
``email``                     string
``nickname``                  string
``gender``                    string
``vip``                       string  "standard", "gold", or null
``bio``                       string
``inviter``                   string  Inviter's username
``avatar``                    string
``cover``                     string
``country_code``              string  Country name
``gtoken``                    decimal GToken balance
``is_venvici_member``         bool
============================= ======= =====================================

Transaction
```````````

A JSON object representing a transaction, with the following keys:

================================ ============= =====================================
Key                              Type          Notes
================================ ============= =====================================
``username``                     string        GToken unique username
``order_id``                     string        The ID of transaction, issued by vendor
``gtoken_transaction_id``        uuid          The internal ID of the transaction, used by GToken
``price``                        decimal(16,3) 
``final_amount``                 decimal(16,3)
``tax``                          decimal(16,3)
``service_charge``               decimal(16,3)
``currency``                     string        Default ``USD``
``original_price``               decimal(16,3) The original pre-discount amount on the bill 
``original_final_amount``        decimal(16,3) The post-discount amount ``original_final_amount = original_price * (1 - discount_percentage)``
``original_tax``                 decimal(16,3) The tax (GST) amount (not percentage) on the transaction. Default 0
``original_service_charge``      decimal(16,3) The service charge amount (not percentage) on the transaction. Default 0
``original_currency``            string        Default ``USD``
``discount_percentage``          decimal       Default ``0``. ``10%`` will be saved as ``0.1`` in the database. So that ``final_amount = price * (1 - discount_percentage)``
``revenue_percentage``           decimal
``status``                       string        ``pending``, ``failure``, ``cancelled``, ``success``
``payment_method``               string       
``description``                  string       
``is_venvici_applicable``        boolean       Default ``true``
================================ ============= =====================================


Token Transaction
`````````````````

A JSON object representing a token-consuming transaction, with the following keys:

================================ ============= =====================================
Key                              Type          Notes
================================ ============= =====================================
``username``                     string        GToken unique username
``order_id``                     string        The ID of transaction, issued by vendor
``gtoken_transaction_id``        uuid          The internal ID of the transaction, used by GToken
``token_type``                   string        The name of the token, e.g. Play Token, Sugar Token, or currency code, like SGD or USD
``transaction_type``             string        Currently supported transaction types are ``consumption`` and ``transfer``
``amount``                       decimal       The amount of token after discount used in the transaction
``tax``                          decimal(16,3) The tax (GST) on the transaction. Default 0
``service_charge``               decimal(16,3) The service charge on the transaction. Default 0
``description``                  string
================================ ============= =====================================


Direct GToken Transaction
`````````````````````````

A JSON object representing a direct-charge GToken transaction, with the following keys:

================================ ============= =====================================
Key                              Type          Notes
================================ ============= =====================================
``username``                     string        GToken unique username
``order_id``                     string        The ID of transaction, issued by vendor
``gtoken_transaction_id``        uuid          The internal ID of the transaction, used by GToken
``amount``                       decimal       The amount of token after discount used in the transaction
``description``                  string
================================ ============= =====================================



API Calls
---------

``POST /account/register``
``````````````````````````

Used to *explicitly* register a customer account from a mobile app, meaning that the user has indicated they have no existing account, and filled out a registration form in-app.

**Endpoint**: ``/api/1/account/register``

**Request**:

================= ====== ==============================
Parameter         Type   Notes
================= ====== ==============================
``username *``    string Must be unique
``password *``    string 
``email``         string
``nickname``      string
``gender``        string "male", "female", or "other"
``partner_id *``  uuid
``referral_code`` string
``ip_address``    string If not provided, use the IP address of requester
``country_code``  string If one of two fields: ``country_code`` and ``country name`` not provided, user country will be automatically filled based on ``ip_address`` field.
``country_name``  string
================= ====== ==============================

**Response (JSON)**:

================= ======= ==============================
Key               Type    Notes
================= ======= ==============================
``success``       bool
``message``       string  Human-readable error message
``error_code``    string  Error Code
``session``       string  Access token for later requests
``profile``       profile See ``profile`` definition above
================= ======= ==============================

**Error Messages**:

* EXISTING_USERNAME_EMAIL - 'Account with such username/email already exists'
* MISSING_FIELDS - 'Required field(s) is blank'
* INVALID_PARTNER_ID - 'Invalid Partner ID'
* USERNAME_LENGTH - 'Username is between 3-20 characters'
* INVALID_USERNAME - 'Username does not accept special characters'
* PASSWORD_LENGTH - 'Password must be more than 3 characters'
* NON_EXISTING_REFERRAL_CODE - 'Referral Code does not exist'
* INVALID_COUNTRY - 'Invalid country code or country name'

``POST /account/login-password``
````````````````````````````````

Logs a user in via their GToken username and password. The account must already exist.

**Endpoint**: ``/api/1/account/login-password``

**Request**:

================= ====== ==============================
Parameter         Type   Notes
================= ====== ==============================
``username *``    string
``password *``    string
``partner_id *``  uuid
================= ====== ==============================

**Response (JSON)**:

================= ======= ================================
Key               Type    Notes
================= ======= ================================
``success``       bool
``message``       string  Human-readable error message
``error_code``    string  Error Code
``session``       string  Access token for later requests
``profile``       profile See ``profile`` definition above
================= ======= ================================

**Error Messages**:

* INVALID_USN_PWD - 'Username or Password is incorrect'
* MISSING_FIELDS - 'Required field(s) is blank'
* INVALID_PARTNER_ID - 'Invalid Partner ID'

``POST /account/login-oauth``
`````````````````````````````

Log in via a third-party OAuth provider, e.g. Facebook. Note that this API will **not** implicitly register the user if an account does not already exist, but return an **error message** instead.

**Endpoint**: ``/api/1/account/login-oauth``

**Request**:

================= ====== ==========================================
Parameter         Type   Notes
================= ====== ==========================================
``service *``     string Identifies the third-party service used
``token *``       string Access token returned by third party
``partner_id *``  uuid
================= ====== ==========================================

**Response (JSON)**

================= ======= ================================
Key               Type    Notes
================= ======= ================================
``success``       bool
``message``       string  Human-readable error message
``error_code``    string  Error Code
``session``       string  Access token for later requests
``profile``       profile See ``profile`` definition above
================= ======= ================================

**Error Messages**:

* NON_EXISTING_OAUTH - 'OAuth Account does not exist'
* MISSING_FIELDS - 'Required field(s) is blank'
* INVALID_PARTNER_ID - 'Invalid Partner ID'
* NOT_SUPPORTED_OAUTH_PROVIDER - 'The OAuth Provider is not supported'

.. ``POST /account/connect-password``
.. ``````````````````````````````````

.. Adds a password-based login to an existing account, which must not have one already (i.e. it has only OAuth login).

.. **Endpoint**: ``/api/1/account/connect-password``

.. **Request**:

.. ================= ====== ==========================================
.. Parameter         Type   Notes
.. ================= ====== ==========================================
.. ``session``       string Access token returned by previous login
.. ``game_id``       uuid
.. ``username``      string
.. ``password``      string
.. ================= ====== ==========================================

.. **Response (JSON)**

.. ================= ======= ================================
.. Key               Type    Notes
.. ================= ======= ================================
.. ``success``       bool
.. ``message``       string  Human-readable error message
.. ``error_code``    string  Error Code
.. ================= ======= ================================


``POST /account/connect-oauth``
```````````````````````````````

Adds an OAuth login to an existing account. One account may have multiple OAuth logins.

**Endpoint**: ``/api/1/account/connect-oauth``

**Request**:

================= ====== ==========================================
Parameter         Type   Notes
================= ====== ==========================================
``session *``     string Access token returned by previous login
``partner_id *``  uuid
``service *``     string Identifies the third-party service used
``token *``       string Access token returned by third party
================= ====== ==========================================

**Response (JSON)**

================= ======= ================================
Key               Type    Notes
================= ======= ================================
``success``       bool
``message``       string  Human-readable error message
``error_code``    string  Error Code
================= ======= ================================

**Error Messages**:

* EXISTING_OAUTH - 'OAuth Account already exists'
* INVALID_SESSION - 'Invalid Session'
* MISSING_FIELDS - 'Required field(s) is blank'
* INVALID_PARTNER_ID - 'Invalid Partner ID'
* NOT_SUPPORTED_OAUTH_PROVIDER - 'The OAuth Provider is not supported'

``POST /account/disconnect-oauth``
``````````````````````````````````

Disconnect an OAuth login from an existing account.

**Endpoint**: ``/api/1/account/disconnect-oauth``

**Request**

================= ====== ==========================================
Parameter         Type   Notes
================= ====== ==========================================
``session *``     string Access token returned by previous login
``partner_id *``  uuid
``service *``     string Identifies the third-party service used
================= ====== ==========================================

**Response (JSON)**

================= ======= ================================
Key               Type    Notes
================= ======= ================================
``success``       bool
``message``       string  Human-readable error message
``error_code``    string  Error Code
================= ======= ================================

**Error Messages**:

* OAUTH_USER_NOT_CONNECTED - 'OAuth Account and Customer Account are not connected'
* NON_EXISTING_OAUTH - 'OAuth Account does not exist'
* INVALID_SESSION - 'Invalid Session'
* MISSING_FIELDS - 'Required field(s) is blank'
* INVALID_PARTNER_ID - 'Invalid Partner ID'
* NOT_SUPPORTED_OAUTH_PROVIDER - 'The OAuth Provider is not supported'

``POST /account/check-oauth-connection``
````````````````````````````````````````

Query if the OAuth login (access token) was added to the given account (session).

**Endpoint**: ``/api/1/account/check-oauth-connection``

**Request**

================== ====== ==========================================
Parameter          Type   Notes
================== ====== ==========================================
``session *``      string Access token returned by previous login
``partner_id *``   uuid
``service *``      string Identifies the third-party service used
``token *``        string Access token returned by third party
================== ====== ==========================================

**Response (JSON)**

================== ======= =========================================
Key                Type    Notes
================== ======= =========================================
``success``        bool    ``True`` indicates the connection was made
``message``        string  Human-readable error message
``error_code``     string  Error Code
================== ======= =========================================

**Error Messages**:

* OAUTH_ALREADY_CONNECTED - 'OAuth Account is connected already'
* NON_EXISTING_OAUTH - 'OAuth Account does not exist'
* MISSING_FIELDS - 'Required field(s) is blank'
* INVALID_PARTNER_ID - 'Invalid Partner ID'
* NOT_SUPPORTED_OAUTH_PROVIDER - 'The OAuth Provider is not supported'

``POST /account/profile``
`````````````````````````

Returns profile of a user. By default return the profile of the logged-in user. If ``username`` exists, return the profile of that particular user. Only profile of friends and inviter can be retrieve. If ``session`` exists, return the full public profile, or else return the minimal data to ensure the account existence. May be also used to check whether a session token is still valid.

**Endpoint**: ``/api/1/account/profile``

**Request**

================= ====== ==========================================
Parameter         Type   Notes
================= ====== ==========================================
``partner_id *``  uuid
``session``       string Access token returned by previous login. Optional. If exists, allow richer data.
``username``      string Optional
================= ====== ==========================================

**Response (JSON)**

================= ======= ================================
Key               Type    Notes
================= ======= ================================
``success``       bool
``message``       string  Human-readable error message
``error_code``    string  Error Code
``profile``       profile See ``profile`` definition above
================= ======= ================================

**Error Messages**:

* INVALID_SESSION - 'Invalid Session'
* INVALID_PARTNER_ID - 'Invalid Partner ID'

``POST /account/edit-profile``
``````````````````````````````

Updates profile of logged-in user. Parameters may be omitted, and those fields will be unchanged. If a key is available (e.g. ``bio``) but the value is null, it is understand as removing the value.

**Endpoint**: ``/api/1/account/edit-profile``

**Request**

================= ====== ==========================================
Parameter         Type   Notes
================= ====== ==========================================
``session *``     string Access token returned by previous login
``partner_id *``  uuid
``email``         string
``nickname``      string
``gender``        string "male", "female", or "other"
``bio``           string
``country_code``  string
``country_name``  string
``referral_code`` string
================= ====== ==========================================

**Response (JSON)**

================= ======= ================================
Key               Type    Notes
================= ======= ================================
``success``       bool
``message``       string  Human-readable error message
``error_code``    string  Error Code
``profile``       profile See ``profile`` definition above
================= ======= ================================

**Error Messages**

* INVALID_SESSION - 'Invalid Session'
* INVALID_PARTNER_ID - 'Invalid Partner ID'


``POST /account/change-password``
`````````````````````````````````

Change the password of logged-in user.

**Endpoint**: ``/api/1/account/change-password``

**Request**

==================== ====== ==========================================
Parameter            Type   Notes
==================== ====== ==========================================
``session *``        string Access token returned by previous login
``partner_id *``     uuid
``old_password``     string
``new_password``     string
``confirm_password`` string
==================== ====== ==========================================

**Response (JSON)**

================= ======= ================================
Key               Type    Notes
================= ======= ================================
``success``       bool
``message``       string  Human-readable error message
``error_code``    string  Error Code
================= ======= ================================

**Error Messages**

* INVALID_SESSION - 'Invalid Session'
* INVALID_PARTNER_ID - 'Invalid Partner ID'
* INVALID_USN_PWD - 'Username or Password is incorrect'
* UNIDENTICAL_PASSWORDS - 'Password and Confirm Pass are not identical'


``POST /friend/friend-list``
````````````````````````````

Query the friends list of current user. There is an option to get only the username, or also the profile.

**Endpoint**: ``/api/1/friend/friend-list``

**Request**

===================== ======= ==========================================
Parameter             Type    Notes
===================== ======= ==========================================
``session *``         string  Access token returned by previous login
``partner_id *``      uuid
``include_profile``   boolean If ``true``, return multiple ``profile`` objects. If ``false``, return an array of username. Default to ``false``
``status``            string  Can be either ``accepted``, ``pending``, ``waiting`` or ``rejected``. Default to ``accepted``
===================== ======= ==========================================

**Response (JSON)**

================= ======= ================================
Key               Type    Notes
================= ======= ================================
``success``       bool
``message``       string  Human-readable error message
``error_code``    string  Error Code
``friends``       JSON    ``{<username>:<profile>}`` or ``[<username>]`` depends on ``include_profile`` value
================= ======= ================================

**Error Messages**

* INVALID_SESSION - 'Invalid Session'
* INVALID_PARTNER_ID - 'Invalid Partner ID'
* MISSING_FIELDS - 'Required field(s) is blank'

``POST /friend/search``
```````````````````````

Return the list of users whose username, nickname or email matches the keyword. TThe order priority is friend first, stranger later.

**Endpoint**: ``/api/1/friend/search``

**Request**

===================== ======= ==========================================
Parameter             Type    Notes
===================== ======= ==========================================
``session *``         string  Access token returned by previous login
``partner_id *``      uuid
``keyword *``         string
``offset``            int     Default 0
``count``             int     Default 10
===================== ======= ==========================================

**Response (JSON)**

================= ======= ================================
Key               Type    Notes
================= ======= ================================
``success``       bool
``message``       string  Human-readable error message
``error_code``    string  Error Code
``users``         JSON    ``[{user_profile}, {user_profile},..]``
``count``         int     The total count of search result
================= ======= ================================

**Error Messages**

* INVALID_SESSION - 'Invalid Session'
* INVALID_PARTNER_ID - 'Invalid Partner ID'
* MISSING_FIELDS - 'Required field(s) is blank'

``POST /friend/send-request``
`````````````````````````````

Send a friend request to GToken.

**Endpoint**: ``/api/1/friend/send-request``

**Request**

===================== ====== ==========================================
Parameter             Type   Notes
===================== ====== ==========================================
``session *``         string Access token returned by previous login
``partner_id *``      uuid
``friend_username *`` string GToken unique username
===================== ====== ==========================================

**Response (JSON)**

================= ======= ================================
Key               Type    Notes
================= ======= ================================
``success``       bool
``message``       string  Human-readable error message
``error_code``    string  Error Code
================= ======= ================================

**Error Messages**

* INVALID_PARTNER_ID - 'Invalid Partner ID'
* INVALID_SESSION - 'Invalid Session'
* MISSING_FIELDS - 'Required field(s) is blank'
* NON_EXISTING_USER - 'User Account does not exist'
* REQUEST_ALREADY_SENT - 'Transaction has already been processed'


``POST /friend/respond-request``
````````````````````````````````

Accept/Reject a friend request from GToken. Can also be used to unfriend.

**Endpoint**: ``/api/1/friend/respond-request``

**Request**

===================== ====== ==========================================
Parameter             Type   Notes
===================== ====== ==========================================
``session *``         string Access token returned by previous login
``partner_id *``      uuid
``friend_username *`` string GToken unique username
``status *``          string Either ``accepted`` or ``rejected``
===================== ====== ==========================================

**Response (JSON)**

================= ======= ================================
Key               Type    Notes
================= ======= ================================
``success``       bool
``message``       string  Human-readable error message
``error_code``    string  Error Code
``friends``       Array   Array of the user's friends' username, can be used for reconfirmation
================= ======= ================================

**Error Messages**

* INVALID_PARTNER_ID - 'Invalid Partner ID'
* INVALID_SESSION - 'Invalid Session'
* MISSING_FIELDS - 'Required field(s) is blank'
* NON_EXISTING_USER - 'User Account does not exist'
* NON_EXISTING_FRIEND_REQUEST - 'Friend request does not exist'
* INVALID_FRIEND_REQUEST_STATUS - 'Friend request status must be either `accepted` or `rejected`'

``POST /transaction/get-conversion-rate``
`````````````````````````````````````````

Register an transaction to GToken. A successful API call will either create and return the transaction info with GToken transaction ID which will be used in the next call to excute the transaction.

**Endpoint**: ``/api/1/transaction/get-conversion-rate``

**Request**:

=============================== ============== ==============================
Parameter                       Type           Notes
=============================== ============== ==============================
``partner_id *``                uuid
``hashed_token *``              string         Hashed value of secret key
``source_currency``             string         Currency code ISO 4217
``destination_currency``        string         Currency code ISO 4217
``month (optional)``            string         ``%m``. Default to current month
``year (optional)``             string         ``%Y``. Default to current year
=============================== ============== ==============================

**Response (JSON)**

=============================== ======= ======================================
Key                             Types   Notes
=============================== ======= ======================================
``success``                     bool
``message``                     string  Human-readable error message
``error_code``                  string  Error Code
``source_currency``
``destination_currency``        string
``exchange_rate``               decimal
=============================== ======= ======================================

**Error Messages**:

* INVALID_PARTNER_ID - 'Invalid Partner ID'
* INVALID_HASHED_TOKEN - 'Invalid hashed token'
* MISSING_FIELDS - 'Required field(s) is blank'
* INVALID_CURRENCY_CODE - 'Currency code not found (ISO 4217)'


``POST /transaction/create-transaction``
````````````````````````````````````````

Register an transaction to GToken. A successful API call will either create and return the transaction info with GToken transaction ID which will be used in the next call to excute the transaction.

**Endpoint**: ``/api/1/transaction/create-transaction``

**Request**:

=============================== ============== ==============================
Parameter                       Type           Notes
=============================== ============== ==============================
``partner_id *``                uuid
``hashed_token *``              string         Hashed value of secret key
``transaction *``               JSON           A simplified version of standard ``transaction`` object. Consists of the keys: ``username *``, ``order_id *``, ``original_price *``, ``original_final_amount *``, ``original_currency *``, ``discount_percentage *``, ``revenue_percentage *``, ``description *``, ``payment_method *``, ``original_tax``, ``original_service_charge``.
``ip_address``                  string         The IP address of the user, if possible
=============================== ============== ==============================

**Response (JSON)**

================= ====== ==========================================
Key               Types  Notes
================= ====== ==========================================
``success``       bool
``message``       string Human-readable error message
``error_code``    string Error Code
``transaction``   JSON   See ``transaction`` definition above
================= ====== ==========================================

**Error Messages**:

* INVALID_JSON_TRANSACTION
* INVALID_PARTNER_ID - 'Invalid Partner ID'
* INVALID_HASHED_TOKEN - 'Invalid hashed token'
* MISSING_FIELDS - 'Required field(s) is blank'
* NON_EXISTING_USER - 'User Account does not exist'
* EXISTING_ORDERID - 'Order ID already exists'
* INVALID_TRANSACTION_STATUS - 'Invalid transaction status'
* INVALID_CURRENCY_CODE - 'Currency code not found (ISO 4217)'
* INVALID_JSON_TRANSACTION - 'Invalid transaction info (JSON)'


``POST /transaction/retrieve-transaction``
``````````````````````````````````````````

Retreive an transaction on GToken. The transaction must have been created in a previous step. A successful API call will return all information about that transaction.

**Endpoint**: ``/api/1/transaction/retrieve-transaction``

**Request**:

=============================== ============== ==============================
Parameter                       Type           Notes
=============================== ============== ==============================
``partner_id *``                uuid
``hashed_token *``              string         Hashed value of secret key
``gtoken_transaction_id``       string         
``order_id``                    string 
=============================== ============== ==============================

**Response (JSON)**

================= ====== ==========================================
Key               Types  Notes
================= ====== ==========================================
``success``       bool
``message``       string Human-readable error message
``error_code``    string Error Code
``transaction``   JSON   See ``transaction`` definition above
================= ====== ==========================================

**Error Messages**:

* INVALID_PARTNER_ID - 'Invalid Partner ID'
* INVALID_HASHED_TOKEN - 'Invalid hashed token'
* MISSING_FIELDS - 'Required field(s) is blank'
* TRANSACTION_NOT_FOUND - 'Transaction ID is not found'


``POST /transaction/execute-transaction``
`````````````````````````````````````````

Execute an transaction on GToken. The transaction must have been created in a previous step and haven't been completed yet. A successful API call will change the transaction status to ``completed``.

**Endpoint**: ``/api/1/transaction/execute-transaction``

**Request**:

=============================== ============== ==============================
Parameter                       Type           Notes
=============================== ============== ==============================
``partner_id *``                uuid
``hashed_token *``              string         Hashed value of secret key
``gtoken_transaction_id *``     string         
``status *``                    string         'failure', 'success', 'cancelled'
=============================== ============== ==============================

**Response (JSON)**

================= ====== ==========================================
Key               Types  Notes
================= ====== ==========================================
``success``       bool
``message``       string Human-readable error message
``error_code``    string Error Code
``transaction``   JSON   See ``transaction`` definition above
================= ====== ==========================================

**Error Messages**:

* INVALID_PARTNER_ID - 'Invalid Partner ID'
* INVALID_HASHED_TOKEN - 'Invalid hashed token'
* MISSING_FIELDS - 'Required field(s) is blank'
* INVALID_TRANSACTION_STATUS - 'Invalid transaction status'
* TRANSACTION_NOT_FOUND - 'Transaction ID is not found'
* TRANSACTION_ALREADY_PROCESSED - 'Transaction already process'


``POST /transaction/record-token-transaction``
``````````````````````````````````````````````

Record all the transactions made in partner systems back to GToken. Most token-consuming transactions (Play Token, Sugar Token, etc) involves platform-specific logics and therefore GToken doesn't do much rather than recording and displaying the transactions as they are. This API shouldn't be used for storage or processing purpose. For example, storing a ``pending`` transaction for late procession isn't what this API was designed for.

**Endpoint**: ``/api/1/transaction/record-token-transaction``

**Request**:

=============================== ============== ==============================
Parameter                       Type           Notes
=============================== ============== ==============================
``partner_id *``                uuid
``hashed_token *``              string         Hashed value of secret key
``token_transaction *``         JSON           Keys ``username *``, ``order_id *``, ``token_type *``, ``transaction_type *``, ``amount *``, ``tax``, ``service_charge``, ``description``. See ``token transaction`` definition above.
``ip_address``                  string         IP Address of the user, if possible
=============================== ============== ==============================

**Response (JSON)**

======================= ====== ==========================================
Key                     Types  Notes
======================= ====== ==========================================
``success``             bool
``message``             string Human-readable error message
``error_code``          string Error Code
``token_transaction``   JSON   See ``token transaction`` definition above
======================= ====== ==========================================

**Error Messages**:

* INVALID_JSON_TRANSACTION
* INVALID_PARTNER_ID - 'Invalid Partner ID'
* INVALID_HASHED_TOKEN - 'Invalid hashed token'
* MISSING_FIELDS - 'Required field(s) is blank'
* EXISTING_ORDERID - 'Order ID already exists'

``POST /transaction/direct-charge-gtoken``
``````````````````````````````````````````

Allow partner to charge directly to user's GToken Balance. GToken consumed by this APi is not eligible for commission.

**Endpoint**: ``/api/1/transaction/direct-charge-gtoken``

**Request**:

=============================== ============== ==============================
Parameter                       Type           Notes
=============================== ============== ==============================
``partner_id *``                uuid
``hashed_token *``              string         Hashed value of secret key
``direct_gtoken_transaction *`` JSON           Keys ``username *``, ``order_id *``, ``amount *``, ``description``. See ``direct gtoken transaction`` definition above.
``ip_address``                  string         IP Address of the user, if possible
=============================== ============== ==============================

**Response (JSON)**

======================= ====== ==========================================
Key                     Types  Notes
======================= ====== ==========================================
``success``             bool
``message``             string Human-readable error message
``error_code``          string Error Code
``token_transaction``   JSON   See ``token transaction`` definition above
======================= ====== ==========================================

**Error Messages**:

* INVALID_JSON_TRANSACTION
* INVALID_PARTNER_ID - 'Invalid Partner ID'
* INVALID_HASHED_TOKEN - 'Invalid hashed token'
* MISSING_FIELDS - 'Required field(s) is blank'
* EXISTING_ORDERID - 'Order ID already exists'
* INSUFFICIENT_AMOUNT - 'The amount is not sufficient to proceed transaction'


``POST /transaction/check-gtoken-balance``
``````````````````````````````````````````

Allow partner to check if user's GToken Balance is sufficient for intended transactions.

**Endpoint**: ``/api/1/transaction/check-gtoken-balance``

**Request**:

=============================== ============== ==============================
Parameter                       Type           Notes
=============================== ============== ==============================
``partner_id *``                uuid
``hashed_token *``              string         Hashed value of secret key
``username``                    string         GToken unique username
``amount``                      decimal        The amount needed to be checked
=============================== ============== ==============================

**Response (JSON)**

======================= ====== ==========================================
Key                     Types  Notes
======================= ====== ==========================================
``success``             bool   ``True`` if balance is sufficient. ``False`` otherwise.
``message``             string Human-readable error message
``error_code``          string Error Code
======================= ====== ==========================================

**Error Messages**:

* INVALID_PARTNER_ID - 'Invalid Partner ID'
* INVALID_HASHED_TOKEN - 'Invalid hashed token'
* MISSING_FIELDS - 'Required field(s) is blank'
* NON_EXISTING_USER - 'User Account does not exist'
* INSUFFICIENT_AMOUNT - 'The amount is not sufficient to proceed transaction'
