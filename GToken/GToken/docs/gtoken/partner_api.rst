Partner API
===========

This API is used by third-party partner companies to offer extra services such as VIP membership.

.. contents::


Authentication
--------------

The partner service acts on behalf of a user. Accordingly, they should first authenticate the user via our OAuth2 provider, and obtain an access token.


API Calls
---------

``OAuth2 Endpoints``
````````````````````

We will expose the standard OAuth2 functions to act as a provider. See the documentation for `OAuth2 Servers`_ in Flask-OAuthLib.

.. _`OAuth2 Servers`: https://flask-oauthlib.readthedocs.org/en/latest/oauth2.html


``GET /partner/1/profile``
``````````````````````````
Retrieve a user's profile information.

**Endpoint**: ``/partner/1/profile``

**Request**:

================= ====== =====================================
Parameter         Type   Notes
================= ====== =====================================
``access_token``  string User access token obtained via OAuth2
================= ====== =====================================

**Response (JSON)**:

======================= ====== ==============================
Key                     Type   Notes
======================= ====== ==============================
``success``             bool
``message``             string Human-readable error message
``nickname``            string
``email``               string
``gender``              string
``vip``                 string "standard", "gold", or null
``goplay_token``        float  Available GoPlay Token balance
``free_goplay_token``   float  Available Free GoPlay Token balance
======================= ====== ==============================

``POST /partner/1/vip``
```````````````````````

Purchase VIP status for a user.

**Endpoint**: ``/partner/1/vip``

**Request**:

================= ====== =====================================
Parameter         Type   Notes
================= ====== =====================================
``access_token``  string User access token obtained via OAuth2
``level``         string "standard" or "gold"
================= ====== =====================================

**Response (JSON)**:

================= ====== ==============================
Key               Type   Notes
================= ====== ==============================
``success``       bool
``message``       string Human-readable error message
================= ====== ==============================
