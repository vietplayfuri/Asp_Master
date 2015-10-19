namespace GoPlay.Core
{
    public partial class GoPlayApi
    {
        
        #region Singleton
        private GoPlayApi() { }

        public static readonly GoPlayApi Instance;

        static GoPlayApi()
        {
            Instance = new GoPlayApi();
        }
        #endregion
    }
}
