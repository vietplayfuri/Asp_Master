SDK
===

The SDK is a facade to the larger collection of RESTful APIs. The SDK provides developers with a modern, native library for accessing GToken service.

.. contents::

Versioning
----------

SDK is available in multiple versions. e.g.: ``0.1``, ``1.12``, ``1.21``.

Latest version of SDK will have access to latest GToken services, and old versions will be kept available for as long as possible.

SDK hax major and minor updates.

Minor updates usually means adding new optional fields, removing obsolete fields, or changing data type. The client must still work as expected if minor updates are obmitted. Minor updates are marked by the number after the dot in the version number, e.g ``x.0`` to ``x.1``.

If the SDK must be changed in a way that breaks existing clients, it will be made in a major update. Major updates change the number before the dot in the version number, e.g ``1.x`` to ``2.x``.

Availability
------------

Version 0 of the SDK is made available on iOS, Android and Unity. Other platforms will be added upon request and can always resort to direct RESTful API calls.

iOS
```

The GToken SDK for iOS is a framework that you can add to your XCode project. To add the SDK, open the SDK folder and drag the ``GTokenSDK.framework`` folder into the Frameworks section of your Project Navigator.

Configuration values such as game guid are kept in ``.plist`` file.

Once the SDK is set up, you can use the SDK from any of your implementation files by adding the GToken SDK header file:

``#import <GTokenSDK/GTokenSDK.h>``

Android
```````

The GToken SDK for Android is a framework which can be linked to your project in different IDEs (Eclipse, IntelliJ, and AndroidStudio). The SDK is compatible with Java 6 and 7.

Configuration values such as game guid are kept in the project's strings file.

Unity
`````

The GToken SDK for Unity is a Unity Pakage file (``.unitypackage``). The package can be added into Unity project as a Custom Package (Assets > Import Package)

Configuration values such as game guid are kept in the package settings menu.


Common Structures
-----------------

GTUser
``````

.. code-block:: java

   public class GTUser {
     protected int uid;
     protected string account;
     protected string nickname;
     protected string email;
     protected string gender;
     protected string vip;
     protected string countryCode;
     protected double goplayToken;
     protected double freeGoplayToken;
     protected string session;
   }

GTError
```````

.. code-block:: java

   public class GTError {
     protected int errorCode;
     protected string errorMessage;
   }

GTGame
``````

.. code-block:: java

   public class GTGame {
     protected string id;
   }

GTExchange
``````````

.. code-block:: java

   public class GTExchange {
     protected string id;
     protected string exchangeOptionIdentifier;
     protected string exchangeOptionType;
     protected double value;
     protected quantity int;
     protected boolean isFree;
   }

GTGameProgress
``````````````

.. code-block:: java
   
   public class GTGameProgress {
     protected string data;
     protected string metadata;
     protected Date savedAt;
   }

GTAccountCallback
`````````````````

.. code-block:: java

   public interface GTAccountCallback {
     /**
      * @param user the current user, or null if there is no user
      * @param error null if there is no error
      */
     public void onDataReturned(GTUser user, GTError error) {
       ...
     }
   }

GTExchangeCallback
``````````````````

.. code-block:: java

   public interface GTExchangeCallback {
     /**
      * @param user the current exchange, or null if there is no transaction
      * @param error null if there is no error
      */
     public void onDataReturned(GTExchange exchange, GTError error) {
       ...
     }
   }

GTGameProgressCallback
``````````````````````

.. code-block:: java

   public interface GTGameProgressCallback {
     /**
      * @param progress the current game progress, or null if there is error
      * @param error null if there is no error
      */
     public void onDataReturned(GTGameProgress progress, GTError error) {
       ...
     }
   }

SDK Functions
-------------

As the SDK is a wrapper layer over GToken APIs, it provides functions to access corresponding APIs.

``Register``
````````````

Used to explicitly register a customer account, meaning that the user has indicated they have no existing account, and filled out a registration form in-app.

*Only username and password fields must appear in registration form. Other optional fields can be skipped if it can make the UI cleaner*

Signature: ``public static void GTUser.register(Map userMap, GTAccountCallback callback)``

Params:

* ``userMap`` hashmap<string, string> with following structure

================= ==============================
Key               Notes
================= ==============================
``username *``    Must be unique
``password *``     
``email``       
``nickname``      
``gender``        "male", "female", or "other"
``referral_code`` 
``ip_address``    If not provided, use the IP address of requester
================= ==============================

* ``callback`` an implementation of GTAccountCallback


``LoginPassword``
`````````````````

