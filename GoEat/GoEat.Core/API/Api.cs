namespace GoEat.Core
{
    public partial class GoEatApi
    {

        #region Singleton
        private GoEatApi() { }

        public static readonly GoEatApi Instance;

        static GoEatApi()
        {
            Instance = new GoEatApi();
        }
        #endregion

    }
}
