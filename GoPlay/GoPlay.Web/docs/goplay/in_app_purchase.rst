``POST /game/in-app-purchase``
``````````````````````````````

Allows game client to deduce GoPlay Token balance directly. This is the illegitimate way of buying directly inside the app itself. We made this API available for china devs game we help distribute in China and are unaffected by Google/Apple guidelines.

**Endpoint**: ``/api/1/game/in-app-purchase``

**Request**

=============================== ====== ==========================================
Parameter                       Type   Notes
=============================== ====== ==========================================
``session *``                   string Access token returned by previous login
``game_id *``                   guid
``exchange_option_identifier``  string Unique string that identifies the item. Can be used for both ``CreditType`` and ``Package``
``quantity``                    int    Quantity of the credit or package to be exchanged. Typically 01 for ``Package``
``amount (obsolete)``           int    Actually means *quantity*. Meant to be replaced by ``quantity`` param, but is still supported for legacy code. Typically 01 for ``Package``
=============================== ====== ==========================================

**Response (JSON)**

================= ======== ================================
Key               Type     Notes
================= ======== ================================
``success``       bool
``message``       string   Human-readable error message
``error_code``    string   Error Code
``exchange``      exchange See ``exchange`` definition above
================= ======== ================================

**Error Messages**:

* NO_EXCHANGE_GAME - 'Exchange Option does not belong to game'
* GAME_REMOVED - 'Game is not active or has been removed'
* EXCHANGE_REMOVED - 'Exchange Option has been removed'
* NEGATIVE_TRANSACTION - 'Exchange amount needs to be a positive integer'
* PACKAGE_QUANTITY - 'Exchange amount needs to be 01'
* INSUFFICIENT_BALANCE - 'Insufficient Balance'
* MISSING_FIELDS - 'Required field(s) is blank'
* INVALID_SESSION - 'Invalid Session'
* INVALID_GAME_ID - 'Invalid Game ID'