Logs a user in via their GToken username and password. The account must already exist.

Signature: ``public static void GTUser.loginPassword(string username, string password, GTAccountCallback callback)``

Params:

* ``username``
* ``password``
* ``callback`` an implementation of GTAccountCallback


``Profile``
```````````

Returns profile of logged-in user. May be used to check whether a session token is still valid.

Signature: ``public void GTUser.profile(GTAccountCallback callback)``

Params:

* ``callback`` an implementation of GTAccountCallback


``ProfileEdit``
```````````````

Updates profile of logged-in user. Parameters may be omitted, and those fields will be unchanged.

Signature: ``public void GTUser.profileEdit(Map userMap, GTAccountCallback callback)``

Params:

* ``userMap`` hashmap<string, string> with following structure

================= ==============================
Key               Notes
================= ==============================
``email``       
``nickname``      
``gender``        "male", "female", or "other"
``referral_code`` 
================= ==============================

* ``callback`` an implementation of GTAccountCallback


``InAppPurchase``
`````````````````

Allows game client to deduce Play Token balance directly. This is the illegitimate way of buying directly inside the app itself. We made this API available for china devs game we help distribute in China and are unaffected by Google/Apple guidelines.

Signature: ``public void GTGame.inAppPurchase(GTUser user, string exchangeOptionIdentifier, int quantity, GTExchangeCallback callback)``

Params:

* ``user`` instance of GTUser with valid session
* ``exchangeOptionIdentifier`` Unique string that identifies the item. Can be used for both ``CreditType`` and ``Package``
* ``quantity`` Quantity of the credit or package to be exchanged. Typically 01 for PackageQuantity of the credit or package to be exchanged. Typically 01 for ``Package``
* ``callback`` an implementation of GTExchangeCallback


``SaveProgress``
````````````````

Allows game client to save progress directly with GToken server. The progress is saved in a string field, so game save data can be a text xml or json. The progress is saved together with a meta field and a saved_at datetime. Each time user wants to save data, the game’d better check the actual state of the data on GToken server. The game will then have to decide what to do, prompt the user that there is a more recent game save on the server or just overwrite it without prompting by analysing the meta, for example if the progression in the game is better on the local save than the cloud one, game client can choose to overwrite without prompting the player.

Signature: ``public void GTGame.saveProgress(GTUser user, GTGameProgress progress, GTGameProgressCallback callback)``

Params:

* ``user`` instance of ``GTUser`` with valid ``session``
* ``progress`` instance of ``GTGameProgress``, ``savedAt`` attribute is ignored
* ``callback`` an implementation of ``GTGameProgressCallback``


``GetProgress``
```````````````

Allows game client to retrieve progress directly from GToken server. The progress is saved in a string field, either as xml or json. The progress is saved together with a meta field and a saved_at datetime. The game will then have to decide what to do, prompt the user that there is a more recent game save on the server or just overwrite it without prompting by analysing the meta, for example if the progression in the game is better on the local save than the cloud one, game client can choose to overwrite without prompting the player.

Signature: ``public void GTGame.getProgress(GTUser user, GTGameProgressCallback callback)``

Params:

* ``user`` instance of ``GTUser`` with valid ``session``
* ``callback`` an implementation of ``GTGameProgressCallback``


``GetUnfulfilledExchanges``
```````````````````````````

Returns a list of unfulfilled exchanges made on GToken website.

Signature: ``public void GTGame.getUnfulfilledExchanges(GTUser user, GTExchangeCallback callback)``

Params:

* ``user`` instance of ``GTUser`` with valid ``session``
* ``callback`` an implementation of ``GTExchangeCallback``


``FulfillExchange``
```````````````````

Fulfilled an exchange made on GToken website

Signature: ``public void GTGame.fulfillExchange(GTUser user, GTExchange exchange, GTExchangeCallback callback)``

Params:

* ``user`` instance of ``GTUser`` with valid ``session``
* ``exchange`` instance of ``GTExchange``
* ``callback`` an implementation of ``GTExchangeCallback``


Facebook Login Flow
-------------------

Currently the hardest part to use GToken APIs is Facebook (or any OAuth Provider) login. The suggested flow is documented at this link_.

As we are building an SDK, I would love to see if SDK can make the flow simpler and easier to use. All suggestions are welcomed.

.. _link: http://dev2.gtoken.com/docs/hld/_build/html/api.html#facebook-login-register-flow
