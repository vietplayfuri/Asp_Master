using GoPlay.Dal;
using GoPlay.Models;
using Newtonsoft.Json.Linq;
using Platform.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GoPlay.Core
{
    public partial class GoPlayApi
    {
        public Result<Package> RetrieveExchangeHandler(customer_account user,
            Game game, decimal amount, CreditType creditType = null, GoPlay.Models.Package package = null)
        {
            //    var exchangeHandlers = {
            //    '8b1d8776e813536ecfy': MineManiaExchangeHandler,
            //    'ob5d4579e123381ecfy': SushiZombieExchangeHandler,
            //    '853461dsfwdgf85m0op': SlamdunkExchangeHandler,
            //    'c4c8d825a0ee6a78': FishingHeroExchangeHandler
            //        return exchangeHandlers.get(game.guid, StandardExchangeHandler)(user, game, exchangeOption, amount)
            //}
            return null;
        }

        public Result<Package> GetPackage(int id, bool? isActive = null, bool? isArchived = null)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetPackage(db, id, isActive, isArchived);
            }
        }

        public Result<Package> GetPackage(string identifier, bool? isActive = null, bool? isArchived = null)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetPackage(db, identifier, isActive, isArchived);
            }
        }

        public Result<List<Package>> GetPackages(int? gameId, bool? isActive = true, bool isArchived = false)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetPackages(db, gameId, isActive, isArchived);
            }
        }

        public Result<CreditType> GetCreditType(int id, bool? isActive = null, bool? isArchived = null)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetCreditType(db, id, isActive, isArchived);
            }
        }

        public Result<CreditType> GetCreditType(string identifier, bool? isActive = true, bool isArchived = false)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetCreditType(db, identifier, isActive, isArchived);
            }
        }


        public Result<List<CreditType>> GetCreditTypes(int? gameId, bool? isActive = true, bool isArchived = false)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.GetCreditTypes(db, gameId, isActive, isArchived);
            }
        }

        public Result<List<CreditType>> GetCreditTypesForAdminUser(string roleName, 
            int userId, bool? creditArchive = null, bool? gameArchive = null)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                switch (roleName)
                {
                    case GoPlayConstantValues.S_ROLE_ADMIN:
                        return repo.GetCreditTypesForAdminUser(db);
                    case GoPlayConstantValues.S_ROLE_GAME_ADMIN:
                        creditArchive = creditArchive.HasValue ? creditArchive.Value : default(bool);
                        gameArchive = gameArchive.HasValue ? gameArchive.Value : default(bool);
                        return repo.GetCreditTypesForAdminUser(db, userId, creditArchive.Value, gameArchive.Value);
                }
            }

            return Result<List<CreditType>>.Null(ErrorCodes.INVALID_ROLE);
        }

        public Result<CreditType> GetCreditTypeForAdminUser(string roleName, int creditTypeId)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                switch (roleName)
                {
                    case GoPlayConstantValues.S_ROLE_ADMIN:
                        return repo.GetCreditTypeById(db, creditTypeId);
                    case GoPlayConstantValues.S_ROLE_GAME_ADMIN:
                        return repo.GetCreditType(db, creditTypeId, null, false);
                }
            }

            return Result<CreditType>.Null(ErrorCodes.INVALID_ROLE);
        }

        public Result<List<Package>> GetPackagesForAdminUser(string roleName, int userId,
            bool? creditArchive = null, bool? gameArchive = null)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                switch (roleName)
                {
                    case GoPlayConstantValues.S_ROLE_ADMIN:
                        return repo.GetPackagesForAdminUser(db);
                    case GoPlayConstantValues.S_ROLE_GAME_ADMIN:
                        creditArchive = creditArchive.HasValue ? creditArchive.Value : default(bool);
                        gameArchive = gameArchive.HasValue ? gameArchive.Value : default(bool);
                        return repo.GetPackagesForAdminUser(db, userId, creditArchive.Value, gameArchive.Value);
                }
            }

            return Result<List<Package>>.Null(ErrorCodes.INVALID_ROLE);
        }


        public Result<Package> GetPackageForAdminUser(string roleName, int packageId,
            bool? isActive = null, bool? isArchive = null)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                switch (roleName)
                {
                    case GoPlayConstantValues.S_ROLE_ADMIN:
                        return repo.GetPackageForAdminUser(db, packageId);
                    case GoPlayConstantValues.S_ROLE_GAME_ADMIN:
                        return repo.GetPackageForAdminUser(db, packageId, isActive, isArchive);
                }
            }

            return Result<Package>.Null(ErrorCodes.INVALID_ROLE);
        }


        /// <summary>
        /// Get exchange option : --> return type (string + id of package or creditType)
        /// </summary>
        /// <param name="identifier">Condition to search</param>
        /// <param name="isActive">Condition to search</param>
        /// <param name="isArchived">Condition to search</param>
        /// <returns>string - int : name - id of table</returns>
        public Tuple<string, int> GetExchangeOption(string identifier, bool? isActive = null, bool? isArchived = null)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                var package = repo.GetPackage(db, identifier, isActive, isArchived).Data;
                if (package != null)
                {
                    return Tuple.Create<string, int>(GoPlayConstantValues.S_PACKAGE, package.id);
                }

                CreditType creditType = repo.GetCreditType(db, identifier, isActive, isArchived).Data;
                if (creditType != null)
                {
                    return Tuple.Create<string, int>(GoPlayConstantValues.S_CREDIT_TYPE, package.id);
                }
            }
            return null;
        }

        public List<external_exchange> GetExternalExchanges(int customerId, int gameId, string identifier = null, string transactionId = null)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                if (!string.IsNullOrEmpty(identifier))
                {
                    return repo.GetExternalExchange(db, customerId, gameId, identifier).Data;
                }

                if (!string.IsNullOrEmpty(transactionId))
                {
                    return repo.GetExternalExchange(db, customerId, gameId, null, transactionId).Data;
                }
            }

            return null;
        }

        public bool CreateExternalExchange(external_exchange externalExchange)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.CreateExternalExchange(db, externalExchange);
            }
        }

        public void MineManiaExchangeHandler()
        {
            //var generateTransactions = GenerateTransactions();

        }

        public void Calculate(dynamic generic,
            out decimal freePlaytoken,
            out decimal playtoken,
            customer_account user, int in_game_amount)
        {
            CalculatePlayToken(generic, out freePlaytoken, out playtoken, user, in_game_amount);
        }

        public void CalculatePlayToken(dynamic generic,
            out decimal freePlaytoken, out decimal playtoken,
            customer_account user, decimal purchaseAmount)
        {
            freePlaytoken = 0;
            playtoken = 0;
            if (generic.free_exchange_rate > 0)
            {
                decimal maxCapacity = CalculateInGameAmount(generic, user.free_play_token.Value, "free_play_token");
                maxCapacity = Math.Round(maxCapacity, 2);

                if (maxCapacity >= purchaseAmount)
                {
                    freePlaytoken = Math.Round(purchaseAmount / generic.free_exchange_rate, 3);
                    playtoken = 0;
                    return;
                }
            }

            if (generic.exchange_rate > 0)
            {
                decimal maxCapacity = CalculateInGameAmount(generic, user.play_token.Value, "play_token");
                maxCapacity = Math.Round(maxCapacity, 2);

                if (maxCapacity >= purchaseAmount)
                {
                    playtoken = Math.Round(purchaseAmount / generic.exchange_rate, 3);
                    freePlaytoken = 0;
                    return;
                }
            }
        }

        public decimal CalculateInGameAmount(dynamic generic, decimal playTokenAmount, string balance_type)
        {
            if (balance_type == "free_play_token")
                return generic.free_exchange_rate * playTokenAmount;
            if (balance_type == "play_token")
                return generic.exchange_rate * playTokenAmount;

            return -1;
        }

        public bool UpdateCreditType(CreditType creditType)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.UpdateCreditType(db, creditType);
            }
        }


        public int CreateCreditType(CreditType creditType)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.CreateCreditType(db, creditType);
            }
        }

        public int CreatePackage(Package package)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.CreatePackage(db, package);
            }
        }

        public bool UpdatePackage(Package package)
        {
            var repo = Repo.Instance;
            using (var db = repo.OpenConnectionFromPool())
            {
                return repo.UpdatePackage(db, package);
            }
        }
    }
}
